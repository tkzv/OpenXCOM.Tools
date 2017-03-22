using System;
using System.Collections.Generic;
using System.IO;


namespace XCom
{
	public delegate void ParseLineDelegate(KeyVal keyVal, VarCollection vars);


	public static class GameInfo
	{
		private static Palette _palette = Palette.UFOBattle;

		private static ImageInfo _imageInfo;
		private static TilesetDesc _tilesetDesc;

		private static Dictionary<Palette, Dictionary<string, PckFile>> _pckHash;

		public static event ParseLineDelegate ParseLine;


		public static void Init(Palette pal, DSShared.PathInfo info)
		{
			Directory.SetCurrentDirectory(info.Path); // change to /settings dir
			xConsole.Init(20);

			_palette = pal;
			_pckHash = new Dictionary<Palette, Dictionary<string, PckFile>>();

			using (var sr = new StreamReader(File.OpenRead(info.FullPath))) // open Paths.Cfg
			{
				var vars = new VarCollection(sr);	// this object is going to hold all sorts of keyval pairs
													// be careful you don't duplicate/overwrite a var since the following loop
				KeyVal keyVal;						// is going to rifle through all the config files and throw it together ...
				LogFile.WriteLine("[1]GameInfo.Init parse Paths.cfg");
				while ((keyVal = vars.ReadLine()) != null) // parse Paths.Cfg; will not return lines that start '$' (or whitespace lines)
				{
					LogFile.WriteLine(". [1]iter Paths.cfg keyVal= " + keyVal.Keyword);
					switch (keyVal.Keyword.ToUpperInvariant())
					{
						case "MAPDATA": // ref to MapEdit.Cfg
							LogFile.WriteLine(". [1]Paths.cfg MAPDATA keyVal.Value= " + keyVal.Value);
							_tilesetDesc = new TilesetDesc(keyVal.Value, vars); // this is spooky, not a delightful way.
							break;

						case "IMAGES": // ref to Images.Cfg
							LogFile.WriteLine(". [1]Paths.cfg IMAGES keyVal.Value= " + keyVal.Value);
							_imageInfo = new ImageInfo(keyVal.Value, vars);
							break;

						case "USEBLANKS":
						case "CURSOR":
							goto default; // ... doh.
						default:
							LogFile.WriteLine(". [1]Paths.cfg default");
							if (ParseLine != null)			// this is just stupid. 'clever' but stupid.
								ParseLine(keyVal, vars);	// TODO: handle any potential errors in ParseLine() instead of aliasing aliases to delegates.
							else
								xConsole.AddLine("GameInfo: Error in Paths.cfg file: " + keyVal); // this is just wrong. It doesn't catch an error in paths.cfg at all ....
							break;
					}
				}
			}

			Directory.SetCurrentDirectory(SharedSpace.Instance.GetString(SharedSpace.AppDir)); // change back to app dir
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
			var pathfile = basePath + baseName;

			var palHash = _pckHash[pal];
			if (!palHash.ContainsKey(pathfile))
			{
				using (var pckStream = File.OpenRead(pathfile + ".PCK")) // TODO: check if these catch lowercase extensions.
				using (var tabStream = File.OpenRead(pathfile + ".TAB"))
				{
					palHash.Add(pathfile, new PckFile(
													pckStream,
													tabStream,
													bpp,
													pal));
				}
			}

			return _pckHash[pal][pathfile];
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
