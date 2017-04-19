using System;
using System.Drawing;

using XCom;


namespace PckView
{
	/// <summary>
	/// PaletteView form.
	/// </summary>
	internal sealed class PaletteView
		:
			System.Windows.Forms.Form
	{
//		internal event PaletteIndexChangedEventHandler PaletteIndexChangedEvent;


		private PalettePanel _pPalette;


		internal PaletteView()
		{
			InitializeComponent();

			OnResize(null);
		}


		private void OnPaletteIndexChanged(int id)
		{
			string text = string.Format(
									System.Globalization.CultureInfo.CurrentCulture,
									"id:{0} (0x{0:X2})",
									id);

			Color color = _pPalette.Palette[id];
			text += string.Format(
								System.Globalization.CultureInfo.CurrentCulture,
								" r:{0} g:{1} b:{2} a:{3}",
								color.R,
								color.G,
								color.B,
								color.A);

			lblStatus.Text = text;

//			if (PaletteIndexChangedEvent != null)
//				PaletteIndexChangedEvent(id);
		}

		internal Palette Palette
		{
			set { _pPalette.Palette = value; }
		}

		protected override void OnResize(EventArgs e)
		{
			if (_pPalette != null)
			{
				_pPalette.Width  = ClientSize.Width;
				_pPalette.Height = ClientSize.Height - lblStatus.Height;

				lblStatus.Location = new Point(
											_pPalette.Left,
											_pPalette.Bottom);

				lblStatus.Width = ClientSize.Width;
			}
		}


		#region Windows Form Designer generated code

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}


		// This will get deleted from InitializeComponent() when any changes are
		// made in the designer ... and trying to make it stick with default
		// initialization doesn't work either. So copy it back in at the top of
		// InitializeComponent() after making changes in the designer.

//			this._pPalette = new PalettePanel();

		// And this will probably get deleted also:

//			this._pPalette.PaletteIndexChangedEvent += OnPaletteIndexChanged;


		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._pPalette = new PalettePanel();
			this.lblStatus = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _pPalette
			// 
			this._pPalette.Dock = System.Windows.Forms.DockStyle.Fill;
			this._pPalette.Location = new System.Drawing.Point(0, 0);
			this._pPalette.Name = "_pPalette";
			this._pPalette.Size = new System.Drawing.Size(292, 260);
			this._pPalette.TabIndex = 0;
			this._pPalette.PaletteIndexChangedEvent += OnPaletteIndexChanged;
			// 
			// lblStatus
			// 
			this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lblStatus.Location = new System.Drawing.Point(0, 260);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(292, 14);
			this.lblStatus.TabIndex = 0;
			// 
			// PaletteView
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(292, 274);
			this.Controls.Add(this._pPalette);
			this.Controls.Add(this.lblStatus);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PaletteView";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "PalView";
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Label lblStatus;
	}
}
