using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public abstract class SplitPane : SingleItemContainer<Grid>
	{
		private readonly ObservableCollection<Widget> _widgets = new ObservableCollection<Widget>();
		private readonly List<ImageButton> _handles = new List<ImageButton>();
		private ImageButton _handleDown;
		private int? _mouseCoord;
		private int _handlesSize;

		[JsonIgnore]
		[HiddenInEditor]
		public abstract Orientation Orientation { get; }

		[HiddenInEditor]
		public ObservableCollection<Widget> Widgets
		{
			get { return _widgets; }
		}

		[JsonIgnore]
		[HiddenInEditor]
		public ImageButtonStyle HandleStyle { get; private set; }

		public event EventHandler ProportionsChanged;

		protected SplitPane(SplitPaneStyle style)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			Widget = new Grid((GridStyle)null);

			_widgets.CollectionChanged += WidgetsOnCollectionChanged;

			if (style != null)
			{
				ApplySplitPaneStyle(style);
			}
		}

		public float GetProportion(int widgetIndex)
		{
			if (widgetIndex < 0 || widgetIndex >= Widgets.Count)
			{
				return 0.0f;
			}

			var result = Orientation == Orientation.Horizontal
				? Widget.ColumnsProportions[widgetIndex*2].Value
				: Widget.RowsProportions[widgetIndex*2].Value;

			return result;
		}

		public override void OnMouseMoved(Point position)
		{
			base.OnMouseMoved(position);

			if (_mouseCoord == null)
			{
				return;
			}

			var bounds = Bounds;
			if (bounds.Width == 0)
			{
				return;
			}

			var handleIndex = Widget.Widgets.IndexOf(_handleDown);
			Grid.Proportion firstProportion, secondProportion;
			float fp;

			if (Orientation == Orientation.Horizontal)
			{
				var firstWidth = position.X - bounds.X - _mouseCoord.Value;

				for (var i = 0; i < handleIndex - 1; ++i)
				{
					firstWidth -= Widget.GetColumnWidth(i);
				}

				fp = (float) Widgets.Count*firstWidth/(bounds.Width - _handlesSize);

				firstProportion = Widget.ColumnsProportions[handleIndex - 1];
				secondProportion = Widget.ColumnsProportions[handleIndex + 1];
			}
			else
			{
				var firstHeight = position.Y - bounds.Y - _mouseCoord.Value;

				for (var i = 0; i < handleIndex - 1; ++i)
				{
					firstHeight -= Widget.GetRowHeight(i);
				}

				fp = (float) Widgets.Count*firstHeight/(bounds.Height - _handlesSize);

				firstProportion = Widget.RowsProportions[handleIndex - 1];
				secondProportion = Widget.RowsProportions[handleIndex + 1];
			}

			if (fp >= 0 && fp <= 2.0f)
			{
				var fp2 = firstProportion.Value + secondProportion.Value - fp;
				firstProportion.Value = fp;
				secondProportion.Value = fp2;
				FireProportionsChanged();
			}
		}

		private void FireProportionsChanged()
		{
			var ev = ProportionsChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		private void HandleOnUp(object sender, EventArgs args)
		{
			_handleDown = null;
			_mouseCoord = null;
		}

		private void HandleOnDown(object sender, EventArgs args)
		{
			_handleDown = (ImageButton)sender;
			_mouseCoord = Orientation == Orientation.Horizontal
				? Desktop.MousePosition.X - _handleDown.Bounds.X
				: Desktop.MousePosition.Y - _handleDown.Bounds.Y;
		}

		private void WidgetsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			Reset();
		}

		private void GetProportions(int leftWidgetIndex,
			out Grid.Proportion leftProportion,
			out Grid.Proportion rightProportion)
		{
			var baseIndex = leftWidgetIndex*2;
			leftProportion = Orientation == Orientation.Horizontal
				? Widget.ColumnsProportions[baseIndex]
				: Widget.RowsProportions[baseIndex];
			rightProportion = Orientation == Orientation.Horizontal
				? Widget.ColumnsProportions[baseIndex + 2]
				: Widget.RowsProportions[baseIndex + 2];
		}

		public float GetSplitterPosition(int leftWidgetIndex)
		{
			Grid.Proportion leftProportion, rightProportion;
			GetProportions(leftWidgetIndex, out leftProportion, out rightProportion);

			var total = leftProportion.Value + rightProportion.Value;

			return leftProportion.Value/total;
		}

		public void SetSplitterPosition(int leftWidgetIndex, float proportion)
		{
			Grid.Proportion leftProportion, rightProportion;
			GetProportions(leftWidgetIndex, out leftProportion, out rightProportion);

			var total = leftProportion.Value + rightProportion.Value;

			var fp = proportion*total;
			var fp2 = total - fp;
			leftProportion.Value = fp;
			rightProportion.Value = fp2;
		}

		public void Reset()
		{
			// Clear
			Widget.Widgets.Clear();
			_handles.Clear();
			_handlesSize = 0;

			Widget.ColumnsProportions.Clear();
			Widget.RowsProportions.Clear();

			var i = 0;

			var handleSize = Orientation == Orientation.Horizontal
				? HandleStyle.Background.Size.X
				: HandleStyle.Background.Size.Y;

			foreach (var w in _widgets)
			{
				Grid.Proportion proportion;
				if (i > 0)
				{
					// Add splitter
					var handle = new ImageButton(HandleStyle)
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Stretch,
						CanFocus = false
					};

					handle.Down += HandleOnDown;
					handle.Up += HandleOnUp;

					proportion = new Grid.Proportion(Grid.ProportionType.Auto);

					if (Orientation == Orientation.Horizontal)
					{
						_handlesSize += handleSize;
						handle.GridPositionX = i*2 - 1;
						Widget.ColumnsProportions.Add(proportion);
					}
					else
					{
						_handlesSize += handleSize;
						handle.GridPositionY = i*2 - 1;
						Widget.RowsProportions.Add(proportion);
					}

					Widget.Widgets.Add(handle);
					_handles.Add(handle);
				}

				proportion = i < _widgets.Count - 1
					? new Grid.Proportion(Grid.ProportionType.Part, 1.0f)
					: new Grid.Proportion(Grid.ProportionType.Fill, 1.0f);

				// Set grid coord and add widget itself
				if (Orientation == Orientation.Horizontal)
				{
					w.GridPositionX = i*2;
					Widget.ColumnsProportions.Add(proportion);
				}
				else
				{
					w.GridPositionY = i*2;
					Widget.RowsProportions.Add(proportion);
				}

				Widget.Widgets.Add(w);

				++i;
			}

			foreach (var h in _handles)
			{
				if (Orientation == Orientation.Horizontal)
				{
					h.WidthHint = handleSize;
					h.HeightHint = null;
				}
				else
				{
					h.WidthHint = null;
					h.HeightHint = handleSize;
				}
			}

			FireProportionsChanged();
		}

		public void ApplySplitPaneStyle(SplitPaneStyle style)
		{
			ApplyWidgetStyle(style);

			HandleStyle = style.HandleStyle;
		}
	}
}