using System.Runtime.InteropServices;

namespace StbTextEditSharp
{
	public struct FindState
	{
		public float x;
		public float y;
		public float height;
		public int first_char;
		public int length;
		public int prev_first;

		public void FindCharPosition(TextEdit str, int n, bool single_line)
		{
			TextEditRow r = new TextEditRow();
			int prev_start = (int)(0);
			int z = (int)(str.Length);
			int i = (int)(0);
			int first = 0;
			if ((n) == (z))
			{
				if (single_line)
				{
					r = str.Handler.LayoutRow(0);
					y = (float)(0);
					first_char = (int)(0);
					length = (int)(z);
					height = (float)(r.ymax - r.ymin);
					x = (float)(r.x1);
				}
				else
				{
					y = (float)(0);
					x = (float)(0);
					height = (float)(1);
					while ((i) < (z))
					{
						r = str.Handler.LayoutRow(i);
						prev_start = (int)(i);
						i += (int)(r.num_chars);
					}
					first_char = (int)(i);
					length = (int)(0);
					prev_first = (int)(prev_start);
				}
				return;
			}

			y = (float)(0);
			for (; ; )
			{
				r = str.Handler.LayoutRow(i);
				if ((n) < (i + r.num_chars))
					break;
				prev_start = (int)(i);
				i += (int)(r.num_chars);
				y += (float)(r.baseline_y_delta);
			}
			first_char = (int)(first = (int)(i));
			length = (int)(r.num_chars);
			height = (float)(r.ymax - r.ymin);
			prev_first = (int)(prev_start);
			x = (float)(r.x0);
			for (i = (int)(0); (first + i) < (n); ++i)
			{
				x += (float)(1);
			}
		}
	}
}
