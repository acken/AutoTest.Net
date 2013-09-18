using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using AutoTest.Messages;
using AutoTest.CoreExtensions;

namespace AutoTest.Core.ForeignLanguageProviders.Php
{
	public class PhpUnitRunner
    {
        public List<TestRunResults> Run(string arguments, string workingDirectory, Action<string> onLine) {
            var file = Path.GetTempFileName();
            if (File.Exists(file))
                File.Delete(file);
            var results = new List<TestRunResults>();
            var errors = new StringBuilder();
            var lastStatus = DateTime.Now;
            var proc = new Process();
            proc .Query(
                "phpunit",
                "--log-junit \"" + file + "\" --tap " + arguments,
                false,
                workingDirectory,
                (error, line) => {
                    if (error) {
                        errors.AppendLine(line);
                        return;
                    }
                    if (DateTime.Now.Subtract(lastStatus) > TimeSpan.FromMilliseconds(300)) {
                        onLine(line);
                        lastStatus = DateTime.Now;
                    }
                });
            
            if (errors.Length != 0) {
                results
                    .Add(
                        new TestRunResults(
                            "PHP Parse error",
                            "PHP Parse error",
                            true,
                            TestRunner.Any,
                            new[] {
                                PhpUnitParseErrorParser.Parse(errors.ToString())
                            }));
            }
            if (errors.Length == 0 && File.Exists(file)) {
                results.AddRange(
                    JUnitXmlParser.Parse(File.ReadAllText(file)));
            }
            return results;
        }
    }
}