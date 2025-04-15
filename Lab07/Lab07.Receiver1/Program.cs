using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory
{
    Uri = new Uri("amqps://bdvlehqs:OGJ3DslVWX2Eo2WGMPiMvbTjy5Eu5S5p@ostrich.lmq.cloudamqp.com/bdvlehqs")
};
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();
await channel.QueueDeclareAsync(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
await channel.BasicQosAsync(0, 1, false);
Console.WriteLine($"[Receiver1] Waiting for messages. Press any key to exit...");
var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var text = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[Receiver1] Received \"{text}\" with headers");
    foreach (var (key, o) in ea.BasicProperties.Headers!)
    {
        var value = Encoding.UTF8.GetString((byte[])o!);
        Console.WriteLine($"  {key}: {value}");
    }
    
    await Task.Delay(2000);
    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
};
await channel.BasicConsumeAsync("task_queue", false, consumer);
Console.ReadLine();
