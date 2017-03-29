using System;
using System.Drawing;

using XCom;


namespace MapView
{
	internal enum CursorState
	{
		Select,
		Aim,
		SelectMControl,
		Waypoint,
		Throw
	};


	internal class CursorSprite
	{
		private CursorState _state;
		private readonly PckSpriteCollection _pckPack;


		public CursorSprite(PckSpriteCollection pckPack)
		{
			_state = CursorState.Select;
			_pckPack = pckPack;

			foreach (PckImage image in pckPack)
				image.Image.MakeTransparent(pckPack.Pal.Transparent);
		}


/*		public CursorState State
		{
			get { return _state; }
			set { _state = value; }
		} */

/*		public PckSpriteCollection PckSpriteCollection
		{
			get { return _file; }
		} */

		public void DrawHigh(
				Graphics g,
				int x, int y,
				bool over,
				bool top)
		{
			Bitmap image;
			if (top && _state != CursorState.Aim)
			{
				image = (over) ? _pckPack[1].Image
							   : _pckPack[0].Image;
			}
			else
				image = _pckPack[2].Image;

			g.DrawImage(
					image,
					x, y,
					(int)(image.Width  * Globals.PckImageScale),
					(int)(image.Height * Globals.PckImageScale));
		}

		public void DrawLow(
				Graphics g,
				int x, int y,
				int i,
				bool over,
				bool top)
		{
			Bitmap image;

			if (top)
			{
				switch (_state)
				{
					case CursorState.Aim:
						image = _pckPack[7 + i % 4].Image;
						break;

					case CursorState.SelectMControl:
						image = _pckPack[11 + i % 2].Image;
						break;

					case CursorState.Throw:
						image = _pckPack[15 + i % 2].Image;
						break;

					case CursorState.Waypoint:
						image = _pckPack[13 + i % 2].Image;
						break;

					default:
						image = (over) ? _pckPack[4].Image
									   : _pckPack[3].Image;
						break;
				}
			}
			else
				image = _pckPack[5].Image;

			g.DrawImage(
					image,
					x, y,
					(int)(image.Width  * Globals.PckImageScale),
					(int)(image.Height * Globals.PckImageScale));
		}
	}
}
