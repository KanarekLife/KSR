namespace Contracts;

public record StartOrder
{
    public Guid OrderId { get; init; } = Guid.NewGuid();
    public int Quantity { get; init; }
    public string ClientId { get; init; } = string.Empty;
}

public record ConfirmationRequest
{
    public Guid OrderId { get; init; }
    public int Quantity { get; init; }
}

public record Confirmation
{
    public Guid OrderId { get; init; }
}

public record Rejection
{
    public Guid OrderId { get; init; }
}

public record InventoryRequest
{
    public Guid OrderId { get; init; }
    public int Quantity { get; init; }
}

public record InventoryAvailable
{
    public Guid OrderId { get; init; }
}

public record InventoryUnavailable
{
    public Guid OrderId { get; init; }
}

public record OrderAccepted
{
    public Guid OrderId { get; init; }
    public int Quantity { get; init; }
}

public record OrderRejected
{
    public Guid OrderId { get; init; }
    public int Quantity { get; init; }
}

public record OrderTimeout
{
    public Guid OrderId { get; init; }
}