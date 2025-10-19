using EurofirmsTest.Infrastructure.MappingProfiles;
using Microsoft.EntityFrameworkCore;
using PruebaEurofirms.Domain;
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

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<EurofirmsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("EurofirmsSqlite")));

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

app.UseAuthorization();

app.MapControllers();

app.Run();
