using Hive.Versioning;
using IPA.Config.Data;
using IPA.Config.Stores;

namespace SiraUtil.Converters
{
    /// <summary>
    /// A config converter for BSIPA which can serialize and deserialize Hive.Versioning <see cref="Version"/> values.
    /// </summary>
    public class HiveVersionConverter : ValueConverter<Version>
    {
        /// <summary>
        /// Converts a config value text to a SemVer <see cref="Version"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override Version FromValue(Value value, object parent)
        {
            return value is Text t
                ? new Version(t.Value)
                :         throw new System.ArgumentException("Value cannot be parsed into a Semver Version", nameof(value));
        }

        /// <summary>
        /// Converts a SemVer <see cref="Version"/> into a config value text.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override Value ToValue(Version obj, object parent)
        {
            return Value.Text(obj.ToString());
        }
    }
}