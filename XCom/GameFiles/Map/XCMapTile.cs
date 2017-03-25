using System;
using System.Collections.Generic;

using XCom.Interfaces.Base;


namespace XCom
{
	public class XCMapTile
		:
		MapTileBase
	{
		public enum QuadrantType
		{
			Ground,
			West,
			North,
			Content
		};

		private TileBase _ground;
		private TileBase _north;
		private TileBase _west;
		private TileBase _content;


		internal XCMapTile(
				TileBase ground,
				TileBase west,
				TileBase north,
				TileBase content)
		{
			_ground  = ground;
			_north   = north;
			_west    = west;
			_content = content;

			Blank = false;
			DrawAbove = true;
		}

		public static XCMapTile BlankTile
		{
			get
			{
				var tile = new XCMapTile(null, null, null, null);
				tile.Blank = true;
				return tile;
			}
		}

		public bool Blank
		{ get; set; }

		public TileBase this[QuadrantType quad]
		{
			get
			{
				switch (quad)
				{
					case QuadrantType.Ground:  return Ground;
					case QuadrantType.West:    return West;
					case QuadrantType.North:   return North;
					case QuadrantType.Content: return Content;
				}
				return null;
			}

			set { ChangeMapQuadrant(quad, value); }
		}

		public TileBase North
		{
			get { return _north; }
			set { ChangeMapQuadrant(QuadrantType.North, value); }
		}

		public TileBase Content
		{
			get { return _content; }
			set { ChangeMapQuadrant(QuadrantType.Content, value); }
		}

		public TileBase Ground
		{
			get { return _ground; }
			set { ChangeMapQuadrant(QuadrantType.Ground, value); }
		}

		public TileBase West
		{
			get { return _west; }
			set { ChangeMapQuadrant(QuadrantType.West, value); }
		}

		public RouteNode Node
		{ get; set; }

		public override TileBase[] UsedTiles
		{
			get
			{
				var list = new List<TileBase>();

				if (Ground  != null) list.Add(Ground);
				if (West    != null) list.Add(West);
				if (North   != null) list.Add(North);
				if (Content != null) list.Add(Content);

				return list.ToArray();
			}
		}

		private void ChangeMapQuadrant(QuadrantType quad, TileBase value)
		{
			switch (quad)
			{
				case QuadrantType.Ground:  _ground  = value; break;
				case QuadrantType.West:    _west    = value; break;
				case QuadrantType.North:   _north   = value; break;
				case QuadrantType.Content: _content = value; break;
			}
		}
	}
}
