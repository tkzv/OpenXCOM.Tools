using System;
using System.Collections.Generic;

using XCom.Interfaces.Base;


namespace XCom
{
	public enum QuadrantType
	{
		Ground,
		West,
		North,
		Content
	};


	public sealed class XCMapTile
		:
			MapTileBase
	{
		#region Fields & Properties
		private TileBase _ground;
		public TileBase Ground
		{
			get { return _ground; }
			set { ChangeMapQuadrant(QuadrantType.Ground, value); }
		}

		private TileBase _west;
		public TileBase West
		{
			get { return _west; }
			set { ChangeMapQuadrant(QuadrantType.West, value); }
		}

		private TileBase _north;
		public TileBase North
		{
			get { return _north; }
			set { ChangeMapQuadrant(QuadrantType.North, value); }
		}

		private TileBase _content;
		public TileBase Content
		{
			get { return _content; }
			set { ChangeMapQuadrant(QuadrantType.Content, value); }
		}

		public bool Blank
		{ get; set; }

		public static XCMapTile BlankTile
		{
			get
			{
				var tile = new XCMapTile(null, null, null, null);
				tile.Blank = true;
				return tile;
			}
		}

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

		public RouteNode Node
		{ get; set; }

		public override TileBase[] UsedTiles
		{
			get
			{
				var tileList = new List<TileBase>();

				if (Ground  != null) tileList.Add(Ground);
				if (West    != null) tileList.Add(West);
				if (North   != null) tileList.Add(North);
				if (Content != null) tileList.Add(Content);

				return tileList.ToArray();
			}
		}
		#endregion


		#region cTor
		internal XCMapTile(
				TileBase ground,
				TileBase west,
				TileBase north,
				TileBase content)
		{
			_ground  = ground;
			_west    = west;
			_north   = north;
			_content = content;

			DrawAbove = true;
		}
		#endregion


		#region Methods
		private void ChangeMapQuadrant(QuadrantType quad, TileBase tile)
		{
			switch (quad)
			{
				case QuadrantType.Ground:  _ground  = tile; break;
				case QuadrantType.West:    _west    = tile; break;
				case QuadrantType.North:   _north   = tile; break;
				case QuadrantType.Content: _content = tile; break;
			}
		}
		#endregion
	}
}
