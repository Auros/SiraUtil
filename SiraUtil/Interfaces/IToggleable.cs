namespace SiraUtil.Interfaces
{
    /// <summary>
    /// Controls the state for something to be toggled.
    /// </summary>
    public interface IToggleable
    {
        /// <summary>
        /// The toggleability.
        /// </summary>
        bool Status { get; set; }
    }
}