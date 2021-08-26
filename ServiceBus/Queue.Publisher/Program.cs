using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace Queue.Publisher
{
    internal class Program
    {
        private static string _connectionString = "connstr";
        private static string _queueName = "queue";
        private static ServiceBusClient _client;
        private static ServiceBusSender _sender;
        private const int numOfMessages = 15;

        private static async Task Main(string[] args)
        {
            _client = new ServiceBusClient(_connectionString);
            _sender = _client.CreateSender(_queueName);

            Console.WriteLine("Press to select message type");
            Console.WriteLine("1 - normal message");
            Console.WriteLine("2 - scheduled message");

            var input = Console.ReadKey().KeyChar;
            var scheduled = input == '2';

            using ServiceBusMessageBatch messageBatch = await _sender.CreateMessageBatchAsync();

            for (int i = 1; i <= numOfMessages; i++)
            {
                var msg = new ServiceBusMessage($"Message {i}");

                if (scheduled) msg.ScheduledEnqueueTime = DateTimeOffset.Now.AddMinutes(2);

                if (!messageBatch.TryAddMessage(msg))
                {
                    throw new Exception($"\nThe message {i} is too large to fit in the batch");
                }
            }

            try
            {
                await _sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue.");
            }
            finally
            {
                await _sender.DisposeAsync();
                await _client.DisposeAsync();
            }

            Console.WriteLine("Press any key to end the application");
            Console.ReadKey();
        }
    }
}