namespace SpotifyAPI
{
    public class SpotifyPlaylistTrack
    {
        public string added_at { get; set; }
        public SpotifyUser added_by { get; set; }
        public bool is_local { get; set; }
        public SpotifyTrack track { get; set; }
    }
}