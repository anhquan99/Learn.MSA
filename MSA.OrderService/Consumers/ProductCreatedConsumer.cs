using MassTransit;
using MSA.Common.Contracts.Domain;
using MSA.Common.Contracts.Domain.Events.Product;
using MSA.Common.PostgresMassTransit.PostgresDB;
using MSA.OrderService.Domain;
using MSA.OrderService.Infrastructure.Data;

namespace MSA.OrderService.Consumers;
public class ProductCreatedConsumers : IConsumer<ProductCreated>
{
    private readonly IRepository<Product> productRepostiory;
    private readonly PostgresUnitOfWork<MainDbContext> uow;
    public ProductCreatedConsumers(IRepository<Product> productRepository, PostgresUnitOfWork<MainDbContext> uow)
    {
        this.productRepostiory = productRepository;
        this.uow = uow;
    }


    public async Task Consume(ConsumeContext<ProductCreated> context)
    {
        var message = context.Message;
        var product = new Product
        {
            Id = new Guid(),
            ProductId = message.ProductId
        };
        await productRepostiory.CreateAsync(product);
        await uow.SaveChangeAysnc();
    }
}