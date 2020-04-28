using System;
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

        private string _methodFilterName;

        public string MethodFilterName
        {
            get => _methodFilterName;
            set => this.RaiseAndSetIfChanged(ref _methodFilterName, value);
        }

        public ObservableCollection<MethodFilterViewModel> MethodFilters { get; } = new ObservableCollection<MethodFilterViewModel>();

        public ReactiveCommand<Unit, Unit> Connect { get; }
        public ReactiveCommand<Unit, Unit> Disconnect { get; }
        public ReactiveCommand<Unit, Unit> AddMethodFilter { get; }
        public ReactiveCommand<MethodFilterViewModel, Unit> RemoveMethodFilter { get; }

        public MainWindowViewModel(IHubConnector connector, IApplicationLogService logService)
        {
            _connector = connector;
            _logService = logService;

            logService.MessageReceived += (sender, message) => LogMessages += $"{message}{Environment.NewLine}";
            
            var canConnect = this.WhenAnyValue(
                x => x.HubUrl, x => x.IsConnected,
                (hubUrl, isConnected) => 
                    !string.IsNullOrEmpty(hubUrl) && !isConnected);

            var canDisconnect = this.WhenAnyValue(
                x => x.HubUrl, x => x.IsConnected,
                (hubUrl, isConnected) => 
                    !string.IsNullOrEmpty(hubUrl) && isConnected);

            var canAddFilter = this.WhenAnyValue(
                x => x.MethodFilterName,
                (name) => !string.IsNullOrWhiteSpace(name));

            Connect = ReactiveCommand.CreateFromTask(async () => await ConnectAsync(),
                canConnect);

            Disconnect = ReactiveCommand.CreateFromTask(async () => await DisconnectAsync(),
                canDisconnect);

            AddMethodFilter = ReactiveCommand.CreateFromTask(async () => await AddMethodFilterAsync(),
                canAddFilter);

            RemoveMethodFilter = ReactiveCommand.CreateFromTask<MethodFilterViewModel, Unit>(async (method) => await RemoveMethodFilterAsync(method));
        }

        private async Task<Unit> RemoveMethodFilterAsync(MethodFilterViewModel method)
        {
            MethodFilters.Remove(method);
            return await Task.FromResult(Unit.Default);
        }

        private async Task<Unit> AddMethodFilterAsync()
        {
            MethodFilters.Add(new MethodFilterViewModel(MethodFilterName, RemoveMethodFilter));
            MethodFilterName = string.Empty;
            return await Task.FromResult(Unit.Default);
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
