using System.Text.RegularExpressions;

namespace PdfReaderApp.Services
{
    public class FornecedorParser
    {
        private readonly Dictionary<string, string> _fornecedores; // NIF -> Nome

        public FornecedorParser(Dictionary<string, string> fornecedores)
        {
            _fornecedores = fornecedores;
        }

        public string? ExtrairFornecedor(string texto)
        {
            // 1️⃣ Primeiro: procurar por NIF
            var nifsEncontrados = Regex.Matches(texto, @"\b\d{9}\b")
                                        .Select(m => m.Value)
                                        .Distinct();

            foreach (var nif in nifsEncontrados)
            {
                if (_fornecedores.ContainsKey(nif))
                    return nif;
            }

            // 2️⃣ Se não achar por NIF, tentar nome do fornecedor
            foreach (var fornecedor in _fornecedores)
            {
                if (texto.IndexOf(fornecedor.Value, StringComparison.OrdinalIgnoreCase) >= 0)
                    return fornecedor.Key;
            }

            // 3️⃣ Se nada encontrado, retorna null
            return null;
        }
    }
}
