using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Pipes;

namespace AutoTest.TestRunners.Shared.Communication
{
    public class PipeClient
    {
        public void Listen(string pipe, Action<string> onRecieve)
        {
            using (var client = new NamedPipeClientStream(".", pipe, PipeDirection.In, PipeOptions.None))
            {
                client.Connect();
                while (client.IsConnected)
                {
                    var str = new StreamString(client).ReadString();
                    if (str == "<<exiting>>")
                        break;
                    onRecieve(str);
                }
            }
        }
    }
}
