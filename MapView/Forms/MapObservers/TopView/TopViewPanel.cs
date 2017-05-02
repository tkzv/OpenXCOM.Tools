using System;
using System.Drawing;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TopViews
{
	internal sealed class TopViewPanel
		:
			TopViewPanelParent
	{
		#region Fields & Properties
		private ColorTools _colorWest;
		private ColorTools _colorNorth;
		private ColorTools _colorContent;

		internal ToolStripMenuItem Ground
		{ get; set; }

		internal ToolStripMenuItem North
		{ get; set; }

		internal ToolStripMenuItem West
		{ get; set; }

		internal ToolStripMenuItem Content
		{ get; set; }

		internal QuadrantPanel QuadrantsPanel
		{ get; set; }
		#endregion


		#region cTor
		/// <summary>
		/// TopViewPanel cTor.
		/// </summary>
		internal TopViewPanel()
		{
			MainViewUnderlay.Instance.MainViewOverlay.MouseDragEvent += OnMouseDrag;

			(this as Control).KeyDown += OnEditKeyDown;
		}
		#endregion


		#region EventCalls
		private void OnEditKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control)
			{
				switch (e.KeyCode)
				{
					case Keys.X:
						MainViewUnderlay.Instance.MainViewOverlay.Copy();
						MainViewUnderlay.Instance.MainViewOverlay.ClearSelection();
						break;

					case Keys.C:
						MainViewUnderlay.Instance.MainViewOverlay.Copy();
						break;

					case Keys.V:
						MainViewUnderlay.Instance.MainViewOverlay.Paste();
						break;
				}
			}
			else
			{
				switch (e.KeyCode)
				{
					case Keys.Delete:
						MainViewUnderlay.Instance.MainViewOverlay.ClearSelection();
						break;
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, Border3DStyle.Etched);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			Focus(); // needed for KeyDown events.

			OnMouseDrag();

			if (e.Button == MouseButtons.Right)
				QuadrantsPanel.SetSelected(e.Button, 1);
		}
		#endregion


		#region Methods
//		internal protected override void DrawTileBlobs(
		internal void DrawTileBlobs(
				MapTileBase tile,
				Graphics g,
				int x, int y)
		{
			var mapTile = (XCMapTile)tile;

			if (_colorWest == null)
				_colorWest = new ColorTools(TopPens[TopView.WestColor]);

			if (_colorNorth == null)
				_colorNorth = new ColorTools(TopPens[TopView.NorthColor]);

			if (_colorContent == null)
				_colorContent = new ColorTools(TopBrushes[TopView.ContentColor], _colorNorth.Pen.Width);


			if (Ground.Checked && mapTile.Ground != null)
				BlobService.DrawFloor(
									g,
									TopBrushes[TopView.FloorColor],
									x, y);

			if (Content.Checked && mapTile.Content != null)
				BlobService.DrawContent(
									g,
									_colorContent,
									x, y,
									mapTile.Content);

			if (West.Checked && mapTile.West != null)
				BlobService.DrawContent(
									g,
									_colorWest,
									x, y,
									mapTile.West);

			if (North.Checked && mapTile.North != null)
				BlobService.DrawContent(
									g,
									_colorNorth,
									x, y,
									mapTile.North);
		}
		#endregion
	}
}
