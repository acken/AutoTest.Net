using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoTest.WinForms
{
    public enum MessageType
    {
        Information,
        Warning,
        Error
    }

    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageType Type { get; private set; }

        public MessageRecievedEventArgs(MessageType type)
        {
            Type = type;
        }
    }

    public interface IInformationForm
    {
        event EventHandler<MessageRecievedEventArgs> MessageArrived;

        Form Form { get; }
    }
}
