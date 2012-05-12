using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Communication;

namespace AutoTest.TestRunners.Shared.Options
{
    [Serializable]
    public class RunSettings
    {
        public AssemblyOptions Assembly { get; private set; }
        public string[] IgnoreCategories { get; private set; }
        public ConnectionOptions ConnectOptions { get; private set; }

        public RunSettings(AssemblyOptions assembly, string[] ignoreCategories, ConnectionOptions options)
        {
            Assembly = assembly;
            IgnoreCategories = ignoreCategories;
            ConnectOptions = options;
        }
    }
}
