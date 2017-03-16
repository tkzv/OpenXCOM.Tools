using System;
using System.Collections.Generic;


namespace XCom.Interfaces.Base
{
	public interface IMap_Observer
	{
		void SelectedTileChanged(IMap_Base sender, SelectedTileChangedEventArgs e);
		void HeightChanged(IMap_Base sender, HeightChangedEventArgs e);


		IMap_Base Map
		{ set; get;}

		Dictionary<string, IMap_Observer> MoreObservers
		{ get; }

		DSShared.Windows.RegistryInfo RegistryInfo
		{ get; set; }
	}

	/// <summary>
	/// EventArgs with an IMap_Base for when a SetMap event fires.
	/// </summary>
	public class SetMapEventArgs
		:
		EventArgs
	{
		private readonly IMap_Base _map;


		public SetMapEventArgs(IMap_Base map)
		{
			_map = map;
		}


		public IMap_Base Map
		{
			get { return _map; }
		}
	}


	/// <summary>
	/// EventArgs with a MapLocation and MapTile for when a SelectedTileChanged event fires.
	/// </summary>
	public class SelectedTileChangedEventArgs
		:
		EventArgs
	{
		private MapLocation _selLoc;
		private readonly MapTileBase _selTile;


		public SelectedTileChangedEventArgs(MapLocation selLoc, MapTileBase selTile)
		{
			_selLoc = selLoc;
			_selTile = selTile;
		}


		public MapLocation MapPosition
		{
			get { return _selLoc; }
		}

		public MapTileBase SelectedTile
		{
			get { return _selTile; }
		}
	}

	/// <summary>
	/// EventArgs for when a HeightChanged event fires.
	/// </summary>
	public class HeightChangedEventArgs
		:
		EventArgs
	{
		private readonly int _hNew;
		private readonly int _hOld;


		public HeightChangedEventArgs(int hOld, int hNew)
		{
			_hNew = hNew;
			_hOld = hOld;
		}


		public int NewHeight
		{
			get { return _hNew; }
		}

		public int OldHeight
		{
			get { return _hOld; }
		}
	}
}
