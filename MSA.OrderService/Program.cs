using MSA.Common.Contracts.Settings;
using MSA.OrderService.Domain;
using MSA.OrderService.Infrastructure.Data;
using MSA.Common.PostgresMassTransit.PostgresDB;
using MSA.OrderService.Services;

var builder = WebApplication.CreateBuilder(args);

PostgresDBSetting serviceSetting = builder.Configuration.GetSection(nameof(PostgresDBSetting)).Get<PostgresDBSetting>();


// Add services to the container.
builder.Services
        .AddPostgres<MainDbContext>()
        .AddPostgresRepositories<MainDbContext, Order>()
        .AddPostgresUnitofWork<MainDbContext>();

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
builder.Services.AddHttpClient<IProductService, ProductService>(cfg =>
{
    cfg.BaseAddress = new Uri("https://localhost:5002");
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();
