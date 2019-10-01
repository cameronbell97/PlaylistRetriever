﻿using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using PlaylistRetriever.Services;
using SpotifyAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;

namespace PlaylistRetriever
{
    // TODO : Comment functions properly
    /// <summary>
    /// ViewModel class for Main Window
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // Declarations //
        private const string SPOTIFY_PLAYLIST_COLUMNS_TO_RETRIEVE = "collaborative,description,id,name,owner,is_public,tracks(total,limit),type";
        private const string DATA_DIRECTORY = "data";
        private const string KEY_FILENAME = "key.spk";
        private const string REGEX_KEYFILE_FORMAT = @"^\S+$";
        private const string DEFAULT_FILE_EXT = "csv";

        private bool _loadEnabled = true;
        private bool _saveEnabled = false;
        private string _playlistLoadStatus = string.Empty;
        private string _formatString = string.Empty;
        private string _saveLocation = string.Empty;
        private string _spotifyApiAccessKey = string.Empty;

        private List<SpotifyPlaylist> playlists = null;
        private PlaylistWriter playlistWriter = null;
        private SpotifyClient spotifyClient = null;

        public event PropertyChangedEventHandler PropertyChanged;

        // Constructors //
        public MainWindowViewModel()
        {
            PlaylistID = string.Empty;
            SaveLocation = string.Empty;
            FormatString = string.Empty;
            PlaylistLoadStatus = string.Empty;
            SelectedTab = 0;
            playlistWriter = new PlaylistWriter();
            spotifyClient = new SpotifyClient();

            ApiAccessKey = CheckSubDirectoryForAccessKey(System.AppDomain.CurrentDomain.BaseDirectory) ?? string.Empty;
        }

        // Properties //
        public string PlaylistID { get; set; }
        public string SaveLocation
        {
            get { return _saveLocation; }
            set
            {
                _saveLocation = value;
                OnPropertyChanged("SaveLocation");
            }
        }
        public string FormatString
        {
            get { return _formatString; }
            set
            {
                _formatString = value;
                OnPropertyChanged("FormatString");
            }
        }
        internal void FormatColumnsAsDefault()
        {
            PlaylistWriter.PlaylistColumn[] columns =
            {
                PlaylistWriter.PlaylistColumn.TrackName,
                PlaylistWriter.PlaylistColumn.TrackArtists,
                PlaylistWriter.PlaylistColumn.AlbumName
            };

            if (FormatColumns(columns))
            {
                CheckSaveButton();
            }
        }

        public string PlaylistLoadStatus
        {
            get { return _playlistLoadStatus; }
            set
            {
                _playlistLoadStatus = value;
                OnPropertyChanged("PlaylistLoadStatus");
            }
        }
        public bool SaveEnabled
        {
            get { return _saveEnabled; }
            set
            {
                _saveEnabled = value;
                OnPropertyChanged("SaveEnabled");
            }
        }
        public bool LoadEnabled
        {
            get { return _loadEnabled; }
            set
            {
                _loadEnabled = value;
                OnPropertyChanged("LoadEnabled");
            }
        }
        public bool ReloadPlaylistEnabled
        {
            get
            {
                return false;
            }
            set
            {

            }
        }
        public bool DeletePlaylistEnabled
        {
            get
            {
                return false;
            }
            set
            {

            }
        }
        public string ApiAccessKey
        {
            get { return _spotifyApiAccessKey; }
            set
            {
                _spotifyApiAccessKey = value;
                OnPropertyChanged("ApiAccessKey");
            }
        }
        public int SelectedTab { get; set; }
        public string PlaylistName { get; set; }
        public ListView LoadedPlaylistsList { get; set; }
        
        // Public Methods //
        internal void CheckSaveButton()
        {
            SaveEnabled = ValidateSaveRequirements();
        }

        /// <summary>
        /// This function interacts with the SpotifyClient to retrieve a playlist from the given Playlist ID.
        /// </summary>
        /// <returns>A Task for asynchronous operation.</returns>
        internal async Task LoadPlaylistFromIDAsync()
        {
            if (PlaylistID == null || PlaylistID == string.Empty)
            {
                MessageBox.Show("Playlist ID cannot be blank", "Error", MessageBoxButton.OK);
                return;
            }

            Queue<bool> colComponentState = new Queue<bool>();

            try
            {
                // Take the state of controls that shouldn't be touched
                colComponentState.Enqueue(LoadEnabled);
                colComponentState.Enqueue(SaveEnabled);
                LoadEnabled = false;
                SaveEnabled = false;

                // Gets PlaylistID from a URL only if a URL is given, otherwise returned string is identical
                string playlistIDExtraction = SpotifyClient.GetPlaylistIDFromUrl(PlaylistID);

                // Gets the API URL from the playlist ID
                await LoadPlaylistFromUrl(SpotifyClient.GetPlaylistApiUrlFromID(playlistIDExtraction));
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error loading playlist information from Spotify:\r\n{0}", ex.Message), "Error");
                PlaylistLoadStatus = "An error occurred loading playlist";
            }
            finally
            {
                // Reload state of controls
                LoadEnabled = colComponentState.Dequeue();
                SaveEnabled = colComponentState.Dequeue();
                UpdateLoadedPlaylists();
            }
        }

