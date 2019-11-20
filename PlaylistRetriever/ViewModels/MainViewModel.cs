using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PlaylistRetriever.Models;
using PlaylistRetriever.Services;
using SpotifyAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace PlaylistRetriever.ViewModels
{
    // TODO : Comment functions properly
    /// <summary>
    /// ViewModel class for Main Window
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        // Declarations //
        private const string DEFAULT_FILE_EXT = "csv";
        private const string SPOTIFY_SCOPES = "user-library-read playlist-read-private playlist-read-collaborative";
        // Constructors //
        public MainViewModel()
        {
            // Init Commands
            LogInToSpotifyCommand = new RelayCommand(LogInToSpotify);

            UriRegistrationService.RegisterUriScheme();
        }
        // Properties //

        private SpotifyClient SpotifyClient { get; set; } = new SpotifyClient();


        // Relay Command Properties //

        public RelayCommand<Window> CloseCommand { get; private set; } = new RelayCommand<Window>(Close);
        public RelayCommand LogInToSpotifyCommand { get; private set; }


        // Methods //

        internal void LogInToSpotify()
        {
            // TODO : Implement
            SpotifyClient = new SpotifyClient();
            SpotifyClient.AccessCredentials.ClientID = "c5a6957e8a78401e9aff6e7cc9922866"; // TODO : REMOVE URGENTLY

            SpotifyClient.OpenLogIn();
        }


        // Static Methods //

        private static void Close(Window window)
        {
            if (window != null)
                window.Close();
        }
    }
}
