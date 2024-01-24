namespace Songerr.Services
{
    using FFMpegCore.Enums;
    using FFMpegCore;
    using Google.Apis.Services;
    using Google.Apis.YouTube.v3;
    using MediaToolkit;
    using MediaToolkit.Model;
    using Newtonsoft.Json.Linq;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using VideoLibrary;
    using YoutubeDLSharp.Options;
    using YoutubeDLSharp;

    public class SongerrService : ISongerrService
    {
        private readonly YouTubeService _youtubeService;

        public SongerrService(string apiKey, string appName)
        {
            _youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = appName
            });
        }

        public async ValueTask<string> DownloadFirstVideoAsMp3(string videoTitle)
        {
            try
            {
                // Search for the video
                var searchListRequest = _youtubeService.Search.List("snippet");
                searchListRequest.Q = videoTitle;
                searchListRequest.Type = "video";
                searchListRequest.MaxResults = 3;

                var searchListResponse = await searchListRequest.ExecuteAsync();

                // Get detailed information about each video
                var videoIds = searchListResponse.Items.Select(item => item.Id.VideoId).ToList();
                var videoDetailsRequest = _youtubeService.Videos.List("statistics");
                videoDetailsRequest.Id = string.Join(",", videoIds);

                var videoDetailsResponse = await videoDetailsRequest.ExecuteAsync();

                // Sort the videos by view count
                var sortedVideos = videoDetailsResponse.Items.OrderByDescending(item => item.Statistics.ViewCount == null ? 0UL : ulong.Parse(item.Statistics.ViewCount.ToString())).ToList();

                // The first video in the sorted list is the most viewed video
                var firstVideoId = sortedVideos[0].Id;

                // Fetch video information from YouTube Data API
                var apiUrl = $"https://www.googleapis.com/youtube/v3/videos?part=snippet&id={firstVideoId}&key={_youtubeService.ApiKey}";
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(apiUrl);
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    JObject parsed = JObject.Parse(jsonResponse);
                    string title = parsed["items"][0]["snippet"]["title"].ToString();
                    string artist = parsed["items"][0]["snippet"]["channelTitle"].ToString();

                    // Replace invalid characters in the directory name
                    artist = new string(artist.Where(c => !Path.GetInvalidPathChars().Contains(c)).ToArray());
                    artist = artist.Replace(' ', '_').Replace('.', '_').Replace('-', '_');

                    // Define the root directory path
                    string rootDirectoryPath = @"C:\Media\Music";

                    // Create a directory named after the channel title under the root directory
                    string directoryPath = Path.Combine(rootDirectoryPath, artist);
                    Directory.CreateDirectory(directoryPath);

                    // Download the audio of the video
                    var youtubeDl = new YoutubeDL();
                    var result = await youtubeDl.RunAudioDownload("https://www.youtube.com/watch?v=" + firstVideoId);

                    // The path of the downloaded audio file
                    string outputMp3Path = result.Data;

                    // Rename the output file
                    string newFileName = $"{title}.mp3";
                    string newFilePath = Path.Combine(directoryPath, newFileName);
                    File.Move(outputMp3Path, newFilePath);

                    return newFilePath;
                }




            }
            catch (Exception ex)
            {
                // Log the exception details here
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }

}
