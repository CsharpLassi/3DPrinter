using System;

namespace DSVG
{
    public class ActionInterface : WindowInterface
    {
        private Action<string> _callback = null;

        public ActionInterface(string name, Action<string> callback)
        {
            Name = name;
            _callback = callback;
        }

        public override void OnInteract(string input)
        {
            if (_callback != null)
            {
                _callback(input);
            }
        }
    }
}

