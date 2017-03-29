using System;
using System.Collections.Generic;
using System.IO;


namespace XCom
{
	public delegate void ParseLineEventHandler(KeyvalPair keyval, Varidia vars);


	public static class GameInfo
	{
		private static Palette _palette = Palette.UfoBattle;

		private static ImageInfo _imageInfo;
		private static TilesetDesc _tilesetDesc;

		private static Dictionary<Palette, Dictionary<string, PckSpriteCollection>> _pckDict;

		public static event ParseLineEventHandler ParseLine;


		public static void Init(Palette pal, DSShared.PathInfo info)
		{
			Directory.SetCurrentDirectory(info.Path);	// change to /settings dir
			XConsole.Init(20);							// note that prints LogFile to settings dir also

			_palette = pal;
			_pckDict = new Dictionary<Palette, Dictionary<string, PckSpriteCollection>>();

			using (var sr = new StreamReader(File.OpenRead(info.FullPath))) // open Paths.Cfg
			{
				var vars = new Varidia(sr);	// this object is going to hold all sorts of keyval pairs
											// be careful you don't duplicate/overwrite a var since the following loop
				KeyvalPair keyVal;			// is going to rifle through all the config files and throw it together ...
				//LogFile.WriteLine("[1]GameInfo.Init parse Paths.cfg");
				while ((keyVal = vars.ReadLine()) != null) // parse Paths.Cfg; will not return lines that start '$' (or whitespace lines)
				{
					//LogFile.WriteLine(". [1]iter Paths.cfg keyVal= " + keyVal.Keyword);
					switch (keyVal.Keyword.ToUpperInvariant())
					{
						case "MAPDATA": // ref to MapEdit.Cfg
							//LogFile.WriteLine(". [1]Paths.cfg MAPDATA keyVal.Value= " + keyVal.Value);
							_tilesetDesc = new TilesetDesc(keyVal.Value, vars); // this is spooky, not a delightful way.
							break;

						case "IMAGES": // ref to Images.Cfg
							//LogFile.WriteLine(". [1]Paths.cfg IMAGES keyVal.Value= " + keyVal.Value);
							_imageInfo = new ImageInfo(keyVal.Value, vars);
							break;

						case "USEBLANKS":
						case "CURSOR":
							goto default; // ... doh.
						default:
							//LogFile.WriteLine(". [1]Paths.cfg default");
							if (ParseLine != null)			// this is just stupid. 'clever' but stupid.
								ParseLine(keyVal, vars);	// TODO: handle any potential errors in ParseLine() instead of aliasing aliases to delegates.
							else
								XConsole.AdZerg("GameInfo: Error in Paths.cfg file: " + keyVal); // this is just wrong. It doesn't catch an error in paths.cfg at all ....
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

		internal static Palette DefaultPalette
		{
			get { return _palette; }
		}

		internal static PckSpriteCollection GetPckPack(string imageSet)
		{
			return _imageInfo.Images[imageSet].GetPckPack(_palette);
		}

		public static PckSpriteCollection CachePckPack(
				string basePath,
				string baseName,
				int bpp,
				Palette pal)
		{
			if (_pckDict == null)
				_pckDict = new Dictionary<Palette, Dictionary<string, PckSpriteCollection>>();

			if (!_pckDict.ContainsKey(pal))
				_pckDict.Add(pal, new Dictionary<string, PckSpriteCollection>());

//			if (_pckHash[pal][basePath + baseName] == null)
			var pathfile = basePath + baseName;

			var palHash = _pckDict[pal];
			if (!palHash.ContainsKey(pathfile))
			{
				using (var pckStream = File.OpenRead(pathfile + ".PCK")) // TODO: check if these catch lowercase extensions.
				using (var tabStream = File.OpenRead(pathfile + ".TAB")) // they should, it's for Windows.
				{
					palHash.Add(pathfile, new PckSpriteCollection(
															pckStream,
															tabStream,
															bpp,
															pal));
				}
			}

			return _pckDict[pal][pathfile];
		}

		public static void ClearPckCache(string basePath, string baseName)
		{
			var path = basePath + baseName;

			foreach (var palHash in _pckDict.Values)
				palHash.Remove(path);
		}
	}
}
