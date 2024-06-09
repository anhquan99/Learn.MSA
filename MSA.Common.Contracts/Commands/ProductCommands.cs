namespace MSA.Common.Domain.Commands.Product;
public record ValidateProduct
{
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
}