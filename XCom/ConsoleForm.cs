using System;
using System.Windows.Forms;

using DSShared.Windows;


namespace XCom
{
	public partial class ConsoleForm
		:
		Form
	{
		public ConsoleForm()
		{
			InitializeComponent();

			XCom.xConsole.Init(100);
			XCom.xConsole.BufferChanged += xConsole_BufferChanged; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			new RegistryInfo(this); // <- looks like this writes some registry entries ....

			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
		}


		private void xConsole_BufferChanged(Node current)
		{
			string buffer = current._st + "\n";
			Node curr = current._next;

			while (current != curr)
			{
				buffer = buffer + curr._st + "\n";
				curr = curr._next;
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
