using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using System.Reflection;
using System.IO;

namespace AutoTest.TestRunners.Shared.Targeting
{
    public class TestProcessSelector
    {
        public string Get(string testAssembly)
        {
            var locator = new AssemblyParser();
            return Get(locator.GetTargetFramework(testAssembly));
        }

        public string Get(Version version)
        {
            var path = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var executable = Path.Combine(path, string.Format("AutoTest.TestRunner.v{0}.{1}.exe", version.Major, version.Minor));
            if (!File.Exists(executable))
                executable = Path.Combine(path, "AutoTest.TestRunner.exe");
            return executable;
        }
    }
}
