using SpotifyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAPI
{
    public class SpotifyClient
    {
        // Constants //

        private const string SPOTIFY_API_URL = "https://api.spotify.com";

        private const string SPOTIFY_AUTH_URL = "http://accounts.spotify.com/authorize";
        private const string SPOTIFY_AUTH_QS_REDIR_URI = "sptretrieve%3A%2F%2Floginaccept%2F"; // TODO : Change

        private const string DEFAULT_SPOTIFY_AUTH_SCOPES = "user-library-read playlist-read-private playlist-read-collaborative";

        // Constructors //

        public SpotifyClient()
        {
        }


        // Properties //

        public SpotifyAccessCredentials AccessCredentials { get; private set; } = new SpotifyAccessCredentials();


        // Methods //

        public void OpenLogIn(string scopes = DEFAULT_SPOTIFY_AUTH_SCOPES)
        {
            // Return Cases
            if (string.IsNullOrWhiteSpace(scopes))
                return;
            if (string.IsNullOrWhiteSpace(AccessCredentials.ClientID))
                return;

            // Build Auth URL
            StringBuilder authAddressBuilder = new StringBuilder(SPOTIFY_AUTH_URL);
            authAddressBuilder.Append("?");
            authAddressBuilder.Append("response_type=code");
            authAddressBuilder.Append("&");
            authAddressBuilder.Append($"client_id={AccessCredentials.ClientID}");
            authAddressBuilder.Append("&");
            authAddressBuilder.Append($"scope={scopes}");
            authAddressBuilder.Append("&");
            authAddressBuilder.Append($"redirect_uri={SPOTIFY_AUTH_QS_REDIR_URI}");

            // Direct user to Auth site
            System.Diagnostics.Process.Start(authAddressBuilder.ToString());
        }
    }
}
