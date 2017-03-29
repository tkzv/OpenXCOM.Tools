using System;
using System.Drawing;

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

			if (!record.UfoDoor && !record.HumanDoor)
				MakeAnimate();
			else
				StopAnimate();

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

		public void MakeAnimate()
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

		public void StopAnimate()
		{
			Images[0] = _pckPack[_record.Image1];
			Images[1] = _pckPack[_record.Image1];
			Images[2] = _pckPack[_record.Image1];
			Images[3] = _pckPack[_record.Image1];
			Images[4] = _pckPack[_record.Image1];
			Images[5] = _pckPack[_record.Image1];
			Images[6] = _pckPack[_record.Image1];
			Images[7] = _pckPack[_record.Image1];
		}
	}
}
