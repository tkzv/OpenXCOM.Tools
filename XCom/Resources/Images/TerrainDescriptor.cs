using System;
using System.Collections;

using XCom.Resources.Map;


namespace XCom
{
	/// <summary>
	/// Describes information about terrains: the path to the PCK, TAB, and MCD
	/// files.
	/// </summary>
	public sealed class TerrainDescriptor
//		: IComparable // TODO: should override Equals, ==, !=, <, > // so long, IComparable.
	{
		#region Fields
		private readonly Hashtable _recordsTable;
		#endregion


		#region Properties
		public string Label
		{ get; private set; }

		public string Path
		{ get; set; }
		#endregion


		#region cTor
		public TerrainDescriptor(string label, string path)
		{
			Label = label;
			Path  = path;

			_recordsTable = new Hashtable(3);
		}
		#endregion


		#region Methods
		public PckSpriteCollection GetImageset()
		{
			return GetImageset(ResourceInfo.Palette);
		}

		internal PckSpriteCollection GetImageset(Palette pal)
		{
			return ResourceInfo.LoadSpriteset(Path, Label, 2, pal);
		}

		public McdRecordCollection GetMcdRecords()
		{
			return GetMcdRecords(ResourceInfo.Palette, new XCTileFactory());
		}

		internal McdRecordCollection GetMcdRecords(Palette pal, XCTileFactory tileFactory)
		{
			if (_recordsTable[pal] == null)
			{
				var tiles = tileFactory.CreateTiles(Label, Path, GetImageset(pal));
				_recordsTable[pal] = new McdRecordCollection(tiles);
			}
			return _recordsTable[pal] as McdRecordCollection;
		}

		public void ClearMcdTable()
		{
			_recordsTable.Clear();
		}

//		public override string ToString()
//		{
//			return Label;
//		}
//		public int CompareTo(object obj)
//		{
//			return String.CompareOrdinal(Label, obj.ToString());
//		}
		#endregion
	}
}
