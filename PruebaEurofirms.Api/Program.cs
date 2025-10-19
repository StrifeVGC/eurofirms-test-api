using EurofirmsTest.Infrastructure.MappingProfiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PruebaEurofirms.Domain;
using PruebaEurofirms.Domain.Entities;
using PruebaEurofirms.Domain.Repositories.Contract;
using PruebaEurofirms.Infrastructure.Clients.Implementation;
using PruebaEurofirms.Infrastructure.Handlers;
using PruebaEuroFirms.Domain.Repositories.Implementation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Rick and Morty Info API",
        Version = "v1",
        Description = "This API allows you to query, filter, and manage characters from the Rick and Morty universe.",
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Enter full JWT token including 'Bearer ' prefix",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = null
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<EurofirmsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("EurofirmsSqlite")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<EurofirmsDbContext>()
    .AddDefaultTokenProviders();

var jwtKey = builder.Configuration["JwtKey"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("JWT key not configured.");
}

var keyBytes = Convert.FromBase64String(jwtKey);
var signingKey = new SymmetricSecurityKey(keyBytes);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ClockSkew = TimeSpan.Zero
    };
});

var baseUrl = builder.Configuration["RickAndMortyApi:BaseUrl"];
if (!string.IsNullOrEmpty(baseUrl))
    builder.Services.AddHttpClient<IRickAndMortyApiClient, RickAndMortyApiClient>(client =>
    {
        client.BaseAddress = new Uri(baseUrl);
    });
else
    throw new InvalidOperationException("RickAndMortyApi:BaseUrl configuration is missing.");

builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<IEpisodeRepository, EpisodeRepository>();



builder.Services.AddAutoMapper(cfg => { }, typeof(RickAndMortyCharacterApiResponseDTOToEpisodeProfile));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ImportCharactersCommandHandler).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
