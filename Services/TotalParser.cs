using System.Text.RegularExpressions;

namespace PdfReaderApp.Services
{
    public class TotalParser
    {
        public decimal? ExtrairTotal(string texto)
        {
            // 1️⃣ Procurar diretamente por linhas contendo "Total"
            var totalMatch = Regex.Match(texto, @"Total[^0-9]*(\d+[.,]\d{2})", RegexOptions.IgnoreCase);
            if (totalMatch.Success)
            {
                if (decimal.TryParse(totalMatch.Groups[1].Value.Replace(",", "."), out var valorTotal))
                    return valorTotal;
            }

            // 2️⃣ Procurar valores associados a "EUR"
            var valoresEur = Regex.Matches(texto, @"EUR[^0-9]*(\d+[.,]\d{2})", RegexOptions.IgnoreCase)
                .Select(m => decimal.TryParse(m.Groups[1].Value.Replace(",", "."), out var valor) ? (decimal?)valor : null)
                .Where(v => v.HasValue)
                .Select(v => v.Value)
                .ToList();

            if (valoresEur.Any())
            {
                // Normalmente o TOTAL é o maior valor encontrado
                return valoresEur.Max();
            }

            // 3️⃣ Se nada encontrado
            return null;
        }
    }
}
