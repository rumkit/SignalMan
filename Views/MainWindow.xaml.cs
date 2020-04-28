using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SignalMan.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            var logTextBox = this.FindControl<TextBox>("LogTextBox");
            logTextBox.GetObservable(TextBox.TextProperty).Subscribe( _ => logTextBox.CaretIndex = int.MaxValue);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
