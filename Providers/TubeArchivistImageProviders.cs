using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Emby.Plugin.TubeArchivistMetadata.TubeArchivist;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Emby.Plugin.TubeArchivistMetadata.Providers
{
    /// <summary>
    /// TubeArchivist Series image provider.
    /// </summary>
    public class TubeArchivistSeriesImageProvider : IRemoteImageProvider, IHasOrder
    {
        private readonly ILogger<TubeArchivistSeriesImageProvider> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TubeArchivistSeriesImageProvider"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="httpClientFactory">HTTP client factory.</param>
        public TubeArchivistSeriesImageProvider(ILogger<TubeArchivistSeriesImageProvider> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc />
        public string Name => "TubeArchivist";

        /// <inheritdoc />
        public int Order => 1;

        /// <inheritdoc />
        public bool Supports(BaseItem item)
        {
            return item is Series;
        }

        /// <inheritdoc />
        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new List<ImageType>
            {
                ImageType.Primary,
                ImageType.Banner,
                ImageType.Art
            };
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            var config = Plugin.Instance?.Configuration;
            if (config == null || string.IsNullOrEmpty(config.TubeArchivistUrl) || string.IsNullOrEmpty(config.TubeArchivistApiKey))
            {
                return Enumerable.Empty<RemoteImageInfo>();
            }

            var channelId = item.GetProviderId(Name);
            if (string.IsNullOrEmpty(channelId))
            {
                return Enumerable.Empty<RemoteImageInfo>();
            }

            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                var api = new TubeArchivistApi(httpClient, _logger, config.TubeArchivistUrl, config.TubeArchivistApiKey);
                var channel = await api.GetChannelAsync(channelId, cancellationToken).ConfigureAwait(false);

                if (channel == null)
                {
                    return Enumerable.Empty<RemoteImageInfo>();
                }

                var images = new List<RemoteImageInfo>();

                // Primary image (thumbnail)
                if (!string.IsNullOrEmpty(channel.ChannelThumbUrl))
                {
                    images.Add(new RemoteImageInfo
                    {
                        ProviderName = Name,
                        Type = ImageType.Primary,
                        Url = GetFullImageUrl(config.TubeArchivistUrl, channel.ChannelThumbUrl)
                    });
                }

                // Banner image
                if (!string.IsNullOrEmpty(channel.ChannelBannerUrl))
                {
                    images.Add(new RemoteImageInfo
                    {
                        ProviderName = Name,
                        Type = ImageType.Banner,
                        Url = GetFullImageUrl(config.TubeArchivistUrl, channel.ChannelBannerUrl)
                    });
                }

                // TV Art image
                if (!string.IsNullOrEmpty(channel.ChannelTvArtUrl))
                {
                    images.Add(new RemoteImageInfo
                    {
                        ProviderName = Name,
                        Type = ImageType.Art,
                        Url = GetFullImageUrl(config.TubeArchivistUrl, channel.ChannelTvArtUrl)
                    });
                }

                return images;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting images for series {ItemName}", item.Name);
                return Enumerable.Empty<RemoteImageInfo>();
            }
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient();
            return httpClient.GetAsync(url, cancellationToken);
        }

        private static string GetFullImageUrl(string baseUrl, string imageUrl)
        {
            if (Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                return imageUrl;
            }

            return $"{baseUrl.TrimEnd('/')}/{imageUrl.TrimStart('/')}";
        }
    }

    /// <summary>
    /// TubeArchivist Episode image provider.
    /// </summary>
    public class TubeArchivistEpisodeImageProvider : IRemoteImageProvider, IHasOrder
    {
        private readonly ILogger<TubeArchivistEpisodeImageProvider> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TubeArchivistEpisodeImageProvider"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="httpClientFactory">HTTP client factory.</param>
        public TubeArchivistEpisodeImageProvider(ILogger<TubeArchivistEpisodeImageProvider> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc />
        public string Name => "TubeArchivist";

        /// <inheritdoc />
        public int Order => 1;

        /// <inheritdoc />
        public bool Supports(BaseItem item)
        {
            return item is Episode;
        }

        /// <inheritdoc />
        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new List<ImageType>
            {
                ImageType.Primary
            };
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            var config = Plugin.Instance?.Configuration;
            if (config == null || string.IsNullOrEmpty(config.TubeArchivistUrl) || string.IsNullOrEmpty(config.TubeArchivistApiKey))
            {
                return Enumerable.Empty<RemoteImageInfo>();
            }

            var videoId = item.GetProviderId(Name);
            if (string.IsNullOrEmpty(videoId))
            {
                return Enumerable.Empty<RemoteImageInfo>();
            }

            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                var api = new TubeArchivistApi(httpClient, _logger, config.TubeArchivistUrl, config.TubeArchivistApiKey);
                var video = await api.GetVideoAsync(videoId, cancellationToken).ConfigureAwait(false);

                if (video == null || string.IsNullOrEmpty(video.VidThumbUrl))
                {
                    return Enumerable.Empty<RemoteImageInfo>();
                }

                return new List<RemoteImageInfo>
                {
                    new RemoteImageInfo
                    {
                        ProviderName = Name,
                        Type = ImageType.Primary,
                        Url = GetFullImageUrl(config.TubeArchivistUrl, video.VidThumbUrl)
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting images for episode {ItemName}", item.Name);
                return Enumerable.Empty<RemoteImageInfo>();
            }
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient();
            return httpClient.GetAsync(url, cancellationToken);
        }

        private static string GetFullImageUrl(string baseUrl, string imageUrl)
        {
            if (Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                return imageUrl;
            }

            return $"{baseUrl.TrimEnd('/')}/{imageUrl.TrimStart('/')}";
        }
    }
}
