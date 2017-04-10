using System;
using System.Collections.Generic;


namespace XCom.Interfaces.Base
{
	/// <summary>
	/// Parent of MapObserverControl0 and MapObserverControl1.
	/// </summary>
	public interface IMapObserver
	{
		IMapBase BaseMap
		{ set; get;}

		Dictionary<string, IMapObserver> MoreObservers
		{ get; }

//		DSShared.Windows.RegistryInfo RegistryInfo
//		{ get; set; }


		void OnSelectedTileChanged(IMapBase sender, SelectedTileChangedEventArgs e);

		void OnHeightChanged(IMapBase sender, HeightChangedEventArgs e);
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
		private MapLocation _location;
		public MapLocation Location
		{
			get { return _location; }
		}

		private readonly MapTileBase _baseTile;
		public MapTileBase SelectedTile
		{
			get { return _baseTile; }
		}


		internal SelectedTileChangedEventArgs(MapLocation location, MapTileBase baseTile)
		{
			_location = location;
			_baseTile = baseTile;
		}
	}

	/// <summary>
	/// EventArgs for when a HeightChanged event fires.
	/// </summary>
	public sealed class HeightChangedEventArgs
		:
			EventArgs
	{
		private readonly int _height;
		public int Height
		{
			get { return _height; }
		}

/*		private readonly int _heightOld;
		public int OldHeight
		{
			get { return _heightOld; }
		} */


//		public HeightChangedEventArgs(int heightOld, int heightNew)
		internal HeightChangedEventArgs(int height)
		{
			_height = height;
//			_heightOld = heightOld;
		}
	}
}
