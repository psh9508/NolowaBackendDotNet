using SharedLib.MessageQueue;

namespace Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            SetServiceEnvironment().Wait(TimeSpan.FromSeconds(10));

            app.Run();
        }

        private static async Task SetServiceEnvironment()
        {
            // Set MessageQueue
            var messageQueue = new MessageQueueService("gateway");
            
            await messageQueue.ConnectionAsync(new MessageQueueConnectionData()
            {
                HostName = "localhost",
                VirtualHostName = "/"
            }).ConfigureAwait(false);
        }
    }
}