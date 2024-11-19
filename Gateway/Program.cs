using Autofac;
using Autofac.Extensions.DependencyInjection;
using NolowaBackendDotNet.Core.MessageQueue;
using NolowaNetwork;
using NolowaNetwork.Module;
using NolowaNetwork.System;
using NolowaNetwork.System.Worker;
using Serilog;

namespace Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("The application has been started!");

            var messageBus = host.Services.GetService<IMessageBus>() ?? throw new Exception();
            bool connectResult = messageBus.Connect(new()
            {
                Address = "localhost",
                ExchangeName = "nolowa.topic",
                VirtualHostName = "/",
                ServerName = "gateway",
                Port = 6672,
                UserName = "admin",
                Password = "adminasdf123!",
            });

            if(connectResult == false)
                throw new InvalidOperationException("RabbitMQ connection failed");

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory()) // autofac »ç¿ë
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterType<Gateway.MessageQueue.MessageHandler>().As<IMessageHandler>();
                    builder.RegisterType<Gateway.MessageQueue.MessageTypeResolver>().As<IMessageTypeResolver>();

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