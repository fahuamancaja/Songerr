using Songerr.Models;
using System.Text.RegularExpressions;

namespace Songerr.Services
{
    public class ParserService : IParserService
    {
        public void ParseVideoUrl(SongModel playlistModel)
        {
            // Updated regex to match m.youtube.com and optional parameters
            var regex = new Regex(@"(?:youtu\.be\/|youtube\.com\/watch\?v=|m\.youtube\.com\/watch\?v=)([^&?]+)(?:.*)?", RegexOptions.IgnoreCase);

            // Match the URL against the regex
            var match = regex.Match(playlistModel.Id);

            // If a match is found, return the video ID; otherwise, return null
            if (match.Success)
            {
                playlistModel.Id = match.Groups[1].Value;
            }
        }
        public string RemoveBracesAndTrailingSpaces(string input)
        {
            string result = Regex.Replace(input, "\\[[^\\]]*\\]", string.Empty);
            result = result.Trim();

            return result;
        }
        public string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9 ]", "");
        }
        public string MoveFileToCorrectLocation(SongModel playListModel)
        {
            string titleName = RemoveBracesAndTrailingSpaces(Path.GetFileName(playListModel.Filepath));
            string rootDirectoryPath = @"E:\Music";

            string newFileName = Path.ChangeExtension(titleName, ".mp3");
            string newFilePath = Path.Combine(rootDirectoryPath, newFileName);

            // Copy the file to the new location, overwriting if it already exists
            File.Copy(playListModel.Filepath, newFilePath, true);

            // Delete the original file
            File.Delete(playListModel.Filepath);

            return newFilePath;
        }
    }
}
