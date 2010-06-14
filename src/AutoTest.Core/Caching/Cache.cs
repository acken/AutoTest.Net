using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Configuration;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Core.Caching
{
    class Cache : ICache
    {
        private IServiceLocator _services;
        private List<IRecord> _records = new List<IRecord>();

        public int Count { get { return _records.Count; } }

        public Cache(IServiceLocator services)
        {
            _services = services;
        }

        public void Add<T>(string key) where T : IRecord
        {
            var index = findIndex(key);
            if (index < 0)
                index = createRecord<T>(key);
            prepareRecord<T>(index);
        }

        public bool Exists(string key)
        {
            return findIndex(key) >= 0;
        }

        public T Get<T>(string key) where T : IRecord
        {
            return Get<T>(findIndex(key));
        }

        public T Get<T>(int index) where T : IRecord
        {
            if (index < 0)
                return default(T);
            prepareRecord<T>(index);
            return (T)_records[index];
        }

        private int createRecord<T>(string key) where T : IRecord
        {
            var creator = _services.Locate<ICreate<T>>();
            _records.Add(creator.Create(key));
            int index = _records.Count - 1;
            return index;
        }

        private void prepareRecord<T>(int index) where T : IRecord
        {
            T record = (T)_records[index];
            var preparer = _services.Locate<IPrepare<T>>();
            T preparedRecord = preparer.Prepare(record, new Action<T>(r => _records.Add(r)));
            if (preparedRecord !=  null)
                _records[index] = preparedRecord;
        }

        private int findIndex(string key)
        {
            return _records.FindIndex(0, p => p.Key.Equals(key));
        }
    }
}
