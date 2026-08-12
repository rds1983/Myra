using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using Myra.Attributes;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D
{
	/// <summary>
	/// Represents the thicknesses of the four sides of a rectangle (left, right, top, bottom).
	/// </summary>
	public struct Thickness
	{
		/// <summary>
		/// Gets a thickness with all values set to zero.
		/// </summary>
		public static readonly Thickness Zero = new Thickness();

		/// <summary>
		/// Gets or sets the left thickness.
		/// </summary>
		[Range(0)]
		public int Left { get; set; }

		/// <summary>
		/// Gets or sets the right thickness.
		/// </summary>
		[Range(0)]
		public int Right { get; set; }

		/// <summary>
		/// Gets or sets the top thickness.
		/// </summary>
		[Range(0)]
		public int Top { get; set; }

		/// <summary>
		/// Gets or sets the bottom thickness.
		/// </summary>
		[Range(0)]
		public int Bottom { get; set; }

		/// <summary>
		/// Gets the total horizontal thickness (left + right).
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int Width
		{
			get
			{
				return Left + Right;
			}
		}

		/// <summary>
		/// Gets the total vertical thickness (top + bottom).
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int Height
		{
			get
			{
				return Top + Bottom;
			}
		}

		/// <summary>
		/// Gets a value indicating whether all sides have the same thickness.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public bool SameSize
		{
			get
			{
				return (Left == Top && Top == Right && Right == Bottom);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Thickness"/> struct with individual values for each side.
		/// </summary>
		/// <param name="left">The left thickness.</param>
		/// <param name="top">The top thickness.</param>
		/// <param name="right">The right thickness.</param>
		/// <param name="bottom">The bottom thickness.</param>
		public Thickness(int left, int top, int right, int bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Thickness"/> struct with separate horizontal and vertical values.
		/// </summary>
		/// <param name="horizontalValue">The thickness for left and right sides.</param>
		/// <param name="verticalValue">The thickness for top and bottom sides.</param>
		public Thickness(int horizontalValue, int verticalValue) : this(horizontalValue, verticalValue, horizontalValue, verticalValue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Thickness"/> struct with the same value for all sides.
		/// </summary>
		/// <param name="value">The thickness for all sides.</param>
		public Thickness(int value) : this(value, value, value, value)
		{
		}

		/// <summary>
		/// Returns a string representation of the thickness.
		/// </summary>
		/// <returns>A string representation in the format "value", "h, v", or "l, t, r, b".</returns>
		public override string ToString()
		{
			if (SameSize)
			{
				return Left.ToString();
			}

			if (Left == Right && Top == Bottom)
			{
				return string.Format("{0}, {1}", Left, Top);
			}

			return string.Format("{0}, {1}, {2}, {3}", Left, Top, Right, Bottom);
		}

		/// <summary>
		/// Parses a thickness value from a string.
		/// </summary>
		/// <remarks>
		/// Supported formats: "5" (all sides), "5, 10" (horizontal, vertical), "1, 2, 3, 4" (left, top, right, bottom).
		/// </remarks>
		/// <param name="s">The string to parse.</param>
		/// <returns>The parsed thickness, or Zero if the string is null or empty.</returns>
		/// <exception cref="ArgumentException">The string format is invalid.</exception>
		public static Thickness FromString(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return Zero;
			}

			var parts = (from p in s.Split(',') select p.Trim()).ToArray();
			if (parts.Length != 1 && parts.Length != 2 && parts.Length != 4)
			{
				throw new ArgumentException(string.Format("Could not convert string '{0}' to Thickness", s));
			}

			if (parts.Length == 1)
			{
				return new Thickness(int.Parse(parts[0]));
			}

			if (parts.Length == 2)
			{
				return new Thickness(int.Parse(parts[0]), int.Parse(parts[1]));
			}

			return new Thickness(int.Parse(parts[0]),
				int.Parse(parts[1]),
				int.Parse(parts[2]),
				int.Parse(parts[3]));
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current thickness.
		/// </summary>
		/// <param name="obj">The object to compare with the current thickness.</param>
		/// <returns>True if the specified object is equal to the current thickness; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Thickness))
			{
				return false;
			}

			var thickness = (Thickness)obj;
			return Left == thickness.Left &&
				   Right == thickness.Right &&
				   Top == thickness.Top &&
				   Bottom == thickness.Bottom;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			var hashCode = 551583723;
			hashCode = hashCode * -1521134295 + Left.GetHashCode();
			hashCode = hashCode * -1521134295 + Right.GetHashCode();
			hashCode = hashCode * -1521134295 + Top.GetHashCode();
			hashCode = hashCode * -1521134295 + Bottom.GetHashCode();
			return hashCode;
		}

		/// <summary>
		/// Determines whether two thickness values are equal.
		/// </summary>
		/// <param name="a">The first thickness to compare.</param>
		/// <param name="b">The second thickness to compare.</param>
		/// <returns>True if the two thickness values are equal; otherwise, false.</returns>
		public static bool operator ==(Thickness a, Thickness b)
		{
			return a.Equals(b);
		}

		/// <summary>
		/// Determines whether two thickness values are not equal.
		/// </summary>
		/// <param name="a">The first thickness to compare.</param>
		/// <param name="b">The second thickness to compare.</param>
		/// <returns>True if the two thickness values are not equal; otherwise, false.</returns>
		public static bool operator !=(Thickness a, Thickness b)
		{
			return !(a == b);
		}

		/// <summary>
		/// Subtracts the thickness from a rectangle.
		/// </summary>
		/// <param name="a">The rectangle to subtract from.</param>
		/// <param name="b">The thickness to subtract.</param>
		/// <returns>A new rectangle with the thickness applied, shrinking the rectangle.</returns>
		public static Rectangle operator -(Rectangle a, Thickness b)
		{
			var result = a;
			result.X += b.Left;
			result.Y += b.Top;

			result.Width -= b.Width;
			if (result.Width < 0)
			{
				result.Width = 0;
			}

			result.Height -= b.Height;
			if (result.Height < 0)
			{
				result.Height = 0;
			}

			return result;
		}
	}
}