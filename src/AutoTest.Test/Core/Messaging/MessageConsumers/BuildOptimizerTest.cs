using System;
using AutoTest.Core.Caching;
using Rhino.Mocks;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Messaging.MessageConsumers;
using NUnit.Framework;
namespace AutoTest.Test.Core.Messaging.MessageConsumers
{
	[TestFixture]
	public class BuildOptimizerTest
	{
		private BuildOptimizer _optimizer;
		private ICache _cache;
		private RunInfo[] _runInfos;
		
		[SetUp]
		public void SetUp()
		{
			// Project dependency graph
			//
			//		    Project2
			//		   /
			//Project0			 Project5
			//		   \ 		/
			//			Project6
			//					\
			//Project1			Project4
			//	      \        /
			//		   Project3
			//
			var projectList = new string[]
									{ 
										"Project0", 
										"Project1", 
										"Project2", 
										"Project3", 
										"Project4", 
										"Project5", 
										"Project6"
									};
			_cache = MockRepository.GenerateMock<ICache>();
			var document = new ProjectDocument(ProjectType.CSharp);
			document.AddReferencedBy(new string[] { "Project2", "Project6" });
			_cache.Stub(c => c.Get<Project>("Project0")).Return(new Project("Project0", document));
			document = new ProjectDocument(ProjectType.CSharp);
			document.AddReferencedBy("Project3");
			_cache.Stub(c => c.Get<Project>("Project1")).Return(new Project("Project1", document));
			document = new ProjectDocument(ProjectType.CSharp);
			document.AddReference("Project0");
			_cache.Stub(c => c.Get<Project>("Project2")).Return(new Project("Project2", document));
			document = new ProjectDocument(ProjectType.CSharp);
			document.AddReference("Project1");
			document.AddReferencedBy("Project4");
			_cache.Stub(c => c.Get<Project>("Project3")).Return(new Project("Project3", document));
			document = new ProjectDocument(ProjectType.CSharp);
			document.AddReference(new string[] { "Project6", "Project3" });
			_cache.Stub(c => c.Get<Project>("Project4")).Return(new Project("Project4", document));                                                                      
			document = new ProjectDocument(ProjectType.CSharp);
			document.AddReference("Project6");
			_cache.Stub(c => c.Get<Project>("Project5")).Return(new Project("Project5", document));
			document = new ProjectDocument(ProjectType.CSharp);
			document.AddReference("Project0");
			document.AddReferencedBy(new string[] { "Project4", "Project5" });
			_cache.Stub(c => c.Get<Project>("Project6")).Return(new Project("Project6", document));
			                                                                                                                                                                                                                                                                                                                        
			_optimizer = new BuildOptimizer(_cache);
			_runInfos = _optimizer.AssembleBuildConfiguration(projectList);
		}
		
		[Test]
		public void Should_only_build_projects_without_referencedbys()
		{
			_runInfos[0].ShouldBeBuilt.ShouldBeFalse();
			_runInfos[1].ShouldBeBuilt.ShouldBeFalse();
			_runInfos[2].ShouldBeBuilt.ShouldBeTrue();
			_runInfos[3].ShouldBeBuilt.ShouldBeFalse();
			_runInfos[4].ShouldBeBuilt.ShouldBeTrue();
			_runInfos[5].ShouldBeBuilt.ShouldBeTrue();
			_runInfos[6].ShouldBeBuilt.ShouldBeFalse();
		}
		
		[Test]
		public void Should_set_assembly_path_to_build_source()
		{
			Assert.Fail("Make sure assembly is set to the path of the actual built project");
		}
	}
}