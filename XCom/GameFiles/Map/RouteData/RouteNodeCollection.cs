using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


// when determining spawn positions the following nodes are tried
// Leader > Nav > Eng> Unknown8 > Med > Soldier > Any
// Nav > Eng > Soldier > Any
// Human is only used for xcom units
// civilians can only start on any nodes
// unknown6 is only used as an alternate choice to a medic
// unknown8 is only used by squad leaders and commanders

// unknown8 = any alien
// unknown6 = Medic,Navigator,Soldier,Terrorist

	/*
		UFO			TFTD
		Commander	Commander
		Leader		Navigator
		Engineer	Medic
		Medic		Technition
		Navigator	SquadLeader
		Soldier		Soldier
	*/


#region about the RMP file

/*
// Route Record for RMP files
//
// RMP records describe the points in a lattice and the links connecting
// them.  However, special values of rmpindex are:
//   FF = Not used
//   FE = Exit terrain to north
//   FD = Exit terrain to east
//   FC = Exit terrain to south
//   FB = Exit terrain to west

typedef struct latticelink
{
	unsigned char rmpindex; // Index of a linked point in the
lattice
	unsigned char distance; // Approximate distance from current
point
	unsigned char linktype; // Uses RRT_xxxx
} LatticeLink;

#define MaxLinks 5

typedef struct rmprec
{
	unsigned char row;
	unsigned char col;
	unsigned char lvl;
	unsigned char zero03;			// Always 0
	LatticeLink latlink[MaxLinks];	// 5 3-byte entries (shown above)
	unsigned char type1;			// Uses RRT_xxxx: 0,1,2,4
observed
	unsigned char type2;			// Uses RRR_xxxx: 0,1,2,3,4,5, ,7,
observed
	unsigned char type3;			// Uses RRR_xxxx: 0,1,2,3,4,5,6,7,8
observed
	unsigned char type4;			// Almost always 0
	unsigned char type5;			// 0=Don't use 1=Use most of the time...2+
= Use less and less often, 0 thru A observed
} RmpRec;

// Types of Units

#define RRT_Any		0
#define RRT_Flying	1
#define RRT_Small	2
#define RRT_4		4 // Unknown

// Ranks of Units

#define RRR_0			0 // Unknown
#define RRR_Human		1
#define RRR_Soldier		2
#define RRR_Navigator	3
#define RRR_Leader		4
#define RRR_Engineer	5
#define RRR_6			6 // Commander?
#define RRR_Medic		7
#define RRR_8			8 // Unknown
*/
#endregion


namespace XCom
{
	public enum UnitType
	{
		Any         = 0,
		Flying      = 1,
		Small       = 2,
		FlyingLarge = 3,
		Large       = 4
	};

	public enum UnitRankUfo
	{
		Civilian        = 0,
		XCom            = 1,
		Soldier         = 2,
		Navigator       = 3,
		LeaderCommander = 4,
		Engineer        = 5,
		Misc1           = 6,
		Medic           = 7,
		Misc2           = 8
	};

	public enum UnitRankTftd
	{
		Civilian        = 0,
		XCom            = 1,
		Soldier         = 2,
		SquadLeader     = 3,
		LeaderCommander = 4,
		Medic           = 5,
		Misc1           = 6,
		Technician      = 7,
		Misc2           = 8
	};

	public enum SpawnUsage
	{
		NoSpawn = 0,
		Spawn1  = 1,
		Spawn2  = 2,
		Spawn3  = 3,
		Spawn4  = 4,
		Spawn5  = 5,
		Spawn6  = 6,
		Spawn7  = 7,
		Spawn8  = 8,
		Spawn9  = 9,
		Spawn10 = 10
	};

	public enum NodeImportance
	{
		Zero  = 0,
		One   = 1,
		Two   = 2,
		Three = 3,
		Four  = 4,
		Five  = 5,
		Six   = 6,
		Seven = 7,
		Eight = 8,
		Nine  = 9,
		Ten   = 10
	};

	public enum BaseModuleAttack
	{
		Zero  = 0,
		One   = 1,
		Two   = 2,
		Three = 3,
		Four  = 4,
		Five  = 5,
		Six   = 6,
		Seven = 7,
		Eight = 8,
		Nine  = 9,
		Ten   = 10
	};

	public enum LinkType
	{
		None      = 0x00, // pacify FxCop CA1008 BUT DO NOT USE IT.
		NotUsed   = 0xFF, // since valid route-nodes can and will have a value of 0.
		ExitNorth = 0xFE,
		ExitEast  = 0xFD,
		ExitSouth = 0xFC,
		ExitWest  = 0xFB
	};


