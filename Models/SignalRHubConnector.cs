using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace SignalMan.Models
{
    class SignalRHubConnector : IHubConnector
    {
        private HubConnection _connection;
        private CancellationTokenSource _cts;

        public async Task ConnectAsync(string url)
        {
            if (_connection != null)
                throw new InvalidOperationException("Connection already exists");
            _cts = new CancellationTokenSource();
            _connection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .ConfigureLogging(logging =>
                {
                    logging.AddProvider(new ApplicationServiceLoggerProvider());
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .Build();

            await ListenToEventsAsync(_connection, _cts.Token);
        }

        public async Task DisconnectAsync()
        {
            try
            {
                _cts.Cancel();
                await _connection.DisposeAsync();
            }
            finally
            {
                _connection = null;
            }
        }

        private static async Task ListenToEventsAsync(HubConnection connection, CancellationToken token)
        {

            connection.On<dynamic>("nextdevicelist", (message) =>
            {
                Console.WriteLine(message);
            });

            await connection.StartAsync(token);
        }
    }
}
