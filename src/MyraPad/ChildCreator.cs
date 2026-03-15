using System;

namespace MyraPad
{
	internal class ChildCreator
	{
		public string Name { get; }
		public Action Creator { get; }

		public ChildCreator(string name, Action creator)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}

			Name = name;
			Creator = creator ?? throw new ArgumentNullException(nameof(creator));
		}
	}
}
