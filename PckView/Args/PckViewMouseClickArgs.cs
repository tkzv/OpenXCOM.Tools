using System;
using System.Windows.Forms;


namespace PckView.Args
{
	internal sealed class PckViewMouseEventArgs
		:
			MouseEventArgs
	{
		public int Clicked
		{ get; private set; }


		public PckViewMouseEventArgs(MouseEventArgs args, int clicked)
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
