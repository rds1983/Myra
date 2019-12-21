using System.Collections.Generic;

namespace Myra.MML
{
	public interface IHasResources
	{
		/// <summary>
		/// External files used by this object
		/// </summary>
		Dictionary<string, string> Resources { get; }
	}
}
