using Microsoft.Extensions.Logging;

namespace SignalMan.Models
{
    internal class ApplicationServiceLoggerProvider : ILoggerProvider
    {
        public void Dispose()
        {
            
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ApplicationServiceLogger();
        }
    }
}