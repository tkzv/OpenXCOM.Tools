using System;
using System.Collections.Generic;
using System.IO;

using XCom.Resources.Map;


namespace XCom
{
	/// <summary>
	/// Descriptors describe a tileset: a Map, its route-nodes, and terrain. It
	/// also holds the path to its files' parent directory.
	/// A descriptor is accessed *only* through a Group and Category, and is
	/// identified by its tileset-label. This allows multiple tilesets (ie. with
	/// the same label) to be configured differently according to Category and
	/// Group.
	/// </summary>
	public sealed class Descriptor // *snap*
	{
		#region Fields (static)
		public const string PathTerrain = "TERRAIN";
		#endregion


		#region Fields
		private readonly string _dirTerrain;
		#endregion


		#region Properties
		public string Label
		{ get; private set; }

		public string BasePath
		{ get; private set; }

		public List<string> Terrains
		{ get; set; }

		public Palette Pal
		{ get; private set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="tileset"></param>
		/// <param name="terrains"></param>
		/// <param name="basepath"></param>
		/// <param name="palette"></param>
		public Descriptor(
				string tileset,
				List<string> terrains,
				string basepath,
				Palette palette)
		{
			//LogFile.WriteLine("Descriptor cTor tileset= " + tileset);
			//LogFile.WriteLine("");

			Label    = tileset;
			Terrains = terrains;
			BasePath = basepath;
			Pal      = palette;

			_dirTerrain = (Pal == Palette.UfoBattle) ? SharedSpace.ResourceDirectoryUfo
													 : SharedSpace.ResourceDirectoryTftd;
			_dirTerrain = Path.Combine(SharedSpace.Instance.GetShare(_dirTerrain), PathTerrain);

		}
		#endregion


		#region Methods
		/// <summary>
		/// Gets the MCD-records for a given terrain in this Descriptor.
		/// </summary>
		/// <returns></returns>
		public McdRecordCollection GetTerrainRecords(string terrain)
		{
			//LogFile.WriteLine("Descriptor.GetTerrainRecords");

			var tiles = XCTileFactory.CreateTileparts(
													terrain,
													_dirTerrain,
													GetTerrainSpriteset(terrain));
			return new McdRecordCollection(tiles);
		}

		/// <summary>
		/// Gets the spriteset for a given terrain in this Descriptor.
		/// </summary>
		/// <returns></returns>
		public SpriteCollection GetTerrainSpriteset(string terrain)
		{
			//LogFile.WriteLine("Descriptor.GetTerrainSpriteset");

			return ResourceInfo.LoadSpriteset(terrain, _dirTerrain, 2, Pal);	// TODO: Should the '2' be '4' for TFTD ...
		}																		// no, Ufopaedia.org "Image Formats" says TFTD terrains have 2-byte Tab-offsets.
		#endregion


		#region Methods (override)
		/// <summary>
		/// Overrides Object.ToString()
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Label;
		}
		#endregion
	}
}
