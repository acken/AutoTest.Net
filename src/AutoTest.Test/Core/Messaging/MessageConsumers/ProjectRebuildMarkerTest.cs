using System;
using NUnit.Framework;
using AutoTest.Messages;
using Rhino.Mocks;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using System.IO;
using AutoTest.Core;
namespace AutoTest.Test
{
	[TestFixture]
	public class ProjectRebuildMarkerTest
	{
		[Test]
		public void Should_mark_changed_csproj_files_for_rebuild()
		{
            Assert.Ignore();
			var project = new Project("", new ProjectDocument(ProjectType.CSharp));
			var cache = MockRepository.GenerateMock<ICache>();
			cache.Stub(c => c.Get<Project>("")).IgnoreArguments().Return(project);
			
			var message = new FileChangeMessage();
			
			message.AddFile(new ChangedFile(string.Format("TestResources{0}CSharpClassLibrary{0}CSharpClassLibrary.csproj", Path.DirectorySeparatorChar)));
			var marker = new ProjectRebuildMarker(cache);
			marker.HandleProjects(message);
			
			project.Value.RequiresRebuild.ShouldBeTrue();
		}
		
		[Test]
		public void Should_mark_changed_vbproj_files_for_rebuild()
		{
            Assert.Ignore();
			var project = new Project("", new ProjectDocument(ProjectType.VisualBasic));
			var cache = MockRepository.GenerateMock<ICache>();
			cache.Stub(c => c.Get<Project>("")).IgnoreArguments().Return(project);
			
			var message = new FileChangeMessage();
			
			message.AddFile(new ChangedFile(string.Format("TestResources{0}VS2008{0}NUnitTestProjectVisualBasic.vbproj", Path.DirectorySeparatorChar)));
			var marker = new ProjectRebuildMarker(cache);
			marker.HandleProjects(message);
			
			project.Value.RequiresRebuild.ShouldBeTrue();
		}

        [Test]
        public void Should_add_projects_that_doesnt_exist()
        {
            var project = new Project("", new ProjectDocument(ProjectType.VisualBasic));
            var cache = MockRepository.GenerateMock<ICache>();
            var message = new FileChangeMessage();
            message.AddFile(new ChangedFile(string.Format("TestResources{0}VS2008{0}NUnitTestProjectVisualBasic.vbproj", Path.DirectorySeparatorChar)));
            cache.Stub(c => c.Get<Project>(message.Files[0].FullName)).Return(null).Repeat.Once();
            cache.Stub(c => c.Get<Project>(message.Files[0].FullName)).Return(project).Repeat.Once();

            var marker = new ProjectRebuildMarker(cache);
            marker.HandleProjects(message);

            cache.AssertWasCalled(c => c.Add<Project>(message.Files[0].FullName));
        }
	}
}

