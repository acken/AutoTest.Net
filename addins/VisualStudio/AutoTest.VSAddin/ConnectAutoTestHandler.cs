using System;
using System.Collections.Generic;
using System.Text;
using AutoTest.Core.Configuration;

namespace AutoTest.VSAddin
{
    public partial class Connect
    {
        private void bootStrapAutoTest(string watchDirectory)
        {
            BootStrapper.Configure();
            BootStrapper.InitializeCache(watchDirectory);
        }

        private void terminateAutoTest()
        {
            BootStrapper.ShutDown();
        }
    }
}
