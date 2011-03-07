using System;
using System.Reflection;

namespace AutoTest.TestRunners.MSpec.Tests
{
    internal static class StringExtensions
    {
        public static string Path(this string relativePath)
        {
            var ourPath = System.IO.Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            return System.IO.Path.Combine(ourPath, relativePath);
        }
    }
}