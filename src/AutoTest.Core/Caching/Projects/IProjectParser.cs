namespace AutoTest.Core.Caching.Projects
{
    interface IProjectParser
    {
        ProjectDocument Parse(string projectFile, ProjectDocument existingDocument);
    }
}