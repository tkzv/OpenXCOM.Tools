using System;
using System.Drawing;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TopViews
{
	internal sealed class TopViewPanel
		:
			SimpleMapPanel
	{
		public TopViewPanel()
		{
			MapViewPanel.Instance.MapView.MouseDragEvent += OnMouseDrag;
		}


		public ToolStripMenuItem Ground
		{ get; set; }

		public ToolStripMenuItem North
		{ get; set; }

		public ToolStripMenuItem West
		{ get; set; }

		public ToolStripMenuItem Content
		{ get; set; }

		public QuadrantPanel QuadrantPanel
		{ get; set; }


		private SolidPenBrush _colorWest;
		private SolidPenBrush _colorNorth;
		private SolidPenBrush _colorContent;

		protected override void RenderCell(
				MapTileBase tile,
				Graphics g,
				int x, int y)
		{
			var mapTile = (XCMapTile)tile;

			if (mapTile.Ground != null && Ground.Checked)
				DrawService.DrawFloor(
								g,
								Brushes["GroundColor"],
								x, y);


			if (_colorNorth == null)
				_colorNorth = new SolidPenBrush(Pens["NorthColor"]);

			if (mapTile.North != null && North.Checked)
				DrawService.DrawContent(
								g,
								_colorNorth,
								x, y,
								mapTile.North);


			if (_colorWest == null)
				_colorWest = new SolidPenBrush(Pens["WestColor"]);

			if (mapTile.West != null && West.Checked)
				DrawService.DrawContent(
								g,
								_colorWest,
								x, y,
								mapTile.West);


			if (_colorContent == null)
				_colorContent = new SolidPenBrush(Brushes["ContentColor"], _colorNorth.Pen.Width);

			if (mapTile.Content != null && Content.Checked)
				DrawService.DrawContent(
								g,
								_colorContent,
								x, y,
								mapTile.Content);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, Border3DStyle.Etched);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			OnMouseDrag();

			if (e.Button == MouseButtons.Right)
				QuadrantPanel.SetSelected(e.Button, 1);
		}
	}
}
