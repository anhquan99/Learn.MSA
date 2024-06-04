using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MSA.Common.PostgresMassTransit.PostgresDB;

public class AppDbContextBase : DbContext
{
    private readonly IConfiguration configuration;
    private readonly DbContextOptions options;
    public AppDbContextBase(IConfiguration configuration, DbContextOptions options) : base(options)
    {
        this.configuration = configuration;
        this.options = options;
    }
}