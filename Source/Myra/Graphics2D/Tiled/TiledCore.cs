// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D.Tiled
{
	public interface ITmxElement
	{
		string Name { get; }
	}

	public class TmxList<T> : KeyedCollection<string, T> where T : ITmxElement
	{
		private readonly Dictionary<string, int> nameCount
			= new Dictionary<string, int>();

		public new void Add(T t)
		{
			var tName = t.Name;

			// Rename duplicate entries by appending a number
			if (Contains(tName))
				nameCount[tName] += 1;
			else
				nameCount.Add(tName, 0);
			base.Add(t);
		}

		protected override string GetKeyForItem(T item)
		{
			var name = item.Name;
			var count = nameCount[name];

			var dupes = 0;

			// For duplicate keys, append a counter
			// For pathological cases, insert underscores to ensure uniqueness
			while (Contains(name))
			{
				name = name + string.Concat(Enumerable.Repeat("_", dupes)) + count;
				dupes++;
			}

			return name;
		}
	}

	public class PropertyDict : Dictionary<string, string>
	{
		public PropertyDict(XContainer xmlProp)
		{
			if (xmlProp == null) return;

			foreach (var p in xmlProp.Elements("property"))
			{
				string pval;

				var pname = p.Attribute("name").Value;
				try
				{
					pval = p.Attribute("value").Value;
				}
				catch (NullReferenceException)
				{
					// Fallback to element value if no "value"
					pval = p.Value;
				}

				Add(pname, pval);
			}
		}
	}

	public class TmxImage
	{
		public string Source { get; private set; }
		public string Format { get; private set; }
		public Stream Data { get; private set; }
		public TmxColor Trans { get; private set; }
		public int? Width { get; private set; }
		public int? Height { get; private set; }
		public Texture2D Texture { get; set; }

		public TmxImage(XElement xImage, Func<string, RawImage> imageLoader)
		{
			if (xImage == null) return;

			var xSource = xImage.Attribute("source");

			if (xSource != null)
			{
				// Append directory if present
				Source = (string) xSource;
			}
			else
			{
				Format = (string) xImage.Attribute("format");
				var xData = xImage.Element("data");
				var decodedStream = new TmxBase64Data(xData);
				Data = decodedStream.Data;
			}

			var xColor = xImage.Attribute("trans");
			if (xColor != null)
			{
				Trans = new TmxColor(xColor);
			}
			Width = (int?) xImage.Attribute("width");
			Height = (int?) xImage.Attribute("height");

			var rawImage  = !string.IsNullOrEmpty(Source)
				? imageLoader(Source)
				: RawImage.FromStream(Data);

			rawImage.Process(true, Trans != null ? new Color(Trans.R, Trans.G, Trans.B, 0) : default(Color?));

			Texture = rawImage.CreateTexture2D();
			if (Width == null)
			{
				Width = Texture.Width;
			}

			if (Height == null)
			{
				Height = Texture.Height;
			}
		}
	}

	public class TmxColor
	{
		public int R { get; private set; }
		public int G { get; private set; }
		public int B { get; private set; }

		public TmxColor(XAttribute xColor)
		{
			var colorStr = ((string) xColor).TrimStart("#".ToCharArray());

			R = int.Parse(colorStr.Substring(0, 2), NumberStyles.HexNumber);
			G = int.Parse(colorStr.Substring(2, 2), NumberStyles.HexNumber);
			B = int.Parse(colorStr.Substring(4, 2), NumberStyles.HexNumber);
		}
	}

	public class TmxBase64Data
	{
		public Stream Data { get; private set; }

		public TmxBase64Data(XElement xData)
		{
			if ((string) xData.Attribute("encoding") != "base64")
				throw new Exception(
					"TmxBase64Data: Only Base64-encoded data is supported.");

			var rawData = Convert.FromBase64String(xData.Value);
			Data = new MemoryStream(rawData, false);

			var compression = (string) xData.Attribute("compression");
			if (compression == "gzip")
			{
				Data = new GZipStream(Data, CompressionMode.Decompress);
			}
			else if (compression == "zlib")
			{
				// Strip 2-byte header and 4-byte checksum
				// TODO: Validate header here
				var bodyLength = rawData.Length - 6;
				var bodyData = new byte[bodyLength];
				Array.Copy(rawData, 2, bodyData, 0, bodyLength);

				var bodyStream = new MemoryStream(bodyData, false);
				Data = new DeflateStream(bodyStream, CompressionMode.Decompress);

				// TODO: Validate checksum?
			}
			else if (compression != null)
				throw new Exception("TmxBase64Data: Unknown compression.");
		}
	}
}