using Contracts;
using MassTransit;

namespace Lab10.Shop;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);
        
        Event(() => OrderReceived, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => ClientConfirmation, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => ClientRejection, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => WarehouseConfirmation, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => WarehouseRejection, x => x.CorrelateById(context => context.Message.OrderId));
        Schedule(() => OrderTimeout, x => x.TimeoutTokenId, s => { 
            s.Delay = TimeSpan.FromSeconds(10);
            s.Received = x => x.CorrelateById(c => c.Message.OrderId);
        });
        
        Initially(
            When(OrderReceived)
                .Then(Initialize)
                .TransitionTo(AwaitingConfirmations)
                .Schedule(OrderTimeout, context => new OrderTimeout { OrderId = context.Saga.CorrelationId })
                .ThenAsync(SendRequests)
        );
        
        During(AwaitingConfirmations,
            When(ClientConfirmation)
                .Then(context => context.Saga.ClientConfirmed = true)
                .ThenAsync(CheckCompletionStatus),
                
            When(ClientRejection)
                .TransitionTo(Rejected)
                .ThenAsync(SendRejectionToClient),

            When(WarehouseConfirmation)
                .Then(context => context.Saga.WarehouseConfirmed = true)
                .ThenAsync(CheckCompletionStatus),
                
            When(WarehouseRejection)
                .TransitionTo(Rejected)
                .ThenAsync(SendRejectionToClient)
                .TransitionTo(AwaitingConfirmations),

            When(OrderTimeout!.Received)
                .TransitionTo(Rejected)
                .ThenAsync(HandleTimeout)
        );
    }
    
    public State AwaitingConfirmations { get; private set; }
    public State Accepted { get; private set; }
    public State Rejected { get; private set; }
    
    public Event<StartOrder> OrderReceived { get; private set; }
    public Event<Confirmation> ClientConfirmation { get; private set; }
    public Event<Rejection> ClientRejection { get; private set; }
    public Event<InventoryAvailable> WarehouseConfirmation { get; private set; }
    public Event<InventoryUnavailable> WarehouseRejection { get; private set; }
    
    public Schedule<OrderState, OrderTimeout> OrderTimeout { get; private set; }
    
    private static void Initialize(BehaviorContext<OrderState, StartOrder> context)
    {
        context.Saga.Quantity = context.Message.Quantity;
        context.Saga.ClientId = context.Message.ClientId;
            
        Console.WriteLine($"Order {context.Saga.CorrelationId} received from {context.Saga.ClientId} for {context.Saga.Quantity} items");
    }
    
    private static async Task SendRequests(BehaviorContext<OrderState, StartOrder> context)
    {
        var endpoint = await context.GetSendEndpoint(new Uri("queue:warehouse"));
        await endpoint.Send(new InventoryRequest 
        { 
            OrderId = context.Saga.CorrelationId,
            Quantity = context.Saga.Quantity
        });
            
        Console.WriteLine($"Sent inventory request to warehouse for order {context.Saga.CorrelationId}");

        endpoint = await context.GetSendEndpoint(new Uri($"queue:client-{context.Saga.ClientId}"));
        await endpoint.Send(new ConfirmationRequest
        { 
            OrderId = context.Saga.CorrelationId,
            Quantity = context.Saga.Quantity
        });
            
        Console.WriteLine($"Sent confirmation request to client {context.Saga.ClientId} for order {context.Saga.CorrelationId}");
    }
    
    private async Task CheckCompletionStatus(BehaviorContext<OrderState> context)
    {
        Console.WriteLine($"Order {context.Saga.CorrelationId} status: Client confirmed: {context.Saga.ClientConfirmed}, Warehouse confirmed: {context.Saga.WarehouseConfirmed}");
            
        if (context.Saga is { ClientConfirmed: true, WarehouseConfirmed: true })
        {
            context.Saga.CurrentState = nameof(Accepted);
            await SendAcceptanceToClient(context);
        }
    }
    
    private static async Task SendAcceptanceToClient(BehaviorContext<OrderState> context)
    {
        var endpoint = await context.GetSendEndpoint(new Uri($"queue:client-{context.Saga.ClientId}"));
        await endpoint.Send(new OrderAccepted 
        { 
            OrderId = context.Saga.CorrelationId,
            Quantity = context.Saga.Quantity
        });
            
        Console.WriteLine($"Order {context.Saga.CorrelationId} accepted, notification sent to client {context.Saga.ClientId}");
    }

    private static async Task SendRejectionToClient(BehaviorContext<OrderState> context)
    {
        var endpoint = await context.GetSendEndpoint(new Uri($"queue:client-{context.Saga.ClientId}"));
        await endpoint.Send(new OrderRejected 
        { 
            OrderId = context.Saga.CorrelationId,
            Quantity = context.Saga.Quantity
        });
            
        Console.WriteLine($"Order {context.Saga.CorrelationId} rejected, notification sent to client {context.Saga.ClientId}");
    }
    
    private async Task HandleTimeout(BehaviorContext<OrderState, OrderTimeout> context)
    {
        Console.WriteLine($"Order {context.Saga.CorrelationId} timed out after 10 seconds");
        await SendRejectionToClient(context);
    }
}