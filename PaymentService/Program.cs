using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Hangfire;
using Hangfire.Dashboard;
using HangfireBasicAuthenticationFilter;
using Hellang.Middleware.ProblemDetails;
using IdentityLibrary.Models;
using MailSenderLibrary.Implementations;
using MailSenderLibrary.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using NLog.Web;
using PaymentService.Infrastracture;
using PaymentService.Infrastructure;
using PaymentService.Services.Implementations;
using PaymentService.Services.Interfaces;
using PaymentService.Services.Providers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));


#region authConfig
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
#endregion


var mongoDbIdentityConfig = new MongoDbIdentityConfiguration
{
    MongoDbSettings = new MongoDbSettings
    {
        ConnectionString = builder.Configuration.GetSection("DbSettings").GetValue<string>("ConnectionString"),
        DatabaseName = builder.Configuration.GetSection("DbSettings").GetValue<string>("IdentityName"),
    },
    IdentityOptionsAction = opt =>
    {
        opt.Password.RequireDigit = false;
        opt.Password.RequiredLength = 8;
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequireLowercase = false;
        opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        opt.Lockout.MaxFailedAccessAttempts = 5;
        opt.SignIn.RequireConfirmedEmail = true;
        opt.User.RequireUniqueEmail = true;
    }
};

builder.Services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(mongoDbIdentityConfig)
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddSignInManager<SignInManager<ApplicationUser>>()
    .AddRoleManager<RoleManager<ApplicationRole>>();


builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.ConfigureDbSettings(builder.Configuration.GetSection("DbSettings"));
builder.Services.ConfigurePlansSettings(builder.Configuration.GetSection("PlansConfiguration"));
builder.Services.ConfigureMerchantInfoSettings(builder.Configuration.GetSection("MerchantInfo"));
builder.Services.ConfigureAllowedIps(builder.Configuration.GetSection("AllowedIps"));
builder.Services.ConfigureSmtpServer(builder.Configuration.GetSection("EmailConfiguration"));
builder.Services.ConfigureEndpoints(builder.Configuration.GetSection("EndpointsConfig"));

builder.Services.AddSingleton<MailRecurringService>();
builder.Services.AddSingleton<ChargeRecurringService>();
builder.Services.AddSingleton<UsageRecurringService>();
builder.Services.AddSingleton<IEmailService, MailService>();
builder.Services.AddSingleton<IUsageService, UsageService>();
builder.Services.AddSingleton<IPlanService, PlanService>();
builder.Services.AddSingleton<IRobokassaMailService, RobokassaMailService>();
builder.Services.AddScoped<PaymentProcessorService>();
builder.Services.AddTransient<CurrentRequestBearerTokenProvider>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IRecurrentServiceManager, RecurrentServiceManager>();
                


builder.Services.AddControllers();

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

var connString = builder.Configuration.GetSection("DbSettings:ConnectionString").Value;
builder.Services.ConfigureHangFire(connString);
builder.Services.AddHttpClient<RobokassaService>();
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
app.UseHangfireDashboard("/hangfire/dashboard", new DashboardOptions
{
    //IsReadOnlyFunc = (DashboardContext context) => true,
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter
        {
            User = "admin",
            Pass = "admin"
        },
    },
    DisplayStorageConnectionString = false,
});
app.UseProblemDetails();

app.Run();
