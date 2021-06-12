using UnityEngine;
using Zenject;

namespace SiraUtil.Objects
{
    internal class ObjectDiffuser
    {
        private readonly Object _object;

        public ObjectDiffuser(Object @object)
        {
            _object = @object;
        }

        [Inject]
        protected void Resolved()
        {
            Object.Destroy(_object);
        }
    }
}