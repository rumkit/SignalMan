using System;

namespace SignalMan.Models
{
    internal class ApplicationLogService : IApplicationLogService
    {
        private static readonly Lazy<ApplicationLogService> Instance = new Lazy<ApplicationLogService>();

        public static ApplicationLogService Default => Instance.Value;

        public void PublishMessage(string message)
        {
            MessageReceived?.Invoke(this, message);
        }

        public event EventHandler<string> MessageReceived;
    }
}