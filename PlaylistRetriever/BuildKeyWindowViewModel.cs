using SpotifyAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistRetriever
{
    // TODO : Complete (OnPropertyChanged, set text boxes to properties)
    class BuildKeyWindowViewModel : INotifyPropertyChanged
    {
        // Declarations //
        public event PropertyChangedEventHandler PropertyChanged;

        // Constructors //
        public BuildKeyWindowViewModel()
        {
            ApiKey = null;
        }

        // Properties //
        public string ApiKey { get; private set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }


        // Public Methods //
        public void BuildKey()
        {
            ApiKey = SpotifyClient.GetEncodedAPIKey(ClientID, ClientSecret);
        }

        // Protected & Private Methods //
        // TODO : Implement this
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
