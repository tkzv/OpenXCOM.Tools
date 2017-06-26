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
	internal static class SpriteBytesManager
	{
		#region Fields (static)
		private static Form _fBytes;
		private static RichTextBox _rtbBytes;

		private static XCImage _sprite;
		#endregion


		#region Methods (static)
		/// <summary>
		/// Instantiates the bytes-table as a Form.
		/// </summary>
		/// <param name="sprite">the sprite whose bytes to display</param>
		/// <param name="formClosedCallBack">function pointer that unchecks the
		/// menuitem in PckViewForm</param>
		internal static void LoadBytesTable(
				XCImage sprite,
				MethodInvoker formClosedCallBack)
		{
			_sprite = sprite;

			if (_fBytes == null)
			{
				_fBytes = new Form();
				_fBytes.Size = new Size(960, 620);
				_fBytes.Font = new Font("Verdana", 7);
				_fBytes.Text = "Bytes Table";
				_fBytes.FormClosing += (sender, e) => formClosedCallBack();
				_fBytes.FormClosing += OnFormClosing;

				_rtbBytes = new RichTextBox();
				_rtbBytes.Dock = DockStyle.Fill;
				_rtbBytes.Font = new Font("Courier New", 8);
				_rtbBytes.WordWrap = false;
				_rtbBytes.ReadOnly = true;

				_fBytes.Controls.Add(_rtbBytes);
			}

			PrintBytesTable();
			_fBytes.Show();
		}

		/// <summary>
		/// Loads new sprite information when the table is already open/visible.
		/// </summary>
		/// <param name="sprite"></param>
		internal static void ReloadBytesTable(XCImage sprite)
		{
			_sprite = sprite;

			if (_fBytes != null && _fBytes.Visible)
			{
				if (_sprite != null)
				{
					PrintBytesTable();
				}
				else
					_rtbBytes.Clear();
			}
		}

		private static void PrintBytesTable()
		{
			string text = String.Empty;

			int wrapCount = 0;
			int row       = 0;

			foreach (byte b in _sprite.Bindata)
			{
				if (wrapCount % XCImage.SpriteWidth == 0)
				{
					if (++row < 10) text += " ";
					text += row + ":";
				}

				if (b < 10)
					text += "  ";
				else if (b < 100)
					text += " ";

				text += " " + b;

				if (++wrapCount % XCImage.SpriteWidth == 0)
					text += Environment.NewLine;
			}
			_rtbBytes.Text = text;
		}

		/// <summary>
		/// Hides or closes the bytes-table.
		/// </summary>
		/// <param name="close">true to close, else hide</param>
		internal static void HideBytesTable(bool close)
		{
			if (close)
				_fBytes.Close();
			else
				_fBytes.Hide();
		}
		#endregion


		#region Eventcalls (static)
		private static void OnFormClosing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			_fBytes.Hide();
		}
		#endregion
	}
}
