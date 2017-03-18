using System;
using System.Windows.Forms;


namespace PckView.Args
{
	public class PckViewMouseClickArgs
		:
		MouseEventArgs
	{
		public readonly int _clicked;


		public PckViewMouseClickArgs(
				MouseEventArgs args,
				int clicked)
			:
			base(
				args.Button,
				args.Clicks,
				args.X,
				args.Y,
				args.Delta)
		{
			_clicked = clicked;
		}
	}
}
