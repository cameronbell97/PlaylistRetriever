using System.Collections.Generic;

namespace SpotifyAPI
{
    public class SpotifyUser
    {
        public string display_name { get; set; }
        public Dictionary<string, string> external_urls { get; set; }
        public SpotifyFollowers followers { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public SpotifyImage[] images { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }
}