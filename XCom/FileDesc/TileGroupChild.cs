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
//		private string[] _mapOrder;
//		private MapLocation[] _startLoc;
//		private int _tileStart = -1;
//		private int _tileEnd   = -1;

		private const string Tab = "\t";


		internal TileGroupChild(string name)
			:
				base(name)
		{}
		internal TileGroupChild(string name, StreamReader sr, Varidia vars)
			:
				base(name, sr, vars)
		{}


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

			foreach (string keySubsets in Categories.Keys)
			{
				Dictionary<string, MapDescBase> valDesc = Categories[keySubsets];
				if (valDesc != null)
				{
					var deps = new Varidia("Deps");
					foreach (string keyDesc in valDesc.Keys)
					{
						var desc = MapDescDictionary[keyDesc] as MapDescChild;
						if (desc != null)
						{
							string depList = String.Empty;
							if (desc.Terrains.Length != 0)
							{
								int i = 0;
								for (; i != desc.Terrains.Length - 1; ++i)
									depList += desc.Terrains[i] + " ";
	
								depList += desc.Terrains[i];
							}
							deps.AddKeyvalPair(desc.Label, depList);
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
			var desc = new MapDescChild(
									tileset,
									MapPath,
									RoutePath,
									OccultPath,
									new string[0],
									Palette);
			MapDescDictionary[desc.Label]    =
			Categories[category][desc.Label] = desc;
		}

		public override void AddMap(MapDescChild desc, string category)
		{
			MapDescDictionary[desc.Label]    =
			Categories[category][desc.Label] = desc;
		}

//		public override MapDescChild RemoveTileset(string tileset, string category)
//		{
//			var desc = Categories[category][tileset] as MapDescChild;
//			Categories[category].Remove(tileset);
//			return desc;
//		}

		public override void ParseLine(
				string key,
				string val,
				StreamReader sr,
				Varidia vars)
		{
			switch (key.ToUpperInvariant())
			{
				case "FILES":
				{
					var descDictionary = new Dictionary<string, MapDescBase>();
					Categories[val] = descDictionary;
					string lineVars = Varidia.ReadLine(sr, vars);
					while (lineVars.ToUpperInvariant() != "END")
					{
						int pos           = lineVars.IndexOf(':');
						string file       = lineVars.Substring(0, pos);
						string[] terrains = lineVars.Substring(pos + 1).Split(' ');

						var desc = new MapDescChild(
												file,
												MapPath,
												RoutePath,
												OccultPath,
												terrains,
												Palette);

						MapDescDictionary[file] =
						descDictionary[file]    = desc;

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
	}
}
