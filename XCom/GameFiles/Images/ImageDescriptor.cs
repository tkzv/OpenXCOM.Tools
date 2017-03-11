using System;
using System.Collections;
using XCom.GameFiles.Map;


namespace XCom
{
	/// <summary>
	/// Describes information about imagesets: the path to the PCK, TAB, and MCD files.
	/// </summary>
	public class ImageDescriptor
		:
		IComparable
	{
		private readonly Hashtable _mcdTable;


		public ImageDescriptor(string baseName, string basePath)
		{
			BaseName = baseName;
			BasePath = basePath;

			_mcdTable = new Hashtable(3);
		}


		public PckFile GetPckFile(Palette pal)
		{
			return GameInfo.CachePckFile(BasePath, BaseName, 2, pal);
		}

		public PckFile GetPckFile()
		{
			return GetPckFile(GameInfo.DefaultPalette);
		}

		public McdFile GetMcdFile(Palette palette, XcTileFactory _xcTileFactory)
		{
			if (_mcdTable[palette] == null)
			{
				var tiles = _xcTileFactory.CreateTiles(BaseName, BasePath, GetPckFile(palette));
				_mcdTable[palette] = new McdFile(tiles);
			}
			return (McdFile)_mcdTable[palette];
		}

		public McdFile GetMcdFile()
		{
			return GetMcdFile(GameInfo.DefaultPalette, new XcTileFactory());
		}

		public override string ToString()
		{
			return BaseName;
		}

		public int CompareTo(object other)
		{
			return String.CompareOrdinal(BaseName, other.ToString());
		}

		public string BaseName
		{ get; private set; }

		public string BasePath
		{ get; set; }

		public void ClearMcdTable()
		{
			_mcdTable.Clear();
		}
	}
}
