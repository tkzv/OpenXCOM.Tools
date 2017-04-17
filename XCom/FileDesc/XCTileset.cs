using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces;
using XCom.Interfaces.Base;


namespace XCom
{
	internal sealed class XCTileset
		:
			Tileset
	{
//		private string[] _mapOrder;
//		private MapLocation[] _startLoc;
//		private int _tileStart = -1;
//		private int _tileEnd   = -1;

		private const string Tab = "\t";


		internal XCTileset(string name)
			:
				base(name)
		{}

		internal XCTileset(string name, StreamReader sr, Varidia vars)
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
			sw.WriteLine("Tileset:" + Name);
			sw.WriteLine(Tab + "type:1");

			if (vars.Vars[MapPath] != null)
				sw.WriteLine(Tab + "rootpath:" + ((Variable)vars.Vars[MapPath]).Name);
			else
				sw.WriteLine(Tab + "rootpath:" + MapPath);

			if (vars.Vars[RoutePath] != null)
				sw.WriteLine(Tab + "rmpPath:" + ((Variable)vars.Vars[RoutePath]).Name);
			else
				sw.WriteLine(Tab + "rmpPath:" + RoutePath);

			if (vars.Vars[BlankPath] != null)
				sw.WriteLine(Tab + "blankPath:" + ((Variable)vars.Vars[BlankPath]).Name);
			else
				sw.WriteLine(Tab + "blankPath:" + BlankPath);

			sw.WriteLine(Tab + "palette:" + Palette.Label);

			foreach (string keySubsets in Subsets.Keys)
			{
				Dictionary<string, MapDesc> valDesc = Subsets[keySubsets];
				if (valDesc != null)
				{
					var deps = new Varidia("Deps");
					foreach (string keyDesc in valDesc.Keys)
					{
						var desc = MapDescs[keyDesc] as XCMapDesc;
						if (desc != null)
						{
							string depList = String.Empty;
							if (desc.Dependencies.Length != 0)
							{
								int i = 0;
								for (; i != desc.Dependencies.Length - 1; ++i)
									depList += desc.Dependencies[i] + " ";
	
								depList += desc.Dependencies[i];
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

		public override void AddMap(string name, string subset)
		{
			var desc = new XCMapDesc(
								name,
								MapPath,
								BlankPath,
								RoutePath,
								new string[0],
								Palette);
			MapDescs[desc.Label] = desc;
			Subsets[subset][desc.Label] = desc;
		}

		public override void AddMap(XCMapDesc desc, string subset)
		{
			MapDescs[desc.Label] = desc;
			Subsets[subset][desc.Label] = desc;
		}

		public override XCMapDesc RemoveMap(string name, string subset)
		{
			var desc = Subsets[subset][name] as XCMapDesc;
			Subsets[subset].Remove(name);
			return desc;
		}

		public override void ParseLine(
				string key,
				string line,
				StreamReader sr,
				Varidia vars)
		{
			switch (key.ToUpperInvariant())
			{
				case "FILES":
				{
					var dictDescs = new Dictionary<string, MapDesc>();
					Subsets[line] = dictDescs;
					string lineVars = Varidia.ReadLine(sr, vars);
					while (lineVars.ToUpperInvariant() != "END")
					{
						int pos       = lineVars.IndexOf(':');
						string file   = lineVars.Substring(0, pos);
						string[] deps = lineVars.Substring(pos + 1).Split(' ');

						var desc = new XCMapDesc(
											file,
											MapPath,
											BlankPath,
											RoutePath,
											deps,
											Palette);

						MapDescs[file] =
						dictDescs[file] = desc;

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

				default:
//					XConsole.AdZerg(string.Format(
//												System.Globalization.CultureInfo.CurrentCulture,
//												"Unknown line in tileset {0}-> {1}:{2}",
//												Name, key, line));
					break;
			}
		}
	}
}
