using System;
using System.Collections.Generic;
using System.IO;


namespace XCom
{
//	public delegate void ParseConfigLineEventHandler(KeyvalPair keyval, Varidia vars);


	public static class ResourceInfo
	{
		private static GroupHerder _infoTilegroup;
		public static GroupHerder TileGroupInfo
		{
			get { return _infoTilegroup; }
		}

		private static TerrainHerder _infoTerrain;
		public static TerrainHerder TerrainInfo
		{
			get { return _infoTerrain; }
		}

		private static Palette _palette = Palette.UfoBattle;
		internal static Palette Palette
		{
			get { return _palette; }
		}

		private static Dictionary<Palette, Dictionary<string, PckSpriteCollection>> _spritesDictionary;

//		public static event ParseConfigLineEventHandler ParseConfigLineEvent;


		public static void InitializeResources(Palette pal, DSShared.PathInfo pathConfig)
		{
			Directory.SetCurrentDirectory(pathConfig.Path);	// change to /settings dir
			XConsole.Init(20);								// note that prints the LogFile to settings dir also

			_palette = pal;
			_spritesDictionary = new Dictionary<Palette, Dictionary<string, PckSpriteCollection>>();

			var tilesetManager = new TilesetManager(pathConfig.FullPath);

			_infoTilegroup = new GroupHerder(tilesetManager);
			_infoTerrain   = new TerrainHerder(tilesetManager);



/*			using (var sr = new StreamReader(File.OpenRead(pathConfig.FullPath))) // open/read Paths.Cfg
			{
				var vars = new Varidia(sr);	// this object is going to hold all sorts of keyval pairs
											// be careful you don't duplicate/overwrite a var since the following loop
											// is going to rifle through all the config files and throw it together ...
				KeyvalPair keyval;
				while ((keyval = vars.ReadLine()) != null) // parse Paths.Cfg; will skip lines that start '$' or are whitespace
				{
					switch (keyval.Keyword.ToUpperInvariant())
					{
						case "MAPDATA": // get path to MapEdit.Cfg
							_tilegroupInfo = new TileGroupDesc(keyval.Value, vars); // this is spooky, not a delightful way.
							break;

						case "IMAGES": // get path to Images.Cfg
							_terrainInfo = new TerrainDesc(keyval.Value, vars);
							break;

//						case "CURSOR": // done in XCMainWindow.
//						{
//							string directorySeparator = String.Empty;
//							if (!keyval.Value.EndsWith(@"\", StringComparison.Ordinal))
//								directorySeparator = @"\";
//
//							SharedSpace.Instance.SetShare(
//													"cursorFile", //SharedSpace.CursorFile,
//													keyval.Value + directorySeparator + SharedSpace.Cursor);
//							break;
//						}

//						case "CURSOR":
//						case "USEBLANKS":
//							goto default; // ... doh.
//						default:
//							if (ParseConfigLineEvent != null)		// this is just stupid. 'clever' but stupid.
//								ParseConfigLineEvent(keyval, vars);	// TODO: handle any potential errors in OnParseConfigLine() instead of aliasing aliases to delegates.
//							else
//								XConsole.AdZerg("GameInfo: Error in Paths.cfg file: " + keyval); // this is just wrong. It doesn't catch an error in paths.cfg at all ....
//							break;
					}
				}
			} */

			Directory.SetCurrentDirectory(SharedSpace.Instance.GetShare(SharedSpace.ApplicationDirectory)); // change back to app dir
		}

//		internal static PckSpriteCollection GetSpriteset(string spriteset)
//		{
//			return _terrainInfo.Terrains[spriteset].GetImageset(_palette);
//		}

		public static PckSpriteCollection LoadSpriteset(
				string path,
				string file,
				int bpp,
				Palette pal)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("ResourceInfo.PckSpriteCollection");

			if (!String.IsNullOrEmpty(path))
			{
				LogFile.WriteLine(". path= " + path);
				LogFile.WriteLine(". file= " + file);

				if (_spritesDictionary == null)
					_spritesDictionary = new Dictionary<Palette, Dictionary<string, PckSpriteCollection>>();

				if (!_spritesDictionary.ContainsKey(pal))
					_spritesDictionary.Add(pal, new Dictionary<string, PckSpriteCollection>());

//				if (_pckHash[pal][path + file] == null)
				var pf = Path.Combine(path, file);
				LogFile.WriteLine(". pf= " + pf);

				var spritesetDictionary = _spritesDictionary[pal];
				if (!spritesetDictionary.ContainsKey(pf))
				{
					LogFile.WriteLine(". . pf not found in spriteset dictionary -> add new PckSpriteCollection");

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

				return _spritesDictionary[pal][pf];
			}
			return null;
		}

		public static void ClearSpriteset(string path, string file)
		{
			var pf = Path.Combine(path, file);

			foreach (var spritesetDictionary in _spritesDictionary.Values)
				spritesetDictionary.Remove(pf);
		}
	}
}
