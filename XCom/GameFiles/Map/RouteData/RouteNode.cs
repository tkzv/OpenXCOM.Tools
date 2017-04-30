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


		internal RouteNode(byte id, byte[] data)
		{
			Index = id;

			_row = data[0];
			_col = data[1];
			Lev  = data[2]; // NOTE: auto-converts to int-type.

			_links = new Link[LinkSlots];

			int x = 4;
			for (int i = 0; i != LinkSlots; ++i)
			{
				_links[i] = new Link(
									data[x],
									data[x + 1],
									data[x + 2]);
				x += 3;
			}

			UsableType  = (UnitType)data[19];
			SpawnRank   = data[20];
			Priority    = (NodeImportance)data[21];
			Attack      = (BaseModuleAttack)data[22];
			SpawnWeight = (SpawnUsage)data[23];
		}
		internal RouteNode(byte id, byte row, byte col, byte lev)
		{
			Index = id;

			_col = col;
			_row = row;
			Lev  = lev; // NOTE: auto-converts to int-type.

			_links = new Link[LinkSlots];
			for (int i = 0; i != LinkSlots; ++i)
				_links[i] = new Link(Link.NotUsed, 0, 0);

			UsableType  = UnitType.Any;
			SpawnRank   = 0;
			Priority    = NodeImportance.Zero;
			Attack      = BaseModuleAttack.Zero;
			SpawnWeight = SpawnUsage.NoSpawn;
		}


		/// <summary>
		/// Writes data to the stream provided by RouteNodeCollection.Save().
		/// </summary>
		/// <param name="str">the Stream provided by RouteNodeCollection.Save()</param>
		internal void Save(Stream str)
		{
			str.WriteByte(_row);
			str.WriteByte(_col);
			str.WriteByte((byte)Lev);
			str.WriteByte((byte)0);

			for (int i = 0; i != LinkSlots; ++i)
			{
				str.WriteByte(_links[i].Destination);
				str.WriteByte(_links[i].Distance);
				str.WriteByte((byte)_links[i].UsableType);
			}

			str.WriteByte((byte)UsableType);
			str.WriteByte((byte)SpawnRank); // NOTE: is already a byte-type.
			str.WriteByte((byte)Priority);
			str.WriteByte((byte)Attack);
			str.WriteByte((byte)SpawnWeight);
		}

		public override bool Equals(object obj)
		{
			var node = obj as RouteNode;
			return (node == null || Index == node.Index);
		}

		public override int GetHashCode()
		{
			return Index;
		}

		public override string ToString()
		{
			return ("c:" + _col + " r:" + _row + " l:" + Lev);
		}

//		public Link GetLinkedNode(int id)
//		{
//			return _links[id];
//		}
	}
}
