using System;
using System.Collections;

using XCom.GameFiles.Map;


namespace XCom
{
	/// <summary>
	/// Describes information about imagesets: the path to the PCK, TAB, and MCD files.
	/// </summary>
	public sealed class ImageDescriptor
		:
			IComparable // TODO: should override Equals, ==, !=, <, >
	{
		private readonly Hashtable _mcdHash;

		public string Label
		{ get; private set; }

		public string Path
		{ get; set; }


		public ImageDescriptor(string label, string path)
		{
			Label = label;
			Path  = path;

			_mcdHash = new Hashtable(3);
		}


		internal PckSpriteCollection GetPckPack(Palette pal)
		{
			return ResourceInfo.CachePckPack(Path, Label, 2, pal);
		}

		public PckSpriteCollection GetPckPack()
		{
			return GetPckPack(ResourceInfo.DefaultPalette);
		}

		internal McdRecordCollection GetRecordsByPalette(Palette pal, XCTileFactory tileFactory)
		{
			if (_mcdHash[pal] == null)
			{
				var tiles = tileFactory.CreateTiles(Label, Path, GetPckPack(pal));
				_mcdHash[pal] = new McdRecordCollection(tiles);
			}
			return (McdRecordCollection)_mcdHash[pal];
		}

		public McdRecordCollection GetRecords()
		{
			return GetRecordsByPalette(ResourceInfo.DefaultPalette, new XCTileFactory());
		}

		public override string ToString()
		{
			return Label;
		}

		public int CompareTo(object obj)
		{
			return String.CompareOrdinal(Label, obj.ToString());
		}

		public void ClearMcdTable()
		{
			_mcdHash.Clear();
		}
	}
}
