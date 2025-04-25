using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PdfReaderApp.Services;
using dotenv.net;

DotEnv.Load();

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<PdfService>();
        services.AddSingleton<OcrService>();
        services.AddSingleton<PdfToImageService>();
    });

var app = builder.Build();

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
            string textoExtraido = "";

            if (caminho.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                textoExtraido = pdfService.LerTextoPdf(caminho);

                if (string.IsNullOrWhiteSpace(textoExtraido))
                {
                    Console.WriteLine("⚠️ PDF não tem texto! Fazendo OCR das páginas...");

                    var imagens = pdfToImageService.ConverterPdfParaImagens(caminho);
                    foreach (var imgPath in imagens)
                    {
                        textoExtraido += ocrService.LerTextoImagem(imgPath) + "\n";
                        File.Delete(imgPath);
                    }
                }
            }
            else if (caminho.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                     caminho.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                textoExtraido = ocrService.LerTextoImagem(caminho);
            }
            else
            {
                Console.WriteLine("❌ Tipo de arquivo não suportado.");
                return;
            }

            // 🔍 Limpeza do texto OCR
            var textoLimpo = TextoUtils.LimparTextoOcr(textoExtraido);

            Console.WriteLine("\n🧼 Texto limpo:");
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine(textoLimpo);

            var parser = new FaturaParserService();
            var resultado = parser.Extrair(textoLimpo);

            Console.WriteLine("\n🔎 Dados sugeridos:");
            Console.WriteLine($"Número da Fatura: {resultado.numero}");
            Console.WriteLine($"NIF:              {resultado.nif}");
            Console.WriteLine($"Data:             {resultado.data?.ToShortDateString()}");
            Console.WriteLine($"Total:            {resultado.total?.ToString("F2")} EUR");

            // Aqui poderá futuramente abrir UI para revisão/edição!
            Console.WriteLine("\n✅ Confirme os dados acima no seu sistema visual.");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao processar o arquivo: {ex.Message}");
        }
    }
}
