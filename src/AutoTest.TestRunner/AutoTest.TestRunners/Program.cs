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

namespace AutoTest.TestRunners
{
    class Program
    {
        private static Arguments _arguments;
        private static List<TestResult> _results = new List<TestResult>();
        private static string _currentRunner = "";

        static void Main(string[] args)
        {
            //args = new string[] { @"--input=C:\Users\ack\AppData\Local\Temp\tmp15F1.tmp", @"--output=C:\Users\ack\AppData\Local\Temp\tmp4463.tmp", "--startsuspended", "--silent" };
            //args = new string[] { @"--input=C:\Users\ack\AppData\Local\Temp\tmp5F23.tmp", @"--output=C:\Users\ack\AppData\Local\Temp\tmp5F24.tmp", "--silent" };
            var parser = new ArgumentParser(args);
            _arguments = parser.Parse();
            writeHeader();
            if (!File.Exists(_arguments.InputFile) || _arguments.OutputFile == null)
            {
                printUseage();
                return;
            }
            if (_arguments.StartSuspended)
                Console.ReadLine();
            tryRunTests();
        }

        private static void writeHeader()
        {
            write("AutoTest.TestRunner v0.1");
            write("Author - Svein Arne Ackenhausen");
            write("AutoTest.TestRunner is a plugin based generic test runner. ");
            write("");
        }

        private static void tryRunTests()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledExceptionHandler;
            try
            {
                var parser = new OptionsXmlReader(_arguments.InputFile);
                parser.Parse();
                if (!parser.IsValid)
                    return;

                run(parser);

                var writer = new ResultsXmlWriter(_results);
                writer.Write(_arguments.OutputFile);
            }
            catch (Exception ex)
            {
                try
                {
                    var result = new List<TestResult>();
                    result.Add(ErrorHandler.GetError(ex));
                    var writer = new ResultsXmlWriter(result);
                    writer.Write(_arguments.OutputFile);
                }
                catch
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        static void CurrentDomainUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            _results.Add(ErrorHandler.GetError(_currentRunner, args.ExceptionObject.ToString()));
            var writer = new ResultsXmlWriter(_results);
            writer.Write(_arguments.OutputFile);
            Environment.Exit(-1);
        }

        private static void printUseage()
        {
            write("Syntax: AutoTest.TestRunner.exe --input=options_file --output=result_file [--startsuspended] [--silent]");
            write("");
            write("Options format");
            write("<=====================================================>");
            write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            write("<run>");
            write("\t<!--It can contain 0-n plugins. If 0 the runner will load all available plugins-->");
            write("\t<plugin type=\"Plugin.IAutoTestNetTestRunner.Implementation\">C:\\Some\\Path\\PluginAssembly.dll</plugin>");
            write("\t<!--It can contain 0-n runners. The id is what determines which runner will handle that run-->");
            write("\t<runner id=\"NUnit\">");
            write("\t\t<!--It can contain 0-n categories to ignore-->");
            write("\t\t<categories>");
            write("\t\t\t<ignore_category>IgnoreCategory</ignore_category>");
            write("\t\t</categories>");
            write("\t\t<!--It can contain 1-n assemblies to test. Framework is optional-->");
            write("\t\t<test_assembly name=\"C:\\my\\testassembly.dll\" framework=\"3.5\">");
            write("\t\t\t<!--It can contain 0-n tests-->");
            write("\t\t\t<tests>");
            write("\t\t\t\t<test>testassembly.class.test1</test>");
            write("\t\t\t</tests>");
            write("\t\t\t<!--It can contain 0-n members-->");
            write("\t\t\t<members>");
            write("\t\t\t\t<member>testassembly.class2</member>");
            write("\t\t\t</members>");
            write("\t\t\t<!--It can contain 0-n namespaces-->");
            write("\t\t\t<namespaces>");
            write("\t\t\t\t<namespace>testassembly.somenamespace1</namespace>");
            write("\t\t\t</namespaces>");
            write("\t\t</test_assembly>");
            write("\t</runner>");
            write("</run>");
        }

        private static void run(OptionsXmlReader parser)
        {
            foreach (var plugin in getPlugins(parser))
            {
                runByRunner(plugin, parser.Options);
            }
        }

        private static void runByRunner(Plugin plugin, RunOptions options)
        {
            try
            {
                runInSubDomain(plugin, options);
            }
            catch (Exception ex)
            {
                _results.Add(ErrorHandler.GetError(ex));
            }
        }

        private static void runInSubDomain(Plugin plugin, RunOptions options)
        {
            AppDomain childDomain = null;
            try
            {
                // Construct and initialize settings for a second AppDomain.
                AppDomainSetup domainSetup = new AppDomainSetup()
                {
                    ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                    ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
                    ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName,
                    LoaderOptimization = LoaderOptimization.MultiDomainHost
                };

                // Create the child AppDomain used for the service tool at runtime.
                childDomain = AppDomain.CreateDomain(plugin.Type + " app domain", null, domainSetup);

                // Create an instance of the runtime in the second AppDomain. 
                // A proxy to the object is returned.
                ITestRunner runtime = (ITestRunner)childDomain.CreateInstanceAndUnwrap(typeof(TestRunner).Assembly.FullName, typeof(TestRunner).FullName);

                // start the runtime.  call will marshal into the child runtime appdomain
                _results.AddRange(runtime.Run(plugin, options));
            }
            finally
            {
                if (childDomain != null)
                    AppDomain.Unload(childDomain);
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

        private static void write(string message)
        {
            if (!_arguments.Silent)
                Console.WriteLine(message);
        }
    }
}
