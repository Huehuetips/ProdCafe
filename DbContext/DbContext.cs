using Microsoft.EntityFrameworkCore;

namespace ApiEjemplo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // DbSets principales
        public DbSet<Models.Proveedores> Proveedores { get; set; }
        public DbSet<Models.OrdenesCompra> OrdenesCompras { get; set; }
        public DbSet<Models.TiposGrano> TiposGranos { get; set; }
        public DbSet<Models.Etapas> Etapas { get; set; }
        public DbSet<Models.Lotes> Lotes { get; set; }
        public DbSet<Models.Presentaciones> Presentaciones { get; set; }
        public DbSet<Models.Productos> Productos { get; set; }
        public DbSet<Models.LotesTerminados> LotesTerminados { get; set; }
        public DbSet<Models.Clientes> Clientes { get; set; }
        public DbSet<Models.Pedidos> Pedidos { get; set; }
        public DbSet<Models.Rutas> Rutas { get; set; }
        public DbSet<Models.Catacion> Cataciones { get; set; }

        // DbSets para tablas intermedias / relaciones
        public DbSet<Models.OrdenCompraTipoGrano> OrdenCompraTipoGranos { get; set; }
        public DbSet<Models.OrdenCompraTipoGranoLote> OrdenCompraTipoGranoLotes { get; set; }
        public DbSet<Models.LoteEtapa> LoteEtapas { get; set; }
        public DbSet<Models.PedidoLoteTerminado> PedidoLoteTerminados { get; set; }
        public DbSet<Models.PedidoRuta> PedidoRutas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar comportamiento de eliminación en cascada para evitar ciclos
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
