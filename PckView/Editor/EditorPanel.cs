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
		private readonly EditorPane editor;
		public EditorPane Editor
		{
			get { return editor; }
		}


		public EditorPanel(XCImage img)
		{
			editor = new EditorPane(img);
			editor.Location = new Point(0, 0);
			Controls.Add(editor);
		}


		protected override void OnResize(EventArgs eventargs)
		{
			editor.Size = ClientSize;
		}
	}
}
