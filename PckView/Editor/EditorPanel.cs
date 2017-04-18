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
		private readonly EditorPane _editorPane;
		internal EditorPane EditorPane
		{
			get { return _editorPane; }
		}


		internal EditorPanel(XCImage image)
		{
			_editorPane = new EditorPane(image);
			_editorPane.Location = new Point(0, 0);
			Controls.Add(_editorPane);
		}


		protected override void OnResize(EventArgs eventargs)
		{
			_editorPane.Size = ClientSize;
		}
	}
}
