using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces;
using XCom.Interfaces.Base;


namespace XCom
{
	internal sealed class TileGroupChild
		:
			TileGroup
	{
		#region cTors
		/// <summary>
		/// cTor. Load from YAML.
		/// </summary>
		internal TileGroupChild(string labelGroup, Dictionary<string, Tileset> tilesets)
			:
				base(labelGroup)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("TileGroupChild cTor label= " + labelGroup);

			foreach (string keyTileset in tilesets.Keys)
			{
				LogFile.WriteLine(". keyTileset= " + keyTileset);

				if (!Descriptors.ContainsKey(keyTileset))
//				if (!Categories.ContainsKey(tilesets[key].Category))
				{
					LogFile.WriteLine(". . Descriptor not found");

					var tileset = tilesets[keyTileset];

					if (tileset.Group == labelGroup)
					{
						LogFile.WriteLine(". . . tileset belongs to Group");
						LogFile.WriteLine(". . . tileset.Category= " + tileset.Category);

						if (!Categories.ContainsKey(tileset.Category))
						{
							LogFile.WriteLine(". . . . Create new Category");

							Categories[tileset.Category] = new Dictionary<string, DescriptorBase>();
						}

						LogFile.WriteLine(". . . tileset.Type= " + tileset.Type);

						var descriptor = new Descriptor(
													tileset.Type,
													MapDirectory,
													RouteDirectory,
													OccultDirectory,
													tileset.Terrains,
													Palette);

						Descriptors[tileset.Type]                  =
						Categories[tileset.Category][tileset.Type] = descriptor;
					}
				}
				else LogFile.WriteLine(". . key already found - bypass.");
			}
		}

		internal TileGroupChild(string label)
			:
				base(label)
		{}
		#endregion


		#region Methods
		public override void Save(StreamWriter sw, Varidia vars)
		{
			// TODO: possibly save YAML Config here.
		}

		public override void AddTileset(string tileset, string category)
		{
			var descriptor = new Descriptor(
										tileset,
										MapDirectory,
										RouteDirectory,
										OccultDirectory,
										new List<string>(),
										Palette);
			Descriptors[tileset]          =
			Categories[category][tileset] = descriptor;
		}

		public override void AddTileset(Descriptor descriptor, string category)
		{
			Descriptors[descriptor.Label]          =
			Categories[category][descriptor.Label] = descriptor;
		}

//		public override Descriptor RemoveTileset(string tileset, string category)
//		{
//			var desc = Categories[category][tileset] as Descriptor;
//			Categories[category].Remove(tileset);
//			return desc;
//		}
		#endregion
	}
}
