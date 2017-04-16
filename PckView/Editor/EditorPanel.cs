using System;
using System.Drawing;
using System.Windows.Forms;

using XCom.Interfaces;


namespace PckView
{
	internal sealed class EditorPanel
		:
			Panel
	{
		private readonly EditorPane _editPane;
		internal EditorPane Editor
		{
			get { return _editPane; }
		}


		internal EditorPanel(XCImage img)
		{
			_editPane = new EditorPane(img);
			_editPane.Location = new Point(0, 0);
			Controls.Add(_editPane);
		}


		protected override void OnResize(EventArgs eventargs)
		{
			_editPane.Size = ClientSize;
		}
	}
}
