namespace XCom
{
	public class Link
	{
		public const byte NotUsed   = 0xFF;
		public const byte ExitNorth = 0xFE;
		public const byte ExitEast  = 0xFD;
		public const byte ExitSouth = 0xFC;
		public const byte ExitWest  = 0xFB;


		public Link(byte id, byte dist, byte type)
		{
			Destination = id;
			Distance = dist;
			UsableType = (UnitType)type;
		}


		/// <summary>
		/// Gets or sets the index of the destination node.
		/// </summary>
		public byte Destination
		{ get; set; }

		/// <summary>
		/// Gets or sets the distance to the destination node.
		/// </summary>
		public byte Distance
		{ get; set; }

		/// <summary>
		/// Gets or sets the unit type that can use this link.
		/// </summary>
		public UnitType UsableType
		{ get; set; }
	}
}
