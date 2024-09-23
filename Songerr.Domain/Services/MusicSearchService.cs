using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using Songerr.Domain.Models;
using Songerr.Infrastructure.Interfaces;
using Songerr.Infrastructure.Models;
using Songerr.Infrastructure.OptionSettings;
using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Domain.Services;

public class MusicSearchService(ISpotifyClientSearch spotifyClientSearch) : IMusicSearchService
{
    public async Task SearchSpotifyMetaData(SongModel? songModel)
    {
        var accessToken = await spotifyClientSearch.GetSpotifyAccessTokenAsync() ?? throw new Exception("Failed to retrieve access token.");
        
        var spotifySong = await spotifyClientSearch.GetSpotifyMetaData(songModel, accessToken);

        if (spotifySong?.tracks?.items != null)
            AddAlbumToModel(spotifySong?.tracks?.items.FirstOrDefault()!, songModel);
    }
    
    private static void AddAlbumToModel(Item? metaData, SongModel? songModel)
    {
        try
        {
            if (songModel != null) songModel.Album = RemoveBracesAndTrailingSpacesAndSpecialChars(metaData?.album?.name);
        }
        catch (Exception ex)
        {
            Log.Error($"Error: {ex.Message}");
        }
    }

    private static string? RemoveBracesAndTrailingSpacesAndSpecialChars(string? input)
    {
        // First, remove parentheses, square brackets, and curly braces along with their content
        if (input != null)
        {
            var result = Regex.Replace(Regex.Replace(Regex.Replace(input, @"\([^)]*\)", ""), @"\[[^\]]*\]", ""),
                @"\{[^}]*\}", "");

            // Then, remove any special characters except spaces, underscores, and hyphens
            result = Regex.Replace(result, @"[^0-9A-Za-z _-]", "");

            // Finally, remove trailing spaces
            result = Regex.Replace(result, @"\s+$", "");

            return result;
        }

        return input;
    }
}