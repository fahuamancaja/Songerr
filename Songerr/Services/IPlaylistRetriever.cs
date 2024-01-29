namespace Songerr.Services
{
    public interface IPlaylistRetriever
    {
        Task<IEnumerable<string>> GetPlaylistTitlesAsync(string playlistId);
    }
}
