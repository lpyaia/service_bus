using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace Queue.Consumer
{
    internal class Program
    {
        private static string _connectionString = "connstr";
        private static string _queueName = "queue";
        private static ServiceBusClient _client;
        private static ServiceBusProcessor _processor;

        private static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();

            Console.WriteLine($"Received: {body}");

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

            _processor = _client.CreateProcessor(
                _queueName,
                new ServiceBusProcessorOptions { MaxConcurrentCalls = 3 });

            try
            {
                _processor.ProcessMessageAsync += MessageHandler;
                _processor.ProcessErrorAsync += ErrorHandler;

                await _processor.StartProcessingAsync();

                Console.ReadKey();

                Console.WriteLine();
                await _processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
            finally
            {
                await _processor.DisposeAsync();
                await _client.DisposeAsync();
            }
        }
    }
}