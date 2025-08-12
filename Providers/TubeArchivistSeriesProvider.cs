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
    /// TubeArchivist Series (Channel) metadata provider.
    /// </summary>
    public class TubeArchivistSeriesProvider : IRemoteMetadataProvider<Series, SeriesInfo>, IHasOrder
    {
        private readonly ILogger<TubeArchivistSeriesProvider> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TubeArchivistSeriesProvider"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="httpClientFactory">HTTP client factory.</param>
        public TubeArchivistSeriesProvider(ILogger<TubeArchivistSeriesProvider> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc />
        public string Name => "TubeArchivist";

        /// <inheritdoc />
        public int Order => 1;

        /// <inheritdoc />
        public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeriesInfo searchInfo, CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<RemoteSearchResult>>(Array.Empty<RemoteSearchResult>());
        }

        /// <inheritdoc />
        public async Task<MetadataResult<Series>> GetMetadata(SeriesInfo info, CancellationToken cancellationToken)
        {
            var result = new MetadataResult<Series>();

            var config = Plugin.Instance?.Configuration;
            if (config == null || string.IsNullOrEmpty(config.TubeArchivistUrl) || string.IsNullOrEmpty(config.TubeArchivistApiKey))
            {
                return result;
            }

            var channelId = ExtractChannelId(info);
            if (string.IsNullOrEmpty(channelId))
            {
                return result;
            }

            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                var api = new TubeArchivistApi(httpClient, _logger, config.TubeArchivistUrl, config.TubeArchivistApiKey);
                var channel = await api.GetChannelAsync(channelId, cancellationToken).ConfigureAwait(false);

                if (channel != null)
                {
                    var series = new Series
                    {
                        Name = channel.ChannelName,
                        Overview = TruncateOverview(channel.ChannelDescription, config.ChannelOverviewLength),
                        PremiereDate = channel.ChannelLastRefresh.HasValue ? DateTimeOffset.FromUnixTimeSeconds(channel.ChannelLastRefresh.Value).DateTime : null,
                        Status = channel.ChannelActive ? SeriesStatus.Continuing : SeriesStatus.Ended
                    };

                    series.SetProviderId(Name, channelId);

                    // Add community rating based on subscriber count
                    if (channel.ChannelSubs.HasValue && channel.ChannelSubs.Value > 0)
                    {
                        // Scale subscriber count to a rating (1-10)
                        var rating = Math.Min(10.0f, Math.Max(1.0f, (float)Math.Log10(channel.ChannelSubs.Value)));
                        series.CommunityRating = rating;
                    }

                    result.Item = series;
                    result.HasMetadata = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting metadata for series {Name}", info.Name);
            }

            return result;
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient();
            return httpClient.GetAsync(url, cancellationToken);
        }

        private static string? ExtractChannelId(SeriesInfo info)
        {
            // Try to get channel ID from provider ID
            if (info.ProviderIds.TryGetValue("TubeArchivist", out var providerId))
            {
                return providerId;
            }

            // Try to extract from path
            var path = info.Path;
            if (!string.IsNullOrEmpty(path))
            {
                var directoryName = Path.GetFileName(path);
                if (!string.IsNullOrEmpty(directoryName) && directoryName.StartsWith("UC", StringComparison.OrdinalIgnoreCase))
                {
                    return directoryName;
                }
            }

            return null;
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
