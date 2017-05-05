namespace XCom
{
	public class Link
	{
		public const byte NotUsed   = 0xFF;
		public const byte ExitNorth = 0xFE;
		public const byte ExitEast  = 0xFD;
		public const byte ExitSouth = 0xFC;
		public const byte ExitWest  = 0xFB;


		internal Link(byte id, byte distance, byte type)
		{
			Destination = id;
			Distance = distance;
			UsableType = (UnitType)type;
		}


		/// <summary>
		/// Gets/Sets the index of the destination node.
		/// </summary>
		public byte Destination
		{ get; set; }

		/// <summary>
		/// Gets/Sets the distance to the destination node.
		/// </summary>
		public byte Distance
		{ get; set; }

		/// <summary>
		/// Gets/Sets the unit type that can use this link.
		/// </summary>
		public UnitType UsableType
		{ get; set; }
	}
}
