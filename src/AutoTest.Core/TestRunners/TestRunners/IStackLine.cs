namespace AutoTest.Core.TestRunners
{
    public interface IStackLine
    {
        string Method { get; }
        string File { get; }
        int LineNumber { get; }
        string ToString();
    }
}