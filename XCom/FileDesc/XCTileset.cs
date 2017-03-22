using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using XCom.Interfaces;
using XCom.Interfaces.Base;


namespace XCom
{
	public class XCTileset
		:
		IXCTileset
	{
//		private string[] _mapOrder;

//		private MapLocation[] _startLoc;

//		private int _tileStart = -1;
//		private int _tileEnd   = -1;


		public XCTileset(string name)
			:
			base(name)
		{}

		public XCTileset(string name, StreamReader sr, VarCollection vars)
			:
			base(name, sr, vars)
		{}


/*
		public MapLocation[] StartLocations
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

		public string[] MapOrder
		{
			get { return _mapOrder; }
		}

		public string[] Order
		{
			get { return _mapOrder; }
		}
*/

//		public override IMap GetMap(ShipDescriptor xCom, ShipDescriptor alien)
//		{ return new Type1Map(this, xCom, alien); }

/*		public override void Save(StreamWriter sw, VarCollection vars)
		{
			sw.WriteLine("Tileset:" + name);
			sw.WriteLine("\ttype:1");

			if (vars.Vars[rootPath] == null)
				sw.WriteLine("\trootpath:" + rootPath);
			else
				sw.WriteLine("\trootpath:" + ((Variable)vars.Vars[rootPath]).Name);

			if (vars.Vars[rmpPath] == null)
				sw.WriteLine("\trmpPath:" + rootPath);
			else
				sw.WriteLine("\trmpPath:" + ((Variable)vars.Vars[rmpPath]).Name);

			if (vars.Vars[blankPath] == null)
				sw.WriteLine("\tblankPath:" + blankPath);
			else
				sw.WriteLine("\tblankPath:" + ((Variable)vars.Vars[blankPath]).Name);

			sw.WriteLine("\tpalette:" + myPal.Name);

			foreach (string str in subsets.Keys)
			{
				Dictionary<string, IMapDesc> h = subsets[str];
				if (h != null)
				{
					var vc = new VarCollection("Deps");
					foreach (string s in h.Keys)
					{
						var id = (XCMapDesc)maps[s];
						if (id != null)
						{
							string depList = "";
							if (id.Dependencies.Length > 0)
							{
								int i = 0;
								for (; i < id.Dependencies.Length - 1; i++)
									depList += id.Dependencies[i] + " ";
	
								depList += id.Dependencies[i];
							}
							vc.AddVar(id.Name, depList);
						}
					}

					sw.WriteLine("\tfiles:" + str);
	
					foreach (string vKey in vc.Variables)
						((Variable)vc.Vars[vKey]).Write(sw, "\t\t");

					sw.WriteLine("\tend");
				}
			}

			sw.WriteLine("end\n");
			sw.Flush();
		} */

/*		public override void AddMap(string name, string subset)
		{
			var imd = new XCMapDesc(
								name,
								rootPath,
								blankPath,
								rmpPath,
								new string[0],
								myPal);
			maps[imd.Name] = imd;
			subsets[subset][imd.Name] = imd;
		} */

/*		public override void AddMap(XCMapDesc imd, string subset)
		{
			maps[imd.Name] = imd;
			subsets[subset][imd.Name] = imd;
		} */

/*		public override XCMapDesc RemoveMap(string name, string subset)
		{
			var imd = (XCMapDesc)subsets[subset][name];
			subsets[subset].Remove(name);
			return imd;
		} */

/*		public override void ParseLine(
									string keyword,
									string line,
									StreamReader sr,
									VarCollection vars)
		{
			switch (keyword)
			{
				case "files":
				{
					var subset = new Dictionary<string, IMapDesc>();
					subsets[line] = subset;
					string varsLine = VarCollection.ReadLine(sr, vars);
					while (varsLine != "end" && varsLine != "END")
					{
						int idx = varsLine.IndexOf(':');
						string file = varsLine.Substring(0, idx);
						string[] deps = varsLine.Substring(idx + 1).Split(' ');
						var imd = new XCMapDesc(
											file,
											rootPath,
											blankPath,
											rmpPath,
											deps,
											myPal);
						maps[file] = imd;
						subset[file] = imd;
						varsLine = VarCollection.ReadLine(sr, vars);
					}
					break;
				}

				case "order":
					_mapOrder = line.Split(' ');
					break;

				case "starttile":
					_tileStart = int.Parse(line);
					break;

				case "startloc":
				{
					string[] locs = line.Split(' ');
					_startLoc = new MapLocation[locs.Length];
					for (int i = 0; i < locs.Length; i++)
					{
						string[] loc = locs[i].Split(',');
						int r = int.Parse(loc[0]);
						int c = int.Parse(loc[1]);
						int h = int.Parse(loc[2]);
						_startLoc[i] = new MapLocation(r, c, h);
					}
					break;
				}

				case "endtile":
					_tileEnd = int.Parse(line);
					break;

				default:
					xConsole.AddLine(string.Format(
												"Unknown line in tileset {0}-> {1}:{2}",
												name,
												keyword,
												line));
					break;
			}
		} */
	}
}
