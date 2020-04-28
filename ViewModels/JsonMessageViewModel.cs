using System;

namespace SignalMan.ViewModels
{
    public class JsonMessageViewModel : ViewModelBase
    {
        public string Method { get; set; }
        public DateTime Received { get; set; }
        public string Text { get; set; }
    }
}