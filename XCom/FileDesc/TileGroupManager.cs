using System;
using System.Collections.Generic;
using System.IO;

using DSShared;

using XCom.Interfaces;
using XCom.Interfaces.Base;


namespace XCom
{
	public sealed class TileGroupManager
	{
		#region Fields & Properties
//		private readonly string _path;
//		public string Path
//		{
//			get { return _path; }
//		}

		private readonly Dictionary<string, TileGroupBase> _tilegroups = new Dictionary<string, TileGroupBase>();
		public Dictionary<string, TileGroupBase> TileGroups
		{
			get { return _tilegroups; }
		}
		#endregion


		#region cTor
		internal TileGroupManager(TilesetManager tilesetManager)
		{
//			_path = tilesetManager.FullPath; // TODO: not right. not needed.

			foreach (string tilegroup in tilesetManager.Groups)
				TileGroups[tilegroup] = new TileGroupChild(tilegroup, tilesetManager.Tilesets);
		}
		#endregion


		#region Methods
		/// <summary>
		/// Adds a group. Called by XCMainWindow.OnAddGroupClick()
		/// NOTE: Check if the group already exists first.
		/// </summary>
		/// <param name="labelGroup">the label of the group to add</param>
		public void AddTileGroup(string labelGroup)
		{
			TileGroups[labelGroup] = new TileGroupChild(
													labelGroup,
													new List<Tileset>());
		}

		/// <summary>
		/// Deletes a group. Called by XCMainWindow.OnDeleteGroupClick()
		/// </summary>
		/// <param name="labelGroup">the label of the group to delete</param>
		public void DeleteTileGroup(string labelGroup)
		{
			TileGroups.Remove(labelGroup);
		}

		/// <summary>
		/// Creates a new tilegroup and transfers ownership of all Categories
		/// and Descriptors from their previous Group to the specified new
		/// Group. Called by XCMainWindow.OnEditGroupClick()
		/// NOTE: Check if the group and category already exist first.
		/// </summary>
		/// <param name="labelGroup">the new label for the group</param>
		/// <param name="labelGroupPre">the old label of the group</param>
		public void EditTileGroup(string labelGroup, string labelGroupPre)
		{
			TileGroups[labelGroup] = new TileGroupBase(labelGroup);

			foreach (var labelCategory in TileGroups[labelGroupPre].Categories.Keys)
			{
				TileGroups[labelGroup].AddCategory(labelCategory);

				foreach (var descriptor in TileGroups[labelGroupPre].Categories[labelCategory].Values)
				{
					TileGroups[labelGroup].Categories[labelCategory][descriptor.Label] = descriptor;
				}
			}
			DeleteTileGroup(labelGroupPre); // hopefully this won't wipe all Values after transferring ownership.
		}
//					var descriptor = new Descriptor(
//												descriptorPre.Label,	//string tileset,
//												descriptorPre.Terrains,	//List<string> terrains,
//												descriptorPre.BasePath,	//string basepath,
//												descriptorPre.Pal);		//Palette palette);
//
//					TileGroups[labelGroup].Categories[labelCategory].Add(
//																		descriptor.Label,
//																		descriptor);


