using System;
using System.Windows.Forms;

using DSShared.Windows;


namespace XCom
{
	public sealed partial class ConsoleForm
		:
		Form
	{
		public ConsoleForm()
		{
			InitializeComponent();

			XConsole.Init(100);
			XConsole.BufferChanged += xConsole_BufferChanged; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			new RegistryInfo(this); // <- looks like this writes some registry entries ....

			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
		}


		private void xConsole_BufferChanged(Zerg zerg)
		{
			string buffer = zerg.ZergBull + Environment.NewLine;
			Zerg zerg1 = zerg.Post;

			while (zerg != zerg1)
			{
				buffer += zerg1.ZergBull + Environment.NewLine;
				zerg1 = zerg1.Post;
			}
			consoleText.Text = buffer;

			Refresh();
		}

		private void miClose_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
