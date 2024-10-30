using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
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
        var plan = await _planService.GetPlan(planId);
        var outSum = plan.Price.ToString("0.00");
        var invId = await GetInvoiceId();
        var pass1 = _merchantInfo.Password1;

        var receipt = GetReceipt(plan.Price, plan.Name, 1);

        var receiptUrlDecoded = GetUrlDecodedReceipt(receipt);

        var shpUserId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var subscriptionId = await _subscriptionService.CreateSubscription(planId, invId.ToString(), promocode);

        const bool shpIsFirst = true;

        var signatureValue = $"{MerchantLogin}:{outSum}:{invId}:{receiptUrlDecoded}:{pass1}:Shp_isFirst={shpIsFirst}:Shp_subscriptionId={subscriptionId}:Shp_userId={shpUserId}";

        var hashedSignature = GetMd5Hash(signatureValue);

        var robokassaInvoiceId = await GetRobokassaInvoiceId(hashedSignature, MerchantLogin, outSum, invId, receiptUrlDecoded, shpUserId, shpIsFirst, subscriptionId);

        return ($"https://auth.robokassa.ru/Merchant/Index/{robokassaInvoiceId}", subscriptionId) ;
    }


    public bool VerifySignature(string signature, string outSum, string invId, string userId, string isFirst, string subscriptionId)
    {
        var pass2 = _merchantInfo.Password2;

        var signatureValue = $"{outSum}:{invId}:{pass2}:Shp_isFirst={isFirst}:Shp_subscriptionId={subscriptionId}:Shp_userId={userId}";

        var hashedSignature = GetMd5Hash(signatureValue);

        return string.Equals(hashedSignature, signature, StringComparison.CurrentCultureIgnoreCase);
    }

    [Queue("chargings")]
    public async Task<bool> ChargeRecurrentPayment(string previousInvoiceId, string planId, string userId)
    {
        const string url = "https://auth.robokassa.ru/Merchant/Recurring";

        var plan = await _planService.GetPlan(planId);

        var outSum = plan.Price.ToString("0.00");

        var pass1 = _merchantInfo.Password1;

        var invId = await GetInvoiceId();

        var subscriptionId = await _subscriptionService.CreateSubscription(planId, invId.ToString());

        var receipt = GetReceipt(plan.Price, plan.Name, 1);

        var receiptUrlDecoded = GetUrlDecodedReceipt(receipt);

        const bool shpIsFirst = false;

        var signatureValue = $"{MerchantLogin}:{outSum}:{invId}:{receiptUrlDecoded}:{pass1}:Shp_isFirst={shpIsFirst}:Shp_subscriptionId={subscriptionId}:Shp_userId={userId}";

        var hashedSignature = GetMd5Hash(signatureValue);

        var formValues = new Dictionary<string, string>
        {
            { "MerchantLogin", MerchantLogin },
            { "OutSum", outSum },
            { "PreviousInvoiceID", previousInvoiceId },
            { "InvId", invId.ToString() },
            { "Receipt", receiptUrlDecoded },
            { "SignatureValue", hashedSignature },
            { "Shp_isFirst", shpIsFirst.ToString() },
            { "Shp_subscriptionId", subscriptionId },
            { "Shp_userId", userId },
        };

        var content = new FormUrlEncodedContent(formValues);

        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode) return false;
        
        var stringContent = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("The repeated payment was made successfully: {c}", stringContent);
        return true;

    }
        
    private async Task<string> GetRobokassaInvoiceId(string signature, string merchantLogin, string outSum, int invId, string receipt, string shpUserId, bool shpIsFirst, string subscriptionId)
    {
        const string url = "https://auth.robokassa.ru/Merchant/Indexjson.aspx";

        var formValues = new Dictionary<string, string>
        {
            { "MerchantLogin", merchantLogin },
            { "OutSum", outSum },
            { "InvId", invId.ToString() },
            { "Receipt", receipt },
            { "SignatureValue", signature },
            { "Recurring", "true" },
            { "Shp_isFirst", shpIsFirst.ToString() },
            { "Shp_subscriptionId", subscriptionId },
            { "Shp_userId", shpUserId },
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
            catch
            {
                // ignored
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

        return checkoutModel.InvoiceId;
    }

    private static Receipt GetReceipt(decimal price, string name, int quantity)
    {
        var items = new List<RobokassaItem> { new(price, name, quantity) };

        return new Receipt(items);
    }

    private static string GetUrlDecodedReceipt(Receipt receipt)
    {
        var jsonString = JsonSerializer.Serialize(receipt);
        var urlEncodedString = HttpUtility.UrlEncode(jsonString);
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

    private static string GetMd5Hash(string input)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(bytes);
        var builder = new StringBuilder();

        foreach (var t in hashBytes)
        {
            builder.Append(t.ToString("x2"));
        }

        return builder.ToString();
    }
}