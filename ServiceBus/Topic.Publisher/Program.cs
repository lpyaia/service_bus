using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace Topic.Publisher
{
    internal class Program
    {
        private static string _connectionString = "Endpoint=sb://lpyaia.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=rda4pc2ieQ/ucapQ8GpybN6OjeHnR673otOeELVmck8=";
        private static string _topicName = "topic";
        private static ServiceBusClient _client;
        private static ServiceBusSender _sender;
        private const int _numOfMessages = 15;

        private static async Task Main(string[] args)
        {
            _client = new ServiceBusClient(_connectionString);
            _sender = _client.CreateSender(_topicName);

            using ServiceBusMessageBatch messageBatch = await _sender.CreateMessageBatchAsync();

            for (int i = 1; i <= _numOfMessages; i++)
            {
                if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
                {
                    throw new Exception($"\nThe message {i} is too large to fit in the batch");
                }
            }

            try
            {
                await _sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {_numOfMessages} messages has been published to the topic.");
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