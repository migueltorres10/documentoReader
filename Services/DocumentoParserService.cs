namespace PdfReaderApp.Services
{
    public class DocumentoParserService
    {
        private readonly FornecedorParser _fornecedorParser;
        private readonly TipoDocumentoParser _tipoDocumentoParser;
        private readonly DataParser _dataParser;
        private readonly TotalParser _totalParser;
        private readonly NumeroParser _numeroParser;

        public DocumentoParserService(Dictionary<string, string> fornecedores)
        {
            _fornecedorParser = new FornecedorParser(fornecedores);
            _tipoDocumentoParser = new TipoDocumentoParser();
            _dataParser = new DataParser();
            _totalParser = new TotalParser();
            _numeroParser = new NumeroParser();
        }

        public DocumentoDados Extrair(string texto)
        {
            var numero = _numeroParser.ExtrairNumero(texto);
            var nifFornecedor = _fornecedorParser.ExtrairFornecedor(texto);
            var idTipoDoc = _tipoDocumentoParser.ExtrairTipoDocumento(texto);
            var data = _dataParser.ExtrairData(texto);
            var total = _totalParser.ExtrairTotal(texto);

            return new DocumentoDados
            {
                Numero = numero,
                NifFornecedor = nifFornecedor,
                IdTipoDocumento = idTipoDoc,
                Data = data,
                Total = total,
                Ano = data?.Year.ToString() ?? DateTime.Now.Year.ToString()
            };
        }
    }

    public class DocumentoDados
    {
        public string? Numero { get; set; }
        public string? NifFornecedor { get; set; }
        public int? IdTipoDocumento { get; set; }
        public DateTime? Data { get; set; }
        public decimal? Total { get; set; }
        public string? Ano { get; set; }
    }
}
