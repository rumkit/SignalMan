using System;

namespace SignalMan.Models
{
    public class HubMethodHandler
    {
        public string MethodName { get; set; }
        public Action<object> MethodCallback { get;set; }
    }
}
