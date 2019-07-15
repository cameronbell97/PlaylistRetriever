using System.Collections.Generic;

namespace SpotifyAPI
{
    public class SpotifyAlbumSimple
    {
        public string album_group { get; set; }
        public string album_type { get; set; }
        public SpotifyArtistSimple[] artists { get; set; }
        //public string[] available_markets { get; set; }
        //public Dictionary<string, string> external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public SpotifyImage[] images { get; set; }
        public string name { get; set; }
        public string release_date { get; set; }
        public string release_date_precision { get; set; }
        //public SpotifyRestrictions restrictions { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }
}