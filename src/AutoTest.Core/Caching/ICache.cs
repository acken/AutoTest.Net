namespace AutoTest.Core.Caching
{
    public interface ICache
    {
        int Count { get; }
        void Add<T>(string key) where T : IRecord;
        bool Exists(string key);
        T Get<T>(string key) where T : IRecord;
        T Get<T>(int index) where T : IRecord;
    }
}