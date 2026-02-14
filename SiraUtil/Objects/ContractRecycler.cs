using System;
using Zenject;

namespace SiraUtil.Objects
{
    internal class ContractRecycler : IDisposable
    {
        private readonly DiContainer _container;

        public ContractRecycler(DiContainer container)
        {
            _container = container.AncestorContainers[0];
        }

        public void Dispose()
        {
            while (_container.Unbind<RedecoratorRegistration>())
            {

            }
        }
    }
}