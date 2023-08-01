using System.Net;
using System.Text;
using System.Text.Json;
using ArispexBlacklistSystemBot.Data;
using ArispexBlacklistSystemBot.Responses;
using Microsoft.Extensions.Configuration;

namespace ArispexBlacklistSystemBot.Services;

public class BlacklistService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public BlacklistService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async ValueTask<bool> TestKeyAsync()
    {
        using var httpClient = new HttpClient();
        var response =
            await httpClient.GetAsync(
                _configuration["ServerBaseUrl"] + "/api/v1/blacklist?key=" + _configuration["Key"]);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            return true;
        }

        return false;
    }

    public async ValueTask<AddBlacklistEntryResponse> AddBlacklistEntryAsync(string qq, string reason, string note)
    {
        using var httpClient = new HttpClient();
        var data = new
        {
            key = _configuration["Key"],
            qq = qq,
            reason = reason,
            note = note
        };
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(_configuration["ServerBaseUrl"] + "/api/v1/blacklist", content);
        return await JsonSerializer.DeserializeAsync<AddBlacklistEntryResponse>(
            await response.Content.ReadAsStreamAsync());
    }

    public async ValueTask<DeleteBlacklistEntryResponse> DeleteBlacklistEntryAsync(string qq)
    {
        using var httpClient = new HttpClient();
        var data = new
        {
            key = _configuration["Key"]
        };
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var httpRequest = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(_configuration["ServerBaseUrl"] + "/api/v1/blacklist/" + qq),
            Content = content
        };
        var response = await httpClient.SendAsync(httpRequest);
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return new DeleteBlacklistEntryResponse()
            {
                Success = true,
                Msg = "删除成功"
            };
        }

        return await JsonSerializer.DeserializeAsync<DeleteBlacklistEntryResponse>(
            await response.Content.ReadAsStreamAsync());
    }

    public async ValueTask<BlacklistEntryInfoResponse> GetBlacklistEntryInfoAsync(string qq)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(_configuration["ServerBaseUrl"] + "/api/v1/blacklist/" + qq + "?key=" +
                                                 _configuration["Key"]);
        return await JsonSerializer.DeserializeAsync<BlacklistEntryInfoResponse>(
            await response.Content.ReadAsStreamAsync());
    }
}