using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Emby.Plugin.TubeArchivistMetadata.TubeArchivist
{
    /// <summary>
    /// TubeArchivist Channel model.
    /// </summary>
    public class TubeArchivistChannel
    {
        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; } = string.Empty;

        [JsonPropertyName("channel_name")]
        public string ChannelName { get; set; } = string.Empty;

        [JsonPropertyName("channel_banner_url")]
        public string? ChannelBannerUrl { get; set; }

        [JsonPropertyName("channel_thumb_url")]
        public string? ChannelThumbUrl { get; set; }

        [JsonPropertyName("channel_tvart_url")]
        public string? ChannelTvArtUrl { get; set; }

        [JsonPropertyName("channel_description")]
        public string? ChannelDescription { get; set; }

        [JsonPropertyName("channel_last_refresh")]
        public long? ChannelLastRefresh { get; set; }

        [JsonPropertyName("channel_views")]
        public long? ChannelViews { get; set; }

        [JsonPropertyName("channel_subs")]
        public long? ChannelSubs { get; set; }

        [JsonPropertyName("channel_active")]
        public bool ChannelActive { get; set; }

        [JsonPropertyName("channel_subscribed")]
        public bool ChannelSubscribed { get; set; }
    }

    /// <summary>
    /// TubeArchivist Video model.
    /// </summary>
    public class TubeArchivistVideo
    {
        [JsonPropertyName("youtube_id")]
        public string YoutubeId { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("channel")]
        public TubeArchivistChannel? Channel { get; set; }

        [JsonPropertyName("vid_thumb_url")]
        public string? VidThumbUrl { get; set; }

        [JsonPropertyName("date_downloaded")]
        public long? DateDownloaded { get; set; }

        [JsonPropertyName("published")]
        public string? Published { get; set; }

        [JsonPropertyName("duration")]
        public int? Duration { get; set; }

        [JsonPropertyName("view_count")]
        public long? ViewCount { get; set; }

        [JsonPropertyName("like_count")]
        public long? LikeCount { get; set; }

        [JsonPropertyName("dislike_count")]
        public long? DislikeCount { get; set; }

        [JsonPropertyName("comment_count")]
        public long? CommentCount { get; set; }

        [JsonPropertyName("tags")]
        public List<string>? Tags { get; set; }

        [JsonPropertyName("subtitles")]
        public List<TubeArchivistSubtitle>? Subtitles { get; set; }

        [JsonPropertyName("media_url")]
        public string? MediaUrl { get; set; }

        [JsonPropertyName("media_size")]
        public long? MediaSize { get; set; }

        [JsonPropertyName("vid_last_refresh")]
        public long? VidLastRefresh { get; set; }
    }

    /// <summary>
    /// TubeArchivist Subtitle model.
    /// </summary>
    public class TubeArchivistSubtitle
    {
        [JsonPropertyName("ext")]
        public string? Extension { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("lang")]
        public string? Language { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }
    }

    /// <summary>
    /// TubeArchivist Progress model.
    /// </summary>
    public class TubeArchivistProgress
    {
        [JsonPropertyName("youtube_id")]
        public string YoutubeId { get; set; } = string.Empty;

        [JsonPropertyName("position")]
        public long Position { get; set; }

        [JsonPropertyName("watched")]
        public bool Watched { get; set; }

        [JsonPropertyName("date_played")]
        public long? DatePlayed { get; set; }
    }

    /// <summary>
    /// TubeArchivist Search Response model.
    /// </summary>
    public class TubeArchivistSearchResponse
    {
        [JsonPropertyName("query")]
        public TubeArchivistSearchQuery? Query { get; set; }

        [JsonPropertyName("results")]
        public List<TubeArchivistSearchResult>? Results { get; set; }
    }

    /// <summary>
    /// TubeArchivist Search Query model.
    /// </summary>
    public class TubeArchivistSearchQuery
    {
        [JsonPropertyName("term")]
        public string? Term { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("from")]
        public int From { get; set; }
    }

    /// <summary>
    /// TubeArchivist Search Result model.
    /// </summary>
    public class TubeArchivistSearchResult
    {
        [JsonPropertyName("_source")]
        public object? Source { get; set; }

        [JsonPropertyName("_index")]
        public string? Index { get; set; }

        [JsonPropertyName("_type")]
        public string? Type { get; set; }

        [JsonPropertyName("_id")]
        public string? Id { get; set; }

        [JsonPropertyName("_score")]
        public double? Score { get; set; }
    }
}
