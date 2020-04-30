using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalMan.Models
{
    public delegate void JsonMessageReceived(string method, string text);

    public interface IHubConnector
    {
        Task ConnectAsync(string url);
        Task DisconnectAsync();

        void AddMethod(string method);
        void RemoveMethod(string method);

        event JsonMessageReceived MessageReceived;
    }
}
