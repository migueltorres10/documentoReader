using System.Text.RegularExpressions;

namespace PdfReaderApp.Services
{
    public class NumeroParser
    {
        public string? ExtrairNumero(string texto)
        {
            // 1️⃣ Procurar número típico de fatura
            var numeroMatch = Regex.Match(texto, @"\b(FT|FR|NC|GR|R)\s?\d{9,12}/\d{1,6}\b", RegexOptions.IgnoreCase);

            if (numeroMatch.Success)
            {
                return numeroMatch.Value.Trim();
            }

            // 2️⃣ Se não encontrar, retornar null
            return null;
        }
    }
}
