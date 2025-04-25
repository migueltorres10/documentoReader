using UglyToad.PdfPig;

namespace PdfReaderApp.Services
{
    public class PdfService
    {
        public string LerTextoPdf(string caminhoArquivo)
        {
            if (!File.Exists(caminhoArquivo))
                throw new FileNotFoundException("Arquivo não encontrado.", caminhoArquivo);

            using var document = PdfDocument.Open(caminhoArquivo);
            var texto = string.Join("\n", document.GetPages().Select(p => p.Text));

            return texto;
        }
    }
}
