using System;
using System.Collections.Generic;
using System.IO;

using DSShared;


namespace XCom
{
	public static class ResourceInfo
	{
		public static TileGroupManager TileGroupInfo
		{ get; private set; }

		public static TerrainManager TerrainInfo
		{ get; private set; }

		private static readonly Dictionary<Palette, Dictionary<string, SpriteCollection>> _spritesDictionary
						  = new Dictionary<Palette, Dictionary<string, SpriteCollection>>();


		/// <summary>
		/// Initializes/ loads info about XCOM resources.
		/// </summary>
		/// <param name="pathConfig"></param>
		public static void InitializeResources(PathInfo pathConfig)
		{
			Directory.SetCurrentDirectory(pathConfig.Path); // change to /settings dir // TODO: screw settings dir.
//			XConsole.Init(20);

			var tilesetManager = new TilesetManager(pathConfig.FullPath);

			TileGroupInfo = new TileGroupManager(tilesetManager);
			TerrainInfo   = new TerrainManager(tilesetManager);

			Directory.SetCurrentDirectory(SharedSpace.Instance.GetShare(SharedSpace.ApplicationDirectory)); // change back to app dir
		}

		/// <summary>
		/// Loads a given spriteset for UFO or TFTD.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="path"></param>
		/// <param name="lenTabOffset"></param>
		/// <param name="pal"></param>
		/// <returns></returns>
		public static SpriteCollection LoadSpriteset(
				string file,
				string path,
				int lenTabOffset,
				Palette pal)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("ResourceInfo.SpriteCollection");

			if (!String.IsNullOrEmpty(path))
			{
				LogFile.WriteLine(". path= " + path);
				LogFile.WriteLine(". file= " + file);

				if (!_spritesDictionary.ContainsKey(pal))
					_spritesDictionary.Add(pal, new Dictionary<string, SpriteCollection>());

				var pf = Path.Combine(path, file);
				LogFile.WriteLine(". pf= " + pf);

				var spritesetDictionary = _spritesDictionary[pal];
				if (!spritesetDictionary.ContainsKey(pf))
				{
					LogFile.WriteLine(". . pf not found in spriteset dictionary -> add new SpriteCollection");

					using (var strPck = File.OpenRead(pf + SpriteCollection.PckExt))
					using (var strTab = File.OpenRead(pf + SpriteCollection.TabExt))
					{
						spritesetDictionary.Add(pf, new SpriteCollection(
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

		/// <summary>
		/// Clears a given spriteset from the dictionary.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="file"></param>
		public static void ClearSpriteset(string path, string file)
		{
			var pf = Path.Combine(path, file);

			foreach (var spritesetDictionary in _spritesDictionary.Values)
				spritesetDictionary.Remove(pf);
		}

//		internal static SpriteCollection GetSpriteset(string spriteset)
//		{
//			return _terrainInfo.Terrains[spriteset].GetImageset(_palette);
//		}
	}
}
