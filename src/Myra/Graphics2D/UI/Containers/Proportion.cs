using Myra.Utility;
using System;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	public enum ProportionType
	{
		Auto,
		Part,
		Fill,
		Pixels
	}

	public class Proportion
	{
		public static readonly Proportion Auto = new Proportion(ProportionType.Auto);
		public static readonly Proportion Fill = new Proportion(ProportionType.Fill);

		public static readonly Proportion GridDefault = new Proportion(ProportionType.Part, 1.0f);
		public static readonly Proportion StackPanelDefault = new Proportion(ProportionType.Auto);

		private ProportionType _type;
		private float _value = 1.0f;

		public ProportionType Type
		{
			get { return _type; }

			set
			{
				if (value == _type) return;
				_type = value;
				FireChanged();
			}
		}

		[DefaultValue(1.0f)]
		public float Value
		{
			get { return _value; }
			set
			{
				if (value.EpsilonEquals(_value))
				{
					return;
				}

				_value = value;
				FireChanged();
			}
		}

		public event EventHandler Changed;

		public Proportion()
		{
		}

		public Proportion(ProportionType type)
		{
			_type = type;
		}

		public Proportion(ProportionType type, float value)
			: this(type)
		{
			_value = value;
		}

		public override string ToString()
		{
			if (_type == ProportionType.Auto || _type == ProportionType.Fill)
			{
				return _type.ToString();
			}

			if (_type == ProportionType.Part)
			{
				return string.Format("{0}: {1:0.00}", _type, _value);
			}

			// Pixels
			return string.Format("{0}: {1}", _type, (int)_value);
		}

		private void FireChanged()
		{
			var ev = Changed;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}
	}
}
