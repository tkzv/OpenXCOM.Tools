using System;


namespace PckView
{
	internal sealed partial class Help
		:
			System.Windows.Forms.Form
	{
		internal Help()
		{
			InitializeComponent();

			lblHelp.Text = GetHelpText();
		}

		private static string GetHelpText()
		{
			return "sry. No help available atm"
					+ Environment.NewLine + Environment.NewLine
				+ "kL_note: PckView presently allows only the viewing of PCK files."
					+ Environment.NewLine + Environment.NewLine
				+ "While the sprites and palettes can be viewed in detail, "
				+ "neither editing nor the saving of anything is currently allowed."
					+ Environment.NewLine + Environment.NewLine
				+ "ps, note that entries #254 and #255 of the palettes isn't "
				+ "standard, eg.";

//			return
//				"Right-click an image to save/replace/delete/etc individual images."
//					+ Environment.NewLine + Environment.NewLine
//				+ "To add new images, add them to the blank space at the bottom."
//					+ Environment.NewLine + Environment.NewLine
//				+ "When editing BMP files, DO NOT add any colors that are not " +
//					"part of the palette. The game works on palettes, which means " +
//					"for any given image, only 256 colors can be used. The colors " +
//					"have been encoded into each BMP that gets saved, so use only " +
//					"those colors." + Environment.NewLine + Environment.NewLine
//				+ "If you find a bug in the program, and can reproduce it reliably, " +
//					"clone the repo at GitHub and fix it.";
		}
	}
}
