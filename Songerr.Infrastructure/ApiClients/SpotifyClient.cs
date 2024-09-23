using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
using Songerr.Infrastructure.Interfaces;
using Songerr.Infrastructure.Models;
using Songerr.Infrastructure.OptionSettings;
using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Infrastructure.ApiClients;

public class SpotifyClient(IOptions<SpotifySettings> settings) : ISpotifyClientSearch
{
    private readonly SpotifySettings _settings = settings.Value;
    
    public async Task<string> GetSpotifyAccessTokenAsync()
    {
        var client = new RestClient("https://accounts.spotify.com/api/token");
        var request = new RestRequest();
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("grant_type", "client_credentials");
        request.AddParameter("client_id", _settings.ClientId);
        request.AddParameter("client_secret", _settings.ClientSecret);

        var response = await client.PostAsync(request);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
            return jsonResponse?.access_token;
        }

        return null;
    }

    public async Task<SpotifyResults?> GetSpotifyMetaData(SongModel? songModel, string accessToken)
    {
        var url = "https://api.spotify.com/v1/search";
        var request = new FlurlRequest(url)
            .SetQueryParam("q", $"artist:{songModel.Author} track:{songModel.Title}")
            .SetQueryParam("type", "track")
            .WithHeader("Authorization", $"Bearer {accessToken}");

        var response = await request.GetAsync();
        
        // Check if the response is successful
        if (response.StatusCode == 200)
        {
            return await response.GetJsonAsync<SpotifyResults>();
        }
        
        // Log the error and return null
        Log.Error($"Failed to fetch Spotify metadata. Status code: {response.StatusCode}");
        
        return null;
    }
}