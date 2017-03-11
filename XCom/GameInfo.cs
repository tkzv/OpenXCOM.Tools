using System;
using System.Collections.Generic;
using System.IO;


namespace XCom
{
	public delegate void ParseLineDelegate(KeyVal keyVal, VarCollection vars);


	public static class GameInfo
	{
		private static Palette _palette = Palette.UFOBattle;
//		private static Palette _palette = Palette.TFTDBattle;

		private static ImageInfo _imageInfo;
		private static TilesetDesc _tilesetDesc;
//		private static IWarningHandler WarningHandler;

		private static Dictionary<Palette, Dictionary<string, PckFile>> _pckHash;

		public static event ParseLineDelegate ParseLine;

		public static void Init(Palette palette, DSShared.PathInfo paths)
		{
			_palette = palette;
			_pckHash = new Dictionary<Palette, Dictionary<string, PckFile>>();

			var vars = new VarCollection(new StreamReader(File.OpenRead(paths.ToString())));

			Directory.SetCurrentDirectory(paths.Path);

			xConsole.Init(20);

			KeyVal keyVal = null;
			while ((keyVal = vars.ReadLine()) != null)
			{
				switch (keyVal.Keyword)
				{
					case "mapdata": // MapEdit.dat ref in Paths.pth
						_tilesetDesc = new TilesetDesc(keyVal.Rest, vars);
						break;

					case "images": // Images.dat ref in Paths.pth
						_imageInfo = new ImageInfo(keyVal.Rest, vars);
						break;

					default:
						if (ParseLine != null)
							ParseLine(keyVal, vars);
						else
							xConsole.AddLine("GameInfo: Error in Paths.pth file: " + keyVal);
						break;
				}
			}

			vars.BaseStream.Close();
		}

		public static ImageInfo ImageInfo
		{
			get { return _imageInfo; }
		}

		public static TilesetDesc TilesetInfo
		{
			get { return _tilesetDesc; }
		}

		public static Palette DefaultPalette
		{
			get { return _palette; }
			set { _palette = value; }
		}

		public static PckFile GetPckFile(string imageSet)
		{
			return _imageInfo.Images[imageSet].GetPckFile(_palette);
		}

		public static PckFile CachePckFile(
				string basePath,
				string baseName,
				int bpp,
				Palette pal)
		{
			if (_pckHash == null)
				_pckHash = new Dictionary<Palette, Dictionary<string, PckFile>>();

			if (!_pckHash.ContainsKey(pal))
				_pckHash.Add(pal, new Dictionary<string, PckFile>());

//			if (_pckHash[pal][basePath + baseName] == null)
			var path = basePath + baseName;

			var palHash = _pckHash[pal];
			if (!palHash.ContainsKey(path))
			{
				using (var pckStream = File.OpenRead(path + ".PCK")) // TODO: check if these catch lowercase extensions.
				using (var tabStream = File.OpenRead(path + ".TAB"))
				{
					palHash.Add(path, new PckFile(
												pckStream,
												tabStream,
												bpp,
												pal));
				}
			}

			return _pckHash[pal][path];
		}

		public static void ClearPckCache(string basePath, string baseName)
		{
			var path = basePath + baseName;

			foreach (var palHash in _pckHash.Values)
//				if (palHash.ContainsKey(path)) // kL_note: should not be needed here.
				palHash.Remove(path);
		}
	}
}
