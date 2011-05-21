using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Pipes;
using System.IO;

namespace AutoTest.TestRunners.Shared.Communication
{
    public class PipeServer : IDisposable
    {
        private string _name;
        private NamedPipeServerStream _server = null;
        private bool _exit = false;
        private Stack<string> _unsentMessages = new Stack<string>();

        public PipeServer(string name)
        {
            _name = name;
            var thread = new Thread(run);
            thread.Start(name);
        }

        public void Send(string message)
        {
            _unsentMessages.Push(message);
        }

        private void run(object state)
        {
            try
            {
                _server = new NamedPipeServerStream(state.ToString(), PipeDirection.Out);
                _server.WaitForConnection();
                while (_server.IsConnected)
                {
                    if (_exit && _unsentMessages.Count == 0)
                        break;
                    while (_unsentMessages.Count > 0)
                        send(_unsentMessages.Pop());
                    Thread.Sleep(15);
                }
            }
            catch
            {
            }
        }

        private void send(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            var base64Str = Convert.ToBase64String(bytes);
            var base64 = Encoding.UTF8.GetBytes(base64Str);
            var buffer = new byte[base64.Length + 1];
            Array.Copy(base64, buffer, base64.Length);
            buffer[buffer.Length - 1] = 0;
            _server.Write(buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            if (_server != null)
            {
                _exit = true;
                Thread.Sleep(20);
                if (_server.IsConnected)
                    _server.Disconnect();
                _server.Dispose();
            }   
        }
    }
}
