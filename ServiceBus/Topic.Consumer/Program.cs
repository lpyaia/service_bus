using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace Topic.Consumer
{
    internal class Program
    {
        private static string _connectionString = "Endpoint=sb://lpyaia.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=rda4pc2ieQ/ucapQ8GpybN6OjeHnR673otOeELVmck8=";
        private static string _topicName = "topic";
        private static string _subscriptionName = "S3";
        private static ServiceBusClient _client;
        private static ServiceBusProcessor _processor;

        private static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body} from subscription: {_subscriptionName}");

            await args.CompleteMessageAsync(args.Message);
        }

        private static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private static async Task Main(string[] args)
        {
            _client = new ServiceBusClient(_connectionString);

            var options = new ServiceBusProcessorOptions()
            {
                MaxConcurrentCalls = 3
            };

            _processor = _client.CreateProcessor(_topicName, _subscriptionName, options);

            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync();

            Console.WriteLine("Wait for a minute and then press any key to end the processing");
            Console.ReadKey();

            // stop processing
            Console.WriteLine("\nStopping the receiver...");

            await _processor.StopProcessingAsync();

            Console.WriteLine("Stopped receiving messages");
        }
    }
}