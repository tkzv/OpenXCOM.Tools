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
				base(label)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("TileGroupChild cTor");

			foreach (string key in tilesets.Keys)
			{
				LogFile.WriteLine(". key= " + key);

				if (!Descriptors.ContainsKey(key))
//				if (!TilesetCategories.ContainsKey(tilesets[key].Category))
				{
					LogFile.WriteLine(". . key not found - ADD");

					var tileset = tilesets[key];

					LogFile.WriteLine(". . tileset.Category= " + tileset.Category);
					if (!Categories.ContainsKey(tileset.Category))
						Categories[tileset.Category] = new Dictionary<string, DescriptorBase>();

					LogFile.WriteLine(". . tileset.Type= " + tileset.Type);

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
				else LogFile.WriteLine(". . key already found - bypass.");
			}
		}


		internal TileGroupChild(string label)
			:
				base(label)
		{}

//		internal TileGroupChild(string label, StreamReader sr, Varidia vars)
//			:
//				base(label, sr, vars)
//		{} 
		#endregion


		#region Methods
		public override void Save(StreamWriter sw, Varidia vars)
		{
//			sw.WriteLine("Tileset:" + Label);
//			sw.WriteLine(Tab + "type:1");
//
//			if (vars.Vars[MapDirectory] != null)
//				sw.WriteLine(Tab + "rootpath:" + ((Variable)vars.Vars[MapDirectory]).Name);
//			else
//				sw.WriteLine(Tab + "rootpath:" + MapDirectory);
//
//			if (vars.Vars[RouteDirectory] != null)
//				sw.WriteLine(Tab + "rmpPath:" + ((Variable)vars.Vars[RouteDirectory]).Name);
//			else
//				sw.WriteLine(Tab + "rmpPath:" + RouteDirectory);
//
//			if (vars.Vars[OccultDirectory] != null)
//				sw.WriteLine(Tab + "blankPath:" + ((Variable)vars.Vars[OccultDirectory]).Name);
//			else
//				sw.WriteLine(Tab + "blankPath:" + OccultDirectory);
//
//			sw.WriteLine(Tab + "palette:" + Palette.Label);
//
//			foreach (string categories in TilesetCategories.Keys)
//			{
//				Dictionary<string, DescriptorBase> descriptors = TilesetCategories[categories];
//				if (descriptors != null)
//				{
//					var terrains = new Varidia("Deps");
//					foreach (string key in descriptors.Keys)
//					{
//						var descriptor = TilesetDescriptors[key] as Descriptor;
//						if (descriptor != null)
//						{
//							string terrainList = String.Empty;
//							if (descriptor.Terrains.Count != 0)
//							{
//								int i = 0;
//								for (; i != descriptor.Terrains.Count - 1; ++i)
//									terrainList += descriptor.Terrains[i] + " ";
//	
//								terrainList += descriptor.Terrains[i];
//							}
//							terrains.AddKeyvalPair(descriptor.Label, terrainList);
//						}
//					}
//
//					sw.WriteLine(Tab + "files:" + categories);
//	
//					foreach (string terrain in terrains.Variables)
//						((Variable)terrains.Vars[terrain]).Write(sw, Tab + Tab);
//
//					sw.WriteLine(Tab + "end");
//				}
//			}
//
//			sw.WriteLine("end" + Environment.NewLine);
//			sw.Flush();
		}

		public override void AddMap(string tileset, string category)
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

		public override void AddMap(Descriptor descriptor, string category)
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

/*		public override void ParseLine(
				string key,
				string category,
				StreamReader sr,
				Varidia vars)
		{
			switch (key.ToUpperInvariant())
			{
				case "FILES":
				{
					var descriptors = new Dictionary<string, DescriptorBase>();
					TilesetCategories[category] = new Dictionary<string, DescriptorBase>();

					string lineVars = Varidia.ReadLine(sr, vars);
					while (lineVars.ToUpperInvariant() != "END")
					{
						int pos           = lineVars.IndexOf(':');
						string tileset    = lineVars.Substring(0, pos);
						string[] terrains = lineVars.Substring(pos + 1).Split(' ');

						var terrainList = new List<string>(terrains);

						var descriptor = new Descriptor(
													tileset,
													MapDirectory,
													RouteDirectory,
													OccultDirectory,
													terrainList,
													Palette);

						TilesetDescriptors[tileset]          =
						TilesetCategories[category][tileset] = descriptor;

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
		} */

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
