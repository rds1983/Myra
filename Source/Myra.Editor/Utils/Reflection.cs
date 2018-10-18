using System;

namespace Myra.Editor.Utils
{
	public static class Reflection
	{
		public static string GetOnlyTypeName(this Type type)
		{
			var result = type.Name;
			if (type.IsGenericType)
			{
				int iBacktick = result.IndexOf('`');
				if (iBacktick > 0)
				{
					result = result.Remove(iBacktick);
				}
			}

			return result;
		}

		public static string GetFriendlyName(this Type type)
		{
			string friendlyName = type.Name;

			if (type.IsNested && type.DeclaringType != null)
			{
				friendlyName = type.DeclaringType.GetFriendlyName() + "." + friendlyName;
			}

			if (type.IsGenericType)
			{
				int iBacktick = friendlyName.IndexOf('`');
				if (iBacktick > 0)
				{
					friendlyName = friendlyName.Remove(iBacktick);
				}
				friendlyName += "<";
				Type[] typeParameters = type.GetGenericArguments();
				for (int i = 0; i < typeParameters.Length; ++i)
				{
					string typeParamName = typeParameters[i].Name;
					friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
				}
				friendlyName += ">";
			}

			return friendlyName;
		}

		public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
		{
			while (toCheck != null && toCheck != typeof(object))
			{
				var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
				if (generic == cur)
				{
					return true;
				}
				toCheck = toCheck.BaseType;
			}
			return false;
		}

		public static Type FindGenericType(this Type givenType, Type genericType)
		{
			var interfaceTypes = givenType.GetInterfaces();

			foreach (var it in interfaceTypes)
			{
				if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
					return it;
			}

			if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
				return givenType;

			Type baseType = givenType.BaseType;
			if (baseType == null) return null;

			return FindGenericType(baseType, genericType);
		}

		public static bool IsNullablePrimitive(this Type type)
		{
			return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
					type.GetGenericArguments()[0].IsPrimitive);
		}


		public static Type GetNullableType(this Type type)
		{
			return type.GetGenericArguments()[0];
		}

		public static bool IsNumericInteger(this Type t)
		{
			switch (Type.GetTypeCode(t))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
					return true;
				default:
					return false;
			}
		}

		public static bool IsNumericType(this Type t)
		{
			if (IsNumericInteger(t))
			{
				return true;
			}

			switch (Type.GetTypeCode(t))
			{
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				default:
					return false;
			}
		}
	}
}