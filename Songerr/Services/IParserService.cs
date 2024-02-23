using Songerr.Models;

namespace Songerr.Services
{
    public interface IParserService
    {
        void ParseVideoUrl(PlaylistModel playlistModel);
        string RemoveBracesAndTrailingSpaces(string input);
        string RemoveSpecialCharacters(string str);
        string MoveFileToCorrectLocation(PlaylistModel playlistModel);
    }
}
