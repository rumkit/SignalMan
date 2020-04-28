using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
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
        private readonly JsonFormatter _jsonFormatter = new JsonFormatter();

        public event JsonMessageReceived MessageReceived;

        public async Task ConnectAsync(string url, IEnumerable<string> methodSubscriptions = null)
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

            SubscribeToEvents(_connection, methodSubscriptions);

            await _connection.StartAsync(_cts.Token);
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

        private void SubscribeToEvents(HubConnection connection, IEnumerable<string> subscriptions)
        {
            if (subscriptions == null)
                return;

            foreach (var methodName in subscriptions)
            {
                connection.On<JsonElement>(methodName, async (element) =>
                {
                    var jsonString = await _jsonFormatter.ConvertToStringAsync(element);
                    MessageReceived?.Invoke(methodName, jsonString);
                });
            }
        }
    }
}
