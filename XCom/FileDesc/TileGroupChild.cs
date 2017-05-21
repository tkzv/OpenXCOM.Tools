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
		internal TileGroupChild(string labelGroup, List<Tileset> tilesets)
			:
				base(labelGroup)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("TileGroupChild cTor label= " + labelGroup);

			foreach (var tileset in tilesets)
			{
				LogFile.WriteLine(". tileset.Type= " + tileset.Type);

				if (tileset.Group == labelGroup)
				{
					LogFile.WriteLine(". . tileset belongs to Group");
					LogFile.WriteLine(". . tileset.Category= " + tileset.Category);

					if (!Categories.ContainsKey(tileset.Category))
					{
						LogFile.WriteLine(". . . Create new Category");

						Categories[tileset.Category] = new Dictionary<string, Descriptor>();
					}

					bool isUfo = true;
					if      (labelGroup.StartsWith("ufo", StringComparison.OrdinalIgnoreCase))
					{
						isUfo = true;
					}
					else if (labelGroup.StartsWith("tftd", StringComparison.OrdinalIgnoreCase))
					{
						isUfo = false;
					}

					var pal = Palette.UfoBattle;
					if (isUfo)
					{
						pal = Palette.UfoBattle;
					}
					else
					{
						pal = Palette.TftdBattle;
					}

					if (String.IsNullOrEmpty(tileset.BasePath))
					{
						if (isUfo)
							tileset.BasePath = SharedSpace.Instance.GetShare(SharedSpace.ResourcesDirectoryUfo);
						else
							tileset.BasePath = SharedSpace.Instance.GetShare(SharedSpace.ResourcesDirectoryTftd);
					}

					var descriptor = new Descriptor(
												tileset.Type,
												tileset.Terrains,
												tileset.BasePath,
												pal);

					Categories[tileset.Category][tileset.Type] = descriptor;
				}
				else LogFile.WriteLine(". . tileset not in this Group - bypass.");
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
			string basepath = String.Empty;	// TODO: fix this in PathsEditor.
			var pal = Palette.UfoBattle;	// TODO: fix this in PathsEditor.

			var descriptor = new Descriptor(
										tileset,
										new List<string>(),
										basepath,
										pal);
			Categories[category][tileset] = descriptor;
		}

		public override void AddTileset(Descriptor descriptor, string category)
		{
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
