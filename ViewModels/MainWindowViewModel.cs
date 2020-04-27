using System.Collections.ObjectModel;
using ReactiveUI;
using SignalMan.Models;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace SignalMan.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IHubConnector _connector;
        private IApplicationLogService _logService;

        public string HubUrl { get; set; }
        public ReactiveCommand<Unit, Unit> Connect { get; }
        public ReactiveCommand<Unit, Unit> Disconnect { get; }
        public ObservableCollection<string> LogMessages { get; }

        public MainWindowViewModel(IHubConnector connector, IApplicationLogService logService)
        {
            _connector = connector;
            _logService = logService;

            LogMessages = new ObservableCollection<string>();
            logService.MessageReceived += (sender, message) => LogMessages.Add(message);

            Connect = ReactiveCommand.CreateFromTask(async () => await ConnectAsync(),
                Observable.FromAsync(async () => await Task.FromResult(true)));
            Disconnect = ReactiveCommand.CreateFromTask(async () => await DisconnectAsync(),
                Observable.FromAsync(async () => await Task.FromResult(true)));
        }

        private async Task<Unit> DisconnectAsync()
        {
            await _connector.DisconnectAsync();
            return Unit.Default;
        }

        private async Task<Unit> ConnectAsync()
        {
            await _connector.ConnectAsync(HubUrl);
            return Unit.Default;
        }
    }
}
