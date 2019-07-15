using System.Collections.Generic;

namespace SpotifyAPI
{
    public class SpotifyArtistSimple
    {
        public Dictionary<string, string> external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }
}