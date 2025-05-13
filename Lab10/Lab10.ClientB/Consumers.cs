using Contracts;
using MassTransit;

namespace Lab10.ClientA
{
    public class ConfirmationRequestConsumer : IConsumer<ConfirmationRequest>
    {
        public async Task Consume(ConsumeContext<ConfirmationRequest> context)
        {
            Console.WriteLine($"Received confirmation request for order {context.Message.OrderId} with quantity {context.Message.Quantity}");
            Console.Write("Press 'Y' to confirm the order or any other key to reject: ");
            
            var key = Console.ReadKey().Key;
            Console.WriteLine();
            
            if (key == ConsoleKey.Y)
            {
                await context.Publish(new Confirmation { OrderId = context.Message.OrderId });
                Console.WriteLine($"Order {context.Message.OrderId} confirmed");
            }
            else
            {
                await context.Publish(new Rejection { OrderId = context.Message.OrderId });
                Console.WriteLine($"Order {context.Message.OrderId} rejected");
            }
        }
    }

    public class OrderAcceptedConsumer : IConsumer<OrderAccepted>
    {
        public Task Consume(ConsumeContext<OrderAccepted> context)
        {
            Console.WriteLine($"Order {context.Message.OrderId} for {context.Message.Quantity} items has been accepted!");
            return Task.CompletedTask;
        }
    }

    public class OrderRejectedConsumer : IConsumer<OrderRejected>
    {
        public Task Consume(ConsumeContext<OrderRejected> context)
        {
            Console.WriteLine($"Order {context.Message.OrderId} for {context.Message.Quantity} items has been rejected!");
            return Task.CompletedTask;
        }
    }
}