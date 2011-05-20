using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Pipes;

namespace AutoTest.TestRunners.Shared.Communication
{
    public class PipeServer : IDisposable
    {
        private string _name;
        private NamedPipeServerStream _server = null;
        private bool _exit = false;

        public PipeServer(string name)
        {
            _name = name;
            var thread = new Thread(run);
            thread.Start(name);
        }

        public void Send(string message)
        {
            if (_server == null)
                return;
            new StreamString(_server).WriteString(message);
        }

        private void run(object state)
        {
            try
            {
                _server = new NamedPipeServerStream(state.ToString(), PipeDirection.Out);
                _server.WaitForConnection();
                while (_server.IsConnected)
                {
                    if (_exit)
                    {
                        new StreamString(_server).WriteString("<<exiting>>");
                        break;
                    }
                    Thread.Sleep(15);
                }
            }
            catch
            {
            }
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
