using Myra.Utility;
using System.Reflection;

namespace Myra.Graphics2D.UI.Properties
{
	internal abstract class ReflectionRecord : Record
	{
		public abstract override MemberInfo MemberInfo { get; }

		public override T FindAttribute<T>()
		{
			return MemberInfo.FindAttribute<T>();
		}
	}
}
