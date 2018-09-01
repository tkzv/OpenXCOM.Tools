using System.IO;


namespace XCom
{
	public sealed class RouteNode
	{
		#region Fields (static)
		public const int LinkSlots = 5;
		#endregion


		#region Properties
		public byte Col
		{ get; set; }

		public byte Row
		{ get; set; }

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

		public UnitType Type
		{ get; set; }

		public byte Rank
		{ get; set; }

		public PatrolPriority Patrol
		{ get; set; }

		public BaseAttack Attack
		{ get; set; }

		public SpawnWeight Spawn
		{ get; set; }

		/// <summary>
		/// Gets the index of this RouteNode.
		/// </summary>
		public byte Index
		{ get; set; }
		#endregion


		#region cTors
		/// <summary>
		/// cTor[1]. Creates a node from binary data.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="bindata"></param>
		internal RouteNode(byte id, byte[] bindata)
		{
			Index = id;

			Col = bindata[1]; // note that x & y are switched in the RMP-file.
			Row = bindata[0];
			Lev = bindata[2]; // NOTE: auto-converts to int-type.

			// NOTE: 'bindata[3]' is not used.

			_links = new Link[LinkSlots];

			int offset = 4;
			for (int slotId = 0; slotId != LinkSlots; ++slotId)
			{
				_links[slotId] = new Link(
									bindata[offset],
									bindata[offset + 1],
									bindata[offset + 2]);
				offset += 3;
			}

			Type   = (UnitType)bindata[19];
			Rank   = bindata[20];
			Patrol = (PatrolPriority)bindata[21];
			Attack = (BaseAttack)bindata[22];
			Spawn  = (SpawnWeight)bindata[23];
		}

		/// <summary>
		/// cTor[2]. Creates a node based on row/col/level.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="lev"></param>
		internal RouteNode(byte id, byte row, byte col, byte lev)
		{
			Index = id;

			Col = col;
			Row = row;
			Lev = lev; // NOTE: auto-converts to int-type.

			_links = new Link[LinkSlots];
			for (int slotId = 0; slotId != LinkSlots; ++slotId)
				_links[slotId] = new Link(Link.NotUsed, 0, 0);

			Type   = UnitType.Any;
			Rank   = (byte)0;
			Patrol = PatrolPriority.Zero;
			Attack = BaseAttack.Zero;
			Spawn  = SpawnWeight.NoSpawn;
		}
		#endregion


		#region Methods
		/// <summary>
		/// Writes data to the stream provided by RouteNodeCollection.Save().
		/// </summary>
		/// <param name="fs">the Stream provided by RouteNodeCollection.Save()</param>
		internal void SaveNode(Stream fs)
		{
			fs.WriteByte(Row);
			fs.WriteByte(Col);
			fs.WriteByte((byte)Lev);
			fs.WriteByte((byte)0);

			for (int id = 0; id != LinkSlots; ++id)
			{
				fs.WriteByte(_links[id].Destination);
				fs.WriteByte(_links[id].Distance);
				fs.WriteByte((byte)_links[id].Type);
			}

			fs.WriteByte((byte)Type);
			fs.WriteByte(Rank);
			fs.WriteByte((byte)Patrol);
			fs.WriteByte((byte)Attack);
			fs.WriteByte((byte)Spawn);
		}

//		public Link GetLinkedNode(int id)
//		{
//			return _links[id];
//		}
		#endregion


		#region Methods (override)
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
			return ("c " + Col + "  r " + Row + "  L " + Lev);
		}
		#endregion
	}
}
