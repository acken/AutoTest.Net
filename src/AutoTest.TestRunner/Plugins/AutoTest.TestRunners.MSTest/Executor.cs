using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace AutoTest.TestRunners.MSTest
{
    class Executor : IDisposable
    {
        private Assembly _assembly;
        private Type _executorType;
        private object _executor;
        private Type _commandFactoryType;
        private StreamWriter _outputWriter = null;

        public Executor()
        {
            string assemblyPath = @"C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\PrivateAssemblies\Microsoft.VisualStudio.QualityTools.CommandLine.dll";
            _assembly = Assembly.LoadFrom(assemblyPath);
            _executorType = _assembly.GetType("Microsoft.VisualStudio.TestTools.CommandLine.Executor");
            _executor = Activator.CreateInstance(_executorType);
            setupOutput();
            setupArguments();
        }

        public bool Execute()
        {
            return (bool)_executorType.GetMethod("Execute").Invoke(_executor, null);
        }

        private void setupArguments()
        {
            _commandFactoryType = _assembly.GetType("Microsoft.VisualStudio.TestTools.CommandLine.CommandFactory");
            addArgument("/testcontainer", @"C:\Users\ack\src\TestProjectWithTestList2010\TestProjectWithTestList\bin\AutoTest.NET\TestProject2.dll");
            addArgument("/nologo", null);
            addArgument("/noisolation", null);
            //addArgument("/testmetadata", testMetadataPath);
            addArgument("/resultsfile", @"C:\tmp\blah.txt");
            //addArgument("/runconfig", runConfigPath);
            //addArgument("/searchpathroot", searchPathRoot);
            //addArgument("/testlist", SelectedTestListName);
        }

        private void addArgument(string command, string argument)
        {
            var cmd = _commandFactoryType.GetMethod("CreateCommand").Invoke(null, new object[] { command, argument });
            _executor.GetType().GetMethod("Add").Invoke(_executor, new object[] { cmd });
        }

        private void setupOutput()
        {
            PropertyInfo outputProperty = _executorType.GetProperty("Output", BindingFlags.Public | BindingFlags.Static);
            object output = outputProperty.GetValue(_executor, null);
            FieldInfo standardOutputField = output.GetType().GetField("m_standardOutput", BindingFlags.NonPublic | BindingFlags.Instance);
            _outputWriter = new StreamWriter(@"C:\tmp\meh.txt");
            standardOutputField.SetValue(output, _outputWriter);
        }

        public void Dispose()
        {
            if (_outputWriter != null)
                _outputWriter.Dispose();
        }
    }
}
