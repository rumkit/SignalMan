using System;

namespace SignalMan.Models
{
    public interface IApplicationLogService
    {
        event EventHandler<string> MessageReceived;
    }
}