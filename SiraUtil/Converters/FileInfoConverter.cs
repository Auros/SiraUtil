using System.IO;
using IPA.Config.Data;
using IPA.Config.Stores;

namespace SiraUtil.Converters
{
    /// <summary>
    /// A config converter for BSIPA which can serialize and deserialize IO <see cref="FileInfo"/> values.
    /// </summary>
    public class FileInfoConverter : ValueConverter<FileInfo>
    {
        /// <summary>
        /// Converts a config value to a <see cref="FileInfo"/> instance.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override FileInfo FromValue(Value? value, object parent)
        {
            return value is Text t
                ? new FileInfo(t.Value)
                : throw new System.ArgumentNullException(nameof(value), "Value is not a valid IO Path");
        }

        /// <summary>
        /// Converts a <see cref="FileInfo"/> instance into a string.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override Value? ToValue(FileInfo? obj, object parent)
        {
            return obj is null ? Value.Null() : Value.Text(obj.FullName);
        }
    }
}