		/// <summary>
		/// Saves the TileGroups with their children (categories and tilesets)
		/// to a YAML file.
		/// </summary>
		public void SaveTileGroups()
		{
			string dirSettings = SharedSpace.Instance.GetShare(SharedSpace.SettingsDirectory);

			string pfeMapTree = Path.Combine(dirSettings, "MapConfigTest.yml");	// TEST
			using (var fs = new FileStream(pfeMapTree, FileMode.Create))		// TEST
			{}

//			string pfeMapTree    = Path.Combine(dirSettings, PathInfo.ConfigTilesets);		// "MapConfig.yml"
			string pfeMapTreeOld = Path.Combine(dirSettings, PathInfo.ConfigTilesetsOld);	// "MapConfig.old"

			try
			{
				File.Copy(pfeMapTree, pfeMapTreeOld, true); // backup MapConfig.yml -> MapConfig.old


				using (var fs = new FileStream(pfeMapTree, FileMode.Create))
				using (var sw = new StreamWriter(fs))
				{
					sw.WriteLine("# This is MapConfig.yml for MapViewII.");
					sw.WriteLine("#");
					sw.WriteLine("# 'tilesets' - a list that contains all the blocks");
					sw.WriteLine("# 'type'     - the label of MAP/RMP files for the block");
					sw.WriteLine("# 'terrains' - the label(s) of MCD/PCK/TAB files for the block");
					sw.WriteLine("# 'category' - a header for the tileset, is arbitrary here");
					sw.WriteLine("# 'group'    - a header for the categories, is arbitrary except that the first"   + Environment.NewLine
							   + "#              letters designate the game-type and must be either 'ufo' or"       + Environment.NewLine
							   + "#              'tftd' (case insensitive, with or without a following space).");
					sw.WriteLine("# 'basepath' - the path to the parent directory of the tileset's files (default:" + Environment.NewLine
							   + "#              the resource directory(s) that was/were specified when MapView"    + Environment.NewLine
							   + "#              was installed/configured). Note that Maps are expected to be in a" + Environment.NewLine
							   + "#              subdir called MAPS, Routes in a subdir called ROUTES, and"         + Environment.NewLine
							   + "#              terrains - PCK/TAB/MCD files - in a subdir called TERRAIN.");
					sw.WriteLine("");
					sw.WriteLine("tilesets:");

					bool blankline;
					foreach (string labelGroup in TileGroups.Keys)
					{
						blankline = true;
						sw.WriteLine("");
						sw.WriteLine("#---- " + labelGroup + Padder(labelGroup.Length + 6));

						var oGroup = TileGroups[labelGroup] as TileGroupChild;	// <- fuck inheritance btw. It's not been used properly and is
						foreach (var labelCategory in oGroup.Categories.Keys)	// largely irrelevant and needlessly confusing in this codebase.
						{
							if (!blankline)
								sw.WriteLine("");

							blankline = false;
							sw.WriteLine("#---- " + labelCategory + Padder(labelCategory.Length + 6));

							var category = oGroup.Categories[labelCategory];
							foreach (var labelTileset in category.Keys)
							{
								var descriptor = category[labelTileset];

								sw.WriteLine("  - type: " + descriptor.Label); // =labelTileset
								sw.WriteLine("    terrains:");

								foreach (string terrain in descriptor.Terrains)
									sw.WriteLine("      - " + terrain);

								sw.WriteLine("    category: " + labelCategory);
								sw.WriteLine("    group: " + labelGroup);

								string basepath = descriptor.BasePath;
								switch (oGroup.GroupType)
								{
									case GameType.Ufo:
										if (basepath != SharedSpace.Instance.GetShare(SharedSpace.ResourcesDirectoryUfo))
											sw.WriteLine("    basepath: " + basepath);

										break;

									case GameType.Tftd:
										if (basepath != SharedSpace.Instance.GetShare(SharedSpace.ResourcesDirectoryTftd))
											sw.WriteLine("    basepath: " + basepath);

										break;
								}
							}
						}
					}
				}
			}
			catch
			{
				// TODO: show error.
				throw;
			}
		}

		/// <summary>
		/// Adds padding such as " ---#" out to 80 characters.
		/// </summary>
		/// <param name="len"></param>
		/// <returns></returns>
		private string Padder(int len)
		{
			string pad = String.Empty;
			if (len < 79)
				pad = " ";

			for (int i = 78; i > len; --i)
			{
				pad += "-";
			}

			if (len < 79)
				pad += "#";

			return pad;
		}



		// old function ->
		public TileGroupBase AddTileGroup(
				string label,
				string pathMaps,
				string pathRoutes,
				string pathOccults)
		{
			var tilegroup = new TileGroupChild(label);

			tilegroup.MapDirectory    = pathMaps;
			tilegroup.RouteDirectory  = pathRoutes;
			tilegroup.OccultDirectory = pathOccults;

			return (TileGroups[label] = tilegroup);
		}
		#endregion
	}
}
