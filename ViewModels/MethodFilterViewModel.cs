using System.Reactive;
using ReactiveUI;

namespace SignalMan.ViewModels
{
    public class MethodFilterViewModel : ViewModelBase
    {
        public string Name { get; }

        public ReactiveCommand<MethodFilterViewModel, Unit> Remove { get; }

        public MethodFilterViewModel(string name, ReactiveCommand<MethodFilterViewModel, Unit> remove)
        {
            Name = name;
            Remove = remove;
        }
    }
}