using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using Myra.Attributes;

namespace Myra.Graphics2D.UI
{
	public abstract class SplitPane : Container
	{
		private readonly ObservableCollection<Widget> _widgets = new ObservableCollection<Widget>();
		private readonly GridLayout _layout = new GridLayout();
		private readonly List<Button> _handles = new List<Button>();
		private Button _handleDown;
		private int? _mouseCoord;
		private int _handlesSize;

		[Content]
		[Browsable(false)]
		public override ObservableCollection<Widget> Widgets => _widgets;

		[XmlIgnore]
		[Browsable(false)]
		public abstract Orientation Orientation { get; }

		[XmlIgnore]
		[Browsable(false)]
		public ButtonStyle HandleStyle { get; private set; }

		public event EventHandler ProportionsChanged;

		protected SplitPane(string styleName)
		{
			ChildrenLayout = _layout;
			_widgets.CollectionChanged += WidgetsOnCollectionChanged;

			SetStyle(styleName);
		}

		public float GetProportion(int widgetIndex)
		{
			if (widgetIndex < 0 || widgetIndex >= Widgets.Count)
			{
				return 0.0f;
			}

			var result = Orientation == Orientation.Horizontal
				? _layout.ColumnsProportions[widgetIndex * 2].Value
				: _layout.RowsProportions[widgetIndex * 2].Value;

			return result;
		}

		public override void OnTouchMoved()
		{
			base.OnTouchMoved();

			if (Desktop == null || _mouseCoord == null)
			{
				return;
			}

			var bounds = Bounds;
			if (bounds.Width == 0)
			{
				return;
			}

			var handleIndex = Children.IndexOf(_handleDown);
			Proportion firstProportion, secondProportion;
			float fp;

			var position = ToLocal(Desktop.TouchPosition.Value);
			if (Orientation == Orientation.Horizontal)
			{
				var firstWidth = position.X - _mouseCoord.Value;

				for (var i = 0; i < handleIndex - 1; ++i)
				{
					firstWidth -= _layout.GetColumnWidth(i);
				}

				fp = (float)Widgets.Count * firstWidth / (bounds.Width - _handlesSize);

				firstProportion = _layout.ColumnsProportions[handleIndex - 1];
				secondProportion = _layout.ColumnsProportions[handleIndex + 1];
			}
			else
			{
				var firstHeight = position.Y - _mouseCoord.Value;

				for (var i = 0; i < handleIndex - 1; ++i)
				{
					firstHeight -= _layout.GetRowHeight(i);
				}

				fp = (float)Widgets.Count * firstHeight / (bounds.Height - _handlesSize);

				firstProportion = _layout.RowsProportions[handleIndex - 1];
				secondProportion = _layout.RowsProportions[handleIndex + 1];
			}

			if (fp >= 0 && fp <= 2.0f)
			{
				var fp2 = firstProportion.Value + secondProportion.Value - fp;
				firstProportion.Value = fp;
				secondProportion.Value = fp2;
				FireProportionsChanged();
			}

			InvalidateArrange();
		}

		private void FireProportionsChanged()
		{
			var ev = ProportionsChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		private void HandleOnPressedChanged(object sender, EventArgs args)
		{
			var handle = (Button)sender;

			if (!handle.IsPressed)
			{
				_handleDown = null;
				_mouseCoord = null;
			}
			else if (Desktop != null)
			{
				_handleDown = (Button)sender;

				var handleGlobalPos = _handleDown.ToGlobal(_handleDown.Bounds.Location);
				_mouseCoord = Orientation == Orientation.Horizontal
					? Desktop.TouchPosition.Value.X - handleGlobalPos.X
					: Desktop.TouchPosition.Value.Y - handleGlobalPos.Y;
			}
		}

		private void WidgetsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			Reset();
		}

		private void GetProportions(int leftWidgetIndex,
			out Proportion leftProportion,
			out Proportion rightProportion)
		{
			var baseIndex = leftWidgetIndex * 2;
			leftProportion = Orientation == Orientation.Horizontal
				? _layout.ColumnsProportions[baseIndex]
				: _layout.RowsProportions[baseIndex];
			rightProportion = Orientation == Orientation.Horizontal
				? _layout.ColumnsProportions[baseIndex + 2]
				: _layout.RowsProportions[baseIndex + 2];
		}

		public float GetSplitterPosition(int leftWidgetIndex)
		{
			Proportion leftProportion, rightProportion;
			GetProportions(leftWidgetIndex, out leftProportion, out rightProportion);

			var total = leftProportion.Value + rightProportion.Value;

			return leftProportion.Value / total;
		}

		public void SetSplitterPosition(int leftWidgetIndex, float proportion)
		{
			Proportion leftProportion, rightProportion;
			GetProportions(leftWidgetIndex, out leftProportion, out rightProportion);

			var total = leftProportion.Value + rightProportion.Value;

			var fp = proportion * total;
			var fp2 = total - fp;
			leftProportion.Value = fp;
			rightProportion.Value = fp2;
		}

		public void Reset()
		{
			// Clear
			Children.Clear();
			_handles.Clear();
			_handlesSize = 0;

			_layout.ColumnsProportions.Clear();
			_layout.RowsProportions.Clear();

			var i = 0;

			var handleSize = 0;
			var asImage = HandleStyle.Background as IImage;
			if (asImage != null)
			{
				handleSize = Orientation == Orientation.Horizontal
					? asImage.Size.X
					: asImage.Size.Y;
			}

			foreach (var w in Widgets)
			{
				Proportion proportion;
				if (i > 0)
				{
					// Add splitter
					var handle = new Button(null)
					{
						ReleaseOnTouchLeft = false
					};

					if (Orientation == Orientation.Horizontal)
					{
						handle.VerticalAlignment = VerticalAlignment.Stretch;
					}
					else
					{
						handle.HorizontalAlignment = HorizontalAlignment.Stretch;
					}

					handle.ApplyButtonStyle(HandleStyle);

					handle.PressedChanged += HandleOnPressedChanged;

					proportion = new Proportion(ProportionType.Auto);

					if (Orientation == Orientation.Horizontal)
					{
						_handlesSize += handleSize;
						Grid.SetColumn(handle, i * 2 - 1);
						_layout.ColumnsProportions.Add(proportion);
					}
					else
					{
						_handlesSize += handleSize;
						Grid.SetRow(handle, i * 2 - 1);
						_layout.RowsProportions.Add(proportion);
					}

					Children.Add(handle);
					_handles.Add(handle);
				}

				proportion = i < Widgets.Count - 1
					? new Proportion(ProportionType.Part, 1.0f)
					: new Proportion(ProportionType.Fill, 1.0f);

				// Set grid coord and add widget itself
				if (Orientation == Orientation.Horizontal)
				{
					Grid.SetColumn(w, i * 2);
					_layout.ColumnsProportions.Add(proportion);
				}
				else
				{
					Grid.SetRow(w, i * 2);
					_layout.RowsProportions.Add(proportion);
				}

				Children.Add(w);

				++i;
			}

			foreach (var h in _handles)
			{
				if (Orientation == Orientation.Horizontal)
				{
					h.Width = handleSize;
					h.Height = null;
				}
				else
				{
					h.Width = null;
					h.Height = handleSize;
				}
			}

			FireProportionsChanged();
		}

		public void ApplySplitPaneStyle(SplitPaneStyle style)
		{
			ApplyWidgetStyle(style);

			HandleStyle = style.HandleStyle;
			Reset();
		}

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var splitPane = (SplitPane)w;
			HandleStyle = splitPane.HandleStyle;
		}
	}
}