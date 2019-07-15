using System.Collections.Generic;

namespace SpotifyAPI
{
    public class SpotifyTrack
    {
        // https://developer.spotify.com/documentation/web-api/reference/object-model/#track-object-full

        public SpotifyAlbumSimple album { get; set; }
        public SpotifyArtistSimple[] artists { get; set; }
        //public string[] available_markets { get; set; }
        //public int disc_number { get; set; }
        public int duration_ms { get; set; }
        //public bool explicit { get; set; }
        //public Dictionary<string, string> external_ids { get; set; }
        //public Dictionary<string, string> external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public bool is_playable { get; set; }
        //public SpotifyLinkedTrack linked_from { get; set; }
        //public SpotifyRestrictions restrictions { get; set; }
        public string name { get; set; }
        public int popularity { get; set; }
        public string preview_url { get; set; }
        public int track_number { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
        public bool is_local { get; set; }
    }
}