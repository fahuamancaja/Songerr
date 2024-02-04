namespace Songerr.Services
{
    using Google.Apis.Services;
    using Google.Apis.YouTube.v3;
    using Newtonsoft.Json.Linq;
    using Songerr.Models;
    using System.IO;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using YoutubeDLSharp;

    public class SongerrService : ISongerrService
    {
        private readonly YouTubeService _youtubeService;
        private readonly SongerrSettings _songerrSettings;

        public SongerrService(string apiKey, string appName, SongerrSettings songerrSettings)
        {
            _youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = appName
            });

            _songerrSettings = songerrSettings;
        }

        public async ValueTask<string> DownloadFirstVideoAsMp3(string videoTitle)
        {
            try
            {
                var firstVideoId = await GetFirstVideoId(videoTitle);
                var videoInfo = await GetVideoInfo(firstVideoId);
                var outputMp3Path = await DownloadVideoAsMp3(firstVideoId);
                return MoveFileToCorrectLocation(videoInfo, outputMp3Path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        private async Task<string> GetFirstVideoId(string videoTitle)
        {
            var searchListRequest = _youtubeService.Search.List("snippet");
            searchListRequest.Q = videoTitle;
            searchListRequest.Type = "video";
            searchListRequest.MaxResults = _songerrSettings.MaxResults;


            var searchListResponse = await searchListRequest.ExecuteAsync();
            var videoIds = searchListResponse.Items.Select(item => item.Id.VideoId).ToList();
            var videoDetailsRequest = _youtubeService.Videos.List("statistics");
            videoDetailsRequest.Id = string.Join(",", videoIds);

            var videoDetailsResponse = await videoDetailsRequest.ExecuteAsync();
            var sortedVideos = videoDetailsResponse.Items.OrderByDescending(item => item.Statistics.ViewCount ?? 0).ToList();

            return sortedVideos[0].Id;
        }

        private async Task<(string Title, string Artist)> GetVideoInfo(string videoId)
        {
            var apiUrl = $"https://www.googleapis.com/youtube/v3/videos?part=snippet&id={videoId}&key={_youtubeService.ApiKey}";
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                JObject parsed = JObject.Parse(jsonResponse);
                string title = parsed["items"][0]["snippet"]["title"].ToString();
                string artist = parsed["items"][0]["snippet"]["channelTitle"].ToString();

                artist = new string(artist.Where(c => !Path.GetInvalidPathChars().Contains(c)).ToArray());
                artist = artist.Replace(' ', '_').Replace('.', '_').Replace('-', '_');

                return (title, artist);
            }
        }

        private async Task<string> DownloadVideoAsMp3(string videoId)
        {
            var youtubeDl = new YoutubeDL();
            var result = await youtubeDl.RunAudioDownload($"https://www.youtube.com/watch?v={videoId}");
            return result.Data;
        }

        private string MoveFileToCorrectLocation((string Title, string Artist) videoInfo, string outputMp3Path)
        {
            string titleName = RemoveBracesAndTrailingSpaces(Path.GetFileName(outputMp3Path));
            string rootDirectoryPath = @"E:\Music";

            string newFileName = Path.ChangeExtension(titleName, ".mp3");
            string newFilePath = Path.Combine(rootDirectoryPath, newFileName);

            // Copy the file to the new location, overwriting if it already exists
            File.Copy(outputMp3Path, newFilePath, true);

            // Delete the original file
            File.Delete(outputMp3Path);

            return newFilePath;
        }


        public static string RemoveBracesAndTrailingSpaces(string input)
        {
            string result = Regex.Replace(input, "\\[[^\\]]*\\]", string.Empty);
            result = result.Trim();

            return result;
        }
    }
}
