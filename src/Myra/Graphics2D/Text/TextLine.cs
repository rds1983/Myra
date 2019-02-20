using System.Collections.ObjectModel;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
#endif

namespace Myra.Graphics2D.Text
{
	public class TextLine
	{
		private readonly ObservableCollection<TextRun> _textRuns = new ObservableCollection<TextRun>();
		private Point _size;
		private Point? _renderedPosition;
		private bool _dirty = true;

		public Point Size
		{
			get
			{
				Update();

				return _size;
			}
		}

		public int Count { get; private set; }

		public ObservableCollection<TextRun> TextRuns
		{
			get { return _textRuns; }
		}

		public TextLine()
		{
			_textRuns.CollectionChanged += (s, a) => { _dirty = true; };
		}

		private void Update()
		{
			if (!_dirty)
			{
				return;
			}

			Count = 0;
			_size = Point.Zero;
			foreach (var run in _textRuns)
			{
				run.Update();

				_size.X += run.Size.X;

				if (run.Size.Y > _size.Y)
				{
					_size.Y = run.Size.Y;
				}

				Count += run.Count;
			}

			_dirty = false;
		}

		public void Draw(SpriteBatch batch, Point pos, Color color, float opacity = 1.0f)
		{
			Update();

			_renderedPosition = pos;

			foreach (var run in _textRuns)
			{
				run.Draw(batch, pos, run.Color ?? color, opacity);

				pos.X += run.Size.X;
			}
		}

		public GlyphInfo GetGlyphInfoByIndex(int index)
		{
			if (_renderedPosition == null)
			{
				return null;
			}

			Update();


			foreach (var run in _textRuns)
			{
				var result = run.GetGlyphInfoByIndex(index);

				if (result != null)
				{
					return result;
				}

				index -= run.Count;
			}

			return null;
		}
	}
}
