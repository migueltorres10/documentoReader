using System.Text.RegularExpressions;

namespace PdfReaderApp.Services
{
    public class DataParser
    {
        public DateTime? ExtrairData(string texto)
        {
            // 1️⃣ Procurar datas próximas a palavras-chave
            var dataChaveMatch = Regex.Match(texto, @"(Data (de Emissao|de Emissão|da Fatura|da Factura|Emissao|Emissão|Emitido em))[:\\s]*?(\\d{2}/\\d{2}/\\d{4})", RegexOptions.IgnoreCase);
            if (dataChaveMatch.Success)
            {
                if (DateTime.TryParse(dataChaveMatch.Groups[3].Value, out var dataEncontrada))
                    return dataEncontrada;
            }

            // 2️⃣ Se não achar, procurar todas as datas no texto
            var datasEncontradas = Regex.Matches(texto, @"\b\d{2}/\d{2}/\d{4}\b")
                .Select(m => DateTime.TryParse(m.Value, out var dt) ? dt : (DateTime?)null)
                .Where(d => d.HasValue && d.Value.Date <= DateTime.Today) // Ignora datas futuras
                .OrderByDescending(d => d.Value)
                .Select(d => d.Value)
                .ToList();

            if (datasEncontradas.Any())
                return datasEncontradas.First();

            // 3️⃣ Se não encontrar nada
            return null;
        }
    }
}
