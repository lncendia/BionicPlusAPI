using BionicPlusAPI.Models.Settings;
using BionicPlusAPI.Services.Interfaces;
using DomainObjects.Subscription;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace BionicPlusAPI.Services.Implementations
{
    public class UsageService : IUsageService
    {
        private readonly HttpClient _httpClient;
        private readonly EndpointsConfig _endpoints;

        public UsageService(HttpClient httpClient, IOptions<EndpointsConfig> endpointConfig)
        {
            _httpClient = httpClient;
            _endpoints = endpointConfig.Value;
        }

        public async Task<Usage> DecrementUsageAsync(LimitKind limitKind, string userId)
        {
            var decrementUsage = new DecrementUsage { LimitKind = limitKind, UserId = userId };

            var content = new StringContent(JsonSerializer.Serialize(decrementUsage), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{_endpoints.PaymentServiceUrl}/api/usage/decrement", content);
            if (resp.IsSuccessStatusCode)
            {
                var usage = await resp.Content.ReadFromJsonAsync<Usage>();
                if (usage != null)
                {
                    return usage;
                }
                else
                {
                    throw new ArgumentException($"Can't decrement usage for userId {userId}");
                }
            }
            else
            {
                throw new ArgumentException($"Can't decrement usage for userId {userId} reason: {resp.StatusCode} {await resp.Content.ReadAsStringAsync()}");
            }
        }

        public async Task<Usage> GetUsageAsync(string userId)
        {
            var resp = await _httpClient.GetAsync($"{_endpoints.PaymentServiceUrl}/api/usage/{userId}");
            if (resp.IsSuccessStatusCode)
            {
                var usage = await resp.Content.ReadFromJsonAsync<Usage>();
                if(usage != null)
                {
                    return usage;
                }
                else
                {
                    throw new ArgumentException($"Can't get usage for userId {userId}");
                }
            }
            else
            {
                throw new ArgumentException($"Can't get usage for userId {userId} reason: {resp.StatusCode} {await resp.Content.ReadAsStringAsync()}");
            }
        }

    }
}
