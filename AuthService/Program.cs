using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AuthService.Infrastracture;
using AuthService.Infrastructure;
using AuthService.Models;
using AuthService.Services.Implementations;
using AuthService.Services.Interfaces;
using AuthService.Services.Providers;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Text;
using System.Text.Json.Serialization;
using NLog.Web;
using MailSenderLibrary.Interfaces;
using MailSenderLibrary.Implementations;
using IdentityLibrary.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

builder.Services.ConfigureDbSettings(builder.Configuration.GetSection("DbSettings"));
builder.Services.ConfigureJwtToken(builder.Configuration.GetSection("JwtConfig"));
builder.Services.ConfigureSmtpServer(builder.Configuration.GetSection("EmailConfiguration"));
builder.Services.ConfigureEndpoints(builder.Configuration.GetSection("EndpointsConfig"));
builder.Services.ConfigureSettingsConfig(builder.Configuration.GetSection("SettingsConfig"));
builder.Services.ConfigurePaymentConfig(builder.Configuration.GetSection("PaymentConfig"));

var t = builder.Configuration.GetSection("DbSettings").GetValue<string>("ConnectionString");
var mongoDbIdentityConfig = new MongoDbIdentityConfiguration
{
    MongoDbSettings = new MongoDbSettings
    {
        ConnectionString = builder.Configuration.GetSection("DbSettings").GetValue<string>("ConnectionString"),
        DatabaseName = builder.Configuration.GetSection("DbSettings").GetValue<string>("Name"),
    },
    IdentityOptionsAction = opt =>
    {
        opt.Password.RequireDigit = false;
        opt.Password.RequiredLength = 8;
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequireLowercase = false;
        opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        opt.Lockout.MaxFailedAccessAttempts = 5;
        opt.Tokens.EmailConfirmationTokenProvider = DigitTokenProvider.FourDigitEmail;
        opt.Tokens.PasswordResetTokenProvider = DigitTokenProvider.FourDigitEmail;
        opt.SignIn.RequireConfirmedEmail = true;
        opt.User.RequireUniqueEmail = true;
    }
};

builder.Services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(mongoDbIdentityConfig)
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddSignInManager<SignInManager<ApplicationUser>>()
    .AddRoleManager<RoleManager<ApplicationRole>>()
    .AddDefaultTokenProviders()
    .AddTokenProvider<DigitTokenProvider>(DigitTokenProvider.FourDigitEmail);

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidIssuer = builder.Configuration.GetSection("JwtConfig").GetValue<string>("ValidIssuer"),
    ValidAudiences = builder.Configuration.GetSection("JwtConfig").GetValue<string>("ValidAudiences").Split(","),
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtConfig").GetValue<string>("IssuerSigningKey"))),
    ClockSkew = TimeSpan.Zero,
};

builder.Services.AddAuthentication(x => 
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(t =>
{
    t.RequireHttpsMetadata = false;
    t.SaveToken = true;
    t.TokenValidationParameters = tokenValidationParameters;
});

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddScoped<IEmailService, MailService>();
builder.Services.AddTransient<CurrentRequestBearerTokenProvider>();
builder.Services.AddSingleton<ICaptchaValidator, CaptchaValidator>();

builder.Services.AddProblemDetails(ProblemDetailsConfigurator.Configure);

builder.Services.AddHttpClient<ISubscriptionService, SubscriptionService>()
                .AddHttpMessageHandler<CurrentRequestBearerTokenProvider>();

builder.Services.AddControllers().AddJsonOptions(options =>
 options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddSingleton(tokenValidationParameters);
builder.Services.AddHttpContextAccessor();
builder.Logging.ClearProviders();
builder.Host.UseNLog();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors("corsapp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
