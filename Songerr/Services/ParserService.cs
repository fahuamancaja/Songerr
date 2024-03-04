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
        
        public async Task MoveFileToCorrectLocationAsync(SongModel songModel)
        {
            //string titleName = RemoveBracesAndTrailingSpaces(Path.GetFileNameWithoutExtension(playListModel.Filepath));
            string fileExtension = Path.GetExtension(songModel.Filepath);
            string titlename = $"{songModel.Author} - {songModel.Title}";
            string rootDirectoryPath = @"E:\Music";
            //string rootDirectoryPath = @"E:\Test";
            string albumArtistPath = $"{songModel.Author}\\{songModel.ALbum}";

            string fullAlbumDirectory = Path.Combine(rootDirectoryPath, albumArtistPath);

            string newFileName = Path.ChangeExtension(titlename, fileExtension);
            string newFilePath = Path.Combine(fullAlbumDirectory, newFileName);
            Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));

            // Copy the file to the new location, overwriting if it already exists
            await Task.Run(() => File.Copy(songModel.Filepath, newFilePath, true));

            // Delete the original file
            await Task.Run(() => File.Delete(songModel.Filepath));

            songModel.Filepath = newFilePath;
        }

        public async Task AddMetaDataToFile(SongModel songModel)
        {
           var file = TagLib.File.Create(songModel.Filepath);

            if (!string.IsNullOrWhiteSpace(songModel.Id))
            {
                file.Tag.Title = songModel.Title;
                file.Tag.Performers = new string[] { songModel.Author };
                file.Tag.Album = songModel.ALbum;
                //file.Tag.Track = 1; // Track number

                file.Save();
            }
        }
    }
}
