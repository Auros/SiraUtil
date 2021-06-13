using Hive.Versioning;
using System.Threading.Tasks;

namespace SiraUtil.Web.SiraSync
{
    /// <summary>
    /// Defines a way of getting mod info from an online service.
    /// </summary>
    /// <remarks>
    /// This is a warning. Although this class is public, it is not meant to be inherited normally. Please ONLY receive this through Zenject and do not create your own instance.
    /// Future SiraUtil updates might add more to this interface.
    /// </remarks>
    public interface ISiraSyncService
    {
        /// <summary>
        /// Gets the latest version value of your mod.
        /// </summary>
        /// <returns></returns>
        Task<Version?> LatestVersion();

        /// <summary>
        /// Gets the latest changelog for your mod.
        /// </summary>
        /// <returns></returns>
        Task<string?> LatestChangelog();
    }
}