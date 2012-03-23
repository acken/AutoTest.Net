using System;
using System.Collections.Generic;
using System.Linq;

namespace Simple.Testing.Framework
{
    public class OnTypeGenerator : ISpecificationGenerator
    {
        private readonly List<Type> _types = new List<Type>();

        public OnTypeGenerator(Type type)
        {
            _types.Add(type);
        }

        public OnTypeGenerator(IEnumerable<Type> types)
        {
            _types.AddRange(types);
        }

        public OnTypeGenerator(params Type[] types) : this((IEnumerable<Type>) types)
        {
        }

        public IEnumerable<SpecificationToRun> GetSpecifications()
        {
            return _types.SelectMany(TypeReader.GetSpecificationsIn);
        }
    }
}