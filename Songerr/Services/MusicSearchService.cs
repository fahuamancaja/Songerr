using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Newtonsoft.Json;
using RestSharp;
using Songerr.Models;
using System.Net;

namespace Songerr.Services
{
    public class MusicSearchService : IMusicSearchService
    {
        private readonly string _spotifyApiKey = "<Your Spotify API Key>";
        private readonly string _deezerApiKey = "<Your Deezer API Key>";
        private readonly string _clientId;
        private readonly string _clientSecret;

        public MusicSearchService(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<SpotifyResults> GetSongInfoAsync(string songName)
        {
            // Try Spotify API first
            var spotifyResult = await SearchSpotifyAsync(songName);
            //if (spotifyResult != null)
            //{
                return spotifyResult;
            //}

            //// If Spotify didn't find anything, try Deezer API
            //var deezerResult = await SearchDeezerAsync(songName);
            //return deezerResult;
        }

        private async Task<SpotifyResults> SearchSpotifyAsync(string songName)
        {
            var accessToken = await GetSpotifyAccessTokenAsync();

            if (accessToken == null)
            {
                throw new Exception("Failed to retrieve access token.");
            }

            var client = new RestClient("https://api.spotify.com/v1/search?q=" + songName + "&type=track");
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            var response = await client.GetAsync(request);
            if (response.IsSuccessful)
            {
                var spotifySong = JsonConvert.DeserializeObject<SpotifyResults>(response.Content);
                var songTrack = spotifySong.tracks.items.FirstOrDefault();
                return spotifySong;
            }

            return null;
        }



        private async Task<DeezerSong> SearchDeezerAsync(string songName)
        {
            var client = new RestClient("https://api.deezer.com/search?q=" + songName);
            var request = new RestRequest();
            request.AddHeader("app_id", _deezerApiKey);
            var response = await client.GetAsync(request);
            if (response.IsSuccessful)
            {
                var deezerSong = JsonConvert.DeserializeObject<DeezerSong>(response.Content);
                return deezerSong;
            }

            return null;
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

