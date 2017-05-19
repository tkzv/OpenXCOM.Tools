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
		#region Fields
		private const string Tab = "\t";
		#endregion


		#region cTors
		/// <summary>
		/// cTor. Load from YAML.
		/// </summary>
		internal TileGroupChild(string label, Dictionary<string, Tileset> tilesets)
			:
				base(label, tilesets)
		{
			foreach (string tileset in tilesets.Keys)
			{
				if (!TilesetCategories.ContainsKey(tilesets[tileset].Category))
				{
					var descriptors = new Dictionary<string, DescriptorBase>();
					TilesetCategories[tilesets[tileset].Category] = descriptors;

					var descriptor = new Descriptor(
												tilesets[tileset].Type,
												MapPath,
												RoutePath,
												OccultPath,
												tilesets[tileset].Terrains,
												Palette);

					TilesetDescriptors[tilesets[tileset].Type] =
					descriptors[tilesets[tileset].Type]        = descriptor;
				}
			}
		}


		internal TileGroupChild(string label)
			:
				base(label)
		{}
		internal TileGroupChild(string label, StreamReader sr, Varidia vars)
			:
				base(label, sr, vars)
		{}
		#endregion


		#region Methods
		public override void Save(StreamWriter sw, Varidia vars)
		{
			sw.WriteLine("Tileset:" + Label);
			sw.WriteLine(Tab + "type:1");

			if (vars.Vars[MapPath] != null)
				sw.WriteLine(Tab + "rootpath:" + ((Variable)vars.Vars[MapPath]).Name);
			else
				sw.WriteLine(Tab + "rootpath:" + MapPath);

			if (vars.Vars[RoutePath] != null)
				sw.WriteLine(Tab + "rmpPath:" + ((Variable)vars.Vars[RoutePath]).Name);
			else
				sw.WriteLine(Tab + "rmpPath:" + RoutePath);

			if (vars.Vars[OccultPath] != null)
				sw.WriteLine(Tab + "blankPath:" + ((Variable)vars.Vars[OccultPath]).Name);
			else
				sw.WriteLine(Tab + "blankPath:" + OccultPath);

			sw.WriteLine(Tab + "palette:" + Palette.Label);

			foreach (string keySubsets in TilesetCategories.Keys)
			{
				Dictionary<string, DescriptorBase> valDesc = TilesetCategories[keySubsets];
				if (valDesc != null)
				{
					var deps = new Varidia("Deps");
					foreach (string desc in valDesc.Keys)
					{
						var descriptor = TilesetDescriptors[desc] as Descriptor;
						if (descriptor != null)
						{
							string depList = String.Empty;
							if (descriptor.Terrains.Count != 0)
							{
								int i = 0;
								for (; i != descriptor.Terrains.Count - 1; ++i)
									depList += descriptor.Terrains[i] + " ";
	
								depList += descriptor.Terrains[i];
							}
							deps.AddKeyvalPair(descriptor.Label, depList);
						}
					}

					sw.WriteLine(Tab + "files:" + keySubsets);
	
					foreach (string dep in deps.Variables)
						((Variable)deps.Vars[dep]).Write(sw, Tab + Tab);

					sw.WriteLine(Tab + "end");
				}
			}

			sw.WriteLine("end" + Environment.NewLine);
			sw.Flush();
		}

		public override void AddMap(string tileset, string category)
		{
			var desc = new Descriptor(
									tileset,
									MapPath,
									RoutePath,
									OccultPath,
									new List<string>(),
									Palette);
			TilesetDescriptors[desc.Label]          =
			TilesetCategories[category][desc.Label] = desc;
		}

		public override void AddMap(Descriptor desc, string category)
		{
			TilesetDescriptors[desc.Label]          =
			TilesetCategories[category][desc.Label] = desc;
		}

//		public override Descriptor RemoveTileset(string tileset, string category)
//		{
//			var desc = Categories[category][tileset] as Descriptor;
//			Categories[category].Remove(tileset);
//			return desc;
//		}

		public override void ParseLine(
				string key,
				string value,
				StreamReader sr,
				Varidia vars)
		{
			switch (key.ToUpperInvariant())
			{
				case "FILES":
				{
					var descDictionary = new Dictionary<string, DescriptorBase>();
					TilesetCategories[value] = descDictionary;
					string lineVars = Varidia.ReadLine(sr, vars);
					while (lineVars.ToUpperInvariant() != "END")
					{
						int pos           = lineVars.IndexOf(':');
						string file       = lineVars.Substring(0, pos);
						string[] terrains = lineVars.Substring(pos + 1).Split(' ');

						var terrainList = new List<string>(terrains);

						var desc = new Descriptor(
												file,
												MapPath,
												RoutePath,
												OccultPath,
												terrainList,
												Palette);

						TilesetDescriptors[file] =
						descDictionary[file]     = desc;

						lineVars = Varidia.ReadLine(sr, vars);
					}
					break;
				}

//				case "ORDER":
//					_mapOrder = line.Split(' ');
//					break;

//				case "STARTTILE":
//					_tileStart = int.Parse(line, System.Globalization.CultureInfo.InvariantCulture);
//					break;

//				case "STARTLOC":
//				{
//					string[] locs = line.Split(' ');
//					_startLoc = new MapLocation[locs.Length];
//					for (int i = 0; i < locs.Length; i++)
//					{
//						string[] loc = locs[i].Split(',');
//						int r = int.Parse(loc[0], System.Globalization.CultureInfo.InvariantCulture);
//						int c = int.Parse(loc[1], System.Globalization.CultureInfo.InvariantCulture);
//						int h = int.Parse(loc[2], System.Globalization.CultureInfo.InvariantCulture);
//						_startLoc[i] = new MapLocation(r, c, h);
//					}
//					break;
//				}

//				case "ENDTILE":
//					_tileEnd = int.Parse(line, System.Globalization.CultureInfo.InvariantCulture);
//					break;

//				default:
//					XConsole.AdZerg(string.Format(
//												System.Globalization.CultureInfo.CurrentCulture,
//												"Unknown line in tileset {0}-> {1}:{2}",
//												Name, key, line));
//					break;
			}
		}
		#endregion
	}
}

//		private string[] _mapOrder;
//		private MapLocation[] _startLoc;
//		private int _tileStart = -1;
//		private int _tileEnd   = -1;

/*		public MapLocation[] StartLocations // fix: return a collection or make it a method
		{
			get { return _startLoc; }
		}
		public int StartTile
		{
			get { return _tileStart; }
		}
		public int EndTile
		{
			get { return _tileEnd; }
		}
		public string[] MapOrder // fix: return a collection or make it a method
		{
			get { return _mapOrder; }
		}
		public string[] Order // fix: return a collection or make it a method
		{
			get { return _mapOrder; }
		} */

//		public override IMap GetMap(ShipDescriptor xCom, ShipDescriptor alien)
//		{ return new Type1Map(this, xCom, alien); }
