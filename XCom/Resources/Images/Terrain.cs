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
		#region Fields
		private readonly Hashtable _recordsTable;
		#endregion


		#region Properties
		public string Label
		{ get; private set; }

		public string PathDirectory
		{ get; set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor. Loads from YAML.
		/// </summary>
		/// <param name="label"></param>
		public Terrain(string label)
		{
			Label = label;
			PathDirectory = Path.Combine(SharedSpace.Instance.GetShare(SharedSpace.ResourcesDirectoryUfo), "TERRAIN"); // TODO: TFTD ....

			_recordsTable = new Hashtable(3);
		}


		public Terrain(string label, string path)
		{
			Label = label;
			PathDirectory = path;

			_recordsTable = new Hashtable(3);
		}
		#endregion


		#region Methods
		public PckSpriteCollection GetSpriteset()
		{
			return GetSpriteset(ResourceInfo.Palette);
		}

		private PckSpriteCollection GetSpriteset(Palette pal)
		{
			return ResourceInfo.LoadSpriteset(PathDirectory, Label, 2, pal);
		}

		public McdRecordCollection GetMcdRecords()
		{
			return GetMcdRecords(ResourceInfo.Palette);
		}

		internal McdRecordCollection GetMcdRecords(Palette pal)
		{
			if (_recordsTable[pal] == null)
			{
				var tiles = XCTileFactory.CreateRecords(Label, PathDirectory, GetSpriteset(pal));
				_recordsTable[pal] = new McdRecordCollection(tiles);
			}
			return _recordsTable[pal] as McdRecordCollection;
		}

		public void ClearMcdTable()
		{
			_recordsTable.Clear();
		}

		public override string ToString()
		{
			return Label;
		}
		#endregion
	}
}
