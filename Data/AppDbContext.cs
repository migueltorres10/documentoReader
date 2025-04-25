using Microsoft.EntityFrameworkCore;
using PdfReaderApp.Models;

namespace PdfReaderApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<TipoDocumento> TiposDocumento { get; set; }
        public DbSet<Documento> Documentos { get; set; }
    }
}
