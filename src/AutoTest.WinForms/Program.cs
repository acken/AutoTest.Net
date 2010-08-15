using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AutoTest.Core.Configuration;
using System.Reflection;
using Castle.MicroKernel.Registration;
using System.IO;
using AutoTest.Core.Messaging;
using AutoTest.Core.FileSystem;

namespace AutoTest.WinForms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            tryStartApplication(args);
        }

        private static void  tryStartApplication(string[] args)
		{
			try
			{
				if (userWantedCommandLineHelpPrinted(args))
					return;
                string directoryToWatch = getPossibleCommandArgs(args);
                if ((directoryToWatch = ConfigureApplication(directoryToWatch)) == null)
                    return;
        	    var overviewForm = BootStrapper.Services.Locate<IOverviewForm>();
                overviewForm.SetWatchDirectory(directoryToWatch);
			    notifyOnLoggingSetup();
        	    Application.Run(overviewForm.Form);
        	    BootStrapper.ShutDown();
			}
			catch (Exception exception)
			{
				logException(exception);
			}
		}

		private static bool userWantedCommandLineHelpPrinted(string[] args)
		{
			if (args.Length != 1)
				return false;
			if (args[0] != "--help" && args[0] != "-help" && args[0] != "/help")
				return false;
			writeConsoleUsage();
			return true;
		}
		
		private static void writeConsoleUsage()
		{
			Console.WriteLine("AutoTest.WinForms.exe command line arguments");
			Console.WriteLine("");
			Console.WriteLine("To specify watch directory on startup you can type:");
			Console.WriteLine("\tAutoTest.WinForms.exe \"Path to the directory you want\"");
		}

        private static string getPossibleCommandArgs(string[] args)
        {
            if (args == null)
                return null;
            if (args.Length != 1)
                return null;
            return args[0];
        }

		private static void logException (Exception exception)
		{
			var file = Path.Combine(PathParsing.GetRootDirectory(), "panic.dump");
			using (var writer = new StreamWriter(file))
			{
				writeException(writer, exception);
			}
		}
		
		private static void writeException(StreamWriter writer, Exception exception)
		{
			writer.WriteLine(string.Format("Message: {0}", exception.Message));
			writer.WriteLine("Stack trace:");
			writer.WriteLine(exception.StackTrace);
			if (exception.InnerException != null)
			{
				writer.WriteLine("Inner exception");
				writer.WriteLine("");
				writeException(writer, exception.InnerException);
			}
		}

        private static string ConfigureApplication(string watchDirectory)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bootstrapApplication();
            if (watchDirectory == null)
            {
                watchDirectory = getWatchDirectory();
                if (watchDirectory == null)
                    return null;
            }
            BootStrapper.InitializeCache(watchDirectory);
            return watchDirectory;
        }

        private static string getWatchDirectory()
        {
            var directoryPicker = BootStrapper.Services.Locate<IWatchDirectoryPicker>();
            if (directoryPicker.ShowDialog() == DialogResult.Cancel)
                return null;
            return directoryPicker.DirectoryToWatch;
        }

        public static void bootstrapApplication()
        {
            BootStrapper.Configure();
            BootStrapper.Container
                .Register(Component.For<IOverviewForm>().ImplementedBy<FeedbackForm>())
                .Register(Component.For<IInformationForm>().ImplementedBy<InformationForm>())
                .Register(Component.For<IWatchDirectoryPicker>().ImplementedBy<WatchDirectoryPickerForm>());
        }
		
		private static void notifyOnLoggingSetup()
		{
			var bus = BootStrapper.Services.Locate<IMessageBus>();
			bus.Publish<InformationMessage>(new InformationMessage("Debugging enabled"));
		}
    }
}
