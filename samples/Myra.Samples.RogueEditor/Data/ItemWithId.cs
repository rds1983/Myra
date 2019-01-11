using System;

namespace Myra.Samples.RogueEditor.Data
{
	public class ItemWithId
	{
		private string _id;

		public string Id
		{
			get
			{
				return _id;
			}

			set
			{
				if (value == _id)
				{
					return;
				}

				_id = value;
				FireIdChanged();
			}
		}

		public event EventHandler IdChanged;

		public override string ToString()
		{
			return Id;
		}

		protected void FireIdChanged()
		{
			var ev = IdChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}
	}
}
