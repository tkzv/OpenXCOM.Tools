using System;

using XCom.Interfaces;


namespace XCom
{
	public sealed class XCTile
		:
			XCom.Interfaces.Base.TileBase
	{
		private readonly PckSpriteCollection _pckPack;

		public XCTile Dead
		{ get; set; }

		private XCTile _alternate;
		internal XCTile Alternate
		{
			set { _alternate = value; }
		}


		internal XCTile(
				int id,
				PckSpriteCollection pckPack,
				McdRecord record)
			:
				base(id)
		{
			_pckPack = pckPack;
			Record   = record;

			Images = new XCImage[8]; // every tile-part contains refs to 8 sprites.
			InitializeSprites();
		}


		// re. Animating Sprites
		/// Basically this is how animations operate. For *any* animations to
		/// happen the Animation option has to be turned on. Non-door sprites
		/// always keep their array of sprites and will cycle because turning on
		/// the Animation option starts a timer that does that (see
		/// 'MapView.MainViewPanel').
		/// 
		/// UfoDoor sprites will animate when the Animation option is on and the
		/// Doors option is turned on; but whether or not they animate is
		/// controlled by setting their sprite-arrays to either the first image
		/// or an array of images, like non-door records do.
		/// 
		/// HumanDoors, which also need the Animation option on to animate as
		/// well as the Doors option on, will cycle by flipping their sprite
		/// back and forth between their first sprite and their Alt-tile's first
		/// sprite; they stop animating by setting the entire array to their
		/// first sprite only.

		/// <summary>
		/// Initializes this tilepart's array of sprites.
		/// </summary>
		private void InitializeSprites()
		{
			if (Record.UfoDoor || Record.HumanDoor)
			{
				for (int i = 0; i != 8; ++i)
					Images[i] = _pckPack[Record.Image1];
			}
			else
			{
				Images[0] = _pckPack[Record.Image1];
				Images[1] = _pckPack[Record.Image2];
				Images[2] = _pckPack[Record.Image3];
				Images[3] = _pckPack[Record.Image4];
				Images[4] = _pckPack[Record.Image5];
				Images[5] = _pckPack[Record.Image6];
				Images[6] = _pckPack[Record.Image7];
				Images[7] = _pckPack[Record.Image8];
			}
		}


		/// <summary>
		/// Toggles this tilepart's array of sprites if it's a door-part.
		/// </summary>
		/// <param name="animate">true to animate</param>
		public void SetDoorSprites(bool animate)
		{
			if (Record.UfoDoor || Record.HumanDoor)
			{
				if (animate)
				{
					if (Record.UfoDoor || _alternate == null)
					{
						Images[0] = _pckPack[Record.Image1];
						Images[1] = _pckPack[Record.Image2];
						Images[2] = _pckPack[Record.Image3];
						Images[3] = _pckPack[Record.Image4];
						Images[4] = _pckPack[Record.Image5];
						Images[5] = _pckPack[Record.Image6];
						Images[6] = _pckPack[Record.Image7];
						Images[7] = _pckPack[Record.Image8];
					}
					else
					{
						byte alt = _alternate.Record.Image1;
						for (int i = 4; i != 8; ++i)
							Images[i] = _pckPack[alt];
					}
				}
				else
				{
					for (int i = 0; i != 8; ++i)
						Images[i] = _pckPack[Record.Image1];
				}
			}
		}

		public void SetDoorToAlternateSprite()
		{
			if (Record.UfoDoor || Record.HumanDoor)
			{
				byte alt = _alternate.Record.Image1;
				for (int i = 0; i != 8; ++i)
					Images[i] = _pckPack[alt];
			}
		}
	}
}
