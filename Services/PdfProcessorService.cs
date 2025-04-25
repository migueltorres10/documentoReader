using System.Diagnostics;
using PdfReaderApp.Data;
using PdfReaderApp.Models;

namespace PdfReaderApp.Services
{
    public class PdfProcessorService
    {
        private readonly DocumentoParserService _parserService;
        private readonly DocumentoService _documentoService;
        private readonly AppDbContext _dbContext;

        private const string SumatraPath = @"C:\PdfReaderApp\SumatraPDF\SumatraPDF-3.5.2-64.exe";

        public PdfProcessorService(DocumentoParserService parserService, DocumentoService documentoService, AppDbContext dbContext)
        {
            _parserService = parserService;
            _documentoService = documentoService;
            _dbContext = dbContext;
        }

        public async Task ProcessarPdfsAsync(string pastaBase)
        {
            var pdfs = Directory.GetFiles(pastaBase, "*.pdf");
            var pastaLidos = Path.Combine(pastaBase, "Lidos");
            var pastaErros = Path.Combine(pastaBase, "Erros");

            Directory.CreateDirectory(pastaLidos);
            Directory.CreateDirectory(pastaErros);

            foreach (var pdfPath in pdfs)
            {
                try
                {
                    AbrirPdfComSumatra(pdfPath);

                    // Substituir isto pelo OCR real
                    string textoExtraido = "Simulação de texto extraído";

                    var dados = _parserService.ExtrairDados(textoExtraido);

                    using var form = new MainForm(dados);
                    form.ShowDialog();

                    if (form.DialogResult == DialogResult.OK)
                    {
                        await _documentoService.InserirDocumentoAsync(dados);
                        FileOrganizerService.MoverEOrganizar(pdfPath, dados, pastaBase, pastaLidos);
                    }
                    else
                    {
                        var destinoErro = Path.Combine(pastaErros, Path.GetFileName(pdfPath));
                        File.Move(pdfPath, destinoErro, true);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Erro a processar {Path.GetFileName(pdfPath)}: {ex.Message}");

                    var destinoErro = Path.Combine(pastaErros, Path.GetFileName(pdfPath));
                    if (File.Exists(pdfPath))
                        File.Move(pdfPath, destinoErro, true);
                }
            }
        }

        private void AbrirPdfComSumatra(string pdfPath)
        {
            if (File.Exists(SumatraPath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = SumatraPath,
                    Arguments = $"\"{pdfPath}\"",
                    UseShellExecute = true
                });
            }
            else
            {
                Console.WriteLine($"⚠️ SumatraPDF não encontrado em: {SumatraPath}");
            }
        }
    }

    public static class FileOrganizerService
    {
        public static void MoverEOrganizar(string origem, DocumentoDados dados, string pastaBase, string pastaLidos)
        {
            if (dados.IdTipoDocumento == null || string.IsNullOrWhiteSpace(dados.Ano) || string.IsNullOrWhiteSpace(dados.NomeFornecedor) || string.IsNullOrWhiteSpace(dados.Numero))
                throw new InvalidOperationException("Dados incompletos para mover o ficheiro.");

            var nomeFornecedor = SanitizeFileName(dados.NomeFornecedor);

            string tipoDocFolder = dados.IdTipoDocumento switch
            {
                1 => "Fatura",
                2 => "Fatura-Recibo",
                3 => "Nota de Crédito",
                4 => "Guia de Remessa",
                5 => "Recibo",
                _ => "Outros"
            };

            string destinoPasta = Path.Combine(pastaBase, tipoDocFolder, dados.Ano, nomeFornecedor);
            Directory.CreateDirectory(destinoPasta);

            string novoNome = dados.Numero.Replace("/", "-") + ".pdf";
            string destinoCompleto = Path.Combine(destinoPasta, novoNome);

            File.Move(origem, destinoCompleto, true);

            var destinoLido = Path.Combine(pastaLidos, novoNome);
            File.Copy(destinoCompleto, destinoLido, true);
        }

        private static string SanitizeFileName(string nome)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                nome = nome.Replace(c, '_');
            }
            return nome;
        }
    }
}
