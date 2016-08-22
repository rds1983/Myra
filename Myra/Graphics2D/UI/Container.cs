using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public abstract class Container : Widget
	{
		[JsonIgnore]
		public abstract IEnumerable<Widget> Items { get; }

		protected Container()
		{
			LocationChanged += OnLocationChanged;
		}

		private void OnLocationChanged(object sender, EventArgs eventArgs)
		{
			// Update children locations
			foreach (var child in Items)
			{
				if (!child.Visible)
					continue;

				child.Location = Location + child.LocationInParent;
			}
		}

		public override void OnCharDown(char c)
		{
			base.OnCharDown(c);

			foreach (var child in Items)
			{
				if (!child.Visible)
					continue;

				child.OnCharDown(c);
			}
		}

		public override void InternalRender(SpriteBatch batch)
		{
			var bounds = ClientBounds;

			foreach (var child in Items)
			{
				if (!child.Visible)
					continue;

				var childBounds = child.Bounds;

				// Render if.Visible
				if (childBounds.Intersects(bounds))
				{
					batch.PushClip(childBounds);
					child.Render(batch);
					batch.PopClip();
				}
			}
		}

		public override void UpdateLayout()
		{
			base.UpdateLayout();

			foreach (var child in Items)
			{
				if (!child.Visible)
					continue;

				child.UpdateLayout();
			}
		}

		public override void OnDesktopChanged()
		{
			base.OnDesktopChanged();

			foreach (var child in Items)
			{
				child.Desktop = Desktop;
			}
		}
	}
}