using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

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

			var progress = ProgressBarForm.Instance;
			progress.SetInfo("Sorting: " + labelGroup);
			progress.SetTotal(tilesets.Count);
			progress.ResetProgress();
			progress.Show();


			foreach (var tileset in tilesets)
			{
				LogFile.WriteLine(". tileset.Label= " + tileset.Label);

				if (tileset.Group == labelGroup)
				{
					LogFile.WriteLine(". . tileset belongs to Group");
					LogFile.WriteLine(". . tileset.Category= " + tileset.Category);

					if (!Categories.ContainsKey(tileset.Category))
					{
						LogFile.WriteLine(". . . Create new Category");

						Categories[tileset.Category] = new Dictionary<string, Descriptor>();
					}

					if (String.IsNullOrEmpty(tileset.BasePath))
					{
						switch (GroupType)
						{
							case TileGroup.GameType.Ufo:
								tileset.BasePath = SharedSpace.Instance.GetShare(SharedSpace.ResourcesDirectoryUfo);
								break;
							case TileGroup.GameType.Tftd:
								tileset.BasePath = SharedSpace.Instance.GetShare(SharedSpace.ResourcesDirectoryTftd);
								break;
						}
					}

					var descriptor = new Descriptor(
												tileset.Label,
												tileset.Terrains,
												tileset.BasePath,
												Pal);

					Categories[tileset.Category][tileset.Label] = descriptor;
				}
				else LogFile.WriteLine(". . tileset not in this Group - bypass.");

				progress.UpdateProgress();
			}
			progress.Hide();
		}

		internal TileGroupChild(string labelGroup)
			:
				base(labelGroup)
		{}
		#endregion


		#region Methods
//		public override void SaveTileGroup(StreamWriter sw, Varidia vars)
//		{
//			// TODO: possibly save YAML Config here.
//		}

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
