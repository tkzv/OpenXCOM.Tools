using System;
using System.Drawing;

using XCom.Interfaces;


namespace XCom
{
	public class XCTile
		:
			XCom.Interfaces.Base.TileBase
	{
		private readonly PckSpriteCollection _pckPack;
		private readonly McdEntry _info;


		internal XCTile(
				int id,
				PckSpriteCollection pckPack,
				McdEntry info,
				XCTile[] tiles)
			:
				base(id)
		{
			_pckPack = pckPack;
			_info = info;
			Info  = info;
			Tiles = tiles;

			Images = new XCImage[8]; // every tile-part contains refs to 8 sprites.

			if (!info.UfoDoor && !info.HumanDoor)
				MakeAnimate();
			else
				StopAnimate();

//			Dead      = null;
//			Alternate = null;
		}


		internal XCTile[] Tiles
		{ get; private set; }

		public XCTile Dead
		{ get; set; }

		internal XCTile Alternate
		{ get; set; }

		public void MakeAnimate()
		{
			Images[0] = _pckPack[_info.Image1];
			Images[1] = _pckPack[_info.Image2];
			Images[2] = _pckPack[_info.Image3];
			Images[3] = _pckPack[_info.Image4];
			Images[4] = _pckPack[_info.Image5];
			Images[5] = _pckPack[_info.Image6];
			Images[6] = _pckPack[_info.Image7];
			Images[7] = _pckPack[_info.Image8];
		}

		public void StopAnimate()
		{
			Images[0] = _pckPack[_info.Image1];
			Images[1] = _pckPack[_info.Image1];
			Images[2] = _pckPack[_info.Image1];
			Images[3] = _pckPack[_info.Image1];
			Images[4] = _pckPack[_info.Image1];
			Images[5] = _pckPack[_info.Image1];
			Images[6] = _pckPack[_info.Image1];
			Images[7] = _pckPack[_info.Image1];
		}
	}
}
