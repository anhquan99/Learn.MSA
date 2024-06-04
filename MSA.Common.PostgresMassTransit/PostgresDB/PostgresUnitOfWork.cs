namespace MSA.Common.PostgresMassTransit.PostgresDB;

public class PostgresUnitOfWork<TDbContext> where TDbContext : AppDbContextBase
{
    private readonly TDbContext dbContext;
    public PostgresUnitOfWork(TDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task SaveChangeAysnc() => await dbContext.SaveChangesAsync();
}