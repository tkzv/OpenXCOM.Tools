using System;
using System.Collections.Generic;


namespace XCom.Interfaces.Base
{
	public interface IMapObserver
	{
		void OnSelectedTileChanged(IMapBase sender, SelectedTileChangedEventArgs e);
		void OnHeightChanged(IMapBase sender, HeightChangedEventArgs e);


		IMapBase Map
		{ set; get;}

		Dictionary<string, IMapObserver> MoreObservers
		{ get; }

		DSShared.Windows.RegistryInfo RegistryInfo
		{ get; set; }
	}


/*	/// <summary>
	/// EventArgs with an IMapBase for when a SetMap event fires.
	/// </summary>
	public class SetMapEventArgs
		:
			EventArgs
	{
		private readonly IMapBase _baseMap;
		public IMapBase Map
		{
			get { return _baseMap; }
		}


		internal SetMapEventArgs(IMapBase baseMap)
		{
			_baseMap = baseMap;
		}
	} */


	/// <summary>
	/// EventArgs with a MapLocation and MapTile for when a SelectedTileChanged event fires.
	/// </summary>
	public sealed class SelectedTileChangedEventArgs
		:
			EventArgs
	{
		private MapLocation _selLoc;
		public MapLocation MapPosition
		{
			get { return _selLoc; }
		}

		private readonly MapTileBase _selTile;
		public MapTileBase SelectedTile
		{
			get { return _selTile; }
		}


		internal SelectedTileChangedEventArgs(MapLocation selLoc, MapTileBase selTile)
		{
			_selLoc  = selLoc;
			_selTile = selTile;
		}
	}

	/// <summary>
	/// EventArgs for when a HeightChanged event fires.
	/// </summary>
	public sealed class HeightChangedEventArgs
		:
			EventArgs
	{
		private readonly int _heightNew;
		public int NewHeight
		{
			get { return _heightNew; }
		}

/*		private readonly int _heightOld;
		public int OldHeight
		{
			get { return _heightOld; }
		} */


//		public HeightChangedEventArgs(int heightOld, int heightNew)
		internal HeightChangedEventArgs(int heightNew)
		{
			_heightNew = heightNew;
//			_heightOld = heightOld;
		}
	}
}
