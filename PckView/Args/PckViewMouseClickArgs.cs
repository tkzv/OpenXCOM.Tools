using System;
using System.Windows.Forms;


namespace PckView.Args
{
	internal sealed class PckViewMouseEventArgs
		:
			MouseEventArgs
	{
		internal int Clicked
		{ get; private set; }


		internal PckViewMouseEventArgs(MouseEventArgs args, int clicked)
			:
				base(
					args.Button,
					args.Clicks,
					args.X,
					args.Y,
					args.Delta)
		{
			Clicked = clicked;
		}
	}
}
