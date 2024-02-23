namespace Songerr.Services
{
    public interface IPlaylistRetriever
    {
        Task<List<string>> GetPlaylistTitlesAsync(string playlistId);
        Task<List<string>> DownloadPlaylistSongs(string playlistId);
    }
}
