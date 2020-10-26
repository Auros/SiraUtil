using SemVer;
using IPA.Config.Data;
using IPA.Config.Stores;

namespace SiraUtil.Converters
{
    /// <summary>
    /// A config converter for BSIPA which can serialize and deserialize SemVer <see cref="Version"/> values.
    /// </summary>
    public class VersionConverter : ValueConverter<Version>
    {
        public override Version FromValue(Value value, object parent)
        {
            return value is Text t
                ? new Version(t.Value)
                :         throw new System.ArgumentException("Value cnanot be parsed into a Semver Version", nameof(value));
        }

        public override Value ToValue(Version obj, object parent)
        {
            return Value.Text(obj.ToString());
        }
    }
}