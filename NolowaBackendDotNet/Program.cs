using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
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
using NolowaNetwork.System;
using NolowaNetwork.System.Worker;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using NolowaBackendDotNet.Services;
using NolowaBackendDotNet.Core;
using SharedLib.Dynamodb.Service;

namespace NolowaBackendDotNet
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("The application has been started!");

           var messageBus = host.Services.GetService<IMessageBus>() ?? throw new Exception();
            messageBus.Connect(new()
            {
                Address = "localhost",
                ExchangeName = "nolowa.topic",
                VirtualHostName = "/",
                ServerName = "apiserver",
                Port = 6672,
                UserName = "admin",
                Password = "adminasdf123!",
            });

            await MakeDefaultDatabaseAsync(host.Services.GetService<IAmazonDynamoDB>());

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory()) // autofac 사용
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    #region Server
                    builder.RegisterType<AccountsService>().As<IAccountsService>();
                    builder.RegisterType<AuthenticationService>().As<IAuthenticationService>();
                    builder.RegisterType<PostsService>().As<IPostsService>();
                    builder.RegisterType<SearchService>().As<ISearchService>();
                    builder.RegisterType<JWTTokenProvider>().As<IJWTTokenProvider>();
                    builder.RegisterType<DirectMessageService>().As<IDirectMessageService>();
                    #endregion

                    #region MessageQueue
                    builder.RegisterType<MessageHandler>().As<IMessageHandler>();
                    builder.RegisterType<Core.MessageQueue.MessageTypeResolver>().As<IMessageTypeResolver>();
                    #endregion

                    #region Dynamodb
                    builder.RegisterInstance<IAmazonDynamoDB>(new AmazonDynamoDBClient(new AmazonDynamoDBConfig()
                    {
                        ServiceURL = "http://localhost:8000",
                        Timeout = TimeSpan.FromSeconds(3),
                    }));

                    builder.RegisterType<DynamoDBContext>().As<IDynamoDBContext>().InstancePerLifetimeScope();
                    builder.RegisterType<DdbService>().As<IDbService>().InstancePerLifetimeScope();
                    #endregion

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


        private async static Task MakeDefaultDatabaseAsync(IAmazonDynamoDB lowlevelClient)
        {
            List<string> currentTables = new();

            try
            {
                var tableList = await lowlevelClient.ListTablesAsync();
                currentTables = tableList.TableNames;
            }
            catch (Exception ex)
            {
                // 접속 실패
                throw;
            }

            if (!currentTables.Contains("NolowaDatabase"))
            {
                var request = new CreateTableRequest
                {
                    TableName = "NolowaDatabase",
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "PK",
                            AttributeType = "S"
                        },
                        new AttributeDefinition
                        {
                            AttributeName = "SK",
                            AttributeType = "S"
                        },
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        // "HASH" = hash key, "RANGE" = range key.
                        new KeySchemaElement
                        {
                          AttributeName = "PK",
                          KeyType = "HASH"
                        },
                        new KeySchemaElement
                        {
                          AttributeName = "SK",
                          KeyType = "RANGE"
                        },
                    },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 10,
                        WriteCapacityUnits = 5
                    },
                };

                var response = await lowlevelClient.CreateTableAsync(request);

                Console.WriteLine("Table created with request ID: " + response.ResponseMetadata.RequestId);
            }
        }
    }
}