        internal async Task SavePlaylistFromID()
        {
            if (!ValidateSaveRequirements())
                return;
            try
            {
                SaveEnabled = false;

                switch (SelectedTab)
                {
                    case 0: // Case for tab "Retrieving playlist information from public URL"
                        try
                        {
                            if (playlists.Count == 1)
                            {
                                // Get Path to save to
                                string saveLocation = BrowseForFilePath(StripInvalidFileNameChars(PlaylistName, ',')); // TODO : Remove the need for this - ask for save dialog here instead of saving as property

                                // Return Case
                                if (saveLocation == null)
                                    return;

                                if (await PlaylistWriter.SavePlaylistToFile(saveLocation, playlists[0], playlistWriter.ColumnList))
                                {
                                    MessageBox.Show($"Playlist saved to {saveLocation}", "Success", MessageBoxButton.OK, MessageBoxImage.None);
                                }
                                else
                                {
                                    MessageBox.Show(playlistWriter.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else if (playlists.Count > 1)
                            {
                                // Get Path to save to
                                string saveLocation = BrowseForFolderPath(); // TODO : ask for folder save here

                                // Return Case
                                if (saveLocation == null)
                                    return;

                                playlistWriter.Playlists = playlists;
                                if (await playlistWriter.SavePlaylists(saveLocation, DEFAULT_FILE_EXT))
                                {
                                    MessageBox.Show($"Playlists saved to {saveLocation}", "Success", MessageBoxButton.OK, MessageBoxImage.None);
                                }
                                else
                                {
                                    MessageBox.Show(playlistWriter.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                                
                                // TODO : do message box stuff
                            }
                            else
                            {
                                throw new IndexOutOfRangeException("Can not save less than one playlist");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("Error saving playlist information to file:\r\n{0}", ex.Message), "Error");
                        }
                        break;
                }
            }
            finally
            {
                SaveEnabled = true;
            }
        }

        internal bool FormatColumns(PlaylistWriter.PlaylistColumn[] playlistColumnArray)
        {
            if (playlistColumnArray == null)
                return (playlistWriter.ColumnList != null) ? true : false;
            
            playlistWriter.ColumnList = new List<PlaylistWriter.PlaylistColumn>(playlistColumnArray);

            StringBuilder formatStringBuilder = new StringBuilder();
            foreach (PlaylistWriter.PlaylistColumn oCol in playlistColumnArray)
            {
                formatStringBuilder.Append('[');
                formatStringBuilder.Append(oCol.ToString());
                formatStringBuilder.Append(']');
            }
            FormatString = formatStringBuilder.ToString();
            return true;
        }

        internal string BrowseForFilePath(string playlistName)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Title = "Save Playlist";
                saveDialog.Filter = "CSV File(*.csv)|*.csv|All Files(*.*)|*.*";
                saveDialog.InitialDirectory = Environment.CurrentDirectory;
                saveDialog.FilterIndex = 1;
                saveDialog.RestoreDirectory = true;
                saveDialog.FileName = playlistName;

                if (saveDialog.ShowDialog() ?? false)
                {
                    if (!string.IsNullOrWhiteSpace(saveDialog.FileName))
                    {
                        return saveDialog.FileName.Trim();
                    }
                    else
                    {
                        MessageBox.Show("File path can not be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;
                    }
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Format Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            return null;
        }

        internal void OpenFormatWindow()
        {
            FormatWindow formatWindow = new FormatWindow();
            if (formatWindow.ShowDialog() ?? false) // TODO : Fix dialog opening behind main window
            {
                try
                {
                    //PlaylistWriter.PlaylistColumn[] colColumns = ((FormatWindowViewModel)oFormatWindow.DataContext).ReturningColumns.ToArray();
                    PlaylistWriter.PlaylistColumn[] columns = formatWindow.ReturnColumns.ToArray();

                    if (FormatColumns(columns))
                    {
                        CheckSaveButton();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("An error has occurred.{0}{1}", Environment.NewLine, ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                finally
                {
                    // TODO : ?
                }
            }
        }

        public async Task LoadPlaylistFromUrl(string url)
        {
            PlaylistLoadStatus = "Obtaining Spotify Access Token...";

            if (await Task.Run(() => GetPlaylistAsync(url)))
            {
                PlaylistName = playlists[0].name;
                PlaylistLoadStatus = string.Format("Succesfully loaded \"{0}\"", PlaylistName);
                CheckSaveButton();
            }
            else
            {
                MessageBox.Show(String.Format("An error occurred trying to retrieve playlist from {0}\nPlease ensure the playlist ID is correct, and that the playlist is public.", url), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                PlaylistLoadStatus = "Failed to load playlist";
            }
        }

        /// <summary>
        /// Gets the specified playlist from Spotify, and formats it correctly
        /// </summary>
        /// <param name="playlistApiUrl">The API URL of the playlist</param>
        /// <returns>A task that returns a boolean</returns>
        public async Task<bool> GetPlaylistAsync(string playlistApiUrl)
        {
            // Get Spotify API Access Token
            string accessToken = await Task.Run(() => SpotifyClient.GetAccessTokenAsStringAsync(ApiAccessKey));

            spotifyClient = spotifyClient ?? new SpotifyClient(); // If spotifyClient is null, instantiate

            // Load HTTP Info
            spotifyClient.LoadNewHttpInfo(playlistApiUrl, accessToken);

            // Get playlist from Spotify & convert it into Playlist Object
            PlaylistLoadStatus = "Retrieving playlist from Spotify...";
            SpotifyPlaylist playlist = await Task.Run(() => spotifyClient.GetPlaylistFromUrlAsync(playlistApiUrl, SPOTIFY_PLAYLIST_COLUMNS_TO_RETRIEVE));

            // Check for valid response
            if (playlist == null || playlist.tracks == null || playlist.tracks.items == null || playlist.tracks.items.Length <= 0)
                return false;

            // Assign combined tracks to playlist
            playlists = playlists ?? new List<SpotifyPlaylist>();
            playlists.Add(playlist);
            return true;
        }

        public void BuildKeyButtonClick()
        {
            var response = BuildKeyDialogService.ShowBuildKeyDialog();
            if (response != null)
                ApiAccessKey = response.ApiKey;
        }

        // Protected & Private Methods //
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private string BrowseForFolderPath()
        {
            try
            {
                CommonOpenFileDialog browseDialog = new CommonOpenFileDialog();
                browseDialog.IsFolderPicker = true;
                browseDialog.Title = "Save Playlists";
                browseDialog.InitialDirectory = Environment.CurrentDirectory;
                browseDialog.RestoreDirectory = true;

                if (browseDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    if (!string.IsNullOrWhiteSpace(browseDialog.FileName))
                    {
                        return browseDialog.FileName.Trim();
                    }
                    else
                    {
                        MessageBox.Show("Folder path can not be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            return null;


            //FolderBrowserDialog browseDialog = new FolderBrowserDialog();

            //if (browseDialog.ShowDialog() == DialogResult.OK)
            //{
            //    if (!string.IsNullOrWhiteSpace(browseDialog.SelectedPath))
            //    {
            //        return browseDialog.SelectedPath.Trim();
            //    }
            //    else
            //    {
            //        System.Windows.MessageBox.Show("File path can not be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //        return null;
            //    }
            //}
            //else
            //{
            //    return null;
            //}
        }

        private static string StripInvalidFileNameChars(string playlistName, char? charToReplaceWith = null)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();

            foreach (char invalidChar in invalidChars)
                playlistName = playlistName.Replace(invalidChar, charToReplaceWith ?? ' ');

            return playlistName;
        }

        private void UpdateLoadedPlaylists()
        {
            if (LoadedPlaylistsList == null)
                return;

            ItemCollection playlistList = LoadedPlaylistsList.Items;

            if (playlistList == null || playlists == null)
                return;

            playlistList.Clear();
            foreach (SpotifyPlaylist playlist in playlists)
            {
                playlistList.Add(playlist.name);
            }
        }

        private bool ValidateSaveRequirements()
        {
            //if (tabFromPlaylistID.IsSelected)
            switch (SelectedTab)
            {
                case 0: // Case for tab "Retrieving playlist information from public URL"
                    if (playlists == null || playlists.Count <= 0)
                        return false;
                    if (playlistWriter.ColumnList == null)
                        return false;

                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks the provided directory (usually executing directory) for a subdirectory, and checks for a file with a Spotify access key inside that
        /// </summary>
        /// <param name="baseDirectory">The executing directory</param>
        /// <returns>A Spotify access key or an empty string</returns>
        private string CheckSubDirectoryForAccessKey(string baseDirectory)
        {
            try
            {
                string dataFilePath = EnsureSubdirectoryDataExistence(baseDirectory);
                if (dataFilePath != null)
                {
                    string fileText;
                    fileText = File.ReadAllText(dataFilePath).Trim();
                    if (Regex.IsMatch(fileText, REGEX_KEYFILE_FORMAT))
                        return fileText;
                    else
                        return string.Empty;
                }
                else
                    return string.Empty;
            }
            catch
            {
                return null;
            }
        }

        private bool SaveAccessKeyToSubDirectory(string baseDirectory)
        {
            string keyFilePath = EnsureSubdirectoryDataExistence(baseDirectory);

            if (keyFilePath == null)
                return false;

            // TODO : save key to file
            try
            {
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string EnsureSubdirectoryDataExistence(string baseDirectory)
        {
            try
            {
                // Return Case
                if (!Directory.Exists($"{baseDirectory}"))
                    return null;

                // Create if nonexistant
                string dir = $"{baseDirectory}\\{DATA_DIRECTORY}";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                string path = $"{dir}\\{KEY_FILENAME}";
                if (!File.Exists(path))
                    File.Create(path);

                return path;
            }
            catch
            {
                return null;
            }
        }
    }
}
