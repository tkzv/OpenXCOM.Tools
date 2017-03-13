using System;
using System.Drawing;

using XCom.Interfaces;


namespace XCom
{
	public class XCTile
		:
		XCom.Interfaces.Base.TileBase
	{
		private readonly PckFile _file;
		private readonly McdEntry _info;


		public XCTile(
			int id,
			PckFile file,
			McdEntry info,
			XCTile[] tiles)
			:
			base(id)
		{
			_file  = file;
			_info  = info;
			Info   = info;
			Tiles  = tiles;

			_images = new XCImage[8]; // every tile-part contains refs to 8 sprites.

			if (!info.UfoDoor && !info.HumanDoor)
				MakeAnimate();
			else
				StopAnimate();

			Dead      = null;
			Alternate = null;
		}


		public XCTile[] Tiles
		{ get; private set; }

		public XCTile Dead
		{ get; set; }

		public XCTile Alternate
		{ get; set; }

		public void MakeAnimate()
		{
			_images[0] = _file[_info.Image1];
			_images[1] = _file[_info.Image2];
			_images[2] = _file[_info.Image3];
			_images[3] = _file[_info.Image4];
			_images[4] = _file[_info.Image5];
			_images[5] = _file[_info.Image6];
			_images[6] = _file[_info.Image7];
			_images[7] = _file[_info.Image8];
		}

		public void StopAnimate()
		{
			_images[0] = _file[_info.Image1];
			_images[1] = _file[_info.Image1];
			_images[2] = _file[_info.Image1];
			_images[3] = _file[_info.Image1];
			_images[4] = _file[_info.Image1];
			_images[5] = _file[_info.Image1];
			_images[6] = _file[_info.Image1];
			_images[7] = _file[_info.Image1];
		}
	}
}
