using System.Web;
using AuthService.Configuration;
using AuthService.Dtos;
using AuthService.Infrastructure;
using AuthService.Models;
using AuthService.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AuthService.Services.Implementations;

public class CaptchaValidator: ICaptchaValidator
{
    private const string BaseUrl = "https://www.google.com/recaptcha/api/siteverify";
    private readonly SettingsConfig _settings;
    private readonly HttpClient _httpClient;
    
    public CaptchaValidator(IOptions<SettingsConfig> settingsConfig, HttpClient httpClient)
    {
        _settings = settingsConfig.Value;
        _httpClient = httpClient;
    }
    
    public async Task<bool> Validate(string captchaToken)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["secret"] = _settings.CaptchaSecret;
            query["response"] = captchaToken;
            
            var queryString = query.ToString();
            
            var response = await _httpClient.PostAsync($"{BaseUrl}?{queryString}", null);
            
            var responseBody = await response.Content.ReadAsStringAsync();
            var recaptchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(responseBody);
            
            return recaptchaResponse!.Success; 
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
}