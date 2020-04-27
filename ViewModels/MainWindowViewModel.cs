using System;
using System.Collections.ObjectModel;
using System.Linq;
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
        private readonly IApplicationLogService _logService;

        private string _hubUrl;
        public string HubUrl
        {
            get => _hubUrl;
            set => this.RaiseAndSetIfChanged(ref _hubUrl, value);
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => this.RaiseAndSetIfChanged(ref _isConnected, value);
        }

        private string _logMessages = string.Empty;
        public string LogMessages
        {
            get => _logMessages;
            set => this.RaiseAndSetIfChanged(ref _logMessages, value);
        }

        public ReactiveCommand<Unit, Unit> Connect { get; }
        public ReactiveCommand<Unit, Unit> Disconnect { get; }

        

        public MainWindowViewModel(IHubConnector connector, IApplicationLogService logService)
        {
            _connector = connector;
            _logService = logService;

            logService.MessageReceived += (sender, message) => LogMessages += $"{message}{Environment.NewLine}";
            
            var canConnect = this.WhenAnyValue(
                x => x.HubUrl, x => x.IsConnected,
                (hubUrl, isConnected) => 
                    !string.IsNullOrEmpty(hubUrl) && !isConnected);

            Connect = ReactiveCommand.CreateFromTask(async () => await ConnectAsync(),
                canConnect);

            Disconnect = ReactiveCommand.CreateFromTask(async () => await DisconnectAsync());
        }
        
        private async Task<Unit> DisconnectAsync()
        {
            await _connector.DisconnectAsync();
            IsConnected = false;
            return Unit.Default;
        }

        private async Task<Unit> ConnectAsync()
        {
            IsConnected = true;
            try
            {
                await _connector.ConnectAsync(HubUrl);
            }
            catch
            {
                _logService.PublishMessage($"Connection to {HubUrl} failed");
                IsConnected = false;
            }
            return Unit.Default;
        }
    }
}
