using System;
using System.Drawing;

using XCom;


namespace MapView
{
	internal class CursorSprite
	{
/*		private enum CursorState
		{
			Standard,
			Aim,
			MindControl,
			Waypoint,
			Throw
		}; */


		#region Fields
//		private CursorState _state;
		private readonly PckSpriteCollection _pckPack;
		#endregion


		#region cTor
		/// <summary>
		/// Constructs a CursorSprite.
		/// The CursorSprite is the actual cuboid selector in XCOM resources.
		/// </summary>
		/// <param name="pckPack"></param>
		internal CursorSprite(PckSpriteCollection pckPack)
		{
//			_state = CursorState.Standard;
			_pckPack = pckPack;

			foreach (PckImage image in pckPack)
				image.Image.MakeTransparent(pckPack.Pal.Transparent);
		}
		#endregion


		#region Methods
		/// <summary>
		/// Draws the back half of the cuboid.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
//		/// <param name="over"></param>
		/// <param name="topLevel"></param>
		/// <param name="halfWidth"></param>
		/// <param name="halfHeight"></param>
		internal void DrawCursorBack(
				Graphics g,
				int x, int y,
//				bool over, // always false.
				bool topLevel,
				int halfWidth,
				int halfHeight)
		{
			int id = (topLevel) ? 0
								: 2;
/*			int id = 2;
			if (topLevel)// && _state != CursorState.Aim)
			{
//				id = (over) ? 1 : 0;
				id = 0;
			} */

			var image = _pckPack[id].Image;
			g.DrawImage(
					image,
					x, y,
					halfWidth  * 2, // NOTE: the values for width and height are based on a sprite that's 32x40.
					halfHeight * 5);
//					(int)(image.Width  * Globals.Scale),
//					(int)(image.Height * Globals.Scale));
		}

		/// <summary>
		/// Draws the front half of the cuboid.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
//		/// <param name="aniStep"></param>
//		/// <param name="over"></param>
		/// <param name="topLevel"></param>
		/// <param name="halfWidth"></param>
		/// <param name="halfHeight"></param>
		internal void DrawCursorFront(
				Graphics g,
				int x, int y,
//				int aniStep,
//				bool over, // always false.
				bool topLevel,
				int halfWidth,
				int halfHeight)
		{
			int id = (topLevel) ? 3
								: 5;

/*			int id = 5;
			if (topLevel)
			{
				switch (_state)
				{
					case CursorState.Aim:
						id = 7 + aniStep % 4;
						break;
					case CursorState.MindControl:
						id = 11 + aniStep % 2;
						break;
					case CursorState.Throw:
						id = 15 + aniStep % 2;
						break;
					case CursorState.Waypoint:
						id = 13 + aniStep % 2;
						break;
					default:
//						id = (over) ? 4 : 3;
						id = 3;
						break;
				}
			} */

			var image = _pckPack[id].Image;
			g.DrawImage(
					image,
					x, y,
					halfWidth  * 2, // NOTE: the values for width and height are based on a sprite that's 32x40.
					halfHeight * 5);
//					(int)(image.Width  * Globals.Scale),
//					(int)(image.Height * Globals.Scale));
		}
		#endregion
	}
}

//		public CursorState State
//		{
//			get { return _state; }
//			set { _state = value; }
//		}
//		public PckSpriteCollection PckCursorSprites
//		{
//			get { return _file; }
//		}
