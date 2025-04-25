using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using PdfReaderApp.Data;
using PdfReaderApp.Services;
using dotenv.net;

DotEnv.Load();

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Console.WriteLine("❌ Connection string não encontrada no .env");
            Environment.Exit(1);
        }

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<FornecedorService>();
        services.AddScoped<DocumentoService>();
        services.AddSingleton<DataParser>();
        services.AddSingleton<TotalParser>();
        services.AddSingleton<NumeroParser>();
        services.AddSingleton<TipoDocumentoParser>();
        services.AddSingleton<FornecedorParser>();
        services.AddSingleton<DocumentoParserService>();
        services.AddScoped<PdfProcessorService>();
    });

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

var pastaSelecionada = FolderService.EscolherPasta();
if (string.IsNullOrWhiteSpace(pastaSelecionada))
{
    Console.WriteLine("❌ Nenhuma pasta selecionada. Encerrando...");
    return;
}

try
{
    var fornecedorService = services.GetRequiredService<FornecedorService>();
    var fornecedores = await fornecedorService.ObterFornecedoresAsync();

    var parserService = new DocumentoParserService(fornecedores);
    var documentoService = services.GetRequiredService<DocumentoService>();
    var dbContext = services.GetRequiredService<AppDbContext>();

    var processor = new PdfProcessorService(parserService, documentoService, dbContext);
    await processor.ProcessarPdfsAsync(pastaSelecionada);

    Console.WriteLine("✅ Processamento concluído com sucesso!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Erro inesperado: {ex.Message}");
}
