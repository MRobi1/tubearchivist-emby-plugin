using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Emby.Plugin.TubeArchivistMetadata.TubeArchivist;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Emby.Plugin.TubeArchivistMetadata.Providers
{
    /// <summary>
    /// TubeArchivist Episode (Video) metadata provider.
    /// </summary>
    public class TubeArchivistEpisodeProvider : IRemoteMetadataProvider<Episode, EpisodeInfo>, IHasOrder
    {
        private readonly ILogger<TubeArchivistEpisodeProvider> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TubeArchivistEpisodeProvider"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="httpClientFactory">HTTP client factory.</param>
        public TubeArchivistEpisodeProvider(ILogger<TubeArchivistEpisodeProvider> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc />
        public string Name => "TubeArchivist";

        /// <inheritdoc />
        public int Order => 1;

        /// <inheritdoc />
        public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(EpisodeInfo searchInfo, CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<RemoteSearchResult>>(Array.Empty<RemoteSearchResult>());
        }

        /// <inheritdoc />
        public async Task<MetadataResult<Episode>> GetMetadata(EpisodeInfo info, CancellationToken cancellationToken)
        {
            var result = new MetadataResult<Episode>();

            var config = Plugin.Instance?.Configuration;
            if (config == null || string.IsNullOrEmpty(config.TubeArchivistUrl) || string.IsNullOrEmpty(config.TubeArchivistApiKey))
            {
                return result;
            }

            var videoId = ExtractVideoId(info);
            if (string.IsNullOrEmpty(videoId))
            {
                return result;
            }

            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                var api = new TubeArchivistApi(httpClient, _logger, config.TubeArchivistUrl, config.TubeArchivistApiKey);
                var video = await api.GetVideoAsync(videoId, cancellationToken).ConfigureAwait(false);

                if (video != null)
                {
                    var episode = new Episode
                    {
                        Name = video.Title,
                        Overview = TruncateOverview(video.Description, config.VideoOverviewLength),
                        RunTimeTicks = video.Duration.HasValue ? TimeSpan.FromSeconds(video.Duration.Value).Ticks : null
                    };

                    // Set air date from published date
                    if (!string.IsNullOrEmpty(video.Published) && DateTime.TryParse(video.Published, out var publishedDate))
                    {
                        episode.PremiereDate = publishedDate;
                        episode.ProductionYear = publishedDate.Year;
                        
                        // Set season and episode numbers based on year and date
                        episode.ParentIndexNumber = publishedDate.Year;
                        episode.IndexNumber = publishedDate.DayOfYear;
                    }

                    episode.SetProviderId(Name, videoId);

                    // Add community rating based on like/dislike ratio
                    if (video.LikeCount.HasValue && video.DislikeCount.HasValue)
                    {
                        var totalRating = video.LikeCount.Value + video.DislikeCount.Value;
                        if (totalRating > 0)
                        {
                            var ratio = (double)video.LikeCount.Value / totalRating;
                            episode.CommunityRating = (float)(ratio * 10);
                        }
                    }
                    else if (video.LikeCount.HasValue && video.LikeCount.Value > 0)
                    {
                        // If only likes are available, use a scaled rating
                        episode.CommunityRating = Math.Min(10.0f, Math.Max(5.0f, (float)Math.Log10(video.LikeCount.Value + 1)));
                    }

                    // Add view count as critic rating
                    if (video.ViewCount.HasValue && video.ViewCount.Value > 0)
                    {
                        episode.CriticRating = Math.Min(100, Math.Max(10, (int)Math.Log10(video.ViewCount.Value) * 10));
                    }

                    // Add tags/genres
                    if (video.Tags != null && video.Tags.Count > 0)
                    {
                        episode.Tags = video.Tags.ToArray();
                    }

                    result.Item = episode;
                    result.HasMetadata = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting metadata for episode {Name}", info.Name);
            }

            return result;
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient();
            return httpClient.GetAsync(url, cancellationToken);
        }

        private static string? ExtractVideoId(EpisodeInfo info)
        {
            // Try to get video ID from provider ID
            if (info.ProviderIds.TryGetValue("TubeArchivist", out var providerId))
            {
                return providerId;
            }

            // Try to extract from filename
            var path = info.Path;
            if (!string.IsNullOrEmpty(path))
            {
                var filename = Path.GetFileNameWithoutExtension(path);
                if (!string.IsNullOrEmpty(filename) && filename.Length == 11)
                {
                    return filename;
                }
                
                // Try to extract YouTube ID pattern from filename
                var parts = filename?.Split('_', '-', '[', ']');
                if (parts != null)
                {
                    foreach (var part in parts)
                    {
                        if (part.Length == 11 && IsValidYouTubeId(part))
                        {
                            return part;
                        }
                    }
                }
            }

            return null;
        }

        private static bool IsValidYouTubeId(string id)
        {
            // Basic validation for YouTube ID format
            if (string.IsNullOrEmpty(id) || id.Length != 11)
                return false;

            foreach (var c in id)
            {
                if (!char.IsLetterOrDigit(c) && c != '-' && c != '_')
                    return false;
            }

            return true;
        }

        private static string? TruncateOverview(string? overview, int maxLength)
        {
            if (string.IsNullOrEmpty(overview))
                return overview;

            if (overview.Length <= maxLength)
                return overview;

            var truncated = overview.Substring(0, maxLength);
            var lastSpace = truncated.LastIndexOf(' ');
            
            if (lastSpace > maxLength * 0.8) // Only truncate at word boundary if it's reasonably close
            {
                truncated = truncated.Substring(0, lastSpace);
            }
            
            return truncated + "...";
        }
    }
}
