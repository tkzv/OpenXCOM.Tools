using System;
using System.Collections.Generic;
using System.Windows.Forms;

using XCom.Interfaces.Base;


namespace XCom
{
	public sealed class TileGroupManager
	{
		#region Fields & Properties
		private readonly string _path;
		public string Path
		{
			get { return _path; }
		}

		private readonly Dictionary<string, TileGroupBase> _tilegroups = new Dictionary<string, TileGroupBase>();
		public Dictionary<string, TileGroupBase> TileGroups
		{
			get { return _tilegroups; }
		}
		#endregion


		#region cTor
		internal TileGroupManager(TilesetManager tilesetManager)
		{
			_path = tilesetManager.FullPath; // TODO: not right. not needed.

			foreach (string tilegroup in tilesetManager.Groups)
				_tilegroups[tilegroup] = new TileGroupChild(tilegroup, tilesetManager.Tilesets);
		}
		#endregion


		#region Methods
		public TileGroupBase AddTileGroup(
				string label,
				string pathMaps,
				string pathRoutes,
				string pathOccults)
		{
			var tilegroup = new TileGroupChild(label);

			tilegroup.MapDirectory    = pathMaps;
			tilegroup.RouteDirectory  = pathRoutes;
			tilegroup.OccultDirectory = pathOccults;

			return (_tilegroups[label] = tilegroup);
		}
		#endregion
	}
}
