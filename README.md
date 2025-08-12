# TubeArchivist Metadata Plugin for Emby

This plugin adds metadata provider for [TubeArchivist](https://www.tubearchivist.com/) to Emby Server, offering improved flexibility and native integration compared to previous solutions.

## Overview

The plugin organizes TubeArchivist content as a **Shows** collection in Emby, where:
- Each **channel** becomes a **show/series**  
- Each **video** becomes an **episode**
- Videos are organized into **seasons by year**

The plugin interacts with TubeArchivist APIs to fetch comprehensive metadata for both channels and videos.

## Features

### Metadata Support
- ✅ **Video metadata** (episodes): Title, description, duration, publish date, ratings, tags
- ✅ **Channel metadata** (shows): Name, description, subscriber count, status
- ✅ **Video images**: Thumbnail images for episodes
- ✅ **Channel images**: Thumbnails, banners, and TV art for shows
- ✅ **Seasonal organization**: Videos grouped by publication year
- ✅ **Ratings**: Community ratings based on like/dislike ratios and view counts

### Playback Synchronization  
- 🔄 **Bidirectional sync**: Keep watch progress synchronized between Emby and TubeArchivist
- ⚙️ **Configurable**: Choose one-way or two-way synchronization
- 👥 **Multi-user support**: Sync specific users' watch progress
- ⏱️ **Scheduled sync**: Automatic synchronization at configurable intervals

## Installation

### Method 1: Manual Installation (Recommended)

1. **Download** the latest release (`Emby.Plugin.TubeArchivistMetadata.dll`) from the repository releases section
2. **Create** the plugin directory in your Emby installation:
   ```
   <EmbyServerPath>/plugins/TubeArchivistMetadata/
   ```
3. **Copy** the `Emby.Plugin.TubeArchivistMetadata.dll` file to this directory
4. **Copy** the `meta.json` file to the same directory
5. **Restart** Emby Server to load the plugin

### Method 2: Build from Source

Prerequisites:
- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- Emby Server installation (for reference assemblies)

Build steps:
```bash
# Clone the repository
git clone https://github.com/MRobi1/tubearchivist-emby-plugin.git
cd tubearchivist-emby-plugin

# Set the Emby Server path (adjust as needed)
export EmbyServerPath="/opt/emby-server/system"

# Build the plugin
dotnet build --configuration Release

# Copy to Emby plugins directory
mkdir -p /var/lib/emby/plugins/TubeArchivistMetadata
cp bin/Release/net6.0/Emby.Plugin.TubeArchivistMetadata.dll /var/lib/emby/plugins/TubeArchivistMetadata/
cp meta.json /var/lib/emby/plugins/TubeArchivistMetadata/

# Restart Emby
sudo systemctl restart emby-server
```

## Configuration

### Plugin Configuration

After installation, configure the plugin:

1. Go to **Emby Dashboard** → **Advanced** → **Plugins**
2. Find **TubeArchivist Metadata** and click **Settings**
3. Configure the following settings:

#### Required Settings
- **TubeArchivist URL**: Your TubeArchivist instance URL (e.g., `http://localhost:8000`)
- **TubeArchivist API Key**: Your API key from TubeArchivist settings

#### Optional Settings
- **Collection Display Name**: Name for the collection (default: "TubeArchivist")
- **Channel Overview Length**: Max characters for channel descriptions (50-2000)
- **Video Overview Length**: Max characters for video descriptions (50-2000)

#### Playback Synchronization Settings
- **Sync from Emby to TubeArchivist**: Enable to sync watch progress to TubeArchivist
- **Emby to TubeArchivist Users**: Username to sync (leave empty to disable)
- **Sync from TubeArchivist to Emby**: Enable to sync watch progress from TubeArchivist  
- **TubeArchivist to Emby Users**: Comma-separated usernames to sync
- **Sync Interval**: How often to sync in seconds (30-3600)

### Library Setup

1. **Add Media Library**:
   - Go to **Dashboard** → **Libraries** → **Add Media Library**
   - Select **TV Shows** as content type
   - Set your desired display name
   - Add your TubeArchivist media folder to **Folders**

2. **Configure Metadata Providers**:
   - Scroll to **Metadata downloaders** section
   - **Uncheck all providers** except **TubeArchivist**
   - For **Season** providers, disable all (TubeArchivist handles seasons automatically)

3. **Configure Image Providers**:
   - In **Image fetchers** section
   - **Uncheck all providers** except **TubeArchivist**

4. **Save** and return to home screen

Emby will automatically fetch metadata and images after library creation.

## Usage

### Media Organization

The plugin organizes your TubeArchivist content as follows:

```
TubeArchivist Library/
├── Channel 1 (TV Show)
│   ├── Season 2023/
│   │   ├── Episode 1 (Video from Jan 1, 2023)
│   │   ├── Episode 2 (Video from Jan 15, 2023)
│   │   └── ...
│   ├── Season 2024/
│   │   ├── Episode 1 (Video from Feb 3, 2024)
│   │   └── ...
├── Channel 2 (TV Show)
│   └── ...
```

### Playback Synchronization

The plugin offers flexible synchronization options:

#### Emby → TubeArchivist Sync
- Syncs when users play videos in Emby
- Updates watch progress and completion status in TubeArchivist
- Requires username configuration

#### TubeArchivist → Emby Sync  
- Runs as scheduled task at configured intervals
- Updates Emby with watch progress from TubeArchivist
- Supports multiple users

#### Bidirectional Sync
- Enable both options for full synchronization
- Keeps both systems perfectly in sync

## Docker Integration

If using Docker containers, mount TubeArchivist media as **read-only** in Emby:

```yaml
version: '3.8'
services:
  emby:
    image: emby/embyserver
    volumes:
      - /path/to/emby/config:/config
      - /path/to/tubearchivist/media:/tubearchivist:ro  # Read-only mount
    ports:
      - "8096:8096"
```

This prevents Emby from modifying TubeArchivist files.

## Troubleshooting

### Common Issues

#### Plugin Not Loading
- Verify `.dll` and `meta.json` are in correct plugin directory
- Check Emby logs for loading errors
- Ensure .NET 6.0 runtime is installed

#### No Metadata Appearing
- Verify TubeArchivist URL and API key are correct
- Check TubeArchivist API is accessible from Emby server
- Ensure TubeArchivist metadata providers are enabled and others disabled

#### Images Not Loading
- Check TubeArchivist image URLs are accessible
- Verify image providers configuration
- Check network connectivity between Emby and TubeArchivist

#### Sync Not Working
- Verify user names are spelled correctly
- Check TubeArchivist API permissions
- Review sync interval settings
- Check Emby scheduled tasks for errors

### Debug Information

Enable debug logging in Emby:
1. Go to **Dashboard** → **Advanced** → **Logs**  
2. Set log level to **Debug**
3. Look for `TubeArchivistMetadata` entries

Check TubeArchivist logs for API request information.

## API Compatibility

This plugin is designed to work with:
- **TubeArchivist**: v0.4.0+
- **Emby Server**: 4.8.0+
- **.NET Runtime**: 6.0+

## Differences from Jellyfin Version

Key changes made for Emby compatibility:

### Namespace Changes
- `Jellyfin.Plugin.*` → `Emby.Plugin.*`
- Updated using statements for Emby-specific libraries

### API Differences  
- `MediaBrowser.Controller` interface changes
- Image provider implementation differences
- Configuration page JavaScript API changes

### Plugin Architecture
- Different plugin base classes
- Modified dependency injection setup
- Emby-specific metadata interfaces

## Contributing

Contributions are welcome! Please:

1. **Fork** the repository
2. **Create** a feature branch
3. **Make** your changes
4. **Test** thoroughly with your Emby setup
5. **Submit** a pull request

### Development Setup

```bash
# Install dependencies
dotnet restore

# Build in debug mode  
dotnet build --configuration Debug

# Run tests (when available)
dotnet test
```

## License

This plugin is distributed under the **GPLv3 License**. See `LICENSE` file for details.

## Support

- **GitHub Issues**: Report bugs and request features
- **TubeArchivist Discord**: Community support
- **Documentation**: Check TubeArchivist docs for API information

## Acknowledgments

- **TubeArchivist Team**: For the excellent archiving solution
- **Emby Team**: For the media server platform  
- **Jellyfin Plugin**: Original implementation inspiration

---

**⚠️ Important Notes:**
- Always backup your Emby configuration before installing plugins
- This plugin requires TubeArchivist to be running and accessible
- Read-only mount recommended when using Docker to prevent file conflicts
