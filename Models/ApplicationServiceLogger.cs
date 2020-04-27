using System;
using Microsoft.Extensions.Logging;

namespace SignalMan.Models
{
    internal class ApplicationServiceLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            ApplicationLogService.Default.PublishMessage(formatter(state, exception));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new Scope<TState>(state);
        }

        private class Scope<TState> : IDisposable
        {
            private readonly TState _state;
            public Scope(TState state)
            {
                ApplicationLogService.Default.PublishMessage($"New scope: {_state}");
                _state = state;
            }

            public void Dispose()
            {
                ApplicationLogService.Default.PublishMessage($"Scope end: {_state}");
            }
        }
    }
}