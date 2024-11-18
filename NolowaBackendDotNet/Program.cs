using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NolowaBackendDotNet.Core.MessageQueue;
using NolowaNetwork;
using NolowaNetwork.Module;
using NolowaNetwork.System.Worker;
using Serilog;
using System;

namespace NolowaBackendDotNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("The application has been started!");

            var messageBus = host.Services.GetService<IMessageBus>() ?? throw new Exception();
            messageBus.Connect(new()
            {
                Address = "localhost",
                ExchangeName = "nolowa.topic",
                HostName = "/",
                ServerName = "apiserver",
            });

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory()) // autofac »ç¿ë
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterType<MessageHandler>().As<IMessageHandler>();

                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console()
                        .CreateLogger();

                    builder.RegisterInstance(Log.Logger);

                    var moudle = new RabbitMQModule();
                    moudle.RegisterModule(builder);
                    moudle.SetConfiguration(builder);
                })
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
