using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using XCom.Interfaces;


namespace PckView.Forms.ImageBytes
{
	/// <summary>
	/// Creates a form that shows all the bytes in a sprite.
	/// TODO: allow this to be editable and saveable.
	/// </summary>
	internal static class BytesFormHelper
	{
		private static SelectedSprite _selected;

		private static Form fBytes;
		private static RichTextBox rtbBytes;

		internal static void ShowBytes(
				SelectedSprite selected,
				MethodInvoker formClosedCallBack,
				Point location)
		{
			_selected = selected;
			ShowBytesCore(formClosedCallBack, location);
		}

		internal static void ReloadBytes(SelectedSprite selected)
		{
			_selected = selected;
			ReloadBytesCore();
		}

		private static void ShowBytesCore(
				MethodInvoker formClosedCallBack,
				Point location)
		{
			if (fBytes != null)
			{
				fBytes.BringToFront();
			}
			else if (_selected != null)
			{
				fBytes = new Form();
				fBytes.Size = new Size(640, 480);
				fBytes.Font = new Font("Verdana", 7);

				rtbBytes = new RichTextBox();
				rtbBytes.Dock = DockStyle.Fill;
				rtbBytes.Font = new Font("Courier New", 8);
				rtbBytes.WordWrap = false;
				rtbBytes.ReadOnly = true;

				fBytes.Controls.Add(rtbBytes);

				LoadBytes();

				fBytes.FormClosing += OnFormClosing;
				fBytes.FormClosing += (sender, e) => formClosedCallBack();
				fBytes.Location = location;
				fBytes.Text = "Total Bytes - " + _selected.Image.Bindata.Length;
				fBytes.Show();
			}
		}

		private static void ReloadBytesCore()
		{
			if (fBytes != null)
			{
				if (_selected != null && _selected.Image != null)
				{
					rtbBytes.Clear();
					LoadBytes();
				}
				else
				{
					fBytes.Text   =
					rtbBytes.Text = String.Empty;
				}
			}
		}

		private static void LoadBytes()
		{
			string text = String.Empty;

			int wrapCount = 0;
			int row       = 0;

			foreach (byte b in _selected.Image.Bindata)
			{
				if (wrapCount % XCImageFile.SpriteWidth == 0)
				{
					if (++row < 10) text += " ";
					text += row + ": ";
				}

				if (b < 10)
					text += "  ";
				else if (b < 100)
					text += " ";

				text += " " + b;
				text += (++wrapCount % XCImageFile.SpriteWidth == 0) ? Environment.NewLine
																	 : " ";
			}

			rtbBytes.Text = text;
		}

		private static void OnFormClosing(object sender, CancelEventArgs e)
		{
			fBytes = null;
		}

		internal static void CloseBytes()
		{
			if (fBytes != null)
			{
				fBytes.Close();
				fBytes = null;
			}
		}
	}
}
