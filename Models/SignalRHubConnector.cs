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

        public async Task ConnectAsync(string url, IEnumerable<HubMethodHandler> methodHandlers = null)
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

            SubscribeToEvents(_connection, methodHandlers);

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

        private void SubscribeToEvents(HubConnection connection, IEnumerable<HubMethodHandler> methodHandlers)
        {
            if (methodHandlers == null)
                return;

            foreach (var handler in methodHandlers)
            {
                connection.On<JsonElement>(handler.MethodName, (element) =>
                {
                    var jsonString = HandleJsonElement(element);
                    handler.MethodCallback(jsonString);
                });
            }
        }

        private string HandleJsonElement(JsonElement element)
        {
            var writerOptions = new JsonWriterOptions
            {
                Indented = true
            };

            var documentOptions = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip
            };
            using var ms = new MemoryStream();
            using var writer = new Utf8JsonWriter(ms, writerOptions);

            if (element.ValueKind == JsonValueKind.Object)
            {
                writer.WriteStartObject();
            }
            else
            {
                return null;
            }

            foreach (JsonProperty property in element.EnumerateObject())
            {
                property.WriteTo(writer);
            }

            writer.WriteEndObject();
            writer.Flush();

            using var reader = new StreamReader(new MemoryStream(ms.ToArray()), Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}
