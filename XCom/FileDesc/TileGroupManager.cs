using System;
using System.Collections.Generic;

using XCom.Interfaces.Base;


namespace XCom
{
	public sealed class TileGroupManager
	{
		#region Fields & Properties
		private readonly string _path;
		public string Path
		{
			get { return _path; }
		}

		private readonly Dictionary<string, TileGroupBase> _tilegroups = new Dictionary<string, TileGroupBase>();
		public Dictionary<string, TileGroupBase> TileGroups
		{
			get { return _tilegroups; }
		}
		#endregion


		#region cTor
		internal TileGroupManager(TilesetManager tilesetManager)
		{
			_path = tilesetManager.FullPath; // TODO: not right. not needed.

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
			TileGroups[labelGroup] = new TileGroupBase(labelGroup);
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
