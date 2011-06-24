using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Plugins;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Options;
using System.Runtime.Remoting;
using AutoTest.TestRunners.Shared.Errors;
using System.Reflection;
using System.Threading;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Communication;

namespace AutoTest.TestRunners
{
    class Program
    {
        private static Arguments _arguments;
        private static List<TestResult> _results = new List<TestResult>();
        
        static void Main(string[] args)
        {
            //args = new string[] { @"--input=C:\Users\ack\AppData\Local\Temp\tmp15F1.tmp", @"--output=C:\Users\ack\AppData\Local\Temp\tmp4463.tmp", "--startsuspended", "--silent" };
            //args = new string[] { @"--input=C:\Users\ack\AppData\Local\Temp\tmpEF7D.tmp", @"--output=C:\Users\ack\AppData\Local\Temp\tmp5F24.tmp" };
            var parser = new ArgumentParser(args);
            _arguments = parser.Parse();
            if (_arguments.Logging)
                Logger.SetLogger(new ConsoleLogger());
            writeHeader();
            if (!File.Exists(_arguments.InputFile) || _arguments.OutputFile == null)
            {
                printUseage();
                return;
            }
            Write("Test run options:");
            Write(File.ReadAllText(_arguments.InputFile));
            if (_arguments.StartSuspended)
                Console.ReadLine();
            tryRunTests();
            Write(" ");
            if (File.Exists(_arguments.OutputFile))
            {
                Write("Test run result:");
                Write(File.ReadAllText(_arguments.OutputFile));
            }
			
			// We do this since NUnit threads some times keep staing in running mode even after finished.
            //System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private static void writeHeader()
        {
            Write("AutoTest.TestRunner v0.1");
            Write("Author - Svein Arne Ackenhausen");
            Write("AutoTest.TestRunner is a plugin based generic test runner. ");
            Write("");
        }

        private static void tryRunTests()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledExceptionHandler;
            using (var junction = new PipeJunction(_arguments.Channel))
            {
                try
                {
                    var parser = new OptionsXmlReader(_arguments.InputFile);
                    parser.Parse();
                    if (!parser.IsValid)
                        return;

                    run(parser, junction);

                    var writer = new ResultsXmlWriter(_results);
                    writer.Write(_arguments.OutputFile);
                }
                catch (Exception ex)
                {
                    try
                    {
                        var result = new List<TestResult>();
                        result.Add(ErrorHandler.GetError("Init", ex));
                        var writer = new ResultsXmlWriter(result);
                        writer.Write(_arguments.OutputFile);
                    }
                    catch
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomainUnhandledExceptionHandler;
        }

        public static void CurrentDomainUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var message = getException((Exception)args.ExceptionObject);
            var result = new TestResult("Any", "", "", 0, "An unhandled exception was thrown while running a test", TestState.Panic, message);
            AddResults(result);
            var writer = new ResultsXmlWriter(_results);
            writer.Write(_arguments.OutputFile);
            if (args.IsTerminating)
                Environment.Exit(-1);
        }

        private static string getException(Exception ex)
        {
            var text = ex.ToString();
            if (ex.InnerException != null)
                text += getException(ex.InnerException);
            return text;
        }

        private static void printUseage()
        {
            Write("Syntax: AutoTest.TestRunner.exe --input=options_file --output=result_file [--startsuspended] [--silent] [--logging]");
            Write("");
            Write("Options format");
            Write("<=====================================================>");
            Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            Write("<run>");
            Write("\t<!--It can contain 0-n plugins. If 0 the runner will load all available plugins-->");
            Write("\t<plugin type=\"Plugin.IAutoTestNetTestRunner.Implementation\">C:\\Some\\Path\\PluginAssembly.dll</plugin>");
            Write("\t<!--It can contain 0-n runners. The id is what determines which runner will handle that run-->");
            Write("\t<runner id=\"NUnit\">");
            Write("\t\t<!--It can contain 0-n categories to ignore-->");
            Write("\t\t<categories>");
            Write("\t\t\t<ignore_category>IgnoreCategory</ignore_category>");
            Write("\t\t</categories>");
            Write("\t\t<!--It can contain 1-n assemblies to test.-->");
            Write("\t\t<test_assembly name=\"C:\\my\\testassembly.dll\">");
            Write("\t\t\t<!--It can contain 0-n tests-->");
            Write("\t\t\t<tests>");
            Write("\t\t\t\t<test>testassembly.class.test1</test>");
            Write("\t\t\t</tests>");
            Write("\t\t\t<!--It can contain 0-n members-->");
            Write("\t\t\t<members>");
            Write("\t\t\t\t<member>testassembly.class2</member>");
            Write("\t\t\t</members>");
            Write("\t\t\t<!--It can contain 0-n namespaces-->");
            Write("\t\t\t<namespaces>");
            Write("\t\t\t\t<namespace>testassembly.somenamespace1</namespace>");
            Write("\t\t\t</namespaces>");
            Write("\t\t</test_assembly>");
            Write("\t</runner>");
            Write("</run>");
        }

        private static void run(OptionsXmlReader parser, PipeJunction junction)
        {
            var handles = new List<ManualResetEvent>();
            foreach (var plugin in getPlugins(parser))
            {
                foreach (var run in parser.Options.TestRuns)
                {
                    foreach (var assembly in run.Assemblies)
                    {
                        WriteNow("Running tests for " + assembly.Assembly + " using " + plugin.Type);
                        var pipeName = Guid.NewGuid().ToString();
                        junction.Combine(pipeName);
                        var process = new SubDomainRunner(plugin, run.ID, run.Categories, assembly, _arguments.Logging, pipeName);
                        if (_arguments.RunInParallel)
                        {
                            var handle = new ManualResetEvent(false);
                            handles.Add(handle);
                            ThreadPool.QueueUserWorkItem(process.Run, handle);
                        }
                        else
                            process.Run(null);
                    }
                }
            }
            if (_arguments.RunInParallel)
                WaitHandle.WaitAll(handles.ToArray());
        }

        public static void AddResults(IEnumerable<TestResult> results)
        {
            lock(_results)
            {
                _results.AddRange(results);
            }
        }

        public static void AddResults(TestResult result)
        {
            lock (_results)
            {
                _results.Add(result);
            }
        }

        private static IEnumerable<Plugin> getPlugins(OptionsXmlReader parser)
        {
            if (parser.Plugins.Count() == 0)
                return allPlugins();
            return parser.Plugins;
        }

        private static IEnumerable<Plugin> allPlugins()
        {
            var currentDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var dir = Path.Combine(currentDir, "TestRunners");
            if (!Directory.Exists(dir))
                return new Plugin[] { };
            var locator = new PluginLocator(dir);
            return locator.Locate();
        }

        public static void WriteNow(string message)
        {
            Write(string.Format("{0} {1}", DateTime.Now.ToLongTimeString(), message));
        }

        public static void Write(string message)
        {
            if (!_arguments.Silent)
                Console.WriteLine(message);
        }
    }
}
