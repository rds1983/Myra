using System;
using Microsoft.Xna.Framework;

namespace Myra.Graphics2D.Pixmaps
{
	public abstract class PixmapT<T> : Pixmap where T : struct
	{
		private readonly T[] _data;

		public T[] Data
		{
			get { return _data; }
		}

		protected PixmapT(Point size) : base(size)
		{
			_data = new T[size.X*size.Y];
		}

		public void Load(T[] data)
		{
			Array.Copy(data, 0, _data, 0, Math.Min(data.Length, _data.Length));
		}

		public T Get(Point p)
		{
			if (p.X < 0 ||
			    p.Y < 0 ||
			    p.X >= Size.X ||
			    p.Y >= Size.Y)
			{
				throw new ArgumentOutOfRangeException("p");
			}

			return _data[p.X + p.Y*Size.X];
		}

		public void Set(Point p, T value)
		{
			if (p.X < 0 ||
			    p.Y < 0 ||
			    p.X >= Size.X ||
			    p.Y >= Size.Y)
			{
				throw new ArgumentOutOfRangeException("p");
			}

			_data[p.X + p.Y*Size.X] = value;
		}
	}
}