using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoTest.Core.FileSystem.ProjectLocators
{
    interface ILocateProjects
    {
        ChangedFile[] Locate(string file);
        bool IsProject(string file);
    }
}
