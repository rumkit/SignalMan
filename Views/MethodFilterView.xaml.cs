using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SignalMan.Views
{
    public class MethodFilterView : UserControl
    {
        public MethodFilterView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
