using AuthService.Services.Implementations;
using AuthService.Services.Interfaces;
using AuthService.Services.Providers;
using Hellang.Middleware.ProblemDetails;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Text.Json.Serialization;
using AuthService.Extensions;
using AuthService.Infrastructure;
using NLog.Web;
using MailSenderLibrary.Interfaces;
using MailSenderLibrary.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

builder.Services.AddApplicationOptions(builder.Configuration);

builder.Services.AddAspIdentity(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddSwaggerServices(builder.Configuration);

builder.Services.AddCorsServices();

builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddTransient<IExternalManager, ExternalManager>();
builder.Services.AddSingleton<ICaptchaValidator, GoogleCaptchaValidator>();

builder.Services.AddScoped<IEmailService, MailService>();
builder.Services.AddTransient<CurrentRequestBearerTokenProvider>();

builder.Services.AddProblemDetails(ProblemDetailsConfigurator.Configure);

// todo: HttpClient's логируют информацию о запросах как INFO. Нужно ограничить вывод логов.

builder.Services.AddHttpClient<IExternalManager, ExternalManager>();
builder.Services.AddHttpClient<ISubscriptionService, SubscriptionService>()
    .AddHttpMessageHandler<CurrentRequestBearerTokenProvider>();
builder.Services.AddHttpClient<ICaptchaValidator, GoogleCaptchaValidator>();

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();
builder.Logging.ClearProviders();
builder.Host.UseNLog();


var app = builder.Build();

/*
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var appRole = new ApplicationRole { Name = "USER" };
    await roleManager.CreateAsync(appRole);
} */

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("corsapp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();