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
		private TilepartBase _ground;
		public TilepartBase Ground
		{
			get { return _ground; }
			set { ChangeMapQuadrant(QuadrantType.Ground, value); }
		}

		private TilepartBase _west;
		public TilepartBase West
		{
			get { return _west; }
			set { ChangeMapQuadrant(QuadrantType.West, value); }
		}

		private TilepartBase _north;
		public TilepartBase North
		{
			get { return _north; }
			set { ChangeMapQuadrant(QuadrantType.North, value); }
		}

		private TilepartBase _content;
		public TilepartBase Content
		{
			get { return _content; }
			set { ChangeMapQuadrant(QuadrantType.Content, value); }
		}

		public TilepartBase this[QuadrantType quad]
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

		public override TilepartBase[] UsedTiles
		{
			get
			{
				var partList = new List<TilepartBase>();

				if (Ground  != null) partList.Add(Ground);
				if (West    != null) partList.Add(West);
				if (North   != null) partList.Add(North);
				if (Content != null) partList.Add(Content);

				return partList.ToArray();
			}
		}

		public RouteNode Node
		{ get; set; }

		/// <summary>
		/// Used only by MapInfoForm.
		/// </summary>
		public bool Vacant
		{ get; set; }

		public static XCMapTile VacantTile
		{
			get
			{
				var tile = new XCMapTile(null, null, null, null);
				tile.Vacant = true;
				return tile;
			}
		}
		#endregion


		#region cTor
		internal XCMapTile(
				TilepartBase ground,
				TilepartBase west,
				TilepartBase north,
				TilepartBase content)
		{
			_ground  = ground;
			_west    = west;
			_north   = north;
			_content = content;

			DrawAbove = true;
		}
		#endregion


		#region Methods
		private void ChangeMapQuadrant(QuadrantType quad, TilepartBase part)
		{
			switch (quad)
			{
				case QuadrantType.Ground:  _ground  = part; break;
				case QuadrantType.West:    _west    = part; break;
				case QuadrantType.North:   _north   = part; break;
				case QuadrantType.Content: _content = part; break;
			}
		}
		#endregion
	}
}
