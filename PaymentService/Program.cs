using Hellang.Middleware.ProblemDetails;
using MailSenderLibrary.Implementations;
using MailSenderLibrary.Interfaces;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using NLog.Web;
using PaymentService.Services.Implementations;
using PaymentService.Services.Interfaces;
using PaymentService.Services.Providers;
using PaymentService.Extensions;
using PaymentService.Infrastructure;
using PaymentService.Models.GooglePlayBilling;
using PaymentService.Models.Robokassa;
using PaymentService.Services.GooglePlayBilling;
using PaymentService.Services.Robokassa.Implementations;
using PaymentService.Services.Robokassa.Implementations.Recurring;
using PaymentService.Services.Robokassa.Interfaces;

BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationOptions(builder.Configuration);

builder.Services.AddAspIdentity(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddSwaggerServices(builder.Configuration);

builder.Services.AddHangfireServices(builder.Configuration);

builder.Services.AddCorsServices();

builder.Services.AddSingleton<MailRecurringService>();
builder.Services.AddSingleton<ChargeRecurringService>();
builder.Services.AddSingleton<UsageRecurringService>();
builder.Services.AddSingleton<IEmailService, MailService>();
builder.Services.AddSingleton<IUsageService, UsageService>();
builder.Services.AddSingleton<IPlanService, PlanService>();
builder.Services.AddSingleton<IPaymentMailService, PaymentMailService>();
builder.Services.AddScoped<IPaymentProcessor<RobokassaCallback>, RobokassaPaymentProcessor>();
builder.Services.AddScoped<IPaymentProcessor<GoogleCallback>, GooglePlayBillingProcessor>();
builder.Services.AddScoped<IRobokassaClient, RobokassaClient>();
builder.Services.AddTransient<CurrentRequestBearerTokenProvider>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient<RobokassaClient>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails(ProblemDetailsConfigurator.Configure);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("corsapp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.CreateHangfireDashboard(builder.Configuration);
app.RunCancellationSubscriptionsJob();

app.UseProblemDetails();

app.Run();
