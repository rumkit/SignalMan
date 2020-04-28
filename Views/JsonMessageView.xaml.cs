using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SignalMan.Views
{
    public class JsonMessageView : UserControl
    {
        public JsonMessageView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
