namespace PdfReaderApp.Services
{
    public class TipoDocumentoParser
    {
        private readonly List<(int Id, List<string> PalavrasChave)> _tiposDocumento = new()
        {
            (1, new List<string> { "Fatura", "Factura" }),           // Fatura normal
            (2, new List<string> { "Fatura-Recibo", "Factura-Recibo" }), // Fatura-Recibo
            (3, new List<string> { "Nota de Crédito", "Nota Crédito" }), // Nota de Crédito
            (5, new List<string> { "Recibo" }),                     // Recibo
            (4, new List<string> { "Guia de Remessa", "Guia Remessa" }) // Guia de Remessa
        };

        public int? ExtrairTipoDocumento(string texto)
        {
            foreach (var tipo in _tiposDocumento)
            {
                foreach (var palavra in tipo.PalavrasChave)
                {
                    if (texto.IndexOf(palavra, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return tipo.Id;
                    }
                }
            }

            // Se nenhum tipo de documento for encontrado
            return null;
        }
    }
}
