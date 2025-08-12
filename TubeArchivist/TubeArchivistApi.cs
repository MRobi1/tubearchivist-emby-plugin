using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Json;
using Microsoft.Extensions.Logging;

namespace Emby.Plugin.TubeArchivistMetadata.TubeArchivist
{
    /// <summary>
    /// TubeArchivist API client.
    /// </summary>
    public class TubeArchivistApi
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TubeArchivistApi> _logger;
        private readonly string _baseUrl;
        private readonly string _apiKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="TubeArchivistApi"/> class.
        /// </summary>
        /// <param name="httpClient">HTTP client.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="baseUrl">Base URL.</param>
        /// <param name="apiKey">API key.</param>
        public TubeArchivistApi(HttpClient httpClient, ILogger<TubeArchivistApi> logger, string baseUrl, string apiKey)
        {
            _httpClient = httpClient;
            _logger = logger;
            _baseUrl = baseUrl.TrimEnd('/');
            _apiKey = apiKey;
        }

        /// <summary>
        /// Get channel information by ID.
        /// </summary>
        /// <param name="channelId">Channel ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Channel data.</returns>
        public async Task<TubeArchivistChannel?> GetChannelAsync(string channelId, CancellationToken cancellationToken)
        {
            try
            {
                var url = $"{_baseUrl}/api/channel/{channelId}/";
                var response = await SendRequestAsync(url, cancellationToken).ConfigureAwait(false);
                
                if (response != null)
                {
                    return JsonSerializer.Deserialize<TubeArchivistChannel>(response, JsonDefaults.Options);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting channel {ChannelId}", channelId);
            }

            return null;
        }

        /// <summary>
        /// Get video information by ID.
        /// </summary>
        /// <param name="videoId">Video ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Video data.</returns>
        public async Task<TubeArchivistVideo?> GetVideoAsync(string videoId, CancellationToken cancellationToken)
        {
            try
            {
                var url = $"{_baseUrl}/api/video/{videoId}/";
                var response = await SendRequestAsync(url, cancellationToken).ConfigureAwait(false);
                
                if (response != null)
                {
                    return JsonSerializer.Deserialize<TubeArchivistVideo>(response, JsonDefaults.Options);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting video {VideoId}", videoId);
            }

            return null;
        }

        /// <summary>
        /// Get video progress information.
        /// </summary>
        /// <param name="videoId">Video ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Progress data.</returns>
        public async Task<TubeArchivistProgress?> GetVideoProgressAsync(string videoId, CancellationToken cancellationToken)
        {
            try
            {
                var url = $"{_baseUrl}/api/video/{videoId}/progress/";
                var response = await SendRequestAsync(url, cancellationToken).ConfigureAwait(false);
                
                if (response != null)
                {
                    return JsonSerializer.Deserialize<TubeArchivistProgress>(response, JsonDefaults.Options);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting video progress {VideoId}", videoId);
            }

            return null;
        }

        /// <summary>
        /// Update video progress.
        /// </summary>
        /// <param name="videoId">Video ID.</param>
        /// <param name="progress">Progress data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        public async Task UpdateVideoProgressAsync(string videoId, TubeArchivistProgress progress, CancellationToken cancellationToken)
        {
            try
            {
                var url = $"{_baseUrl}/api/video/{videoId}/progress/";
                var content = JsonSerializer.Serialize(progress, JsonDefaults.Options);
                await SendPostRequestAsync(url, content, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating video progress {VideoId}", videoId);
            }
        }

        /// <summary>
        /// Search for channels or videos.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Search results.</returns>
        public async Task<List<TubeArchivistSearchResult>> SearchAsync(string query, CancellationToken cancellationToken)
        {
            try
            {
                var url = $"{_baseUrl}/api/search/?query={Uri.EscapeDataString(query)}";
                var response = await SendRequestAsync(url, cancellationToken).ConfigureAwait(false);
                
                if (response != null)
                {
                    var searchResponse = JsonSerializer.Deserialize<TubeArchivistSearchResponse>(response, JsonDefaults.Options);
                    return searchResponse?.Results ?? new List<TubeArchivistSearchResult>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for {Query}", query);
            }

            return new List<TubeArchivistSearchResult>();
        }

        private async Task<string?> SendRequestAsync(string url, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Token {_apiKey}");
            
            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            
            _logger.LogWarning("API request failed: {StatusCode} - {Url}", response.StatusCode, url);
            return null;
        }

        private async Task SendPostRequestAsync(string url, string content, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", $"Token {_apiKey}");
            request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            
            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("API POST request failed: {StatusCode} - {Url}", response.StatusCode, url);
            }
        }
    }
}
