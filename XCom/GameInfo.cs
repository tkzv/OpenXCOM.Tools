using System;
using System.Collections.Generic;
using System.IO;


namespace XCom
{
//	public delegate void ParseConfigLineEventHandler(KeyvalPair keyval, Varidia vars);


	public static class GameInfo
	{
		private static Palette _palette = Palette.UfoBattle;

		private static ImageInfo _imageInfo;
		private static TilesetDesc _tilesetDesc;

		private static Dictionary<Palette, Dictionary<string, PckSpriteCollection>> _pckDictionary;

//		public static event ParseConfigLineEventHandler ParseConfigLineEvent;


		public static void Initialize(Palette pal, DSShared.PathInfo pathInfo)
		{
			Directory.SetCurrentDirectory(pathInfo.Path);	// change to /settings dir
			XConsole.Init(20);								// note that prints LogFile to settings dir also

			_palette = pal;
			_pckDictionary = new Dictionary<Palette, Dictionary<string, PckSpriteCollection>>();

			using (var sr = new StreamReader(File.OpenRead(pathInfo.FullPath))) // open Paths.Cfg
			{
				var vars = new Varidia(sr);	// this object is going to hold all sorts of keyval pairs
											// be careful you don't duplicate/overwrite a var since the following loop
				KeyvalPair keyval;			// is going to rifle through all the config files and throw it together ...
				//LogFile.WriteLine("[1]GameInfo.Init parse Paths.cfg");
				while ((keyval = vars.ReadLine()) != null) // parse Paths.Cfg; will not return lines that start '$' (or whitespace lines)
				{
					//LogFile.WriteLine(". [1]iter Paths.cfg keyVal= " + keyVal.Keyword);
					switch (keyval.Keyword.ToUpperInvariant())
					{
						case "MAPDATA": // ref to MapEdit.Cfg
							//LogFile.WriteLine(". [1]Paths.cfg MAPDATA keyVal.Value= " + keyVal.Value);
							_tilesetDesc = new TilesetDesc(keyval.Value, vars); // this is spooky, not a delightful way.
							break;

						case "IMAGES": // ref to Images.Cfg
							//LogFile.WriteLine(". [1]Paths.cfg IMAGES keyVal.Value= " + keyVal.Value);
							_imageInfo = new ImageInfo(keyval.Value, vars);
							break;

						case "CURSOR":
						{
							string directorySeparator = String.Empty;
							if (!keyval.Value.EndsWith(@"\", StringComparison.Ordinal))
								directorySeparator = @"\";

							//LogFile.WriteLine("");
							//LogFile.WriteLine("GameInfo.Initialize");
							//LogFile.WriteLine(". key= " + SharedSpace.CursorFile);
							//LogFile.WriteLine(". val= " + keyval.Value + directorySeparator + SharedSpace.Cursor);
							SharedSpace.Instance.SetShare(
													SharedSpace.CursorFile,
													keyval.Value + directorySeparator + SharedSpace.Cursor);
							break;
						}

//						case "CURSOR":
//						case "USEBLANKS":
//							goto default; // ... doh.
//						default:
							//LogFile.WriteLine(". [1]Paths.cfg default");
//							if (ParseConfigLineEvent != null)		// this is just stupid. 'clever' but stupid.
//								ParseConfigLineEvent(keyval, vars);	// TODO: handle any potential errors in OnParseConfigLine() instead of aliasing aliases to delegates.
//							else
//								XConsole.AdZerg("GameInfo: Error in Paths.cfg file: " + keyval); // this is just wrong. It doesn't catch an error in paths.cfg at all ....
//							break;
					}
				}
			}

			Directory.SetCurrentDirectory(SharedSpace.Instance.GetString(SharedSpace.ApplicationDirectory)); // change back to app dir
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
				string path,
				string file,
				int bpp,
				Palette pal)
		{
			if (_pckDictionary == null)
				_pckDictionary = new Dictionary<Palette, Dictionary<string, PckSpriteCollection>>();

			if (!_pckDictionary.ContainsKey(pal))
				_pckDictionary.Add(pal, new Dictionary<string, PckSpriteCollection>());

//			if (_pckHash[pal][basePath + baseName] == null)
			var pf = path + file;

			var spritesetDictionary = _pckDictionary[pal];
			if (!spritesetDictionary.ContainsKey(pf))
			{
				using (var strPck = File.OpenRead(pf + PckSpriteCollection.PckExt))
				using (var strTab = File.OpenRead(pf + PckSpriteCollection.TabExt))
				{
					spritesetDictionary.Add(pf, new PckSpriteCollection(
																	strPck,
																	strTab,
																	bpp,
																	pal));
				}
			}

			return _pckDictionary[pal][pf];
		}

		public static void ClearPckCache(string path, string file)
		{
			var pf = path + file;

			foreach (var spritesetDictionary in _pckDictionary.Values)
				spritesetDictionary.Remove(pf);
		}
	}
}
