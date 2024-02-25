using Songerr.Models;

namespace Songerr.Services
{
    public interface IParserService
    {
        void ParseVideoUrl(SongModel playlistModel);
        string RemoveBracesAndTrailingSpaces(string input);
        string RemoveSpecialCharacters(string str);
        string MoveFileToCorrectLocation(SongModel playlistModel);
    }
}
