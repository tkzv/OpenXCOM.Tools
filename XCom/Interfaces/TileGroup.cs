using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces.Base;


namespace XCom.Interfaces
{
	public class TileGroup
		:
			TileGroupBase
	{
		public enum GameType
		{
			Ufo,
			Tftd
		}

		#region Fields & Properties
		public GameType GroupType // TODO: 'GroupType' can/should be superceded by 'Pal'
		{ get; private set; }

		public Palette Pal
		{ get; set; }

		public string MapDirectory // TODO: fix this out of PathsEditor.
		{ get; set; }

		public string RouteDirectory // TODO: fix this out of PathsEditor.
		{ get; set; }

		public string OccultDirectory
		{ get; set; }
		#endregion


		#region cTors
		/// <summary>
		/// cTor. Load from YAML.
		/// </summary>
		internal TileGroup(string labelGroup)
			:
				base(labelGroup)
		{
			MapDirectory    = Path.Combine(SharedSpace.Instance.GetShare(SharedSpace.ResourcesDirectoryUfo), "MAPS"); // TODO: These are irrelevant.
			RouteDirectory  = Path.Combine(SharedSpace.Instance.GetShare(SharedSpace.ResourcesDirectoryUfo), "ROUTES");
			OccultDirectory = Path.Combine(SharedSpace.Instance.GetShare(SharedSpace.SettingsDirectory), @"OccultTileData\UFO");

			if (labelGroup.StartsWith("tftd", StringComparison.OrdinalIgnoreCase))
			{
				GroupType = GameType.Tftd;
			}
			else //if (labelGroup.StartsWith("ufo", StringComparison.OrdinalIgnoreCase))
			{
				GroupType = GameType.Ufo;	// NOTE: if the prefix "tftd" is not found at the beginning of
			}								// the group-label then default to UFO basepath and palette.

			switch (GroupType)
			{
				case GameType.Ufo:
					Pal = Palette.UfoBattle;
					break;
				case GameType.Tftd:
					Pal = Palette.TftdBattle;
					break;
			}
			// custom Palette = Palette.GetPalette(val)
		}
		#endregion
	}
}
