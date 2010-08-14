using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Configuration;
using Rhino.Mocks;

namespace AutoTest.WinForms.Test
{
    [TestFixture]
    public class WatchDirectoryPickerFormTest
    {
        private IConfiguration _configuration;
        private WatchDirectoryPickerFormExtendedForTests _form;

        [SetUp]
        public void SetUp()
        {
            _configuration = MockRepository.GenerateMock<IConfiguration>();
            _configuration.Stub(c => c.DirectoryToWatch).Return("First directory item");
            _form = new WatchDirectoryPickerFormExtendedForTests(_configuration);
        }

        [Test]
        public void Should_list_directories_from_configuration()
        {
            var list = _form.GetListViewContent();
            list[0].ShouldEqual("First directory item");;
        }
    }
}
