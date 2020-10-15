using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPA.Config.Data;
using IPA.Config.Stores;
using UnityEngine;

namespace SiraUtil.Converters
{
	public class Vector3Converter : ValueConverter<Vector3>
	{
		public override Vector3 FromValue(Value value, object parent)
		{
			if (value is Map m)
			{
				m.TryGetValue("x", out Value x);
				m.TryGetValue("y", out Value y);
				m.TryGetValue("z", out Value z);
				Vector3 vec = Vector3.zero;
				if (x is FloatingPoint pointX)
				{
					vec.x = (float)pointX.Value;
				}
				if (y is FloatingPoint pointY)
				{
					vec.y = (float)pointY.Value;
				}
				if (z is FloatingPoint pointZ)
				{
					vec.z = (float)pointZ.Value;
				}
				return vec;
			}
			throw new ArgumentException("Value cannot be parsed into a Vector", nameof(value));
		}

		public override Value ToValue(Vector3 obj, object parent)
		{
			var map = Value.Map();
			map.Add("x", Value.Float((decimal)obj.x));
			map.Add("y", Value.Float((decimal)obj.y));
			map.Add("z", Value.Float((decimal)obj.z));
			return map;
		}
	}

	public class Vector2Converter : ValueConverter<Vector2>
	{
		public override Vector2 FromValue(Value value, object parent)
		{
			if (value is Map m)
			{
				m.TryGetValue("x", out Value x);
				m.TryGetValue("y", out Value y);
				Vector2 vec = Vector2.zero;
				if (x is FloatingPoint pointX)
				{
					vec.x = (float)pointX.Value;
				}
				if (y is FloatingPoint pointY)
				{
					vec.y = (float)pointY.Value;
				}
				return vec;
			}
			throw new ArgumentException("Value cannot be parsed into a Vector", nameof(value));
		}

		public override Value ToValue(Vector2 obj, object parent)
		{
			var map = Value.Map();
			map.Add("x", Value.Float((decimal)obj.x));
			map.Add("y", Value.Float((decimal)obj.y));
			return map;
		}
	}
}
