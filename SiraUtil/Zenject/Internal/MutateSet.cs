using System;

namespace SiraUtil.Zenject.Internal
{
    internal struct MutateSet
    {
        public readonly Type typeToMutate;
        public readonly DelegateWrapper onMutate;
        public readonly string locationContractName;

        public MutateSet(Type typeToMutate, string locationContractName, DelegateWrapper onMutate)
        {
            this.onMutate = onMutate;
            this.typeToMutate = typeToMutate;
            this.locationContractName = locationContractName;
        }
    }
}