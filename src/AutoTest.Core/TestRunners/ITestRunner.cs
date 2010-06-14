namespace AutoTest.Core.TestRunners
{
    public interface ITestRunner
    {
        TestRunResults RunTests(string assemblyName);
    }
}