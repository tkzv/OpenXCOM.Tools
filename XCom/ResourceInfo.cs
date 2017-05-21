using System;
using System.Collections.Generic;
using System.IO;


namespace XCom
{
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
		internal static Palette Pal
		{
			get { return _palette; }
		}

		private static Dictionary<Palette, Dictionary<string, PckSpriteCollection>> _spritesDictionary;


		public static void InitializeResources(Palette pal, DSShared.PathInfo pathConfig)
		{
			Directory.SetCurrentDirectory(pathConfig.Path);	// change to /settings dir
			XConsole.Init(20);								// note that prints the LogFile to settings dir also

			_palette = pal;
			_spritesDictionary = new Dictionary<Palette, Dictionary<string, PckSpriteCollection>>();

			var tilesetManager = new TilesetManager(pathConfig.FullPath);

			_infoTilegroup = new GroupHerder(tilesetManager);
			_infoTerrain   = new TerrainHerder(tilesetManager);

			Directory.SetCurrentDirectory(SharedSpace.Instance.GetShare(SharedSpace.ApplicationDirectory)); // change back to app dir
		}

//		internal static PckSpriteCollection GetSpriteset(string spriteset)
//		{
//			return _terrainInfo.Terrains[spriteset].GetImageset(_palette);
//		}

		public static PckSpriteCollection LoadSpriteset(
				string path,
				string file,
				int lenTabOffset,
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
																		lenTabOffset,
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
