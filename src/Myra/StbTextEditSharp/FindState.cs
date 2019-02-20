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
			var r = new TextEditRow();
			var prev_start = 0;
			var z = str.Length;
			var i = 0;
			var first = 0;
			if (n == z)
			{
				if (single_line)
				{
					r = str.Handler.LayoutRow(0);
					y = 0;
					first_char = 0;
					length = z;
					height = r.ymax - r.ymin;
					x = r.x1;
				}
				else
				{
					y = 0;
					x = 0;
					height = 1;
					while (i < z)
					{
						r = str.Handler.LayoutRow(i);
						prev_start = i;
						i += r.num_chars;
					}

					first_char = i;
					length = 0;
					prev_first = prev_start;
				}

				return;
			}

			y = 0;
			for (; ; )
			{
				r = str.Handler.LayoutRow(i);
				if (n < i + r.num_chars)
					break;
				prev_start = i;
				i += r.num_chars;
				y += r.baseline_y_delta;
			}

			first_char = first = i;
			length = r.num_chars;
			height = r.ymax - r.ymin;
			prev_first = prev_start;
			x = r.x0;
			for (i = 0; first + i < n; ++i) x += 1;
		}
	}
}