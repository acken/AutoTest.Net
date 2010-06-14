namespace AutoTest.Core.Configuration
{
    public interface IServiceLocator
    {
        T Locate<T>();
        T[] LocateAll<T>();
    }
}