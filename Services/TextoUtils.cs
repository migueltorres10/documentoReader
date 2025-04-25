using System.Text.RegularExpressions;

namespace PdfReaderApp.Services
{
    public static class TextoUtils
    {
        public static string LimparTextoOcr(string texto)
        {
            var linhas = texto.Split('\n');

            var linhasLimpas = linhas
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Where(l => Regex.IsMatch(l, @"[a-zA-Z0-9]")) // linhas úteis
                .Select(l => Regex.Replace(l, @"\s{2,}", " ")) // espaços repetidos
                .Select(l => Regex.Replace(l, @"\s*:\s*", ": ")) // padroniza dois-pontos
                .ToList();

            var textoJunto = string.Join(" ", linhasLimpas);
            textoJunto = Regex.Replace(textoJunto, @"\s+([.,:])", "$1");

            return textoJunto.Trim();
        }
    }
}
