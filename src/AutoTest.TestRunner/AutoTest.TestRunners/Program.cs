using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Plugins;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Options;

namespace AutoTest.TestRunners
{
    class Program
    {
        private static Arguments _arguments;

        static void Main(string[] args)
        {
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
            try
            {
                var parser = new OptionsXmlReader(_arguments.InputFile);
                parser.Parse();
                if (!parser.IsValid)
                    return;

                var result = run(parser);

                var writer = new ResultsXmlWriter(result);
                writer.Write(_arguments.OutputFile);
            }
            catch (Exception ex)
            {
                try
                {
                    var result = new List<TestResult>();
                    result.Add(new TestResult("", "AutoTest.TestRunner.exe internal error", "", 0, "", TestState.Panic, ex.ToString()));
                    var writer = new ResultsXmlWriter(result);
                    writer.Write(_arguments.OutputFile);
                }
                catch
                {
                    Console.WriteLine(ex.ToString());
                }
            }
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

        private static IEnumerable<TestResult> run(OptionsXmlReader parser)
        {
            var results = new List<TestResult>();
            foreach (var runner in getRunners(parser))
            {
                foreach (var testRun in parser.Options.TestRuns)
                {
                    if (runner.Handles(testRun.ID))
                        results.AddRange(runner.Run(testRun));
                }
            }
            return results;
        }

        private static IEnumerable<IAutoTestNetTestRunner> getRunners(OptionsXmlReader parser)
        {
            if (parser.Plugins.Count() == 0)
                return allPlugins();
            return getRunnersFrom(parser.Plugins);
        }

        private static IEnumerable<IAutoTestNetTestRunner> getRunnersFrom(IEnumerable<Plugin> plugins)
        {
            var runners = new List<IAutoTestNetTestRunner>();
            foreach (var plugin in plugins)
            {
                var runner = plugin.New();
                if (runner != null)
                    runners.Add(runner);
            }
            return runners;
        }

        private static IEnumerable<IAutoTestNetTestRunner> allPlugins()
        {
            var dir = Path.GetFullPath("TestRunners");
            if (!Directory.Exists(dir))
                return new IAutoTestNetTestRunner[] { };
            var locator = new PluginLocator(dir);
            return getRunnersFrom(locator.Locate());
        }

        private static void write(string message)
        {
            if (!_arguments.Silent)
                Console.WriteLine(message);
        }
    }
}
