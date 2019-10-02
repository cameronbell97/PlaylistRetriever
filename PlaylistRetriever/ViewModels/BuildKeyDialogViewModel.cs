using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PlaylistRetriever.Models;
using SpotifyAPI;
using System.ComponentModel;
using System.Windows;

namespace PlaylistRetriever.ViewModels
{
    // TODO : Complete (OnPropertyChanged, set text boxes to properties)
    public class BuildKeyDialogViewModel : ViewModelBase
    {
        // Declarations //
        private string _clientSecret;
        private string _clientID;
        private string _clientSecretSaveState;
        private string _clientIDSaveState;

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
            get => new ApiKeyResponse
            {
                ApiKey = SpotifyClient.GetEncodedAPIKey(ClientID, ClientSecret),
                DialogResult = Cancelled ? DialogResultAction.Cancel : DialogResultAction.Submit
            };
        }

        // Commands //
        public RelayCommand<Window> BuildCommand { get; private set; }
        public RelayCommand<Window> CancelCommand { get; private set; }


        // Methods //
        private void BuildKey(Window window)
        {
            _clientIDSaveState = ClientID;
            _clientSecretSaveState = ClientSecret;
            Cancelled = false;
            if (window != null)
                window.Close();
        }

        private void Cancel(Window window)
        {
            ClientID = _clientIDSaveState;
            ClientSecret = _clientSecretSaveState;
            Cancelled = true;
            if (window != null)
                window.Close();
        }
    }
}
