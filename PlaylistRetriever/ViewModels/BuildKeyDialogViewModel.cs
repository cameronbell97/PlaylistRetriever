using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PlaylistRetriever.Models;
using SpotifyAPI;
using System.ComponentModel;
using System.Windows;

namespace PlaylistRetriever.ViewModels
{
    // TODO : Complete (OnPropertyChanged, set text boxes to properties)
    class BuildKeyDialogViewModel : ViewModelBase
    {
        // Declarations //
        public event PropertyChangedEventHandler PropertyChanged;
        private string _clientSecret;
        private string _clientID;

        // Constructors //
        public BuildKeyDialogViewModel()
        {
            // Initialize Properties
            Cancelled = true;

            // Initialize Commands
            BuildCommand = new RelayCommand<Window>(BuildKey);
            CancelCommand = new RelayCommand<Window>(Cancel);
        }

        // Properties //
        public bool Cancelled { get; private set; }
        public string ClientID
        {
            get => _clientID;
            set
            {
                _clientID = value;
                RaisePropertyChanged(() => ClientID);
            }
        }
        public string ClientSecret
        {
            get => _clientSecret;
            set
            {
                _clientSecret = value;
                RaisePropertyChanged(() => ClientSecret);
            }
        }
        public ApiKeyResponse Response
        {
            get => !Cancelled ? new ApiKeyResponse
            {
                ApiKey = SpotifyClient.GetEncodedAPIKey(ClientID, ClientSecret)
            } : null;
        }

        // Commands //
        public RelayCommand<Window> BuildCommand { get; private set; }
        public RelayCommand<Window> CancelCommand { get; private set; }


        // Methods //
        private void BuildKey(Window window)
        {
            Cancelled = false;
            if (window != null)
                window.Close();
        }

        private void Cancel(Window window)
        {
            Cancelled = true;
            if (window != null)
                window.Close();
        }
    }
}
