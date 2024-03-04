namespace Songerr.Services
{
    public interface ISpotifyService
    {
        Task<List<string>> SpotiftyPlaylistInformation(string playlistId);
    }
}
