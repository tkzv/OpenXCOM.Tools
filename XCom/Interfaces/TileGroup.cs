using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces.Base;


namespace XCom.Interfaces
{
	public class TileGroup // TODO: cTor has inheritors and calls a virtual function.
		:
			TileGroupBase
	{
		#region Fields & Properties
		public Palette Palette
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
		internal TileGroup(string label)
			:
				base(label)
		{
			MapDirectory    = Path.Combine(SharedSpace.Instance.GetShare(SharedSpace.ResourcesDirectoryUfo), "MAPS"); // TODO: These are irrelevant.
			RouteDirectory  = Path.Combine(SharedSpace.Instance.GetShare(SharedSpace.ResourcesDirectoryUfo), "ROUTES");
			OccultDirectory = Path.Combine(SharedSpace.Instance.GetShare(SharedSpace.SettingsDirectory), @"OccultTileData\UFO");

			Palette = ResourceInfo.Pal;
//			Palette = Palette.UfoBattle;
			// TODO: TFTD Palette = Palette.TftdBattle
			// custom Palette = Palette.GetPalette(val)
		}
		#endregion


		#region Methods (virtual)
		public virtual void Save(StreamWriter sw, Varidia vars)
		{}

		public virtual void ParseLine(
				string key,
				string category,
				StreamReader sr,
				Varidia vars)
		{}

		public virtual void AddTileset(string tileset, string category)
		{}

		public virtual void AddTileset(Descriptor descriptor, string category)
		{}

//		public virtual Descriptor RemoveTileset(string tileset, string category)
//		{}
		#endregion
	}
}
