using System.Threading.Tasks;

namespace SignalMan.Models
{
    public interface IHubConnector
    {
        Task ConnectAsync(string url);
        Task DisconnectAsync();
    }
}
