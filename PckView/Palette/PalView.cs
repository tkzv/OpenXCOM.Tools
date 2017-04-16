using System;
using System.Drawing;

using XCom;


namespace PckView
{
	/// <summary>
	/// Summary description for PalView.
	/// </summary>
	internal sealed class PalView
		:
			System.Windows.Forms.Form
	{
		internal event PaletteClickEventHandler PaletteIndexChanged;


		private PalPanel _palPanel;
		private System.Windows.Forms.Label _status;


		internal PalView()
		{
			InitializeComponent();
			OnResize(null); // TODO: Fix "Virtual member call in a constructor."
		}


		private void palClick(int id)
		{
			switch (_palPanel.Mode)
			{
				case SelectMode.Single:
					_status.Text = string.Format(
											System.Globalization.CultureInfo.CurrentCulture,
											"Clicked index: {0} ({1:X})",
											id, id);
					break;

				case SelectMode.Bar:
					_status.Text = "Clicked range: " + id + " - " + (id + PalPanel.Across - 1);
					break;
			}

			Color color = _palPanel.Palette[id];
			_status.Text += string.Format(
									System.Globalization.CultureInfo.CurrentCulture,
									" r:{0} g:{1} b:{2} a:{3}",
									color.R,
									color.G,
									color.B,
									color.A);

			if (PaletteIndexChanged != null)
				PaletteIndexChanged(id);
		}

		internal Palette Palette
		{
//			get { return _palPanel.Palette; }
			set { _palPanel.Palette = value; }
		}

		protected override void OnResize(EventArgs e)
		{
			if (_palPanel != null)
			{
				_palPanel.Width  = ClientSize.Width;
				_palPanel.Height = ClientSize.Height - _status.Height;

				_status.Location = new Point(
										_palPanel.Left,
										_palPanel.Bottom);

				_status.Width = ClientSize.Width;
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

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._status = new System.Windows.Forms.Label();
			this._palPanel = new PckView.PalPanel();
			this.SuspendLayout();
			//
			// status
			//
			this._status.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._status.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._status.Location = new System.Drawing.Point(0, 237);
			this._status.Name = "status";
			this._status.Size = new System.Drawing.Size(292, 16);
			this._status.TabIndex = 0;
			//
			// palPanel
			//
			this._palPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._palPanel.Name = "palPanel";
			this._palPanel.Size = new System.Drawing.Size(292, 237);
			this._palPanel.TabIndex = 0;
			this._palPanel.PaletteIndexChanged += this.palClick;
			//
			// PalView
			//
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 253);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																this._palPanel,
																this._status });
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PalView";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "PalView";
			this.ResumeLayout(false);
		}
		#endregion
	}
}
