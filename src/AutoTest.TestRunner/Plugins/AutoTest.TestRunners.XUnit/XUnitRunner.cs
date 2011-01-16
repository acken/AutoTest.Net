using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Options;
using Xunit;

namespace AutoTest.TestRunners.XUnit
{
    class XUnitRunner
    {
        public void Run(RunnerOptions options)
        {
            XunitProject project = new XunitProject();
            foreach (var runner in options.Assemblies)
            {
                XunitProjectAssembly assembly = new XunitProjectAssembly
                {
                    AssemblyFilename = runner.Assembly,
                    ConfigFilename = null,
                    ShadowCopy = false
                };
                project.AddAssembly(assembly);

                foreach (XunitProjectAssembly asm in project.Assemblies)
                {
                    using (ExecutorWrapper wrapper = new ExecutorWrapper(asm.AssemblyFilename, asm.ConfigFilename, asm.ShadowCopy))
                    {
                        try
                        {
                            var logger = new XUnitLogger();
                            new TestRunner(wrapper, logger).RunAssembly();

                            //++totalAssemblies;
                            //totalTests += logger.TotalTests;
                            //totalFailures += logger.TotalFailures;
                            //totalSkips += logger.TotalSkips;
                            //totalTime += logger.TotalTime;
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
