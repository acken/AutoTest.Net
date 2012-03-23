using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Simple.Testing.ClientFramework;

namespace Simple.Testing.Framework.Tests
{
    public class PassingSpecification<T> : TypedSpecification<T>
    {
        public string GetName()
        {
            return "";
        }

        public Action GetBefore()
        {
            return Noop;
        }

        private void Noop()
        {
            
        }

        public Delegate GetOn()
        {
            return new Action(Noop);
        }

        public Delegate GetWhen()
        {
            return new Action(Noop);
        }

        public IEnumerable<Expression<Func<T, bool>>> GetAssertions()
        {
            return new List<Expression<Func<T, bool>>>();
        }

        public Action GetFinally()
        {
            return Noop;
        }
    }
}