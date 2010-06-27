using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AutoTest.Core.Configuration;
using System.Reflection;
using Castle.MicroKernel.Registration;

namespace AutoTest.WinForms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ConfigureApplication();
            var overviewForm = BootStrapper.Services.Locate<IOverviewForm>();
            Application.Run(overviewForm.Form);
            BootStrapper.ShutDown();
        }

        private static void ConfigureApplication()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            BootstrapApplication();
            BootStrapper.InitializeCache();
        }

        public static void BootstrapApplication()
        {
            BootStrapper.Configure();
            BootStrapper.Container
                .Register(Component.For<IOverviewForm>().ImplementedBy<FeedbackForm>())
                .Register(Component.For<IInformationForm>().ImplementedBy<InformationForm>());
        }
    }
}
