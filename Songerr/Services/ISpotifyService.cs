namespace Songerr.Services
{
    public interface ISpotifyService
    {
        Task<List<string>> GetSongTitlesAndArtistsAsync(string playlistId);
    }
}
