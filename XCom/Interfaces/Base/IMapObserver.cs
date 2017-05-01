using System;
using System.Collections.Generic;


namespace XCom.Interfaces.Base
{
	/// <summary>
	/// Parent of MapObserverControl0 and MapObserverControl1.
	/// </summary>
	public interface IMapObserver
	{
		XCMapBase MapBase
		{ set; get;}

		Dictionary<string, IMapObserver> MoreObservers
		{ get; }

//		DSShared.Windows.RegistryInfo RegistryInfo
//		{ get; set; }


		void OnLocationChanged(XCMapBase sender, LocationChangedEventArgs e);

		void OnLevelChanged(XCMapBase sender, LevelChangedEventArgs e);
	}


/*	/// <summary>
	/// EventArgs with an XCMapBase for when a SetMap event fires.
	/// </summary>
	public class SetMapEventArgs
		:
			EventArgs
	{
		private readonly XCMapBase _mapBase;
		public XCMapBase Map
		{
			get { return _mapBase; }
		}
		internal SetMapEventArgs(XCMapBase mapBase)
		{
			_mapBase = mapBase;
		}
	} */


	/// <summary>
	/// EventArgs with a MapLocation and MapTile for when a LocationChanged
	/// event fires.
	/// </summary>
	public sealed class LocationChangedEventArgs
		:
			EventArgs
	{
		private readonly MapLocation _location;
		public MapLocation Location
		{
			get { return _location; }
		}

		private readonly MapTileBase _baseTile;
		public MapTileBase SelectedTile
		{
			get { return _baseTile; }
		}


		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="location"></param>
		/// <param name="baseTile"></param>
		internal LocationChangedEventArgs(MapLocation location, MapTileBase baseTile)
		{
			_location = location;
			_baseTile = baseTile;
		}
	}

	/// <summary>
	/// EventArgs for when a LevelChanged event fires.
	/// </summary>
	public sealed class LevelChangedEventArgs
		:
			EventArgs
	{
		private readonly int _level;
		public int Level
		{
			get { return _level; }
		}


		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="level">the new level</param>
		internal LevelChangedEventArgs(int level)
		{
			_level = level;
		}
	}
}
