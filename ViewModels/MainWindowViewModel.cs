using System.Collections.ObjectModel;
using ReactiveUI;
using SignalMan.Models;
using System.Reactive;
using System.Threading.Tasks;

namespace SignalMan.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IHubConnector _connector;
        private IApplicationLogService _logService;

        private string _hubUrl;

        public string HubUrl
        {
            get => _hubUrl;
            set => this.RaiseAndSetIfChanged(ref _hubUrl, value);
        }

        public ReactiveCommand<Unit, Unit> Connect { get; }
        public ReactiveCommand<Unit, Unit> Disconnect { get; }
        public ObservableCollection<string> LogMessages { get; }

        public MainWindowViewModel(IHubConnector connector, IApplicationLogService logService)
        {
            _connector = connector;
            _logService = logService;

            LogMessages = new ObservableCollection<string>();
            logService.MessageReceived += (sender, message) => LogMessages.Add(message);

            var canConnect = this.WhenAnyValue(
                x => x.HubUrl,
                (hubUrl) => 
                    !string.IsNullOrEmpty(hubUrl));

            Connect = ReactiveCommand.CreateFromTask(async () => await ConnectAsync(),
                canConnect);

            Disconnect = ReactiveCommand.CreateFromTask(async () => await DisconnectAsync());
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
