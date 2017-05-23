using System;
using System.Collections;
using System.IO;

using XCom.Resources.Map;


namespace XCom
{
	/// <summary>
	/// Describes information about a terrain: the path to the PCK, TAB, and MCD
	/// files.
	/// </summary>
	public sealed class Terrain
	{
		#region Fields & Properties
		private const string TerrainDir = @"\TERRAIN";

		public string Label
		{ get; private set; }

		public string PathTerrain
		{ get; set; }

		private readonly Hashtable _recordsTable;
		#endregion


		#region cTors
		/// <summary>
		/// cTor. Loads from YAML via the TerrainHerder.
		/// </summary>
		/// <param name="label"></param>
		public Terrain(string label)
		{
			Label       = label;
			PathTerrain = Path.Combine(SharedSpace.Instance.GetShare(SharedSpace.ResourcesDirectoryUfo), "TERRAIN"); // TODO: TFTD ....

			_recordsTable = new Hashtable(3);
		}
		/// <summary>
		/// Instantiates when a terrain is added in PathsEditor.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="path"></param>
		public Terrain(string label, string path)
		{
			Label       = label;
			PathTerrain = path;

			_recordsTable = new Hashtable(3);
		}
		#endregion


		#region Methods
//		/// <summary>
//		/// Gets the spriteset for this terrain given a palette.
//		/// </summary>
//		/// <param name="pal"></param>
//		/// <returns></returns>
//		public SpriteCollection GetSpriteset(Palette pal = null)
//		{
//			if (pal == null)
//				pal = Palette.UfoBattle;
//
//			return ResourceInfo.LoadSpriteset(Label, PathTerrain, 2, pal); // TODO: should '2' be '4' for TFTD
//		}

//		/// <summary>
//		/// Gets the MCD-records for this terrain given a palette.
//		/// NOTE: Calling this function instantiates records by palette if a
//		/// table doesn't exist yet.
//		/// </summary>
//		/// <param name="pal"></param>
//		/// <returns></returns>
//		public McdRecordCollection GetMcdRecords(Palette pal = null)
//		{
//			if (pal == null)
//				pal = Palette.UfoBattle;
//
//			if (_recordsTable[pal] == null)
//			{
//				var tiles = XCTileFactory.CreateRecords(Label, PathTerrain, GetSpriteset(pal));
//				_recordsTable[pal] = new McdRecordCollection(tiles);
//			}
//
//			return _recordsTable[pal] as McdRecordCollection;
//		}

//		/// <summary>
//		/// Clears the MCD table ....
//		/// </summary>
//		public void ClearMcdTable()
//		{
//			_recordsTable.Clear();
//		}

		/// <summary>
		/// Override for ToString().
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Label;
		}
		#endregion
	}
}
