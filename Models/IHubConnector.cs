using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalMan.Models
{
    public delegate void JsonMessageReceived(string method, string text);

    public interface IHubConnector
    {
        Task ConnectAsync(string url, IEnumerable<string> methodSubscriptions = null);
        Task DisconnectAsync();

        event JsonMessageReceived MessageReceived;
    }
}
