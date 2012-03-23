using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Simple.Testing.Framework
{
    public class RootGenerator : ISpecificationGenerator
    {
        private readonly Assembly _assembly;

        public RootGenerator(Assembly assembly)
        {
            _assembly = assembly;
        }

        public IEnumerable<SpecificationToRun> GetSpecifications()
        {
            return _assembly.GetTypes().SelectMany(TypeReader.GetSpecificationsIn);
        }
    }
}
