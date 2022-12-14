using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myra
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class RenderAsSliderAttribute : Attribute
	{
		public RenderAsSliderAttribute() { }
		public RenderAsSliderAttribute(int min, int max)
		{
			Min = min;
			Max = max;
		}
		public int Min { get; set; }
		public int Max { get; set; }
	}
}
