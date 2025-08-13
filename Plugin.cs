using System;
using System.Collections.Generic;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Emby.Plugin.TubeArchivistMetadata.Configuration;

namespace Emby.Plugin.TubeArchivistMetadata
{
    /// <summary>
    /// The main plugin class for TubeArchivist metadata provider
    /// </summary>
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
        /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) 
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        /// <summary>
        /// Gets the current plugin instance.
        /// </summary>
        public static Plugin Instance { get; private set; }

        /// <inheritdoc />
        public override string Name => "TubeArchivist Metadata";

        /// <inheritdoc />
        public override string Description => "Provides metadata and images from TubeArchivist for TV shows and movies";

        /// <inheritdoc />
        public override Guid Id => Guid.Parse("A8B5B444-8C8D-4F2A-9B1C-3D4E5F6A7B8C");

        /// <summary>
        /// Gets the plugin web pages.
        /// </summary>
        /// <returns>The plugin web pages.</returns>
        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = this.Name,
                    EmbeddedResourcePath = string.Format("{0}.Configuration.configPage.html", GetType().Namespace)
                }
            };
        }
    }
}
