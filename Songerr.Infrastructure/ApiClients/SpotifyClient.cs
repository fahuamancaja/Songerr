using System.Diagnostics.CodeAnalysis;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Songerr.Domain.Interfaces;
using Songerr.Domain.Models;
using Songerr.Domain.Models.OptionSettings;

namespace Songerr.Infrastructure.ApiClients;

[ExcludeFromCodeCoverage]
public class SpotifyClient(IOptions<SpotifySettings> settings) : ISpotifyClientSearch
{
    private readonly SpotifySettings _settings = settings.Value;

    public async Task<string?> GetSpotifyAccessTokenAsync()
    {
        Log.Information("Attempting to get Spotify access token");

        var response = await "https://accounts.spotify.com/api/token"
            .WithHeader("Content-Type", "application/x-www-form-urlencoded")
            .PostUrlEncodedAsync(new
            {
                grant_type = "client_credentials",
                client_id = _settings.ClientId,
                client_secret = _settings.ClientSecret
            });

        if (!response.ResponseMessage.IsSuccessStatusCode) return null;
        var jsonResponse = JsonConvert.DeserializeObject<dynamic>(await response.GetStringAsync());
        return jsonResponse?.access_token;
    }

    public async Task<SpotifyResults?> GetSpotifyMetaData(SongModel? songModel, string accessToken)
    {
        Log.Information($"Attempting to get Spotify metadata for song: {songModel?.Title} by {songModel?.Author}");

        var request = new FlurlRequest("https://api.spotify.com/v1/search")
            .SetQueryParam("q", $"artist:{songModel?.Author} track:{songModel?.Title}")
            .SetQueryParam("type", "track")
            .WithHeader("Authorization", $"Bearer {accessToken}");

        var response = await request.GetAsync();

        // Check if the response is successful
        if (response.StatusCode == 200) return await response.GetJsonAsync<SpotifyResults>();

        // Log the error and return null
        Log.Error($"Failed to fetch Spotify metadata. Status code: {response.StatusCode}");

        return null;
    }
}