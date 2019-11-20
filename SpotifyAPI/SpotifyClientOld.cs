using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SpotifyAPI
{
    public class SpotifyClientOld
    {
        // Declarations //
        private const string REGEX_MATCH_PLAYLIST_TOP_URL = @"^https:\/\/open\.spotify\.com(\/user\/(\w|\d)+)?\/playlist\/";
        private const string REGEX_MATCH_PLAYLIST_BOTTOM_URL = @"\?\S+$";
        private const string SPOTIFY_API_URL = "https://api.spotify.com";
        private const string SPOTIFY_API_PLAYLIST_URL = "/v1/playlists";
        private const string SPOTIFY_API_TRACKS_URL = "tracks";

        private HttpClient httpClient = new HttpClient();

        // Constructors //
        public SpotifyClientOld()
        {
        }

        // TODO : Make some of these public methods private (they were only public because you were calling them from the ViewModel)
        // Public Methods //
        /// <summary>
        /// Populates the client with data for accessing Spotify
        /// </summary>
        /// <param name="playlistApiUrl"></param>
        /// <param name="spotifyAccessToken"></param>
        public void LoadNewHttpInfo(string playlistApiUrl, string spotifyAccessToken)
        {
            httpClient = new HttpClient(); // Instantiate new httpClient (can't change existing client's properties after using it)

            // Load information into HTTP Client
            httpClient.BaseAddress = new Uri(playlistApiUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", spotifyAccessToken));
        }

        /// <summary>
        /// Gets a playlist from spotify
        /// </summary>
        /// <param name="playlistApiUrl">The API URL of the playlist</param>
        /// <param name="playlistColumns">The columns of the playlist to retrieve</param>
        /// <returns>An HTTP Response containing the Spotify Playlist (if successful)</returns>
        public async Task<HttpResponseMessage> RetrievePlaylistBasicAsync(string playlistApiUrl, string playlistColumns)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["fields"] = playlistColumns;
            string getPlaylistQuery = query.ToString();
            return await httpClient.GetAsync(string.Format("{0}?{1}", playlistApiUrl, getPlaylistQuery));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playlistApiUrl"></param>
        /// <param name="playlistColumns"></param>
        /// <returns></returns>
        public async Task<SpotifyPlaylist> GetPlaylistFromUrlAsync(string playlistApiUrl, string playlistColumns)
        {
            // Get Playlist minus tracks (used to identify how many tracks are in the playlist)
            HttpResponseMessage response = await Task.Run(() => RetrievePlaylistBasicAsync(playlistApiUrl, playlistColumns));

            if (response.IsSuccessStatusCode)
            {
                // Get playlist metadata (# of tracks)
                SpotifyPlaylist responsePlaylist = await ConvertPlaylistFromHttpResponseAsync(response);

                if (responsePlaylist != null && responsePlaylist.tracks != null)
                {
                    // Get playlist chunks
                    int playlistLength = responsePlaylist.tracks.total;
                    int playlistPageSize = responsePlaylist.tracks.limit;
                    int pagesNeeded = playlistLength % playlistPageSize == 0 ? playlistLength / playlistPageSize : (playlistLength / playlistPageSize) + 1;
                    List<Task<HttpResponseMessage>> playlistChunkTasks = new List<Task<HttpResponseMessage>>();
                    for (int i = 0; i < pagesNeeded; i++)
                    {
                        var chunkQuery = HttpUtility.ParseQueryString(string.Empty);
                        chunkQuery["offset"] = (i * playlistPageSize).ToString(); ;
                        string getPlaylistChunkQuery = chunkQuery.ToString();
                        playlistChunkTasks.Add(httpClient.GetAsync($"{playlistApiUrl}/{SPOTIFY_API_TRACKS_URL}?{getPlaylistChunkQuery}"));
                    }
                    Task.WaitAll(playlistChunkTasks.ToArray());

                    // Convert chunks to a playlist
                    await Task.Run(() =>
                    {
                        List<Task<string>> jsonChunks = new List<Task<string>>();
                        foreach (Task<HttpResponseMessage> oChunkTask in playlistChunkTasks)
                        {
                            jsonChunks.Add(oChunkTask.Result.Content.ReadAsStringAsync());
                        }
                        Task.WaitAll(jsonChunks.ToArray());

                        List<Task<SpotifyPaging<SpotifyPlaylistTrack>>> trackListChunks = new List<Task<SpotifyPaging<SpotifyPlaylistTrack>>>();
                        foreach (Task<string> jsonChunk in jsonChunks)
                        {
                            trackListChunks.Add(ConvertTracksAsync(jsonChunk.Result));
                        }
                        Task.WaitAll(trackListChunks.ToArray());

                        List<SpotifyPlaylistTrack> allTracks = new List<SpotifyPlaylistTrack>();
                        foreach (Task<SpotifyPaging<SpotifyPlaylistTrack>> trackChunkTask in trackListChunks)
                        {
                            allTracks.AddRange(trackChunkTask.Result.items);
                        }

                        responsePlaylist.tracks.items = allTracks.ToArray();
                    });

                    return responsePlaylist;
                }
                return null;
            }
            return null;
        }


        // Static Methods //

        /// <summary>
        /// Gets a Spotify access token as a string from the given encoded Client ID + Secret
        /// Credit to Hendrik Bulens: https://hendrikbulens.com/2015/01/07/c-and-the-spotify-web-api-part-i/
        /// </summary>
        /// <param name="encodedClientIDAndSecret"></param>
        /// <returns>A task for asynchronous operation, which returns a Spotify Access Token as a string</returns>
        public static async Task<string> GetAccessTokenAsStringAsync(string encodedClientIDAndSecret)
        {
            SpotifyToken token = new SpotifyToken();
            string postString = "grant_type=client_credentials";

            byte[] postStringAsBytes = Encoding.UTF8.GetBytes(postString);
            string tokenUrl = "https://accounts.spotify.com/api/token";

            WebRequest request = WebRequest.Create(tokenUrl);
            request.Method = "POST";
            request.Headers.Add("Authorization", $"Basic {encodedClientIDAndSecret}");
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postStringAsBytes.Length;

            using Stream dataStream = request.GetRequestStream();

            dataStream.Write(postStringAsBytes, 0, postStringAsBytes.Length);

            using WebResponse response = await request.GetResponseAsync();
            using Stream responseStream = response.GetResponseStream();
            using StreamReader reader = new StreamReader(responseStream);

            string responseFromServer = reader.ReadToEnd();
            token = JsonConvert.DeserializeObject<SpotifyToken>(responseFromServer);

            return token.access_token;
        }

        /// <summary>
        /// Converts JSON representation of a Spotify playlist into an object
        /// </summary>
        /// <param name="playlistJson">Spotify playlist as JSON</param>
        /// <returns>Playlist object</returns>
        public static async Task<SpotifyPlaylist> ConvertPlaylistFromJsonAsync(string playlistJson) => JsonConvert.DeserializeObject<SpotifyPlaylist>(playlistJson);

        /// <summary>
        /// Converts Spotify REST API response into a Playlist object if possible
        /// </summary>
        /// <param name="response">HTTP response</param>
        /// <returns>Playlist object, or null</returns>
        public static async Task<SpotifyPlaylist> ConvertPlaylistFromHttpResponseAsync(HttpResponseMessage response)
        {
            return response.IsSuccessStatusCode ? await ConvertPlaylistFromJsonAsync(response.Content.ReadAsStringAsync().Result) : null;
        }

        private static async Task<SpotifyPaging<SpotifyPlaylistTrack>> ConvertTracksAsync(string sTracksJson) => JsonConvert.DeserializeObject<SpotifyPaging<SpotifyPlaylistTrack>>(sTracksJson);


        // Pure Methods //

        /// <summary>
        /// Gets the playlist ID from a public URL (not the API url)
        /// </summary>
        /// <param name="publicPlaylistURL">Publicly accessible URL of a Spotify Playlist</param>
        /// <returns>Playlist ID, or the input string if it is not in the correct format.</returns>
        public static string GetPlaylistIDFromUrl(string publicPlaylistURL)
        {
            if (Regex.IsMatch(publicPlaylistURL, REGEX_MATCH_PLAYLIST_TOP_URL))
            {
                publicPlaylistURL = Regex.Replace(publicPlaylistURL, REGEX_MATCH_PLAYLIST_TOP_URL, string.Empty);
                publicPlaylistURL = Regex.Replace(publicPlaylistURL, REGEX_MATCH_PLAYLIST_BOTTOM_URL, string.Empty);
            }
            return publicPlaylistURL;
        }

        public static string GetEncodedAPIKey(string clientID, string clientSecret) => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientID}:{clientSecret}"));

        /// <summary>
        /// Returns a string of the Spotify playlist API URL, generated from a given playlist ID
        /// </summary>
        /// <param name="playlistID">The Playlist ID</param>
        /// <returns>API URL</returns>
        public static string GetPlaylistApiUrlFromID(string playlistID) => $"{SPOTIFY_API_URL}{SPOTIFY_API_PLAYLIST_URL}/{playlistID}";
    }
}
