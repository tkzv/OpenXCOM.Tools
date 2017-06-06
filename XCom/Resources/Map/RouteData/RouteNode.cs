using System.IO;


namespace XCom
{
	public sealed class RouteNode
	{
		#region rmprec
/*		typedef struct rmprec
		{
			unsigned char row;
			unsigned char col;
			unsigned char lvl;
			unsigned char zero03;				// Always 0
			LatticeLink   latlink[MaxLinks];	// 5 3-byte entries (shown above)
			unsigned char type1;				// Uses RRT_xxxx: 0,1,2,4
		observed
			unsigned char type2;				// Uses RRR_xxxx: 0,1,2,3,4,5, ,7,
		observed
			unsigned char type3;				// Uses RRR_xxxx: 0,1,2,3,4,5,6,7,8
		observed
			unsigned char type4;				// Almost always 0
			unsigned char type5;				// 0=Don't use 1=Use most of the time...2+
		= Use less and less often, 0 thru A observed
		} RmpRec; */
		#endregion


		#region Fields & Properties
		public const int LinkSlots = 5;


		private readonly byte _col;
		public byte Col
		{
			get { return _col; }
		}

		private readonly byte _row;
		public byte Row
		{
			get { return _row; }
		}

		public int Lev
		{ get; set; }


		private readonly Link[] _links;
		/// <summary>
		/// Gets the link-field at slot.
		/// </summary>
		public Link this[int id]
		{
			get { return _links[id]; }
		}

		public UnitType UsableType
		{ get; set; }

		public byte SpawnRank
		{ get; set; }

		public NodeImportance Priority
		{ get; set; }

		public BaseModuleAttack Attack
		{ get; set; }

		public SpawnUsage SpawnWeight
		{ get; set; }

		/// <summary>
		/// Gets the index of this RouteNode.
		/// </summary>
		public byte Index
		{ get; set; }
		#endregion


		#region cTors
		/// <summary>
		/// cTor1. Creates a node from binary data.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="bindata"></param>
		internal RouteNode(byte id, byte[] bindata)
		{
			Index = id;

			_row = bindata[0]; // note that x & y are switched in the RMP-file.
			_col = bindata[1];
			Lev  = bindata[2]; // NOTE: auto-converts to int-type.

			// NOTE: 'bindata[3]' is not used.

			_links = new Link[LinkSlots];

			int offset = 4;
			for (int slotId = 0; slotId != LinkSlots; ++slotId)
			{
				_links[slotId] = new Link(
									bindata[offset], // note that x & y are reversed in the RMP-file.
									bindata[offset + 1],
									bindata[offset + 2]);
				offset += 3;
			}

			UsableType  = (UnitType)bindata[19];
			SpawnRank   = bindata[20];
			Priority    = (NodeImportance)bindata[21];
			Attack      = (BaseModuleAttack)bindata[22];
			SpawnWeight = (SpawnUsage)bindata[23];
		}

		/// <summary>
		/// cTor2. Creates a node based on rcl.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="lev"></param>
		internal RouteNode(byte id, byte row, byte col, byte lev)
		{
			Index = id;

			_col = col;
			_row = row;
			Lev  = lev; // NOTE: auto-converts to int-type.

			_links = new Link[LinkSlots];
			for (int slotId = 0; slotId != LinkSlots; ++slotId)
				_links[slotId] = new Link(Link.NotUsed, 0, 0);

			UsableType  = UnitType.Any;
			SpawnRank   = 0;
			Priority    = NodeImportance.Zero;
			Attack      = BaseModuleAttack.Zero;
			SpawnWeight = SpawnUsage.NoSpawn;
		}
		#endregion


		#region Methods
		/// <summary>
		/// Writes data to the stream provided by RouteNodeCollection.Save().
		/// </summary>
		/// <param name="fs">the Stream provided by RouteNodeCollection.Save()</param>
		internal void SaveNode(Stream fs)
		{
			fs.WriteByte(_row);
			fs.WriteByte(_col);
			fs.WriteByte((byte)Lev);
			fs.WriteByte((byte)0);

			for (int id = 0; id != LinkSlots; ++id)
			{
				fs.WriteByte(_links[id].Destination);
				fs.WriteByte(_links[id].Distance);
				fs.WriteByte((byte)_links[id].UsableType);
			}

			fs.WriteByte((byte)UsableType);
			fs.WriteByte((byte)SpawnRank); // NOTE: is already a byte-type.
			fs.WriteByte((byte)Priority);
			fs.WriteByte((byte)Attack);
			fs.WriteByte((byte)SpawnWeight);
		}

		public override bool Equals(object obj)
		{
			var other = obj as RouteNode;
			return (other == null || Index == other.Index);
		}

		public override int GetHashCode()
		{
			return Index;
		}

		public override string ToString()
		{
			return ("c " + _col + "  r " + _row + "  l " + Lev);
		}

//		public Link GetLinkedNode(int id)
//		{
//			return _links[id];
//		}
		#endregion
	}
}
