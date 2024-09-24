using System.Text.RegularExpressions;
using Serilog;
using Songerr.Infrastructure.Interfaces;
using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Domain.Services;

public class MusicSearchService(ISpotifyClientSearch spotifyClientSearch) : IMusicSearchService
{
    public async Task SearchSpotifyMetadata(SongModel? songModel)
    {
        var accessToken = await spotifyClientSearch.GetSpotifyAccessTokenAsync() ??
                          throw new Exception("Failed to retrieve access token.");

        var spotifySong = await spotifyClientSearch.GetSpotifyMetaData(songModel, accessToken);

        var firstItem = spotifySong?.tracks?.items?.FirstOrDefault();
        if (firstItem != null) songModel!.Album = CleanAlbumName(firstItem.album?.name);
    }

    private static string? CleanAlbumName(string? input)
    {
        try
        {
            if (string.IsNullOrEmpty(input)) return input;
            // First, remove parentheses, square brackets, and curly braces along with their content

            var result = Regex.Replace(Regex.Replace(Regex.Replace(input, @"\([^)]*\)", ""), @"\[[^\]]*\]", ""),
                @"\{[^}]*\}", "");

            // Then, remove any special characters except spaces, underscores, and hyphens
            result = Regex.Replace(result, @"[^0-9A-Za-z _-]", "");

            // Finally, remove trailing spaces
            result = Regex.Replace(result, @"\s+$", "");

            return result;
        }
        catch (Exception ex)
        {
            Log.Error($"Error cleaning album name: {ex.Message}");
            return null;
        }
    }
}