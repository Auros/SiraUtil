namespace SiraUtil.Services
{
    /// <summary>
    /// Gets the active menu controllers. This works while in the menu scene or the game scene (pause menu controllers).
    /// </summary>
    public interface IMenuControllerAccessor
    {
        /// <summary>
        /// The left VR controller.
        /// </summary>
        VRController LeftController { get; }

        /// <summary>
        /// The right VR controller.
        /// </summary>
        VRController RightController { get; }
    }
}