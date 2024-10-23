using BionicPlusAPI.Data.Concrete;
using BionicPlusAPI.Data.Interfaces;
using BionicPlusAPI.Infrastracture;
using BionicPlusAPI.Services.Implementations;
using BionicPlusAPI.Services.Interfaces;
using Hellang.Middleware.ProblemDetails;
using NLog.Web;
using System.Text.Json.Serialization;
using BionicPlusAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationOptions(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddSwaggerServices(builder.Configuration);

builder.Services.AddS3Services(builder.Configuration);

builder.Services.AddCorsServices();

builder.Services.AddSingleton<ICardRepository, CardRepository>();
builder.Services.AddSingleton<IAnswerRepository, AnswerRepository>();
builder.Services.AddSingleton<ISurveyRepository, SurveyRepository>();
builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddSingleton<IPregnancy, PregnancyRepository>();
builder.Services.AddSingleton<IChildrenService, ChildrenService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IUsageService, UsageService>();

builder.Services.AddProblemDetails(ProblemDetailsConfigurator.Configure);

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


