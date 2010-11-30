using System;
using AutoTest.Messages;
namespace AutoTest.Core
{
	public interface IMarkProjectsForRebuild
	{
		void HandleProjects(FileChangeMessage message);
	}
}

