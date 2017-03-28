using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using MapView.Forms.MapObservers.RouteViews;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TopViews
{
	public class SimpleMapPanel
		:
		MapObserverControl1
	{
		private int _offX = 0;
		private int _offY = 0;

		private readonly GraphicsPath _cell;
		private readonly GraphicsPath _copy;
		private readonly GraphicsPath _sel;

		private int _mR = -1;
		private int _mC = -1;

		private DrawContentService _drawService = new DrawContentService();
		protected DrawContentService DrawService
		{
			get { return _drawService; }
		}

		private int _heightMin = 4;
		public int MinHeight
		{
			get { return _heightMin; }
			set
			{
				_heightMin = value;
				ParentSize(Width, Height);
			}
		}


		public SimpleMapPanel()
		{
			_cell = new GraphicsPath();
			_sel  = new GraphicsPath();
			_copy = new GraphicsPath();
		}


		public void ParentSize(int width, int height)
		{
			if (Map != null)
			{
				var hWidth  = _drawService.HalfWidth;
				var hHeight = _drawService.HalfHeight;
				
				int curWidth = hWidth;

				if (Map.MapSize.Rows > 0 || Map.MapSize.Cols > 0)
				{
					if (height > width / 2) // use width
					{
						hWidth = width / (Map.MapSize.Rows + Map.MapSize.Cols);

						if (hWidth % 2 != 0)
							--hWidth;

						hHeight = hWidth / 2;
					}
					else // use height
					{
						hHeight = height / (Map.MapSize.Rows + Map.MapSize.Cols);
						hWidth = hHeight * 2;
					}
				}

				if (hHeight < _heightMin)
				{
					hWidth  = _heightMin * 2;
					hHeight = _heightMin;
				}

				_drawService.HalfWidth  = hWidth;
				_drawService.HalfHeight = hHeight;

				_offX = 4 + Map.MapSize.Rows * hWidth;
				_offY = 4;

				if (curWidth != hWidth)
				{
					Width  = 8 + (Map.MapSize.Rows + Map.MapSize.Cols) * hWidth;
					Height = 8 + (Map.MapSize.Rows + Map.MapSize.Cols) * hHeight;

					Refresh();
				}
			}
		}

		[Browsable(false), DefaultValue(null)] // NOTE: this is only for the designer, it doesn't actually set the default-value.
		public override IMapBase Map
		{
			set
			{
				base.Map = value;
				_drawService.HalfWidth = 7;
				ParentSize(Parent.Width, Parent.Height);

				Refresh();
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			SetSelectionRect();
		}

		protected void ViewDrag(object sender, EventArgs e)
		{
			SetSelectionRect();
		}

		private void SetSelectionRect()
		{
			var start = GetDragStart();
			var end   = GetDragEnd();

			var hWidth  = _drawService.HalfWidth;
			var hHeight = _drawService.HalfHeight;

			var sel1 = new Point(
							_offX + (start.X - start.Y) * hWidth,
							_offY + (start.X + start.Y) * hHeight);
			var sel2 = new Point(
							_offX + (end.X - start.Y) * hWidth + hWidth,
							_offY + (end.X + start.Y) * hHeight + hHeight);
			var sel3 = new Point(
							_offX + (end.X - end.Y) * hWidth,
							_offY + (end.X + end.Y) * hHeight + hHeight + hHeight);
			var sel4 = new Point(
							_offX + (start.X - end.Y) * hWidth - hWidth,
							_offY + (start.X + end.Y) * hHeight + hHeight);

			_copy.Reset();
			_copy.AddLine(sel1, sel2);
			_copy.AddLine(sel2, sel3);
			_copy.AddLine(sel3, sel4);
			_copy.CloseFigure();

			Refresh();
		}

		private static Point GetDragStart()
		{
			var start = new Point(0, 0);
			start.X = Math.Min(
							MapViewPanel.Instance.MapView.DragStart.X,
							MapViewPanel.Instance.MapView.DragEnd.X);
			start.Y = Math.Min(
							MapViewPanel.Instance.MapView.DragStart.Y,
							MapViewPanel.Instance.MapView.DragEnd.Y);
			return start;
		}

		private static Point GetDragEnd()
		{
			var end = new Point(0, 0);
			end.X = Math.Max(
						MapViewPanel.Instance.MapView.DragStart.X,
						MapViewPanel.Instance.MapView.DragEnd.X);
			end.Y = Math.Max(
						MapViewPanel.Instance.MapView.DragStart.Y,
						MapViewPanel.Instance.MapView.DragEnd.Y);
			return end;
		}

		[Browsable(false), DefaultValue(null)] // NOTE: DefaultValue has meaning only for the designer. Fortunately the default value of the class variable *is* null.
		public Dictionary<string, SolidBrush> Brushes
		{ get; set; }

		[Browsable(false), DefaultValue(null)] // NOTE: DefaultValue has meaning only for the designer. Fortunately the default value of the class variable *is* null.
		public Dictionary<string, Pen> Pens
		{ get; set; }

		public override void SelectedTileChanged(IMapBase sender, SelectedTileChangedEventArgs e)
		{
			MapLocation pt = e.MapPosition;
//			Text = "c: " + pt.Col + " r: " + pt.Row; // I don't think this actually prints anywhere.

			var hWidth  = _drawService.HalfWidth;
			var hHeight = _drawService.HalfHeight;

			int xc = (pt.Col - pt.Row) * hWidth;
			int yc = (pt.Col + pt.Row) * hHeight;

			_sel.Reset();
			_sel.AddLine(
					xc, yc,
					xc + hWidth, yc + hHeight);
			_sel.AddLine(
					xc + hWidth, yc + hHeight,
					xc, yc + 2 * hHeight);
			_sel.AddLine(
					xc, yc + 2 * hHeight,
					xc - hWidth, yc + hHeight);
			_sel.CloseFigure();

			ViewDrag(null, null);

			Refresh();
		}

		protected virtual void RenderCell(
				MapTileBase tile,
				Graphics g,
				int x, int y)
		{}

		protected override void Render(Graphics backBuffer)
		{
			backBuffer.FillRectangle(SystemBrushes.Control, ClientRectangle);

			var hWidth  = _drawService.HalfWidth;
			var hHeight = _drawService.HalfHeight;

			if (Map != null)
			{
				for (int
						r = 0, startX = _offX, startY = _offY;
						r != Map.MapSize.Rows;
						++r, startX -= hWidth, startY += hHeight)
				{
					for (int
							c = 0, x = startX, y = startY;
							c != Map.MapSize.Cols;
							++c, x += hWidth, y += hHeight)
					{
						var mapTile = Map[r, c] as MapTileBase;
						if (mapTile != null)
							RenderCell(mapTile, backBuffer, x, y);
					}
				}

				for (int i = 0; i <= Map.MapSize.Rows; ++i)
					backBuffer.DrawLine(
									Pens["GridColor"],
									_offX - i * hWidth,
									_offY + i * hHeight,
									(Map.MapSize.Cols - i) * hWidth  + _offX,
									(Map.MapSize.Cols + i) * hHeight + _offY);

				for (int i = 0; i <= Map.MapSize.Cols; ++i)
					backBuffer.DrawLine(
									Pens["GridColor"],
									_offX + i * hWidth,
									_offY + i * hHeight,
									i * hWidth  - Map.MapSize.Rows * hWidth  + _offX,
									i * hHeight + Map.MapSize.Rows * hHeight + _offY);

				if (_copy != null)
					backBuffer.DrawPath(Pens["SelectColor"], _copy);

//				if (selected != null) // clicked on
//					backBuffer.DrawPath(new Pen(Brushes.Blue, 2), selected);

				if (   _mR > -1
					&& _mC > -1
					&& _mR < Map.MapSize.Rows
					&& _mC < Map.MapSize.Cols)
				{
					int x = (_mC - _mR) * hWidth  + _offX;
					int y = (_mC + _mR) * hHeight + _offY;

					var selPath = CellPath(x, y);
					backBuffer.DrawPath(Pens["MouseColor"], selPath);
				}
			}
		}

		private GraphicsPath CellPath(int x, int y)
		{
			var hWidth  = _drawService.HalfWidth;
			var hHeight = _drawService.HalfHeight ;

			_cell.Reset();
			_cell.AddLine(
						x, y,
						x + hWidth, y + hHeight);
			_cell.AddLine(
						x + hWidth, y + hHeight,
						x, y + 2 * hHeight);
			_cell.AddLine(
						x, y + 2 * hHeight,
						x - hWidth, y + hHeight);
			_cell.CloseFigure();

			return _cell;
		}

		private Point ConvertCoordsDiamond(int x, int y)
		{
			// 16 is half the width of the diamond
			// 24 is the distance from the top of the diamond to the very top of the image

			var hWidth  = (double)_drawService.HalfWidth;
			var hHeight = (double)_drawService.HalfHeight;

			double x1 =  (x          / (hWidth * 2)) + (y / (hHeight * 2));
			double x2 = -(x - y * 2) / (hWidth * 2);

			return new Point(
						(int)Math.Floor(x1),
						(int)Math.Floor(x2));
		}

		private bool _mDown = false;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (Map != null)
			{
				var pt = ConvertCoordsDiamond(
											e.X - _offX,
											e.Y - _offY);
				Map.SelectedTile = new MapLocation(
												pt.Y,
												pt.X,
												Map.CurrentHeight);

				_mDown = true;
				MapViewPanel.Instance.MapView.SetDrag(pt, pt);
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			_mDown = false;
			MapViewPanel.Instance.MapView.Refresh();

			Refresh();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			var pt = ConvertCoordsDiamond(
										e.X - _offX,
										e.Y - _offY);
			if (pt.X != _mC || pt.Y != _mR)
			{
				_mC = pt.X;
				_mR = pt.Y;

				if (_mDown)
					MapViewPanel.Instance.MapView.SetDrag(
													MapViewPanel.Instance.MapView.DragStart,
													pt);

				Refresh(); // mouseover refresh for TopView.
			}
		}

/*		/// <summary>
		/// Scrolls the z-axis for TopRouteView. Sort of .... no, well no it doesn't.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if		(e.Delta < 0) base.Map.Up();
			else if	(e.Delta > 0) base.Map.Down();
		} */
	}
}
