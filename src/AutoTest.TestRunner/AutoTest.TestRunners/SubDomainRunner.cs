using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Plugins;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Errors;
using System.Threading;
using System.IO;

namespace AutoTest.TestRunners
{
    class SubDomainRunner
    {
        private Plugin _plugin;
        private IEnumerable<string> _categories;
        private AssemblyOptions _assembly;

        public SubDomainRunner(Plugin plugin, IEnumerable<string> categories, AssemblyOptions assembly)
        {
            _plugin = plugin;
            _categories = categories;
            _assembly = assembly;
        }

        public void Run(object waitHandle)
        {
            ManualResetEvent handle = null;
            if (waitHandle != null)
                handle = (ManualResetEvent)waitHandle;
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
                childDomain = AppDomain.CreateDomain(_plugin.Type + " app domain", null, domainSetup);

                // Create an instance of the runtime in the second AppDomain. 
                // A proxy to the object is returned.
                ITestRunner runtime = (ITestRunner)childDomain.CreateInstanceAndUnwrap(typeof(TestRunner).Assembly.FullName, typeof(TestRunner).FullName);

                // start the runtime.  call will marshal into the child runtime appdomain
                Program.AddResults(runtime.Run(_plugin, new RunSettings(_assembly, _categories)));
            }
            catch (Exception ex)
            {
                Program.AddResults(ErrorHandler.GetError(ex));
            }
            finally
            {
                if (childDomain != null)
                    AppDomain.Unload(childDomain);
                if (handle != null)
                    handle.Set();
                Program.WriteNow("Finished running tests for " + _assembly.Assembly);
            }
        }
    }
}
