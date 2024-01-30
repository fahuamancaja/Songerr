using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using RestSharp;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Songerr.Services
{
    public class SpotifyService : ISpotifyService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;

        public SpotifyService(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<List<string>> GetSongTitlesAndArtistsAsync(string playlistId)
        {
            var accessToken = await GetSpotifyAccessTokenAsync();

            var spotify = new SpotifyClient(accessToken);

            var playlist = await spotify.Playlists.Get(playlistId);

            var songs = new List<string>();

            foreach (var item in playlist.Tracks.Items)
            {
                if (item.Track is FullTrack track)
                {
                    var artists = string.Join(", ", track.Artists.Select(artist => artist.Name).ToArray());

                    songs.Add($"{artists} - {track.Name}");
                }
            }

            return songs;

        }
        private async Task<string> GetSpotifyAccessTokenAsync()
        {
            var client = new RestClient("https://accounts.spotify.com/api/token");
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", _clientId);
            request.AddParameter("client_secret", _clientSecret);

            var response = await client.PostAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
                return jsonResponse?.access_token;
            }

            return null;
        }
    }

}
