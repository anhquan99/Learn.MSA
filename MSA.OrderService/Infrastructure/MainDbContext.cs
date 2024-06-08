using Microsoft.EntityFrameworkCore;
using MSA.Common.PostgresMassTransit.PostgresDB;
using MSA.OrderService.Domain;

namespace MSA.OrderService.Infrastructure.Data;
public class MainDbContext : AppDbContextBase
{
    private readonly string _uuidGenerator = "uuid-ossp";
    private readonly string _uuidAlgorithm = "uuid_generate_v4()";
    private readonly IConfiguration configuration;
    public MainDbContext(IConfiguration configuration, DbContextOptions options) : base(configuration, options)
    {
        this.configuration = configuration;
    }
    public DbSet<Order> Orders { get; set; } = default;
    public DbSet<OrderDetail> OrderDetails { get; set; } = default;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension(_uuidGenerator);

        // Orders
        modelBuilder.Entity<Order>().ToTable("orders");
        modelBuilder.Entity<Order>().HasKey(x => x.Id);
        modelBuilder.Entity<Order>().Property(x => x.Id)
                                        .HasColumnType("uuid");

        // Order details
        modelBuilder.Entity<OrderDetail>().ToTable("order_details");
        modelBuilder.Entity<OrderDetail>().HasKey(x => x.Id);
        modelBuilder.Entity<OrderDetail>().Property(x => x.Id)
                                            .HasColumnType("uuid")
                                            .HasDefaultValueSql(_uuidAlgorithm);

        // Relationship
        modelBuilder.Entity<Order>().HasMany(x => x.OrderDetails);
    }
}