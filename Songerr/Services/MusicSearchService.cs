using Newtonsoft.Json;
using RestSharp;
using Songerr.Models;
using System.Net;

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

        public async Task<string> GetSongInfoAsync()
        {
            // Try Spotify API first
            var spotifyResult = await SearchSpotifyAsync();
            //if (spotifyResult != null)
            //{
            return spotifyResult;
            //}

            //// If Spotify didn't find anything, try Deezer API
            //var deezerResult = await SearchDeezerAsync(songName);
            //return deezerResult;
        }

        private async Task<string> SearchSpotifyAsync()
        {
            string[] fileDetails = Directory.GetFiles(@"E:\Music", "*.*", SearchOption.AllDirectories);

            var accessToken = await GetSpotifyAccessTokenAsync();

            if (accessToken == null)
            {
                throw new Exception("Failed to retrieve access token.");
            }
            
            var resultList = new List<string>();
            foreach (string path in fileDetails)
            {
                var result = await SearchSpotifyStructure(path);
                resultList.Add(result);
            }

            var results = CalculateCompletionRate(resultList);

            return results;
        }

        private static string CalculateCompletionRate(List<string> list)
        {
            int totalCount = list.Count;
            int completeCount = list.Count(x => x.ToLower() == "complete"); // Assuming a non-empty string is considered complete
            double completionRate = (double)completeCount / totalCount;
            return $"{completeCount}/{totalCount} completed";
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

        private static string AddMetaData(Item metaData, string fullPath)
        {
            try
            {
                // Load the file
                var file = TagLib.File.Create(fullPath);

                // Modify metadata
                if (metaData != null)
                {
                    file.Tag.Title = metaData.name;
                    file.Tag.Performers = new[] { metaData.artists[0].name };
                    file.Tag.Album = metaData.album.name;
                    DateTime tempDate;
                    file.Tag.Year = DateTime.TryParse(metaData.album.release_date, out tempDate) ? (uint)tempDate.Year : 0;

                    file.Tag.Track = (uint)metaData.track_number;
                    // Add or modify other fields as needed

                    // Save changes
                    file.Save();
                    string newDirectoryPath = Path.Combine(Path.GetDirectoryName(fullPath), metaData.album.name);

                    if (!Directory.Exists(newDirectoryPath))
                    {
                        Directory.CreateDirectory(newDirectoryPath);
                    }

                    var newFullFilePath = Path.Combine(newDirectoryPath, Path.GetFileName(fullPath));

                    File.Move(fullPath, newFullFilePath);

                    string newFileName = Path.Combine(newDirectoryPath, $"{metaData.artists[0].name} - {metaData.name}.mp3");

                    File.Move(newFullFilePath, newFileName);

                    Console.WriteLine("Metadata updated successfully.");

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return string.Empty;
        }
        public async Task<string> SearchSpotifyStructure(string fullPath)
        {
            if (File.Exists(fullPath))
            {
                var accessToken = await GetSpotifyAccessTokenAsync();

                if (accessToken == null)
                {
                    throw new Exception("Failed to retrieve access token.");
                }

                string fileNameWithoutPath = Path.GetFileNameWithoutExtension(fullPath);

                int dashPosition = fileNameWithoutPath.IndexOf("-");

                // Check if dash exists in the string
                if (dashPosition != -1)
                {
                    string artistName = fileNameWithoutPath.Substring(0, dashPosition).TrimEnd();
                    string trackName = fileNameWithoutPath.Substring(dashPosition + 1).TrimStart();

                    var client = new RestClient($"https://api.spotify.com/v1/search?q=artist:{artistName}%20track:{trackName}&type=track");

                    var request = new RestRequest();
                    request.AddHeader("Authorization", $"Bearer {accessToken}");
                    var response = await client.GetAsync(request);
                    if (response.IsSuccessful)
                    {
                        var spotifySong = JsonConvert.DeserializeObject<SpotifyResults>(response.Content);
                        var songTrack = spotifySong.tracks.items.FirstOrDefault();

                        AddMetaData(songTrack, fullPath);
                    }
                }
                else
                {
                    var client = new RestClient("https://api.spotify.com/v1/search?q=" + fileNameWithoutPath + "&type=track");
                    var request = new RestRequest();
                    request.AddHeader("Authorization", $"Bearer {accessToken}");
                    var response = await client.GetAsync(request);
                    if (response.IsSuccessful)
                    {
                        var spotifySong = JsonConvert.DeserializeObject<SpotifyResults>(response.Content);
                        var songTrack = spotifySong.tracks.items.FirstOrDefault();

                        AddMetaData(songTrack, fullPath);
                    }
                }
                return "complete";
            }
            return "File does not exist";
        }
    }
}