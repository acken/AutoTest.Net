using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Messaging
{
    class BlockedMessage
    {
        private object _padLock = new object();
        private Stack<object> _messages = new Stack<object>();

        public Type Type { get; private set; }
        public bool HasBlockedMessages { get { lock (_padLock) { return _messages.Count > 0; } } }

        public BlockedMessage(Type type)
        {
            Type = type;
        }

        public void Push(object message)
        {
            lock (_padLock)
            {
                _messages.Push(message);
            }
        }

        public object Pop()
        {
            lock (_padLock)
            {
                return _messages.Pop();
            }
        }
    }
}
