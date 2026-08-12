using Myra.Events;
using Myra.Utility;
using System;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Specifies how a row or column in a grid or stack panel divides available space.
	/// </summary>
	public enum ProportionType
	{
		/// <summary>The row or column automatically sizes to fit its content.</summary>
		Auto,
		/// <summary>The row or column shares space proportionally with other "Part" rows/columns based on the Value.</summary>
		Part,
		/// <summary>The row or column fills remaining available space.</summary>
		Fill,
		/// <summary>The row or column has a fixed size specified by the Value in pixels.</summary>
		Pixels
	}

	/// <summary>
	/// Defines how much space a row or column in a grid or stack panel should occupy.
	/// </summary>
	public class Proportion
	{
		/// <summary>
		/// Gets a proportion configured for automatic sizing.
		/// </summary>
		public static readonly Proportion Auto = new Proportion(ProportionType.Auto);

		/// <summary>
		/// Gets a proportion configured to fill remaining space.
		/// </summary>
		public static readonly Proportion Fill = new Proportion(ProportionType.Fill);

		/// <summary>
		/// Gets the default proportion for grid layout.
		/// </summary>
		public static readonly Proportion GridDefault = new Proportion(ProportionType.Part, 1.0f);

		/// <summary>
		/// Gets the default proportion for stack panel layout.
		/// </summary>
		public static readonly Proportion StackPanelDefault = new Proportion(ProportionType.Auto);

		private ProportionType _type;
		private float _value = 1.0f;

		/// <summary>
		/// Gets or sets the proportion type.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the proportion value (meaning depends on the proportion type).
		/// </summary>
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

		/// <summary>
		/// Occurs when the proportion has changed.
		/// </summary>
		public event MyraEventHandler Changed;

		/// <summary>
		/// Initializes a new instance of the <see cref="Proportion"/> class with the default type.
		/// </summary>
		public Proportion()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Proportion"/> class with the specified type.
		/// </summary>
		/// <param name="type">The proportion type.</param>
		public Proportion(ProportionType type)
		{
			_type = type;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Proportion"/> class with the specified type and value.
		/// </summary>
		/// <param name="type">The proportion type.</param>
		/// <param name="value">The proportion value.</param>
		public Proportion(ProportionType type, float value)
			: this(type)
		{
			_value = value;
		}

		/// <summary>
		/// Returns a string representation of the proportion.
		/// </summary>
		/// <returns>A string representation of the proportion type and value.</returns>
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
				ev(this, new MyraEventArgs(InputEventType.ProportionChanged));
			}
		}
	}
}
