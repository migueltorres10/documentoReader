using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using PdfReaderApp.Services;
using PdfReaderApp.Data;
using dotenv.net;

DotEnv.Load();

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("❌ Erro: Connection string não encontrada no .env!");
    return;
}

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<FornecedorService>();
        services.AddSingleton<PdfService>();
        services.AddSingleton<OcrService>();
        services.AddSingleton<PdfToImageService>();
    });

var app = builder.Build();
