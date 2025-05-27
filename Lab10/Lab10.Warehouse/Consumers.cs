using Contracts;
using MassTransit;

namespace Lab10.Warehouse
{
    public class InventoryRequestConsumer(InventoryService inventoryService) : IConsumer<InventoryRequest>
    {
        public async Task Consume(ConsumeContext<InventoryRequest> context)
        {
            var orderId = context.Message.OrderId;
            var quantity = context.Message.Quantity;
            
            Console.WriteLine($"Received inventory request for order {orderId} with quantity {quantity}");
            
            var (available, _) = inventoryService.GetInventoryStatus();
            Console.WriteLine($"Current inventory: {available} available items");
            
            if (available >= quantity)
            {
                inventoryService.TryReserveItems(quantity);
                await context.Publish(new InventoryAvailable { OrderId = orderId });
                Console.WriteLine($"Inventory available for order {orderId}, reserved {quantity} items");
            }
            else
            {
                await context.Publish(new InventoryUnavailable { OrderId = orderId });
                Console.WriteLine($"Insufficient inventory for order {orderId}, requested {quantity} but only {available} available");
            }
        }
    }

    public class OrderAcceptedConsumer(InventoryService inventoryService) : IConsumer<OrderAccepted>
    {
        public Task Consume(ConsumeContext<OrderAccepted> context)
        {
            var orderId = context.Message.OrderId;
            var quantity = context.Message.Quantity;
            
            inventoryService.ConfirmReservation(quantity);
            
            Console.WriteLine($"Order {orderId} accepted, removed {quantity} reserved items from inventory");
            
            return Task.CompletedTask;
        }
    }

    public class OrderRejectedConsumer(InventoryService inventoryService) : IConsumer<OrderRejected>
    {
        public Task Consume(ConsumeContext<OrderRejected> context)
        {
            var orderId = context.Message.OrderId;
            var quantity = context.Message.Quantity;
            
            inventoryService.CancelReservation(quantity);
            
            Console.WriteLine($"Order {orderId} rejected, returned {quantity} items to available inventory");
            
            return Task.CompletedTask;
        }
    }
}