	public sealed class RouteNodeCollection
		:
			IEnumerable<RouteNode>
	{
		private readonly List<RouteNode> _nodes;

		public const string RouteExt = ".RMP";

		private readonly string _baseName;
		private readonly string _basePath;


		public static readonly object[] UnitRankUfo =
		{
			new StrEnum("0:Civ-Scout",        XCom.UnitRankUfo.Civilian),
			new StrEnum("1:XCom",             XCom.UnitRankUfo.XCom),
			new StrEnum("2:Soldier",          XCom.UnitRankUfo.Soldier),
			new StrEnum("3:Navigator",        XCom.UnitRankUfo.Navigator),
			new StrEnum("4:Leader/Commander", XCom.UnitRankUfo.LeaderCommander),
			new StrEnum("5:Engineer",         XCom.UnitRankUfo.Engineer),
			new StrEnum("6:Misc1",            XCom.UnitRankUfo.Misc1),
			new StrEnum("7:Medic",            XCom.UnitRankUfo.Medic),
			new StrEnum("8:Misc2",            XCom.UnitRankUfo.Misc2)
		};

		public static readonly object[] UnitRankTftd =
		{
			new StrEnum("0:Civ-Scout",        XCom.UnitRankTftd.Civilian),
			new StrEnum("1:XCom",             XCom.UnitRankTftd.XCom),
			new StrEnum("2:Soldier",          XCom.UnitRankTftd.Soldier),
			new StrEnum("3:Squad Leader",     XCom.UnitRankTftd.SquadLeader),
			new StrEnum("4:Leader/Commander", XCom.UnitRankTftd.LeaderCommander),
			new StrEnum("5:Medic",            XCom.UnitRankTftd.Medic),
			new StrEnum("6:Misc1",            XCom.UnitRankTftd.Misc1),
			new StrEnum("7:Technician",       XCom.UnitRankTftd.Technician),
			new StrEnum("8:Misc2",            XCom.UnitRankTftd.Misc2)
		};

		public static readonly object[] SpawnUsage =
		{
			new StrEnum("0:No Spawn", XCom.SpawnUsage.NoSpawn),
			new StrEnum("1:Spawn",    XCom.SpawnUsage.Spawn1),
			new StrEnum("2:Spawn",    XCom.SpawnUsage.Spawn2),
			new StrEnum("3:Spawn",    XCom.SpawnUsage.Spawn3),
			new StrEnum("4:Spawn",    XCom.SpawnUsage.Spawn4),
			new StrEnum("5:Spawn",    XCom.SpawnUsage.Spawn5),
			new StrEnum("6:Spawn",    XCom.SpawnUsage.Spawn6),
			new StrEnum("7:Spawn",    XCom.SpawnUsage.Spawn7),
			new StrEnum("8:Spawn",    XCom.SpawnUsage.Spawn8),
			new StrEnum("9:Spawn",    XCom.SpawnUsage.Spawn9),
			new StrEnum("10:Spawn",   XCom.SpawnUsage.Spawn10)
		};


		internal RouteNodeCollection(string baseName, string basePath)
		{
			_baseName = baseName;
			_basePath = basePath;

			_nodes = new List<RouteNode>();

			string pathfilext = basePath + baseName + RouteExt;
			if (File.Exists(pathfilext))
			{
				using (var bs = new BufferedStream(File.OpenRead(pathfilext)))
				{
					for (byte i = 0; i < bs.Length / 24; ++i)
					{
						var data = new byte[24];
						bs.Read(data, 0, 24);
						_nodes.Add(new RouteNode(i, data));
					}
				}
			}
		}


		/// <summary>
		/// Saves the .RMP file.
		/// </summary>
		internal void Save()
		{
			using (var fs = File.Create(_basePath + _baseName + RouteExt))
			{
				for (int i = 0; i != _nodes.Count; ++i)
					_nodes[i].Save(fs); // -> RouteNode.Save() writes the node-data
			}
		}

		IEnumerator<RouteNode> IEnumerable<RouteNode>.GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		public RouteNode this[int i]
		{
			get
			{
				if (i > -1 && i < _nodes.Count)
					return _nodes[i];

				return null;
			}
		}

		public int Length
		{
			get { return _nodes.Count; }
		}

		public byte ExtraHeight
		{ get; set; }

		public void Delete(RouteNode node)
		{
			int nodeId = node.Index;

			_nodes.Remove(node);

			foreach (var node0 in _nodes)
			{
				if (node0.Index > nodeId) // shuffle all higher-indexed nodes down 1
					--node0.Index;

				for (int i = 0; i != RouteNode.LinkSlots; ++i)
				{
					var link = node0[i];

					if (link.Destination == nodeId)
					{
						link.Destination = Link.NotUsed;
					}
					else if (link.Destination > nodeId && link.Destination < Link.ExitWest)
					{
						--link.Destination;
					}
				}
			}
		}

		internal RouteNode Add(byte row, byte col, byte height)
		{
			var node = new RouteNode((byte)_nodes.Count, row, col, height);
			_nodes.Add(node);

			return node;
		}

		/// <summary>
		/// Checks for and if necessary deletes nodes that are outside of a
		/// Map's x/y/z bounds. See also RouteService.CheckNodeBounds().
		/// </summary>
		/// <param name="newC"></param>
		/// <param name="newR"></param>
		/// <param name="newH"></param>
		internal void CheckNodeBounds(int newC, int newR, int newH)
		{
			var deletions = new List<RouteNode>();

			foreach (var node in _nodes)
				if (IsOutsideMap(node, newC, newR, newH))
					deletions.Add(node);

			foreach (var entry in deletions)
				Delete(entry);
		}

		internal static bool IsOutsideMap(
				RouteNode node,
				int cols,
				int rows,
				int height)
		{
			return node.Col    < 0 || node.Col    >= cols
				|| node.Row    < 0 || node.Row    >= rows
				|| node.Height < 0 || node.Height >= height;
		}

//		public RmpEntry GetEntryAtHeight(byte currentHeight)
//		{
//			foreach (var route in _entries)
//				if (route.Height == currentHeight)
//					return route;
//
//			return null;
//		}
	}
}
