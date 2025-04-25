using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PdfReaderApp.Services;
using dotenv.net;

// Carrega o .env se existir
DotEnv.Load();

// 🔥 Aqui construímos o Host
var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<PdfService>();
        services.AddSingleton<OcrService>();
        services.AddSingleton<PdfToImageService>();
    });

var app = builder.Build();

// 🔥 Aqui criamos o scope para resolver os serviços
using (var scope = app.Services.CreateScope())
{
    var pdfService = scope.ServiceProvider.GetRequiredService<PdfService>();
    var ocrService = scope.ServiceProvider.GetRequiredService<OcrService>();
    var pdfToImageService = scope.ServiceProvider.GetRequiredService<PdfToImageService>();

    Console.WriteLine("📂 Informe o caminho do arquivo:");
    var caminho = Console.ReadLine()?.Trim().Replace("\u202A", "");

    if (!string.IsNullOrWhiteSpace(caminho))
    {
        try
        {
            if (caminho.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                var texto = pdfService.LerTextoPdf(caminho);

                if (string.IsNullOrWhiteSpace(texto))
                {
                    Console.WriteLine("⚠️ PDF não tem texto! Fazendo OCR das páginas...");

                    var imagens = pdfToImageService.ConverterPdfParaImagens(caminho);
                    texto = "";

                    foreach (var imgPath in imagens)
                    {
                        texto += ocrService.LerTextoImagem(imgPath) + "\n";
                        File.Delete(imgPath); // apaga imagens temporárias
                    }
                }

                Console.WriteLine("\n📄 Texto extraído:");
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine(texto);
            }
            else if (caminho.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                     caminho.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                var texto = ocrService.LerTextoImagem(caminho);
                Console.WriteLine("\n🧠 Texto extraído da imagem:");
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine(texto);
            }
            else
            {
                Console.WriteLine("❌ Tipo de arquivo não suportado.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao processar o arquivo: {ex.Message}");
        }
    }
}
