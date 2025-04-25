using System.Windows.Forms;

namespace PdfReaderApp.Services
{
    public static class FolderService
    {
        public static string? EscolherPasta()
        {
            using var dialog = new FolderBrowserDialog();
            dialog.Description = "Selecione a pasta com os PDFs a processar:";
            if (dialog.ShowDialog() == DialogResult.OK)
                return dialog.SelectedPath;

            return null;
        }
    }
}
