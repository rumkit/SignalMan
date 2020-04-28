using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalMan.Models
{
    public interface IHubConnector
    {
        Task ConnectAsync(string url, IEnumerable<HubMethodHandler> methodHandlers = null);
        Task DisconnectAsync();
    }
}
