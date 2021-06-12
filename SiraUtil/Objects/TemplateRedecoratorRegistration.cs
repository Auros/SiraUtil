using System;

namespace SiraUtil.Objects
{
    /// <summary>
    /// A templator for object redecorator registrators.
    /// </summary>
    /// <typeparam name="TPrefabType">The type of the prefab.</typeparam>
    /// <typeparam name="TParentType">The type of the parent.</typeparam>
    public abstract class TemplateRedecoratorRegistration<TPrefabType, TParentType> : RedecoratorRegistration
    {

        private readonly Func<TPrefabType, TPrefabType> _redecorateCall;

        internal TemplateRedecoratorRegistration(string contract, Func<TPrefabType, TPrefabType> redecorateCall, int priority = 0, bool chain = true) : base(contract, typeof(TPrefabType), typeof(TParentType), priority, chain)
        {
            _redecorateCall = redecorateCall;
        }

        internal override object Redecorate(object value)
        {
            return _redecorateCall.Invoke(((TPrefabType)value)!)!;
        }
    }
}