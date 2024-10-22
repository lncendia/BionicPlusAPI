using Amazon.Runtime;
using Amazon.S3;
using BionicPlusAPI.Data.Concrete;
using BionicPlusAPI.Data.Interfaces;
using BionicPlusAPI.Infrastracture;
using BionicPlusAPI.Services.Implementations;
using BionicPlusAPI.Services.Interfaces;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                });
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

builder.Services.AddHttpContextAccessor();

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


builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var services = builder.Services;

services.ConfigureDbSettings(builder.Configuration.GetSection("DbSettings"));
services.ConfigureEndpoints(builder.Configuration.GetSection("EndpointsConfig"));
services.AddSingleton<ICardRepository, CardRepository>();
services.AddSingleton<IAnswerRepository, AnswerRepository>();
services.AddSingleton<ISurveyRepository, SurveyRepository>();
services.AddSingleton<IImageService, ImageService>();
services.AddSingleton<IPregnancy, PregnancyRepository>();
services.AddSingleton<IChildrenService, ChildrenService>();

services.AddHttpClient<IUsageService, UsageService>();

services.AddProblemDetails(ProblemDetailsConfigurator.Configure);

var awsOption = builder.Configuration.GetAWSOptions();
awsOption.Credentials = new BasicAWSCredentials(builder.Configuration["AWS:AccessKey"], builder.Configuration["AWS:SecretKey"]);
services.AddDefaultAWSOptions(awsOption);
services.AddAWSService<IAmazonS3>();
services.ConfigureS3Settings(builder.Configuration.GetSection("S3Settings"));

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

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
app.UseProblemDetails();
app.Run();


