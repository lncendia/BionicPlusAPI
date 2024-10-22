using AuthService.Models;
using AuthService.Services.Interfaces;
using DomainObjects.Subscription;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Numerics;
using System.Text;


namespace AuthService.Services.Implementations
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly EndpointsConfig _endpoints;
        private readonly HttpClient _httpClient;

        public SubscriptionService(HttpClient httpClient, IOptions<EndpointsConfig> endpointConfig)
        {
            _endpoints = endpointConfig.Value;
            _httpClient = httpClient;
        }


        public async Task<Usage?> GetUsage(string userId)
        {
            var resp = await _httpClient.GetAsync($"{_endpoints.PaymentServiceUrl}/api/usage/{userId}");
            if (resp.IsSuccessStatusCode)
            {
                var stringContent = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<Usage>(stringContent);
            }
            return null;
        }


        public async Task<Plan?> GetPlan(string planId)
        {
            var resp = await _httpClient.GetAsync($"{_endpoints.PaymentServiceUrl}/api/plans?planId={planId}");
            if (resp.IsSuccessStatusCode)
            {
                var stringContent = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<Plan>(stringContent);
            }
            return null;
        }

        public async Task<Subscription?> GetSubscription(string subscriptionId)
        {
            var resp = await _httpClient.GetAsync($"{_endpoints.PaymentServiceUrl}/api/subscription?id={subscriptionId}");
            if (resp.IsSuccessStatusCode)
            {
                var stringContent = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<Subscription>(stringContent);
            }
            return null;
        }

        public async Task<string?> SetFreeSubscription(string userId)
        {
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{_endpoints.PaymentServiceUrl}/api/subscription?userId={userId}", content);
            if(resp.IsSuccessStatusCode)
            {
                return await resp.Content.ReadAsStringAsync();
            }
            return null;
        }

        public async Task<bool> CancelUserSubscription()
        {
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{_endpoints.PaymentServiceUrl}/api/subscription/cancel", content);
            if (resp.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
    }
}
