using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MSA.Common.Contracts.Domain.Events.Order;
using MSA.Common.Contracts.Domain;
using MSA.Common.Domain.Commands.Product;
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
    private readonly ISendEndpointProvider sendEndpointProvider;
    private readonly IPublishEndpoint publishEndpoint;
    public OrderController(IRepository<Order> repository, PostgresUnitOfWork<MainDbContext> uow,
                            IProductService productService,
                            ISendEndpointProvider sendEndpointProvider,
                            IPublishEndpoint publishEndpoint)
    {
        this.repository = repository;
        this.uow = uow;
        this.productService = productService;
        this.sendEndpointProvider = sendEndpointProvider;
        this.publishEndpoint = publishEndpoint;
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
        // var isProductExisted = await productService.IsProductExisted(createOrderDto.ProductId);
        // if (!isProductExisted) return BadRequest("Product is not existed!");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = createOrderDto.UserId,
            OrderStatus = "Order Submitted"
        };
        await repository.CreateAsync(order);
        await uow.SaveChangeAysnc();

        // async validate
        // var endPoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:product-validate-product"));
        // await endPoint.Send(new ValidateProduct
        // {
        //     OrderId = order.Id,
        //     ProductId = createOrderDto.ProductId
        // });

        // async Orchestrator
        await publishEndpoint.Publish<OrderSubmitted>(
            new OrderSubmitted
            {
                OrderId = order.Id,
                ProductId = createOrderDto.ProductId
            }
        );
        await uow.SaveChangeAysnc();
        return CreatedAtAction(nameof(PostAsync), order);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<Guid>> GetByIdAsync(Guid id)
    {
        if (id == null) return BadRequest();
        var order = (await repository.GetAsync(id));
        if (order == null) return Ok(Guid.Empty);
        return Ok(order.Id);
    }
}