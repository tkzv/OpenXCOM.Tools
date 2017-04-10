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
			Info     = record;
//			Tiles    = tiles; // NOTE: Tiles is not used.

			Images = new XCImage[8]; // every tile-part contains refs to 8 sprites.
			SetAnimationSprites(!record.UfoDoor);// && !record.HumanDoor);	// NOTE: Option to animate Ufo doors is different than
																			// the Option for general animations.
//			Dead      = null;
//			Alternate = null;
		}


/*		private XCTile[] _tiles;
		internal XCTile[] Tiles
		{
//			get;
			set { _tiles = value; } // private.
		} */

		public XCTile Dead
		{ get; set; }

/*		private XCTile _tile;
		internal XCTile Alternate // NOTE: the Alternate tile is not used.
		{
//			get { return _tile; }
			set { _tile = value; }
		} */

		public void SetAnimationSprites(bool animate)
		{
			if (animate)
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
				for (int i = 0; i != 8; ++i)
					Images[i] = _pckPack[_record.Image1];
			}
		}
	}
}
