using MediaBrowser.Model.Plugins;

namespace Emby.Plugin.TubeArchivistMetadata.Configuration
{
    /// <summary>
    /// Plugin configuration.
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
        /// </summary>
        public PluginConfiguration()
        {
            TubeArchivistUrl = string.Empty;
            TubeArchivistApiKey = string.Empty;
            CollectionTitle = "TubeArchivist";
            ChannelOverviewLength = 500;
            VideoOverviewLength = 300;
            JellyfinToTubeArchivistPlayback = false;
            TubeArchivistToJellyfinPlayback = false;
            PlaybackUsers = string.Empty;
            FromTubeArchivistUsers = string.Empty;
            SyncIntervalSeconds = 60;
        }

        /// <summary>
        /// Gets or sets the TubeArchivist instance URL.
        /// </summary>
        public string TubeArchivistUrl { get; set; }

        /// <summary>
        /// Gets or sets the TubeArchivist API key.
        /// </summary>
        public string TubeArchivistApiKey { get; set; }

        /// <summary>
        /// Gets or sets the collection title.
        /// </summary>
        public string CollectionTitle { get; set; }

        /// <summary>
        /// Gets or sets the channel overview length.
        /// </summary>
        public int ChannelOverviewLength { get; set; }

        /// <summary>
        /// Gets or sets the video overview length.
        /// </summary>
        public int VideoOverviewLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to sync playback from Emby to TubeArchivist.
        /// </summary>
        public bool JellyfinToTubeArchivistPlayback { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to sync playback from TubeArchivist to Emby.
        /// </summary>
        public bool TubeArchivistToJellyfinPlayback { get; set; }

        /// <summary>
        /// Gets or sets the users to sync playback data for (Emby to TubeArchivist).
        /// </summary>
        public string PlaybackUsers { get; set; }

        /// <summary>
        /// Gets or sets the users to sync playback data for (TubeArchivist to Emby).
        /// </summary>
        public string FromTubeArchivistUsers { get; set; }

        /// <summary>
        /// Gets or sets the sync interval in seconds.
        /// </summary>
        public int SyncIntervalSeconds { get; set; }
    }
}
