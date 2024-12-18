﻿using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using DomainObjects.Subscription;
using Hangfire;
using Microsoft.Extensions.Options;
using PaymentService.Models.Robokassa;
using PaymentService.Services.Interfaces;
using PaymentService.Services.Robokassa.Interfaces;

namespace PaymentService.Services.Robokassa.Implementations;

public class RobokassaClient : IRobokassaClient
{
    private const string MerchantLogin = "babytips";
    private readonly MerchantInfo _merchantInfo;
    private readonly HttpClient _httpClient;
    private readonly ILogger<RobokassaClient> _logger;
    private readonly IHttpContextAccessor _httpContext;
    private readonly IPlanService _planService;
    private readonly ISubscriptionService _subscriptionService;

    public RobokassaClient(IOptions<MerchantInfo> merchantInfo, IPlanService planService, ISubscriptionService subscriptionService, HttpClient httpClient, ILogger<RobokassaClient> logger, IHttpContextAccessor httpContextAccessor) 
    {
        _merchantInfo = merchantInfo.Value;
        _httpClient = httpClient;
        _logger = logger;
        _httpContext = httpContextAccessor;
        _planService = planService;
        _subscriptionService = subscriptionService;
    }

    public async Task<(string link, string subscriptionId)> GetCheckoutLink(string planId, string? promocode)
    {
        var merchantLogin = MerchantLogin;
        var plan = await _planService.GetPlan(planId);
        var outSum = plan.Price.ToString("0.00");
        var invId = await GetInvoiceId();
        var pass1 = _merchantInfo.Password1;

        var receipt = GetReceipt(plan.Price, plan.Name, 1);

        var receiptUrlDecoded = GetUrlDecodedReceipt(receipt);

        var shp_userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var subscriptionId = await _subscriptionService.CreateSubscription(planId, PaymentServiceType.Robokassa, invId.ToString(), promocode);

        var shp_isFirst = true;

        var signatureValue = $"{merchantLogin}:{outSum}:{invId}:{receiptUrlDecoded}:{pass1}:Shp_isFirst={shp_isFirst}:Shp_subscriptionId={subscriptionId}:Shp_userId={shp_userId}";

        string hashedSignature = GetMD5Hash(signatureValue);

        var robokassaInvoiceId = await GetRobokassaInvoiceId(hashedSignature, merchantLogin, outSum, invId, receiptUrlDecoded, shp_userId, shp_isFirst, subscriptionId);

        return ($"https://auth.robokassa.ru/Merchant/Index/{robokassaInvoiceId}", subscriptionId) ;
    }


    public bool VerifySignature(string signature, string outSum, string invId, string userId, bool isFirst, string subscriptionId)
    {
        var pass2 = _merchantInfo.Password2;

        var signatureValue = $"{outSum}:{invId}:{pass2}:Shp_isFirst={isFirst}:Shp_subscriptionId={subscriptionId}:Shp_userId={userId}";

        var hashedSignature = GetMD5Hash(signatureValue);

        return string.Equals(hashedSignature, signature, StringComparison.CurrentCultureIgnoreCase);
    }

    [Queue("chargings")]
    public async Task<bool> ChargeRecurrentPayment(string previousInvoiceId, string planId, string userId)
    {
        var url = "https://auth.robokassa.ru/Merchant/Recurring";

        var plan = await _planService.GetPlan(planId);

        var outSum = plan.Price.ToString("0.00");

        var pass1 = _merchantInfo.Password1;

        var invId = await GetInvoiceId();

        var subscriptionId = await _subscriptionService.CreateSubscription(planId, PaymentServiceType.Robokassa, invId.ToString());

        var receipt = GetReceipt(plan.Price, plan.Name, 1);

        var receiptUrlDecoded = GetUrlDecodedReceipt(receipt);

        var shp_isFirst = false;

        var signatureValue = $"{MerchantLogin}:{outSum}:{invId}:{receiptUrlDecoded}:{pass1}:Shp_isFirst={shp_isFirst}:Shp_subscriptionId={subscriptionId}:Shp_userId={userId}";

        string hashedSignature = GetMD5Hash(signatureValue);

        var formValues = new Dictionary<string, string>
        {
            { "MerchantLogin", MerchantLogin },
            { "OutSum", outSum },
            { "PreviousInvoiceID", previousInvoiceId },
            { "InvId", invId.ToString() },
            { "Receipt", receiptUrlDecoded },
            { "SignatureValue", hashedSignature },
            { "Shp_isFirst", shp_isFirst.ToString() },
            { "Shp_subscriptionId", subscriptionId },
            { "Shp_userId", userId },
        };

        var content = new FormUrlEncodedContent(formValues);

        var response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var stringContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation(stringContent);
            return true;
        }

        return false;
    }
        
    private async Task<string> GetRobokassaInvoiceId(string signature, string merchantLogin, string outSum, int invId, string receipt, string shp_userId, bool shp_isFirst, string subscriptionId)
    {
        var url = $"https://auth.robokassa.ru/Merchant/Indexjson.aspx";

        var formValues = new Dictionary<string, string>
        {
            { "MerchantLogin", merchantLogin },
            { "OutSum", outSum },
            { "InvId", invId.ToString() },
            { "Receipt", receipt },
            { "SignatureValue", signature },
            { "Recurring", "true" },
            { "Shp_isFirst", shp_isFirst.ToString() },
            { "Shp_subscriptionId", subscriptionId },
            { "Shp_userId", shp_userId },
        };

        var content = new FormUrlEncodedContent(formValues);

        var response = await _httpClient.PostAsync(url, content);

        var checkoutModel = new CheckoutModel();

        if (response.IsSuccessStatusCode)
        {
            var stringContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation(stringContent);
            try
            {
                checkoutModel = JsonSerializer.Deserialize<CheckoutModel>(stringContent);
            }
            catch (Exception ex)
            {

            }
        }

        if(checkoutModel == null)
        {
            //add log, throw error
        }

        if (checkoutModel!.Error != null)
        {
            //add log, throw error
        }

        return checkoutModel!.InvoiceId;
    }

    private Receipt GetReceipt(decimal price, string name, int quantity)
    {
        var items = new List<RobokassaItem> { new RobokassaItem(price, name, quantity) };

        return new Receipt(items);
    }

    private string GetUrlDecodedReceipt(Receipt receipt)
    {
        string jsonString = JsonSerializer.Serialize(receipt);

        string urlEncodedString = HttpUtility.UrlEncode(jsonString);
        return urlEncodedString;
    }

    private async Task<int> GetInvoiceId()
    {
        var rnd = new Random();
        var number = rnd.Next(1, int.MaxValue);

        while(await _subscriptionService.CheckInvoiceExist(number))
        {
            number = rnd.Next(1, int.MaxValue);
        }

        return number;
    }

    private static string GetMD5Hash(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }
}