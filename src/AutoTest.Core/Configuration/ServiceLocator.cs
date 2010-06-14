using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;

namespace AutoTest.Core.Configuration
{
    class ServiceLocator : IServiceLocator
    {
        private IWindsorContainer _container = new WindsorContainer();

        public IWindsorContainer Container { get { return _container; } }

        public T Locate<T>()
        {
            return _container.Resolve<T>();
        }

        public T[] LocateAll<T>()
        {
            return _container.ResolveAll<T>();
        }
    }
}
