using System.Text;
using CreativeHubWebApp.Infrastructure;
using CreativeHubWebApp.Repositories;
using CreativeHubWebApp.Services;
using CreativeHubWebApp.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddSingleton<MongoContext>();
builder.Services.AddSingleton<DbInitializer>();

// registracija servisa
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ResourceRepository>();
builder.Services.AddScoped<ResourceService>();
builder.Services.AddScoped<GridFsService>();
builder.Services.AddScoped<SearchRepository>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<CollectionRepository>();
builder.Services.AddScoped<CollectionService>();
builder.Services.AddScoped<StatisticsRepository>();
builder.Services.AddScoped<StatisticsService>();

// podesavanje jwt autentikacije
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// podizanje limita za velicinu uploada
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 200 * 1024 * 1024;  // 200 MB
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 200 * 1024 * 1024;  // 200 MB
});



var app = builder.Build();

// kreiranje indeksa pri startu
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await initializer.InitAsync();
}

// swagger uvek ukljucen zbog testiranja
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();   // mora PRE UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();