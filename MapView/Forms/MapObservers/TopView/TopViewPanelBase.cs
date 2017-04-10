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
	/// <summary>
	/// A base class for TopViewPanel.
	/// </summary>
	internal class TopViewPanelBase
		:
			MapObserverControl1
	{
		private int _offX;
		private int _offY;

		private readonly GraphicsPath _lozSelector;
		private readonly GraphicsPath _lozSelected;
//		private readonly GraphicsPath _lozSel;

		private int _mouseRow = -1;
		private int _mouseCol = -1;

		private DrawContentService _drawService = new DrawContentService();
		internal protected DrawContentService DrawService
		{
			get { return _drawService; }
		}

		private int _heightMin = 4;
		internal int MinHeight
		{
			get { return _heightMin; }
			set
			{
				_heightMin = value;
				HandleParentResize(Width, Height);
			}
		}


		internal protected TopViewPanelBase()
		{
			_lozSelector = new GraphicsPath();
			_lozSelected = new GraphicsPath();
//			_lozSel = new GraphicsPath();
		}


		[Browsable(false), DefaultValue(null)]
		public override IMapBase BaseMap
		{
			set
			{
				base.BaseMap = value;
				_drawService.HalfWidth = 7; // TODO: 7 ... inits to 8 in DrawContentService.
				HandleParentResize(Parent.Width, Parent.Height);

				Refresh();
			}
		}

		internal void HandleParentResize(int width, int height)
		{
			if (BaseMap != null)
			{
				var hWidth  = _drawService.HalfWidth;
				var hHeight = _drawService.HalfHeight;
				
				int curWidth = hWidth;

				if (BaseMap.MapSize.Rows > 0 || BaseMap.MapSize.Cols > 0)
				{
					if (height > width / 2) // use width
					{
						hWidth = width / (BaseMap.MapSize.Rows + BaseMap.MapSize.Cols);

						if (hWidth % 2 != 0)
							--hWidth;

						hHeight = hWidth / 2;
					}
					else // use height
					{
						hHeight = height / (BaseMap.MapSize.Rows + BaseMap.MapSize.Cols);
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

				_offX = 4 + BaseMap.MapSize.Rows * hWidth;
				_offY = 4;

				if (curWidth != hWidth)
				{
					Width  = 8 + (BaseMap.MapSize.Rows + BaseMap.MapSize.Cols) * hWidth;
					Height = 8 + (BaseMap.MapSize.Rows + BaseMap.MapSize.Cols) * hHeight;

					Refresh();
				}
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			DrawSelectedLozenge();
		}

		internal protected void OnMouseDrag()
		{
			DrawSelectedLozenge();
		}

		/// <summary>
		/// Draws a lozenge-border around all tiles that are selected or
		/// being selected.
		/// </summary>
		private void DrawSelectedLozenge()
		{
			if (MainViewPanel.Instance.MainView.IsSelectedTileValid)
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
	
				_lozSelected.Reset();
				_lozSelected.AddLine(sel1, sel2);
				_lozSelected.AddLine(sel2, sel3);
				_lozSelected.AddLine(sel3, sel4);
				_lozSelected.CloseFigure();
	
				Refresh();
			}
		}

		private static Point GetDragStart()
		{
			var start = new Point(0, 0);
			start.X = Math.Min(
							MainViewPanel.Instance.MainView.DragStart.X,
							MainViewPanel.Instance.MainView.DragEnd.X);
			start.Y = Math.Min(
							MainViewPanel.Instance.MainView.DragStart.Y,
							MainViewPanel.Instance.MainView.DragEnd.Y);
			return start;
		}

		private static Point GetDragEnd()
		{
			var end = new Point(0, 0);
			end.X = Math.Max(
						MainViewPanel.Instance.MainView.DragStart.X,
						MainViewPanel.Instance.MainView.DragEnd.X);
			end.Y = Math.Max(
						MainViewPanel.Instance.MainView.DragStart.Y,
						MainViewPanel.Instance.MainView.DragEnd.Y);
			return end;
		}

		[Browsable(false), DefaultValue(null)]
		internal Dictionary<string, SolidBrush> Brushes
		{ get; set; }

		[Browsable(false), DefaultValue(null)]
		internal Dictionary<string, Pen> Pens
		{ get; set; }

		public override void OnSelectedTileChanged(IMapBase sender, SelectedTileChangedEventArgs e)
		{
/*			var pt = e.MapPosition;
//			Text = "c: " + pt.Col + " r: " + pt.Row; // I don't think this actually prints anywhere.

			var hWidth  = _drawService.HalfWidth;
			var hHeight = _drawService.HalfHeight;

			int xc = (pt.Col - pt.Row) * hWidth;
			int yc = (pt.Col + pt.Row) * hHeight;

			_lozSel.Reset();
			_lozSel.AddLine(
					xc, yc,
					xc + hWidth, yc + hHeight);
			_lozSel.AddLine(
					xc + hWidth, yc + hHeight,
					xc, yc + 2 * hHeight);
			_lozSel.AddLine(
					xc, yc + 2 * hHeight,
					xc - hWidth, yc + hHeight);
			_lozSel.CloseFigure();

			OnMouseDrag();

			Refresh(); */
		}

		protected override void Render(Graphics backBuffer)
		{
			backBuffer.FillRectangle(SystemBrushes.Control, ClientRectangle);

			var hWidth  = _drawService.HalfWidth;
			var hHeight = _drawService.HalfHeight;

			if (BaseMap != null)
			{
				for (int
						r = 0, startX = _offX, startY = _offY;
						r != BaseMap.MapSize.Rows;
						++r, startX -= hWidth, startY += hHeight)
				{
					for (int
							c = 0, x = startX, y = startY;
							c != BaseMap.MapSize.Cols;
							++c, x += hWidth, y += hHeight)
					{
						var mapTile = BaseMap[r, c] as MapTileBase;
						if (mapTile != null)
							RenderTile(mapTile, backBuffer, x, y);
					}
				}

				for (int i = 0; i <= BaseMap.MapSize.Rows; ++i)
					backBuffer.DrawLine(
									Pens["GridColor"],
									_offX - i * hWidth,
									_offY + i * hHeight,
									(BaseMap.MapSize.Cols - i) * hWidth  + _offX,
									(BaseMap.MapSize.Cols + i) * hHeight + _offY);

				for (int i = 0; i <= BaseMap.MapSize.Cols; ++i)
					backBuffer.DrawLine(
									Pens["GridColor"],
									_offX + i * hWidth,
									_offY + i * hHeight,
									i * hWidth  - BaseMap.MapSize.Rows * hWidth  + _offX,
									i * hHeight + BaseMap.MapSize.Rows * hHeight + _offY);

				if (MainViewPanel.Instance.MainView.IsSelectedTileValid)
					backBuffer.DrawPath(Pens["SelectColor"], _lozSelected);

				if (   _mouseCol > -1
					&& _mouseRow > -1
					&& _mouseCol < BaseMap.MapSize.Cols
					&& _mouseRow < BaseMap.MapSize.Rows)
				{
					int x = (_mouseCol - _mouseRow) * hWidth  + _offX;
					int y = (_mouseCol + _mouseRow) * hHeight + _offY;

					var sel = GetSelectorPath(x, y);
					backBuffer.DrawPath(Pens["MouseColor"], sel);
				}
			}
		}

		/// <summary>
		/// Forwards the call to TopViewPanel.RenderTile.
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="g"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		internal protected virtual void RenderTile(
				MapTileBase tile,
				Graphics g,
				int x, int y)
		{}


		/// <summary>
		/// Gets the GraphicsPath to draw for ... what.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private GraphicsPath GetSelectorPath(int x, int y)
		{
			var hWidth  = _drawService.HalfWidth;
			var hHeight = _drawService.HalfHeight;

			_lozSelector.Reset();
			_lozSelector.AddLine(
						x, y,
						x + hWidth, y + hHeight);
			_lozSelector.AddLine(
						x + hWidth, y + hHeight,
						x, y + 2 * hHeight);
			_lozSelector.AddLine(
						x, y + 2 * hHeight,
						x - hWidth, y + hHeight);
			_lozSelector.CloseFigure();

			return _lozSelector;
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


		private bool _isMouseDrag;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (BaseMap != null)
			{
				var pt = ConvertCoordsDiamond(
											e.X - _offX,
											e.Y - _offY);
				BaseMap.SelectedTile = new MapLocation(
												pt.Y,
												pt.X,
												BaseMap.CurrentHeight);

				_isMouseDrag = true;
				MainViewPanel.Instance.MainView.SetDrag(pt, pt);
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			_isMouseDrag = false;
			MainViewPanel.Instance.MainView.Refresh();

			Refresh();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			var pt = ConvertCoordsDiamond(
										e.X - _offX,
										e.Y - _offY);
			if (pt.X != _mouseCol || pt.Y != _mouseRow)
			{
				_mouseCol = pt.X;
				_mouseRow = pt.Y;

				if (_isMouseDrag)
					MainViewPanel.Instance.MainView.SetDrag(
														MainViewPanel.Instance.MainView.DragStart,
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
