using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace SignalMan.Models
{
    class SignalRHubConnector : IHubConnector
    {
        private HubConnection _connection;

        public async Task ConnectAsync(string url)
        {
            if (_connection != null)
                throw new InvalidOperationException("Connection already exists");

            _connection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .ConfigureLogging(logging =>
                {
                    logging.AddProvider(new ApplicationServiceLoggerProvider());
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .Build();

            await ListenToEventsAsync(_connection);
        }

        public async Task DisconnectAsync()
        {
            await _connection.DisposeAsync();
            _connection = null;
        }

        private static async Task ListenToEventsAsync(HubConnection connection)
        {
            connection.On<dynamic>("nextdevicelist", (message) =>
            {
                Console.WriteLine(message);
            });

            await connection.StartAsync();
        }
    }
}
