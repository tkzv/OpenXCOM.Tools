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

		public static PckFile GetPckFile(string imageSet, Palette p)
		{
			return _imageInfo.Images[imageSet].GetPckFile(p);
		}

		public static PckFile GetPckFile(string imageSet)
		{
			return GetPckFile(imageSet, _palette);
		}

		public static PckFile CachePck(
									string basePath,
									string basename,
									int bpp,
									Palette p)
		{
			if (_pckHash == null)
				_pckHash = new Dictionary<Palette, Dictionary<string, PckFile>>();

			if (!_pckHash.ContainsKey(p))
				_pckHash.Add(p, new Dictionary<string, PckFile>());

//			if (_pckHash[p][basePath + basename] == null)
			var path = basePath + basename;
			var paletteHash = _pckHash[p];

			if (!paletteHash.ContainsKey(path))
			{
				using (var pckStream = File.OpenRead(path + ".PCK")) // TODO: check if these catch lowercase extensions.
				using (var tabStream = File.OpenRead(path + ".TAB"))
				{
					paletteHash.Add(path, new PckFile(
												pckStream,
												tabStream,
												bpp,
												p));
				}
			}

			return _pckHash[p][basePath + basename];
		}

		public static void ClearPckCache(string basePath, string baseName)
		{
			var path = basePath + baseName;
			foreach (var paleteHash in _pckHash.Values)
				if (paleteHash.ContainsKey(path))
					paleteHash.Remove(path);
		}
	}
}
