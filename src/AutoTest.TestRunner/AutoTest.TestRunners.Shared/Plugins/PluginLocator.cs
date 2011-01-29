using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Options;

namespace AutoTest.TestRunners.Shared.Plugins
{
    [Serializable]
    public class Plugin
    {
        public string Assembly { get; private set; }
        public string Type { get; private set; }

        public Plugin(string assembly, string type)
        {
            Assembly = assembly;
            Type = type;
        }

        public IAutoTestNetTestRunner New()
        {
            try
            {
                var asm = System.Reflection.Assembly.LoadFrom(Assembly);
                var runner = (IAutoTestNetTestRunner)asm.CreateInstance(Type);
                runner.SetLogger(new NullLogger());
                return runner;
            }
            catch
            {
                return null;
            }
        }
    }

    public class PluginLocator
    {
        private string _path;

        public PluginLocator()
        {
            _path = "TestRunners";
        }

        public PluginLocator(string path)
        {
            _path = path;
        }

        public IEnumerable<Plugin> Locate()
        {
            var plugins = new List<Plugin>();
            var currentDirectory = Environment.CurrentDirectory;
            try
            {
                Environment.CurrentDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                var files = Directory.GetFiles(_path);
                foreach (var file in files)
                    plugins.AddRange(getPlugins(file));
            }
            finally
            {
                Environment.CurrentDirectory = currentDirectory;
            }
            return plugins;
        }

        public IEnumerable<Plugin> GetPluginsFrom(RunOptions options)
        {
            var plugins = new List<Plugin>();
            foreach (var plugin in Locate())
            {
                var instance = plugin.New();
                if (options.TestRuns.Where(x => x.ID.Equals(instance.Identifier)).Count() > 0)
                    plugins.Add(plugin);
            }
            return plugins;
        }

        private IEnumerable<Plugin> getPlugins(string file)
        {
            var assembly = loadAssembly(Path.GetFullPath(file));
            if (assembly == null)
                return new Plugin[] {};
            return assembly
                .GetExportedTypes()
                .Where(x => x.GetInterfaces().Contains(typeof(IAutoTestNetTestRunner)) && x.GetConstructor(Type.EmptyTypes) != null)
                .Select(x => new Plugin(file, x.FullName));
        }

        private Assembly loadAssembly(string file)
        {
            try
            {
                return Assembly.LoadFile(file);
            }
            catch
            {
                return null;
            }
        }
    }
}
