using SpotifyAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO : Refactor & <comment> this class
namespace PlaylistRetriever
{
    public class PlaylistWriter
    {
        // Declarations //
        public enum PlaylistColumn
        {
            TrackName,
            TrackArtists,
            AlbumName,
            Duration,
            AddedAt,
            AddedBy,
            IsLocal,
            TrackID
        }


        // Constructors //
        public PlaylistWriter()
        {
            Playlists = null;
            ColumnList = null;
            Error = null;
        }


        // Properties //
        public List<SpotifyPlaylist> Playlists { private get; set; }
        public List<PlaylistColumn> ColumnList { get; set; }
        public string Error { get; private set; }


        // Public Methods //
        public void AddPlaylist(SpotifyPlaylist playlist)
        {
            if (playlist == null)
                return;

            Playlists = Playlists ?? new List<SpotifyPlaylist>();
            Playlists.Add(playlist);
        }

        // TODO : Rewrite to provide detailed error information when returning false
        public async Task<bool> SavePlaylists(string folderpath, string fileExtension)
        {
            switch (Playlists.Count)
            {
                case 0:
                    return false;
                case 1:
                    return await SavePlaylistToFile($"{folderpath}\\{Playlists[0].name}.{fileExtension}", Playlists[0], ColumnList);
                default:
                    if (Playlists.Count < 0)
                        return false;

                    List<Task<bool>> saveJobs = new List<Task<bool>>();

                    foreach (SpotifyPlaylist playlist in Playlists)
                    {
                        saveJobs.Add(SavePlaylistToFile($"{folderpath}\\{playlist.name}.{fileExtension}", playlist, ColumnList));
                    }
                    await Task.WhenAll(saveJobs.ToArray());

                    foreach (Task<bool> task in saveJobs)
                    {
                        if (task.IsFaulted || task.Result != true)
                            return false;
                    }

                    return true;
            }
        }


        public static async Task<string> GetPlaylistAsXml(SpotifyPlaylist playlist, List<PlaylistColumn> columns)
        {
            // Return Cases
            if (playlist == null || columns == null)
                return null;

            // Declares
            StringBuilder playlistAsXml = new StringBuilder();

            // Add title row
            foreach (PlaylistColumn col in columns)
            {
                playlistAsXml.Append(col.ToString());
                if (columns.Last<PlaylistColumn>() == col)
                    playlistAsXml.Append(Environment.NewLine);
                else
                    playlistAsXml.Append(',');
            }

            // Fill with playlist content
            foreach (SpotifyPlaylistTrack oLine in playlist.tracks.items)
            {
                playlistAsXml.AppendLine(GetPlaylistLine(oLine, columns) ?? string.Empty);
            }
            return playlistAsXml.ToString();
        }

        public static async Task<bool> SavePlaylistToFile(string filepath, SpotifyPlaylist playlist, List<PlaylistColumn> playlistColumns)
        {
            if (filepath == null || filepath == string.Empty)
            {
                return false;
            }

            string playlistXml = await GetPlaylistAsXml(playlist, playlistColumns);
            if (playlistXml == null || playlistXml == string.Empty)
                return false;

            using (FileStream oFileStream = File.Create(filepath))
            {
                using (StreamWriter streamWriter = new StreamWriter(oFileStream))
                {
                    await streamWriter.WriteAsync(playlistXml);
                    return true;
                }
            }
        }

        // Private Methods //
        private static string GetPlaylistLine(SpotifyPlaylistTrack oLine, List<PlaylistColumn> columns)
        {
            StringBuilder lineBuilder = new StringBuilder();
            foreach (PlaylistColumn oCol in columns)
            {
                lineBuilder.Append(GetField(oCol, oLine) ?? string.Empty);
                if (columns.Last<PlaylistColumn>() != oCol)
                    lineBuilder.Append(',');
            }
            return lineBuilder.ToString();
        }

        private static string GetField(PlaylistColumn oCol, SpotifyPlaylistTrack oLine)
        {
            StringBuilder fieldBuilder = new StringBuilder();
            fieldBuilder.Append('\"');
            switch (oCol)
            {
                case PlaylistColumn.TrackName:
                    fieldBuilder.Append(oLine.track.name);
                    break;
                case PlaylistColumn.TrackArtists:
                    foreach (SpotifyArtistSimple artist in oLine.track.artists)
                    {
                        fieldBuilder.Append(artist.name);
                        if (oLine.track.artists.Last<SpotifyArtistSimple>() != artist)
                            fieldBuilder.Append(", ");
                    }
                    break;
                case PlaylistColumn.AlbumName:
                    fieldBuilder.Append(oLine.track.album.name);
                    break;
                case PlaylistColumn.Duration:
                    fieldBuilder.Append(TimeSpan.FromMilliseconds(oLine.track.duration_ms).ToString(@"mm\:ss"));
                    break;
                case PlaylistColumn.AddedAt:
                    fieldBuilder.Append(oLine.added_at);
                    break;
                case PlaylistColumn.AddedBy:
                    fieldBuilder.Append(oLine.added_by.display_name ?? oLine.added_by.id);
                    break;
                case PlaylistColumn.IsLocal:
                    fieldBuilder.Append(oLine.is_local);
                    break;
                case PlaylistColumn.TrackID:
                    fieldBuilder.Append(oLine.track.id);
                    break;
            }
            fieldBuilder.Append('\"');
            return fieldBuilder.ToString();
        }
        
    }
}
