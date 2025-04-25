namespace PdfReaderApp.Services
{
    public static class FileOrganizerService
    {
        public static void MoverEOrganizar(string origem, DocumentoDados dados, string pastaBase)
        {
            if (dados.IdTipoDocumento == null || string.IsNullOrWhiteSpace(dados.Ano) || string.IsNullOrWhiteSpace(dados.NifFornecedor) || string.IsNullOrWhiteSpace(dados.Numero))
                throw new InvalidOperationException("Dados incompletos para mover o ficheiro.");

            // Determinar subpastas
            string tipoDocFolder = dados.IdTipoDocumento switch
            {
                1 => "Fatura",
                2 => "Fatura-Recibo",
                3 => "Nota de Crédito",
                4 => "Guia de Remessa",
                5 => "Recibo",
                _ => "Outros"
            };

            string destinoPasta = Path.Combine(pastaBase, tipoDocFolder, dados.Ano, dados.NifFornecedor);
            Directory.CreateDirectory(destinoPasta);

            string novoNome = dados.Numero.Replace("/", "-") + ".pdf";
            string destinoCompleto = Path.Combine(destinoPasta, novoNome);

            File.Move(origem, destinoCompleto);
        }
    }
}
