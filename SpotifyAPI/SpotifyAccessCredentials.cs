using System;
using System.Text;

namespace SpotifyAPI.Models
{
    public class SpotifyAccessCredentials
    {
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string ClientApiKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ClientID))
                    return null;

                if (string.IsNullOrWhiteSpace(ClientSecret))
                    return null;

                return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientID}:{ClientSecret}"));
            }
        }
    }
}