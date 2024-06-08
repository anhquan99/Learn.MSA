using MSA.Common.Domain;

namespace MSA.OrderService.Domain;
public class Product : IEntity
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
}