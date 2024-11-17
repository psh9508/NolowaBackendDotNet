using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NolowaBackendDotNet.Core.MessageQueue;
using SharedLib.MessageQueue;
using System;
using System.Threading.Tasks;

namespace NolowaBackendDotNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("The application has been started!");

            //var messageQeueu = host.Services.GetRequiredService<IMessageQueueService>();
            //messageQeueu.InitAsync(new MessageQueueConnectionData()
            //{
            //    HostName = "localhost",
            //    VirtualHostName = "/",
            //    QueueName = "server",
            //    ExchangeName = "amq.topic",
            //}, new MessageHandler()).Wait(TimeSpan.FromSeconds(10));

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();

                    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                    logging.AddDebug();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
