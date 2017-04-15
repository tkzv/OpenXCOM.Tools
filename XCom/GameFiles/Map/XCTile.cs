using System;

using XCom.Interfaces;


namespace XCom
{
	public sealed class XCTile
		:
			XCom.Interfaces.Base.TileBase
	{
		private readonly PckSpriteCollection _pckPack;

		private readonly McdRecord _record;
/*		internal McdRecord Record
		{
			get { return _record; }
		} */

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
//				XCTile[] tiles)
			:
				base(id)
		{
			_pckPack = pckPack;
			_record  = record;
			Record     = record;
//			Tiles    = tiles; // NOTE: Tiles is not used.

			Images = new XCImage[8]; // every tile-part contains refs to 8 sprites.
			InitializeSprites();

//			Dead      = null;
//			Alternate = null;
		}

		private void InitializeSprites()
		{
			if (_record.UfoDoor || _record.HumanDoor)
			{
				for (int i = 0; i != 8; ++i)
					Images[i] = _pckPack[_record.Image1];
			}
			else
			{
				Images[0] = _pckPack[_record.Image1];
				Images[1] = _pckPack[_record.Image2];
				Images[2] = _pckPack[_record.Image3];
				Images[3] = _pckPack[_record.Image4];
				Images[4] = _pckPack[_record.Image5];
				Images[5] = _pckPack[_record.Image6];
				Images[6] = _pckPack[_record.Image7];
				Images[7] = _pckPack[_record.Image8];
			}
		}


/*		private XCTile[] _tiles;
		internal XCTile[] Tiles
		{
//			get;
			set { _tiles = value; } // private.
		} */

		/// <summary>
		/// Basically this is how animations operate. For *any* animations to
		/// happen the Animation option has to be turned on. Non-door sprites
		/// always keep their array of sprites and will cycle because turning on
		/// the Animation option starts a timer. UfoDoor sprites will animate
		/// when the Animation option is on and the Doors option is turned on;
		/// but whether or not they animate is controlled by setting their
		/// sprite-arrays to either the first image or an array of images.
		/// HumanDoors, which also need the Animation option on to animate as
		/// well as the Doors option on, will cycle by flipping their sprite
		/// back and forth between their first sprite and their Alt-tile's first
		/// sprite; they stop animating by setting the entire array to their
		/// first sprite only.
		/// </summary>
		/// <param name="animate">true to animate</param>
		public void SetAnimationSprites(bool animate)
		{
			if (_record.UfoDoor || _record.HumanDoor)
			{
				if (animate)
				{
					if (_record.UfoDoor || _alternate == null)
					{
						Images[0] = _pckPack[_record.Image1];
						Images[1] = _pckPack[_record.Image2];
						Images[2] = _pckPack[_record.Image3];
						Images[3] = _pckPack[_record.Image4];
						Images[4] = _pckPack[_record.Image5];
						Images[5] = _pckPack[_record.Image6];
						Images[6] = _pckPack[_record.Image7];
						Images[7] = _pckPack[_record.Image8];
					}
					else
					{
						byte altImage = _alternate._record.Image1;
						Images[0] = _pckPack[_record.Image1];
						Images[1] = _pckPack[_record.Image1];
						Images[2] = _pckPack[_record.Image1];
						Images[3] = _pckPack[_record.Image1];
						Images[4] = _pckPack[altImage];
						Images[5] = _pckPack[altImage];
						Images[6] = _pckPack[altImage];
						Images[7] = _pckPack[altImage];
					}
				}
				else
				{
					for (int i = 0; i != 8; ++i)
						Images[i] = _pckPack[_record.Image1];
				}
			}
		}
	}
}
