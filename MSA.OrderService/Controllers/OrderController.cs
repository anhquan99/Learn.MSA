using Microsoft.AspNetCore.Mvc;
using MSA.Common.Contracts.Domain;
using MSA.Common.PostgresMassTransit.PostgresDB;
using MSA.OrderService.Domain;
using MSA.OrderService.Dtos;
using MSA.OrderService.Infrastructure.Data;
using MSA.OrderService.Services;

namespace MSA.OrderService.Controllers;

[ApiController]
[Route("v1/order")]
public class OrderController : ControllerBase
{
    private readonly IRepository<Order> repository;
    private readonly PostgresUnitOfWork<MainDbContext> uow;
    private readonly IProductService productService;
    public OrderController(IRepository<Order> repository, PostgresUnitOfWork<MainDbContext> uow,
                            IProductService productService)
    {
        this.repository = repository;
        this.uow = uow;
        this.productService = productService;
    }
    [HttpGet]
    public async Task<IEnumerable<Order>> GetAsync()
    {
        return (await repository.GetAllAsync()).ToList();
    }
    [HttpPost]
    public async Task<ActionResult<Order>> PostAsync(CreateOrderDto createOrderDto)
    {
        // validate and ensure product exist before creating
        var isProductExisted = await productService.IsProductExisted(createOrderDto.ProductId);
        if (!isProductExisted) return BadRequest();
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = createOrderDto.UserId,
            OrderStatus = "Order Submitted"
        };
        await repository.CreateAsync(order);
        await uow.SaveChangeAysnc();

        return CreatedAtAction(nameof(PostAsync), order);
    }
}