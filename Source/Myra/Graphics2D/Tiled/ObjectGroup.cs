// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml.Linq;

namespace Myra.Graphics2D.Tiled
{
	public class TmxObjectGroup : ITmxElement
	{
		public string Name { get; private set; }

		// TODO: Legacy (Tiled Java) attributes (x, y, width, height)

		public TmxColor Color { get; private set; }
		public DrawOrderType DrawOrder { get; private set; }

		public double Opacity { get; private set; }
		public bool Visible { get; private set; }
		public double OffsetX { get; private set; }
		public double OffsetY { get; private set; }

		public TmxList<TmxObject> Objects { get; private set; }
		public PropertyDict Properties { get; private set; }

		public TmxObjectGroup(XElement xObjectGroup)
		{
			Name = (string) xObjectGroup.Attribute("name") ?? String.Empty;

			var xColor = xObjectGroup.Attribute("color");
			if (xColor != null)
			{
				Color = new TmxColor(xColor);
			}
			Opacity = (double?) xObjectGroup.Attribute("opacity") ?? 1.0;
			Visible = (bool?) xObjectGroup.Attribute("visible") ?? true;
			OffsetX = (double?) xObjectGroup.Attribute("offsetx") ?? 0.0;
			OffsetY = (double?) xObjectGroup.Attribute("offsety") ?? 0.0;

			var drawOrderDict = new Dictionary<string, DrawOrderType>
			{
				{"unknown", DrawOrderType.UnknownOrder},
				{"topdown", DrawOrderType.IndexOrder},
				{"index", DrawOrderType.TopDown}
			};

			var drawOrderValue = (string) xObjectGroup.Attribute("draworder");
			if (drawOrderValue != null)
				DrawOrder = drawOrderDict[drawOrderValue];

			Objects = new TmxList<TmxObject>();
			foreach (var e in xObjectGroup.Elements("object"))
				Objects.Add(new TmxObject(e));

			Properties = new PropertyDict(xObjectGroup.Element("properties"));
		}
	}

	public class TmxObject : ITmxElement
	{
		// Many TmxObjectTypes are distinguished by null values in fields
		// It might be smart to subclass TmxObject
		public int Id { get; private set; }
		public string Name { get; private set; }
		public TmxObjectType ObjectType { get; private set; }
		public string Type { get; private set; }
		public double X { get; private set; }
		public double Y { get; private set; }
		public double Width { get; private set; }
		public double Height { get; private set; }
		public double Rotation { get; private set; }
		public TmxLayerTile Tile { get; private set; }
		public bool Visible { get; private set; }

		public Collection<TmxObjectPoint> Points { get; private set; }
		public PropertyDict Properties { get; private set; }

		public TmxObject(XElement xObject)
		{
			Id = (int?) xObject.Attribute("id") ?? 0;
			Name = (string) xObject.Attribute("name") ?? String.Empty;
			X = (double) xObject.Attribute("x");
			Y = (double) xObject.Attribute("y");
			Width = (double?) xObject.Attribute("width") ?? 0.0;
			Height = (double?) xObject.Attribute("height") ?? 0.0;
			Type = (string) xObject.Attribute("type") ?? String.Empty;
			Visible = (bool?) xObject.Attribute("visible") ?? true;
			Rotation = (double?) xObject.Attribute("rotation") ?? 0.0;

			// Assess object type and assign appropriate content
			var xGid = xObject.Attribute("gid");
			var xEllipse = xObject.Element("ellipse");
			var xPolygon = xObject.Element("polygon");
			var xPolyline = xObject.Element("polyline");

			if (xGid != null)
			{
				Tile = new TmxLayerTile((uint) xGid);
				ObjectType = TmxObjectType.Tile;
			}
			else if (xEllipse != null)
			{
				ObjectType = TmxObjectType.Ellipse;
			}
			else if (xPolygon != null)
			{
				Points = ParsePoints(xPolygon);
				ObjectType = TmxObjectType.Polygon;
			}
			else if (xPolyline != null)
			{
				Points = ParsePoints(xPolyline);
				ObjectType = TmxObjectType.Polyline;
			}
			else ObjectType = TmxObjectType.Basic;

			Properties = new PropertyDict(xObject.Element("properties"));
		}

		public Collection<TmxObjectPoint> ParsePoints(XElement xPoints)
		{
			var points = new Collection<TmxObjectPoint>();

			var pointString = (string) xPoints.Attribute("points");
			var pointStringPair = pointString.Split(' ');
			foreach (var s in pointStringPair)
			{
				var pt = new TmxObjectPoint(s);
				points.Add(pt);
			}
			return points;
		}
	}

	public class TmxObjectPoint
	{
		public double X { get; private set; }
		public double Y { get; private set; }

		public TmxObjectPoint(double x, double y)
		{
			X = x;
			Y = y;
		}

		public TmxObjectPoint(string s)
		{
			var pt = s.Split(',');
			X = double.Parse(pt[0], NumberStyles.Float,
				CultureInfo.InvariantCulture);
			Y = double.Parse(pt[1], NumberStyles.Float,
				CultureInfo.InvariantCulture);
		}
	}

	public enum TmxObjectType
	{
		Basic,
		Tile,
		Ellipse,
		Polygon,
		Polyline
	}

	public enum DrawOrderType
	{
		UnknownOrder = -1,
		TopDown,
		IndexOrder
	}
}