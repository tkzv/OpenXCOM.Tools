using System;
using System.Drawing;

using XCom;


namespace MapView
{
	public enum CursorState
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
		private readonly PckFile _file;


		public CursorSprite(PckFile file)
		{
			_state = CursorState.Select;
			_file = file;

			foreach (PckImage image in file)
				image.Image.MakeTransparent(file.Pal.Transparent);
		}


		/*		public CursorState State
		{
			get { return _state; }
			set { _state = value; }
		} */

/*		public PckFile PckFile
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
				image = (over) ? _file[1].Image
							   : _file[0].Image;
			}
			else
			{
				image = _file[2].Image;
			}

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

			if (top && _state != CursorState.Aim)
			{
				image = (over) ? _file[4].Image
							   : _file[3].Image;

				switch (_state)
				{
					case CursorState.SelectMControl:
						image = _file[11 + i % 2].Image;
						break;

					case CursorState.Throw:
						image = _file[15 + i % 2].Image;
						break;

					case CursorState.Waypoint:
						image = _file[13 + i % 2].Image;
						break;
				}
			}
			else if (top) // top and aim
			{
				image = _file[7 + i % 4].Image;
			}
			else
			{
				image = _file[5].Image;
			}

			g.DrawImage(
					image,
					x, y,
					(int)(image.Width  * Globals.PckImageScale),
					(int)(image.Height * Globals.PckImageScale));
		}
	}
}
