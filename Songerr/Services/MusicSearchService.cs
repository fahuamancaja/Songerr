using Newtonsoft.Json;
using RestSharp;
using Songerr.Models;
using System.Net;
using System.Text.RegularExpressions;

namespace Songerr.Services
{
    public class MusicSearchService : IMusicSearchService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;

        public MusicSearchService(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        //private async Task<DeezerSong> SearchDeezerAsync(string songName)
        //{
        //    var client = new RestClient("https://api.deezer.com/search?q=" + songName);
        //    var request = new RestRequest();
        //    request.AddHeader("app_id", _deezerApiKey);
        //    var response = await client.GetAsync(request);
        //    if (response.IsSuccessful)
        //    {
        //        var deezerSong = JsonConvert.DeserializeObject<DeezerSong>(response.Content);
        //        return deezerSong;
        //    }

        //    return null;
        //}

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
        public async Task SearchSpotifyMetaData(SongModel songModel)
        {
                var accessToken = await GetSpotifyAccessTokenAsync();

                if (accessToken == null)
                {
                    throw new Exception("Failed to retrieve access token.");
                }
                var client = new RestClient($"https://api.spotify.com/v1/search?q=artist:{songModel.Author}%20track:{songModel.Title}&type=track");

                var request = new RestRequest();
                request.AddHeader("Authorization", $"Bearer {accessToken}");
                var response = await client.GetAsync(request);
                if (response.IsSuccessful)
                {
                    var spotifySong = JsonConvert.DeserializeObject<SpotifyResults>(response.Content);
                    var songTrack = spotifySong.tracks.items.FirstOrDefault();

                    AddAlbumToModel(songTrack, songModel);
                    //MoveFileWithMetaData(songTrack, songModel);
            }
        }

        private static void AddAlbumToModel(Item metaData, SongModel songModel)
        {
            try
            {
                songModel.ALbum = RemoveBracesAndTrailingSpaces(metaData.album.name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        public static string RemoveBracesAndTrailingSpaces(string input)
        {
            string result = Regex.Replace(Regex.Replace(input, @"\([^)]*\)", ""), @"\s+$", "");

            return result;
        }
    }
    
}