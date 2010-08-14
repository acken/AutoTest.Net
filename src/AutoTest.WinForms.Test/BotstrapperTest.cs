using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Configuration;

namespace AutoTest.WinForms.Test
{
    [TestFixture]
    public class BotstrapperTest
    {
        private IServiceLocator _locator;

        [SetUp]
        public void SetUp()
        {
            Program.bootstrapApplication();
            _locator = BootStrapper.Services;
        }

        [Test]
        public void Should_register_feedback_form()
        {
            var form = _locator.Locate<IOverviewForm>();
            form.ShouldBeOfType<IOverviewForm>();
        }

        [Test]
        public void Should_register_information_form()
        {
            var form = _locator.Locate<IInformationForm>();
            form.ShouldBeOfType<IInformationForm>();
        }

        [Test]
        public void Should_register_directoy_picker_form()
        {
            var form = _locator.Locate<IWatchDirectoryPicker>();
            form.ShouldBeOfType<IWatchDirectoryPicker>();
        }
    }
}
