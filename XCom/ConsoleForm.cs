using System;
using System.Windows.Forms;

using DSShared.Windows;


namespace XCom
{
	public sealed partial class ConsoleForm
		:
			Form
	{
		internal ConsoleForm()
		{
			InitializeComponent();

			XConsole.Init(100);
			XConsole.BufferChanged += OnBufferChanged; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			var regInfo = new RegistryInfo(this, "Console"); // <- looks like this writes some registry entries ....

			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
		}


		private void OnBufferChanged(Zerg zerg)
		{
			string buffer = zerg.ZergBull + Environment.NewLine;
			Zerg zerg1 = zerg.Post;

			while (zerg != zerg1)
			{
				buffer += zerg1.ZergBull + Environment.NewLine;
				zerg1 = zerg1.Post;
			}
			rtbConsole.Text = buffer;

			Refresh();
		}

		private void OnCloseClick(object sender, EventArgs e)
		{
			Close();
		}
	}
}
