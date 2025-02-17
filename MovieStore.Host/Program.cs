using Marten;
using Minio;
using MovieStore.Host.Contracts;
using MovieStore.Host.Models;
using MovieStore.Host.Repositories;
using MovieStore.Host.Services;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMarten(master =>
{
    master.Connection(configuration.GetConnectionString("PostgresConnection")!);

    master.UseSystemTextJsonForSerialization();

    master.AutoCreateSchemaObjects = AutoCreate.All;
});

builder.Services.AddSingleton<IMinioClient>(m =>
{
    var minioConfig = configuration.GetSection("Minio").Get<MinioConfiguration>();
            
    return new MinioClient()
        .WithEndpoint(minioConfig!.Endpoint)
        .WithCredentials(minioConfig.AccessKey, minioConfig.SecretKey)
        .WithSSL(false)
        .Build();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });

});

builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddSingleton<IMinioService, MinioService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.MapControllers();

app.Run();