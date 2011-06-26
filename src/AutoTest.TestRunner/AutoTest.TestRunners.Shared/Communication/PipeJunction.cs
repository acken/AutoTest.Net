using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using System.Threading;
using System.IO;

namespace AutoTest.TestRunners.Shared.Communication
{
    public class PipeJunction : IDisposable
    {
        private NamedPipeServerStream _server;
        private List<Thread> _pipes = new List<Thread>();
        private Stack<byte[]> _messages = new Stack<byte[]>();
        private bool _exit = false;

        public PipeJunction(string pipeName)
        {
            // The blocking synchronous way of using it is used here because there is a bug in mono
            // the PipeOptions enum does not have the correct int values compared to ms .net version
            _server = new NamedPipeServerStream(pipeName, PipeDirection.Out);
            var connect = new Thread(() => _server.WaitForConnection());
            _pipes.Add(connect);
            var server = new Thread(startServer);
            _pipes.Add(server);
            connect.Start();
            server.Start();
        }

        public void Combine(string pipe)
        {
            var handler = new Thread(run);
            _pipes.Add(handler);
            handler.Start(pipe);
        }

        private void run(object state)
        {
            new PipeClient().Listen(state.ToString(), (x) => { queue(x); });
        }

        private void queue(byte[] item)
        {
            _messages.Push(item);
        }

        private void startServer()
        {
            try
            {
                while (!_exit)
                {
                    if (!_server.IsConnected || _messages.Count == 0)
                    {
                        Thread.Sleep(15);
                        continue;
                    }
                    var bytes = _messages.Pop();
                    var buffer = new byte[bytes.Length + 1];
                    Array.Copy(bytes, buffer, bytes.Length);
                    buffer[buffer.Length - 1] = 0;
                    _server.Write(buffer, 0, buffer.Length);
                }
                if (_server.IsConnected)
                    _server.Disconnect();
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            _exit = true;
            Thread.Sleep(20);
            // Make sure we kill the waiting threads so the app can quit
            _pipes.ForEach(x => x.Abort());
        }
    }
}
