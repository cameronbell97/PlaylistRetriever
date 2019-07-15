using System.Collections.Generic;

namespace SpotifyAPI
{
    public class SpotifyPlaylist
    {
        #region Declarations
        public bool collaborative { get; set; }
        public string description { get; set; }
        public Dictionary<string, string> external_urls { get; set; }
        public SpotifyFollowers followers { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public SpotifyImage[] images { get; set; }
        public string name { get; set; }
        public SpotifyUser owner { get; set; }
        public bool? is_public { get; set; } // This field is actually called public so idk what to do with it
        public string snapshot_id { get; set; }
        public SpotifyPaging<SpotifyPlaylistTrack> tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
        #endregion

        #region Constructors
        public SpotifyPlaylist()
        {

        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion
    }
}