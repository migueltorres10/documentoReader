using System;
using System.Windows.Forms;

namespace PdfReaderApp
{
    public class MainForm : Form
    {
        private readonly TextBox txtTextoExtraido;
        private readonly TextBox txtNumero;
        private readonly TextBox txtNifFornecedor;
        private readonly ComboBox cmbTipoDocumento;
        private readonly DateTimePicker dtpData;
        private readonly TextBox txtAno;
        private readonly TextBox txtTotal;
        private readonly Button btnConfirmar;
        private readonly Button btnCancelar;

        public MainForm(DocumentoDados dados)
        {
            Text = "Validação de Documento";
            Width = 800;
            Height = 600;
            StartPosition = FormStartPosition.CenterScreen;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 9,
                ColumnCount = 2,
                Padding = new Padding(10),
                AutoSize = true
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            layout.Controls.Add(new Label { Text = "Texto OCR:", Anchor = AnchorStyles.Left }, 0, 0);
            txtTextoExtraido = new TextBox { Multiline = true, ReadOnly = true, Height = 150, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill };
            layout.Controls.Add(txtTextoExtraido, 1, 0);

            layout.Controls.Add(new Label { Text = "Número:", Anchor = AnchorStyles.Left }, 0, 1);
            txtNumero = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtNumero, 1, 1);

            layout.Controls.Add(new Label { Text = "NIF Fornecedor:", Anchor = AnchorStyles.Left }, 0, 2);
            txtNifFornecedor = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtNifFornecedor, 1, 2);

            layout.Controls.Add(new Label { Text = "Tipo Documento:", Anchor = AnchorStyles.Left }, 0, 3);
            cmbTipoDocumento = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTipoDocumento.Items.AddRange(new[] { "Fatura", "Fatura-Recibo", "Nota de Crédito", "Recibo", "Guia de Remessa" });
            layout.Controls.Add(cmbTipoDocumento, 1, 3);

            layout.Controls.Add(new Label { Text = "Data:", Anchor = AnchorStyles.Left }, 0, 4);
            dtpData = new DateTimePicker { Format = DateTimePickerFormat.Short, Dock = DockStyle.Fill };
            layout.Controls.Add(dtpData, 1, 4);

            layout.Controls.Add(new Label { Text = "Ano:", Anchor = AnchorStyles.Left }, 0, 5);
            txtAno = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtAno, 1, 5);

            layout.Controls.Add(new Label { Text = "Total:", Anchor = AnchorStyles.Left }, 0, 6);
            txtTotal = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtTotal, 1, 6);

            btnConfirmar = new Button { Text = "Confirmar", Dock = DockStyle.Fill };
            btnCancelar = new Button { Text = "Cancelar", Dock = DockStyle.Fill };

            var buttonLayout = new TableLayoutPanel { ColumnCount = 2, Dock = DockStyle.Fill };
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            buttonLayout.Controls.Add(btnConfirmar, 0, 0);
            buttonLayout.Controls.Add(btnCancelar, 1, 0);

            layout.Controls.Add(buttonLayout, 0, 7);
            layout.SetColumnSpan(buttonLayout, 2);

            Controls.Add(layout);

            PreencherCampos(dados);

            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            btnConfirmar.Click += (s, e) => { AtualizarDados(dados); DialogResult = DialogResult.OK; Close(); };
        }

        private void PreencherCampos(DocumentoDados dados)
        {
            txtTextoExtraido.Text = "Texto extraído do OCR aqui (atualizar no futuro)";
            txtNumero.Text = dados.Numero;
            txtNifFornecedor.Text = dados.NifFornecedor;
            cmbTipoDocumento.SelectedIndex = dados.IdTipoDocumento.HasValue ? dados.IdTipoDocumento.Value - 1 : -1;
            dtpData.Value = dados.Data ?? DateTime.Today;
            txtAno.Text = dados.Ano;
            txtTotal.Text = dados.Total?.ToString("F2");
        }

        private void AtualizarDados(DocumentoDados dados)
        {
            dados.Numero = txtNumero.Text.Trim();
            dados.NifFornecedor = txtNifFornecedor.Text.Trim();
            dados.IdTipoDocumento = cmbTipoDocumento.SelectedIndex >= 0 ? cmbTipoDocumento.SelectedIndex + 1 : (int?)null;
            dados.Data = dtpData.Value.Date;
            dados.Ano = txtAno.Text.Trim();
            dados.Total = decimal.TryParse(txtTotal.Text.Replace(",", "."), out var total) ? total : (decimal?)null;
        }
    }
}
