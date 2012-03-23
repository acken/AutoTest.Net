using System.Collections.Generic;

namespace Simple.Testing.Framework
{
    public interface ISpecificationGenerator
    {
        IEnumerable<SpecificationToRun> GetSpecifications();
    }
}