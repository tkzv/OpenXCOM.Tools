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
	#region Enums
	// NOTE: Only 'UnitRankUfo' and 'UnitRankTftd' need to be enumerated as
	// byte-type. Otherwise the EnumString class goes snakey when
	// RouteView.OnSpawnRankSelectedIndexChanged() fires. For reasons, it cannot
	// handle the cast automatically like the other enumerated types here appear
	// to. But I left the others as bytes also for safety.
	public enum UnitType
		:
			byte
	{
		Any         = 0,
		Flying      = 1,
		Small       = 2,
		FlyingLarge = 3,
		Large       = 4
	};

	public enum UnitRankUfo
		:
			byte
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
		:
			byte
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
		:
			byte
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
		:
			byte
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
		:
			byte
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
		:
			byte
	{
		None      = 0x00, // pacify FxCop CA1008 BUT DO NOT USE IT.
		NotUsed   = 0xFF, // since valid route-nodes can and will have a value of 0.
		ExitNorth = 0xFE,
		ExitEast  = 0xFD,
		ExitSouth = 0xFC,
		ExitWest  = 0xFB
	};
	#endregion


	/// <summary>
	/// This class reads, saves, and generally manages all the information in a
	/// .RMP file. It's like the parent of RouteNode.
	/// </summary>
	public sealed class RouteNodeCollection
		:
			IEnumerable<RouteNode>
	{
		#region Fields
		private readonly List<RouteNode> _nodes;

		public const string RouteExt  = ".RMP";
		public const string RoutesDir = "ROUTES";

		private string FullPath
		{ get; set; }


		public static readonly object[] UnitRankUfo =
		{
			new EnumString("0:Civ-Scout",        XCom.UnitRankUfo.Civilian),
			new EnumString("1:XCom",             XCom.UnitRankUfo.XCom),
			new EnumString("2:Soldier",          XCom.UnitRankUfo.Soldier),
			new EnumString("3:Navigator",        XCom.UnitRankUfo.Navigator),
			new EnumString("4:Leader/Commander", XCom.UnitRankUfo.LeaderCommander),
			new EnumString("5:Engineer",         XCom.UnitRankUfo.Engineer),
			new EnumString("6:Misc1",            XCom.UnitRankUfo.Misc1),
			new EnumString("7:Medic",            XCom.UnitRankUfo.Medic),
			new EnumString("8:Misc2",            XCom.UnitRankUfo.Misc2)
		};

		public static readonly object[] UnitRankTftd =
		{
			new EnumString("0:Civ-Scout",        XCom.UnitRankTftd.Civilian),
			new EnumString("1:XCom",             XCom.UnitRankTftd.XCom),
			new EnumString("2:Soldier",          XCom.UnitRankTftd.Soldier),
			new EnumString("3:Squad Leader",     XCom.UnitRankTftd.SquadLeader),
			new EnumString("4:Leader/Commander", XCom.UnitRankTftd.LeaderCommander),
			new EnumString("5:Medic",            XCom.UnitRankTftd.Medic),
			new EnumString("6:Misc1",            XCom.UnitRankTftd.Misc1),
			new EnumString("7:Technician",       XCom.UnitRankTftd.Technician),
			new EnumString("8:Misc2",            XCom.UnitRankTftd.Misc2)
		};

		public static readonly object[] SpawnUsage =
		{
			new EnumString("0:No Spawn", XCom.SpawnUsage.NoSpawn),
			new EnumString("1:Spawn",    XCom.SpawnUsage.Spawn1),
			new EnumString("2:Spawn",    XCom.SpawnUsage.Spawn2),
			new EnumString("3:Spawn",    XCom.SpawnUsage.Spawn3),
			new EnumString("4:Spawn",    XCom.SpawnUsage.Spawn4),
			new EnumString("5:Spawn",    XCom.SpawnUsage.Spawn5),
			new EnumString("6:Spawn",    XCom.SpawnUsage.Spawn6),
			new EnumString("7:Spawn",    XCom.SpawnUsage.Spawn7),
			new EnumString("8:Spawn",    XCom.SpawnUsage.Spawn8),
			new EnumString("9:Spawn",    XCom.SpawnUsage.Spawn9),
			new EnumString("10:Spawn",   XCom.SpawnUsage.Spawn10)
		};
		#endregion


		#region Properties
		public RouteNode this[int id]
		{
			get
			{
				if (id > -1 && id < _nodes.Count)
					return _nodes[id];

				return null;
			}
		}

		public int Length
		{
			get { return _nodes.Count; }
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor. Reads the .RMP file and adds its data as RouteNodes to a List.
		/// </summary>
		/// <param name="routes"></param>
		/// <param name="basepath"></param>
		internal RouteNodeCollection(string routes, string basepath)
		{
			_nodes = new List<RouteNode>();

			FullPath = Path.Combine(basepath, RoutesDir);
			FullPath = Path.Combine(FullPath, routes + RouteExt);

			if (File.Exists(FullPath))
			{
				using (var bs = new BufferedStream(File.OpenRead(FullPath)))
				{
					for (byte id = 0; id < bs.Length / 24; ++id)
					{
						var bindata = new byte[24];
						bs.Read(bindata, 0, 24);

						_nodes.Add(new RouteNode(id, bindata));
					}
				}
			}
		}
		#endregion


		#region Interface requirements
		/// <summary>
		/// Gets an enumerator for the node-list.
		/// </summary>
		/// <returns></returns>
		IEnumerator<RouteNode> IEnumerable<RouteNode>.GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		/// <summary>
		/// Gets another enumerator for the node-list.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}
		#endregion


		#region Methods
		/// <summary>
		/// Saves the .RMP file.
		/// </summary>
		internal void SaveRoutes()
		{
			SaveNodes(FullPath);
		}

		/// <summary>
		/// Saves the .RMP file as a different file.
		/// </summary>
		/// <param name="pf">the path+file to save as</param>
		internal void SaveRoutes(string pf)
		{
			string pfe = pf + RouteExt;
			Directory.CreateDirectory(Path.GetDirectoryName(pfe));
			SaveNodes(pfe);
		}

		/// <summary>
		/// Saves the route-nodes to a .RMP file.
		/// </summary>
		/// <param name="pfe">path+file+extension</param>
		private void SaveNodes(string pfe)
		{
			using (var fs = File.Create(pfe))
			{
				for (int id = 0; id != _nodes.Count; ++id)
					_nodes[id].SaveNode(fs); // -> RouteNode.SaveNode() writes each node-data
			}
		}

		/// <summary>
		/// Adds a node to the node-list.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="lev"></param>
		/// <returns></returns>
		internal RouteNode AddNode(byte row, byte col, byte lev)
		{
			var node = new RouteNode((byte)_nodes.Count, row, col, lev);
			_nodes.Add(node);

			return node;
		}

		/// <summary>
		/// Deletes a node from the node-list.
		/// </summary>
		/// <param name="node">the node to delete</param>
		public void DeleteNode(RouteNode node)
		{
			int nodeId = node.Index;

			_nodes.Remove(node);

			foreach (var node0 in _nodes)
			{
				if (node0.Index > nodeId) // shuffle all higher-indexed nodes down 1
					--node0.Index;

				for (int slotId = 0; slotId != RouteNode.LinkSlots; ++slotId)
				{
					var link = node0[slotId];

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

		/// <summary>
		/// Checks of a given node is outside the Map boundaries.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="cols"></param>
		/// <param name="rows"></param>
		/// <param name="levs"></param>
		/// <returns></returns>
		internal static bool IsOutsideMap(
				RouteNode node,
				int cols,
				int rows,
				int levs)
		{
			return node.Col < 0 || node.Col >= cols
				|| node.Row < 0 || node.Row >= rows
				|| node.Lev < 0 || node.Lev >= levs;
		}

//		public RmpEntry GetEntryAtHeight(byte currentHeight)
//		{
//			foreach (var route in _entries)
//				if (route.Height == currentHeight)
//					return route;
//
//			return null;
//		}
		#endregion
	}
}
