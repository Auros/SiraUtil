using System;

namespace SiraUtil.Zenject.Internal
{
    internal struct ExposeSet
    {
        public readonly Type typeToExpose;
        public readonly string locationContractName;

        public ExposeSet(Type typeToExpose, string locationContractName)
        {
            this.typeToExpose = typeToExpose;
            this.locationContractName = locationContractName;
        }
    }
}