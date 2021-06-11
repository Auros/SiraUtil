using System;

namespace SiraUtil.Sabers
{
    /// <summary>
    /// Allows you to register a new saber model.
    /// </summary>
    /// <remarks>
    /// If you bind this type in the game scene container, it will be picked up and the one with the highest
    /// priority will be 
    /// </remarks>
    public class SaberModelRegistration
    {
        /// <summary>
        /// The priority at which this model is preferred. The highest priority will be used as the active saber model.
        /// </summary>
        public int Priority { get; }

        internal Type? LeftType { get; }
        internal Type? RightType { get; }
        internal SaberModelController? LeftTemplate { get; }
        internal SaberModelController? RightTemplate { get; }
        internal Func<SaberModelController>? LeftInstruction { get; }
        internal Func<SaberModelController>? RightInstruction { get; }

        /// <summary>
        /// Creates a new saber model registration.
        /// </summary>
        /// <typeparam name="T">The type of the SaberModelController to make.</typeparam>
        /// <param name="priority">The priority at which this model is preferred. The highest priority will be used as the active saber model.</param>
        /// <returns></returns>
        public static SaberModelRegistration Create<T>(int priority = 0) where T : SaberModelController
        {
            return new SaberModelRegistration(typeof(T), priority);
        }

        /// <summary>
        /// Creates a new registration.
        /// </summary>
        /// <param name="modelControllerType">The type of the model to use. This must inherit from <see cref="SaberModelController"/></param>
        /// <param name="priority">The priority at which this model is preferred. The highest priority will be used as the active saber model.</param>
        public SaberModelRegistration(Type modelControllerType, int priority = 0) 
        {
            if (!modelControllerType.IsSubclassOf(typeof(SaberModelController)))
                throw new ArgumentException($"{modelControllerType.Name} does not inherit from {nameof(SaberModelController)}", nameof(modelControllerType));

            Priority = priority;
            LeftType = modelControllerType;
            RightType = modelControllerType;
        }

        /// <summary>
        /// Creates a new registration.
        /// </summary>
        /// <param name="leftModelControllerType">The type to use for the left saber model. This must inherit from <see cref="SaberModelController"/>. This will be instantiated through Zenject.</param>
        /// <param name="rightModelControllerType">The type to use for the right saber model. This must inherit from <see cref="SaberModelController"/>. This will be instantiated through Zenject.</param>
        /// <param name="priority">The priority at which this model is preferred. The highest priority will be used as the active saber model.</param>
        public SaberModelRegistration(Type leftModelControllerType, Type rightModelControllerType, int priority = 0)
        {
            if (leftModelControllerType is null)
                throw new ArgumentNullException(nameof(leftModelControllerType));

            if (rightModelControllerType is null)
                throw new ArgumentNullException(nameof(rightModelControllerType));

            Priority = priority;
            LeftType = leftModelControllerType;
            RightType = rightModelControllerType;
        }

        /// <summary>
        /// Creates a new registration based on a prefab.
        /// </summary>
        /// <param name="prefab">The prefab to use for the saber models. This will be instantiated through Zenject.</param>
        /// <param name="priority">The priority at which this model is preferred. The highest priority will be used as the active saber model.</param>
        public SaberModelRegistration(SaberModelController prefab, int priority = 0)
        {
            if (prefab is null)
                throw new ArgumentNullException(nameof(prefab));

            Priority = priority;
            LeftTemplate = prefab;
            RightTemplate = prefab;
        }

        /// <summary>
        /// Creates a new registration based on a prefab.
        /// </summary>
        /// <param name="leftModelPrefab">The prefab to use for the left saber model. This will be instantiated through Zenject.</param>
        /// <param name="rightModelPrefab">The prefab to use for the right saber model. This will be instantiated through Zenject.</param>
        /// <param name="priority">The priority at which this model is preferred. The highest priority will be used as the active saber model.</param>
        public SaberModelRegistration(SaberModelController leftModelPrefab, SaberModelController rightModelPrefab, int priority = 0)
        {
            if (leftModelPrefab is null)
                throw new ArgumentNullException(nameof(leftModelPrefab));

            if (rightModelPrefab is null)
                throw new ArgumentNullException(nameof(rightModelPrefab));

            Priority = priority;
            LeftTemplate = leftModelPrefab;
            RightTemplate = rightModelPrefab;
        }

        /// <summary>
        /// Creates a new registration based on an instruction.
        /// </summary>
        /// <param name="instruction">A function which is called when a new prefab instance needs to be created. This will be instantiated by you, but have its dependencies injected through the container.</param>
        /// <param name="priority">The priority at which this model is preferred. The highest priority will be used as the active saber model.</param>
        public SaberModelRegistration(Func<SaberModelController> instruction, int priority = 0)
        {
            if (instruction is null)
                throw new ArgumentNullException(nameof(instruction));

            Priority = priority;
            LeftInstruction = instruction;
            RightInstruction = instruction;
        }

        /// <summary>
        /// Creates a new registration based on instructions.
        /// </summary>
        /// <param name="leftModelInstruction">A function which is called when a new prefab instance needs to be created for the left saber. This will be instantiated by you, but have its dependencies injected through the container.</param>
        /// <param name="rightModelInstruction">A function which is called when a new prefab instance needs to be created for the right saber. This will be instantiated by you, but have its dependencies injected through the container.</param>
        /// <param name="priority">The priority at which this model is preferred. The highest priority will be used as the active saber model.</param>
        public SaberModelRegistration(Func<SaberModelController> leftModelInstruction, Func<SaberModelController> rightModelInstruction, int priority = 0)
        {
            if (leftModelInstruction is null)
                throw new ArgumentNullException(nameof(leftModelInstruction));

            if (rightModelInstruction is null)
                throw new ArgumentNullException(nameof(rightModelInstruction));

            Priority = priority;
            LeftInstruction = leftModelInstruction;
            RightInstruction = rightModelInstruction;
        }
    }
}