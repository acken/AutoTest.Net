using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Plugins;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Errors;

namespace AutoTest.TestRunners
{
    class SubDomainRunner
    {
        private Plugin _plugin;
        private RunOptions _options;

        public SubDomainRunner(Plugin plugin, RunOptions options)
        {
            _plugin = plugin;
            _options = options;
        }

        public void Run()
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
                childDomain = AppDomain.CreateDomain(_plugin.Type + " app domain", null, domainSetup);

                // Create an instance of the runtime in the second AppDomain. 
                // A proxy to the object is returned.
                ITestRunner runtime = (ITestRunner)childDomain.CreateInstanceAndUnwrap(typeof(TestRunner).Assembly.FullName, typeof(TestRunner).FullName);

                // start the runtime.  call will marshal into the child runtime appdomain
                Program.AddResults(runtime.Run(_plugin, _options));
            }
            catch (Exception ex)
            {
                Program.AddResults(ErrorHandler.GetError(ex));
            }
            finally
            {
                if (childDomain != null)
                    AppDomain.Unload(childDomain);
            }
        }
    }
}
