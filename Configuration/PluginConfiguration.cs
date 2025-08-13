using MediaBrowser.Model.Plugins;

namespace Emby.Plugin.TubeArchivistMetadata.Configuration
{
    /// <summary>
    /// Configuration for the TubeArchivist metadata plugin
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        /// <summary>
        /// Gets or sets the TubeArchivist URL
        /// </summary>
        public string TubeArchivistUrl { get; set; } = "http://localhost:8000";

        /// <summary>
        /// Gets or sets the TubeArchivist API key
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the collection display name
        /// </summary>
        public string CollectionDisplayName { get; set; } = "TubeArchivist";

        /// <summary>
        /// Gets or sets the maximum channel overview length
        /// </summary>
        public int ChannelOverviewLength { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the maximum video overview length
        /// </summary>
        public int VideoOverviewLength { get; set; } = 1000;

        /// <summary>
        /// Gets or sets a value indicating whether to sync from Emby to TubeArchivist
        /// </summary>
        public bool SyncFromEmby { get; set; } = false;

        /// <summary>
        /// Gets or sets the Emby user to sync to TubeArchivist
        /// </summary>
        public string EmbyToTubeArchivistUsers { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to sync from TubeArchivist to Emby
        /// </summary>
        public bool SyncFromTubeArchivist { get; set; } = false;

        /// <summary>
        /// Gets or sets the TubeArchivist users to sync to Emby (comma-separated)
        /// </summary>
        public string TubeArchivistToEmbyUsers { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sync interval in seconds
        /// </summary>
        public int SyncInterval { get; set; } = 300;
    }
}
