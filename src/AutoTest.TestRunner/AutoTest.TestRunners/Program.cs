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
        static void Main(string[] args)
        {
            //args = new string[] { "testrun.xml", "testoutput.xml" };
            if (args.Length != 2)
            {
                printUseage();
                return;
            }
            var parser = new OptionsXmlReader(args[0]);
            parser.Parse();
            if (!parser.IsValid)
                return;

            var result = run(parser);

            var writer = new ResultsXmlWriter(result);
            writer.Write(args[1]);
        }

        private static void printUseage()
        {
            Console.WriteLine("AutoTest.TestRunner v0.1");
            Console.WriteLine("Author - Svein Arne Ackenhausen");
            Console.WriteLine("AutoTest.TestRunner is a plugin based generic test runner. ");
            Console.WriteLine("");
            Console.WriteLine("Syntax: AutoTest.TestRunner.exe OPTIONS_FILE RESULT_FILE");
            Console.WriteLine("");
            Console.WriteLine("Options format");
            Console.WriteLine("<=====================================================>");
            Console.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            Console.WriteLine("<run>");
            Console.WriteLine("\t<!--It can contain 0-n plugins. If 0 the runner will load all available plugins-->");
            Console.WriteLine("\t<plugin type=\"Plugin.IAutoTestNetTestRunner.Implementation\">C:\\Some\\Path\\PluginAssembly.dll</plugin>");
            Console.WriteLine("\t<!--It can contain 0-n runners. The id is what determines which runner will handle that run-->");
            Console.WriteLine("\t<runner id=\"NUnit\">");
            Console.WriteLine("\t\t<!--It can contain 0-n categories to ignore-->");
            Console.WriteLine("\t\t<categories>");
            Console.WriteLine("\t\t\t<ignore_category>IgnoreCategory</ignore_category>");
            Console.WriteLine("\t\t</categories>");
            Console.WriteLine("\t\t<!--It can contain 1-n assemblies to test. Framework is optional-->");
            Console.WriteLine("\t\t<test_assembly name=\"C:\\my\\testassembly.dll\" framework=\"3.5\">");
            Console.WriteLine("\t\t\t<!--It can contain 0-n tests-->");
            Console.WriteLine("\t\t\t<tests>");
            Console.WriteLine("\t\t\t\t<test>testassembly.class.test1</test>");
            Console.WriteLine("\t\t\t</tests>");
            Console.WriteLine("\t\t\t<!--It can contain 0-n members-->");
            Console.WriteLine("\t\t\t<members>");
            Console.WriteLine("\t\t\t\t<member>testassembly.class2</member>");
            Console.WriteLine("\t\t\t</members>");
            Console.WriteLine("\t\t\t<!--It can contain 0-n namespaces-->");
            Console.WriteLine("\t\t\t<namespaces>");
            Console.WriteLine("\t\t\t\t<namespace>testassembly.somenamespace1</namespace>");
            Console.WriteLine("\t\t\t</namespaces>");
            Console.WriteLine("\t\t</test_assembly>");
            Console.WriteLine("\t</runner>");
            Console.WriteLine("</run>");
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
    }
}
