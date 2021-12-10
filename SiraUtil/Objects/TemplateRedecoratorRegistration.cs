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

        /// <summary>
        /// Creates a new templated redecorator.
        /// </summary>
        /// <param name="contract">The contract type of the redecorator.</param>
        /// <param name="redecorateCall">The callback for redecoration.</param>
        /// <param name="priority">The redecoration priority.</param>
        /// <param name="chain">Whether to chain this redecoration with others. Every redecoration is now aggregated.
        /// The chain will start if the highest priority object has chaining enabled and will stop once a registration
        /// in the aggregate has chaining disabled.</param>
        public TemplateRedecoratorRegistration(string contract, Func<TPrefabType, TPrefabType> redecorateCall, int priority = 0, bool chain = true) : base(contract, typeof(TPrefabType), typeof(TParentType), priority, chain)
        {
            _redecorateCall = redecorateCall;
        }

        /// <summary>
        /// Redecorates through a template.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected internal override object Redecorate(object value)
        {
            return _redecorateCall.Invoke(((TPrefabType)value)!)!;
        }
    }
}