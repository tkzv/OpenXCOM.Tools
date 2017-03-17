using System.IO;


namespace XCom
{
	public class RouteNode
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


		private readonly byte _col;
		private readonly byte _row;
		private int _height;

		private readonly Link[] _links;

		public const int LinkSlots = 5;


		public RouteNode(byte id, byte[] data)
		{
			Index = id;

			_row    = data[0];
			_col    = data[1];
			_height = data[2];

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

			UsableType = (UnitType)data[19];
			UsableRank = data[20];
			Priority   = (NodeImportance)data[21];
			Attack     = (BaseModuleAttack)data[22];
			Spawn      = (SpawnUsage)data[23];
		}

		public RouteNode(byte id, byte row, byte col, byte height)
		{
			Index = id;

			_col    = col;
			_row    = row;
			_height = height;

			_links = new Link[LinkSlots];
			for (int i = 0; i != LinkSlots; ++i)
				_links[i] = new Link(Link.NOT_USED, 0, 0);

			UsableType = UnitType.Any;
			UsableRank = 0;
			Priority   = NodeImportance.Zero;
			Attack     = BaseModuleAttack.Zero;
			Spawn      = SpawnUsage.NoSpawn;
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

		public void Save(FileStream fs) // TODO: wrap this in a 'using' block
		{
//			fs.Write(data, 0, data.Length);

			fs.WriteByte(_row);
			fs.WriteByte(_col);
			fs.WriteByte((byte)_height);
			fs.WriteByte(0);

			for (int i = 0; i != LinkSlots; ++i)
			{
				fs.WriteByte(_links[i].Destination);
				fs.WriteByte(_links[i].Distance);
				fs.WriteByte((byte)_links[i].UsableType);
			}

			fs.WriteByte((byte)UsableType);
			fs.WriteByte((byte)UsableRank);
			fs.WriteByte((byte)Priority);
			fs.WriteByte((byte)Attack);
			fs.WriteByte((byte)Spawn);
		}

/*		public override string ToString()
		{
			return ("c:" + _col + " r:" + _row + " h:" + _height);
		} */

		public byte Col
		{
			get { return _col; }
		}

		public byte Row
		{
			get { return _row; }
		}

		public int Height
		{
			get { return _height; }
			set { _height = value ; }
		}

		public UnitType UsableType
		{ get; set; }

		public byte UsableRank
		{ get; set; }

		public NodeImportance Priority
		{ get; set; }

		public BaseModuleAttack Attack
		{ get; set; }

		public SpawnUsage Spawn
		{ get; set; }

		/// <summary>
		/// Gets the link-field at 'id'.
		/// </summary>
		public Link this[int id]
		{
			get { return _links[id]; }
		}
//		public Link GetLinkedNode(int id)
//		{
//			return _links[id];
//		}

		/// <summary>
		/// Gets the index of this RouteNode.
		/// </summary>
		public byte Index
		{ get; set; }
	}
}
