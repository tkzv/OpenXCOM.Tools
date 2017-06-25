using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using XCom.Interfaces;


namespace PckView.Forms.SpriteBytes
{
	/// <summary>
	/// Creates a form that shows all the bytes in a sprite.
	/// TODO: allow this to be editable and saveable.
	/// </summary>
	internal static class BytesFormHelper
	{
		#region Fields (static)
		private static XCImage _sprite;

		private static Form fBytes;
		private static RichTextBox rtbBytes;
		#endregion


		#region Eventcalls (static)
		private static void OnFormClosing(object sender, CancelEventArgs e)
		{
			fBytes = null;
		}
		#endregion


		#region Methods (static)
		internal static void ShowBytes(
				XCImage sprite,
				MethodInvoker formClosedCallBack)
		{
			_sprite = sprite;
			ShowBytesCore(formClosedCallBack);
		}

		internal static void ReloadBytes(XCImage sprite)
		{
			_sprite = sprite;
			ReloadBytesCore();
		}

		private static void ShowBytesCore(MethodInvoker formClosedCallBack)
		{
			if (fBytes != null)
			{
				fBytes.BringToFront();
			}
			else if (_sprite != null)
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
				fBytes.Text = "Total Bytes - " + _sprite.Bindata.Length;
				fBytes.Show();
			}
		}

		private static void ReloadBytesCore()
		{
			if (fBytes != null)
			{
				if (_sprite != null)
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

			foreach (byte b in _sprite.Bindata)
			{
				if (wrapCount % XCImage.SpriteWidth == 0)
				{
					if (++row < 10) text += " ";
					text += row + ": ";
				}

				if (b < 10)
					text += "  ";
				else if (b < 100)
					text += " ";

				text += " " + b;
				text += (++wrapCount % XCImage.SpriteWidth == 0) ? Environment.NewLine
																 : " ";
			}

			rtbBytes.Text = text;
		}

		internal static void CloseBytes()
		{
			if (fBytes != null)
			{
				fBytes.Close();
//				fBytes.Hide(); // TODO: implement that.
				fBytes = null;
			}
		}
		#endregion
	}
}
