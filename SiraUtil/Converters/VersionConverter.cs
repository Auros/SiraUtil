using SemVer;
using IPA.Config.Data;
using IPA.Config.Stores;

namespace SiraUtil.Converters
{
	public class VersionConverter : ValueConverter<SemVer.Version>
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