using System;
using System.Drawing;
using System.Windows.Forms;

using XCom.Interfaces.Base;


namespace MapView
{
	internal sealed class MainViewUnderlay
		:
			Panel // god I hate these double-panels!!!! cf. MainViewOverlay
	{
		#region Fields & Properties
		private static MainViewUnderlay _instance;
		internal static MainViewUnderlay Instance
		{
			get
			{
				if (_instance == null)
					_instance = new MainViewUnderlay();

				return _instance;
			}
		}

		private MainViewOverlay _mainViewOverlay;
		internal MainViewOverlay MainViewOverlay
		{
			get { return _mainViewOverlay; }
		}

		private XCMapBase _mapBase;
		internal XCMapBase MapBase
		{
			get { return _mapBase; }
			set
			{
				//XCom.LogFile.WriteLine("MainViewUnderlay.MapBase set");

				MainViewOverlay.MapBase = value;

				if (_mapBase != null)
				{
					_mapBase.LocationSelectedEvent -= MainViewOverlay.OnLocationSelected_Main;	// WARNING: if the overlay ever gets removed from the Control
					_mapBase.LevelChangedEvent     -= MainViewOverlay.OnLevelChanged_Main;		// by the code in the cTor, this will likely go defunct. Or not.
				}

				if ((_mapBase = value) != null)
				{
					_mapBase.LocationSelectedEvent += MainViewOverlay.OnLocationSelected_Main;
					_mapBase.LevelChangedEvent     += MainViewOverlay.OnLevelChanged_Main;

					SetOverlaySize();

//					DragStart = _dragStart;	// this might be how to give drags their legitimate
//					DragEnd   = _dragEnd;	// values after initialization to Point(-1/-1).
				}

				OnResize(EventArgs.Empty);
			}
		}


		private int _halfWidth;
		internal int HalfWidth
		{
			set
			{
				_halfWidth                =
				MainViewOverlay.HalfWidth = value; // pass it on to Overlay.
			}
		}

		private int _halfHeight;
		internal int HalfHeight
		{
			set
			{
				_halfHeight                =
				MainViewOverlay.HalfHeight = value; // pass it on to Overlay.
			}
		}


		private readonly HScrollBar _scrollBarH;
		private readonly VScrollBar _scrollBarV;
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		private MainViewUnderlay()
		{
			AnimationUpdateEvent += OnAnimationUpdate; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			_scrollBarV = new VScrollBar();
			_scrollBarV.Dock = DockStyle.Right;
			_scrollBarV.Scroll += OnScrollVert;

			_scrollBarH = new HScrollBar();
			_scrollBarH.Dock = DockStyle.Bottom;
			_scrollBarH.Scroll += OnScrollHori;

//			var mainViewOverlay = new MainViewOverlay(); // what's this for.
//			if (_mainViewOverlay != null)
//			{
//				mainViewOverlay.MapBase = _mainViewOverlay.MapBase;
//				Controls.Remove(_mainViewOverlay);
//			}
//			_mainViewOverlay = mainViewOverlay;

			_mainViewOverlay = new MainViewOverlay();
			_mainViewOverlay.SetMainViewUnderlay(this);

			Controls.AddRange(new Control[]
			{
				_scrollBarV,
				_scrollBarH,
				_mainViewOverlay
			});


//			XCom.LogFile.WriteLine("");
//			XCom.LogFile.WriteLine("MainViewUnderlay cTor");
//			XCom.LogFile.WriteLine(". underlay.Width= " + Width);
//			XCom.LogFile.WriteLine(". underlayHeight= " + Height);
//
//			XCom.LogFile.WriteLine(". underlay client.Width= " + ClientSize.Width);
//			XCom.LogFile.WriteLine(". underlay client.Height= " + ClientSize.Height);
//
//			XCom.LogFile.WriteLine(". overlay.Width= " + _mainViewOverlay.Width);
//			XCom.LogFile.WriteLine(". overlay.Height= " + _mainViewOverlay.Height);
//
//			XCom.LogFile.WriteLine(". overlay client.Width= " + _mainViewOverlay.ClientSize.Width);
//			XCom.LogFile.WriteLine(". overlay client.Height= " + _mainViewOverlay.ClientSize.Height);
		}
		#endregion


		#region EventCalls
//		protected override void OnPaint(PaintEventArgs e)
//		{
//			base.OnPaint(e);
//
//			var graphics = e.Graphics;
//			graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
////			graphics.SmoothingMode = SmoothingMode.HighQuality;
//
//			ControlPaint.DrawBorder3D(graphics, ClientRectangle, Border3DStyle.Etched);
//		}

		/// <summary>
		/// Forces an OnResize event for this Panel. Grants access for
		/// XCMainWindow to place a call or two.
		/// </summary>
		internal void ResizeUnderlay()
		{
			OnResize(EventArgs.Empty);
		}

		protected override void OnResize(EventArgs eventargs)
		{
//			XCom.LogFile.WriteLine("");
//			XCom.LogFile.WriteLine("MainViewUnderlay.OnResize");
//
//			XCom.LogFile.WriteLine("underlay.Width= " + Width);
//			XCom.LogFile.WriteLine("underlay.Height= " + Height);
//
//			XCom.LogFile.WriteLine("underlay client.Width= " + ClientSize.Width);
//			XCom.LogFile.WriteLine("underlay client.Height= " + ClientSize.Height);
//
//			XCom.LogFile.WriteLine("overlay.Width= " + MainViewOverlay.Width);
//			XCom.LogFile.WriteLine("overlay.Height= " + MainViewOverlay.Height);
//
//			XCom.LogFile.WriteLine("overlay client.Width= " + MainViewOverlay.ClientSize.Width);
//			XCom.LogFile.WriteLine("overlay client.Height= " + MainViewOverlay.ClientSize.Height);


			base.OnResize(eventargs);

			if (Globals.AutoScale)
			{
				SetScale();
				SetOverlaySize();
			}
			UpdateScrollers();

//			XCom.LogFile.WriteLine("MainViewUnderlay.OnResize EXIT");
		}

		/// <summary>
		/// A workaround for maximizing the parent-form. See notes at
		/// XCMainWindow.OnResize(). Note that this workaround pertains only to
		/// cases when AutoScale=FALSE.
		/// </summary>
		internal void ResetScrollers()
		{
			_scrollBarV.Value =
			_scrollBarH.Value = 0;

			MainViewOverlay.Location = new Point(0, 0);
		}

		/// <summary>
		/// Handles the scroll-bars.
		/// </summary>
		internal void UpdateScrollers()
		{
			if (Globals.AutoScale)
			{
				_scrollBarV.Visible =
				_scrollBarH.Visible = false;

				_scrollBarV.Value =
				_scrollBarH.Value = 0;

				MainViewOverlay.Location = new Point(0, 0);
			}
			else
			{
				_scrollBarV.Visible = (MainViewOverlay.Height > ClientSize.Height);
				if (_scrollBarV.Visible)
				{
					_scrollBarV.Maximum = Math.Max(
												MainViewOverlay.Height - ClientSize.Height + _scrollBarH.Height,
												0);
				}
/*				else
				{
					MainViewOverlay.Location = new Point(0, 0);
//					_scrollBarH.Width = ClientSize.Width;
				}*/

				_scrollBarH.Visible = (MainViewOverlay.Width > ClientSize.Width);
				if (_scrollBarH.Visible)
				{
					_scrollBarH.Maximum = Math.Max(
												MainViewOverlay.Width - ClientSize.Width + _scrollBarV.Width,
												0);
				}
/*				else
				{
					MainViewOverlay.Location = new Point(0, 0);
//					_scrollBarV.Height = ClientSize.Height;
				} */

				_scrollBarV.Value = Math.Min(_scrollBarV.Value, _scrollBarV.Maximum);
				_scrollBarH.Value = Math.Min(_scrollBarH.Value, _scrollBarH.Maximum);

				OnScrollVert(null, null);
				OnScrollHori(null, null);
			}

			MainViewOverlay.Refresh();
		}

		/// <summary>
		/// Sets the scale-factor. Is used only if AutoScale=TRUE.
		/// </summary>
		internal void SetScale()
		{
			//XCom.LogFile.WriteLine("");
			//XCom.LogFile.WriteLine("MainViewUnderlay.SetScale");

			var normal = GetOverlaySizeRequired(1);
			Globals.Scale = Math.Min(
								(double)Width  / normal.Width,
								(double)Height / normal.Height);
			Globals.Scale = Globals.Scale.Clamp(
											Globals.ScaleMinimum,
											Globals.ScaleMaximum);

			//XCom.LogFile.WriteLine(". scale set to= " + Globals.Scale);
		}

		/// <summary>
		/// Sets this Panel to the size of the current Map including scaling.
		/// </summary>
		internal void SetOverlaySize()
		{
			//XCom.LogFile.WriteLine("");
			//XCom.LogFile.WriteLine("MainViewUnderlay.SetOverlaySize");

			if (MapBase != null)
			{
				//XCom.LogFile.WriteLine(". scale= " + Globals.Scale);
				var required = GetOverlaySizeRequired(Globals.Scale);

				MainViewOverlay.Width  = required.Width  + 2; // don't clip lozenge-tips at right or bottom edges.
				MainViewOverlay.Height = required.Height + 2; // TODO: needs to be further adjusted

				//XCom.LogFile.WriteLine(". set overlay.Width= " + MainViewOverlay.Width);
				//XCom.LogFile.WriteLine(". set overlay.Height= " + MainViewOverlay.Height);

				XCMainWindow.Instance.StatusBarPrintScale();	// TODO: The scale shown is changing based on
			}													// underlay size rather than overlay size. Etc.
		}

		/// <summary>
		/// Gets the required x/y size in pixels for the current MapBase as a
		/// lozenge. Also sets the 'Origin' point and the half-width/height vals.
		/// </summary>
		/// <param name="scale">the current scaling factor</param>
		/// <returns></returns>
		internal Size GetOverlaySizeRequired(double scale)
		{
			//XCom.LogFile.WriteLine("");
			//XCom.LogFile.WriteLine("MainViewUnderlay.GetOverlaySizeRequired");

			if (MapBase != null)
			{
				//XCom.LogFile.WriteLine(". scale= " + Globals.Scale);

				_halfWidth  = (int)(MainViewOverlay.HalfWidthConst  * scale);
				_halfHeight = (int)(MainViewOverlay.HalfHeightConst * scale);

				if (_halfHeight > _halfWidth / 2) // use width
				{
					if (_halfWidth % 2 != 0)
						--_halfWidth;

					_halfHeight = _halfWidth / 2;
				}
				else // use height
				{
					_halfWidth = _halfHeight * 2;
				}

				HalfWidth  = _halfWidth; // set half-width/height for the Overlay.
				HalfHeight = _halfHeight;


				MainViewOverlay.Origin = new Point((MapBase.MapSize.Rows - 1) * _halfWidth, 0);

				int width  = (MapBase.MapSize.Rows + MapBase.MapSize.Cols) * _halfWidth;
				int height = (MapBase.MapSize.Rows + MapBase.MapSize.Cols) * _halfHeight
						   +  MapBase.MapSize.Levs * _halfHeight * 3;

				//XCom.LogFile.WriteLine(". width= " + width);
				//XCom.LogFile.WriteLine(". height= " + height);

				return new Size(width, height);
			}

			//XCom.LogFile.WriteLine(". RET size empty.");
			return Size.Empty;
		}


		private void OnScrollVert(object sender, ScrollEventArgs e)
		{
			//XCom.LogFile.WriteLine("OnVerticalScroll overlay.Left= " + MainViewOverlay.Left);
			MainViewOverlay.Location = new Point(
												MainViewOverlay.Left,
												-_scrollBarV.Value);
			MainViewOverlay.Refresh();
		}

		private void OnScrollHori(object sender, ScrollEventArgs e)
		{
			//XCom.LogFile.WriteLine("OnVerticalScroll overlay.Top= " + MainViewOverlay.Top);
			MainViewOverlay.Location = new Point(
												-_scrollBarH.Value,
												MainViewOverlay.Top);
			MainViewOverlay.Refresh();
		}


		internal void OnCut(object sender, EventArgs e)
		{
			MainViewOverlay.Copy();
			MainViewOverlay.ClearSelection();
		}

		internal void OnCopy(object sender, EventArgs e)
		{
			MainViewOverlay.Copy();
		}

		internal void OnPaste(object sender, EventArgs e)
		{
			MainViewOverlay.Paste();
		}

		internal void OnFill(object sender, EventArgs e)
		{
			MainViewOverlay.FillSelectedTiles();
		}

		private void OnAnimationUpdate(object sender, EventArgs e)
		{
			MainViewOverlay.Refresh();
		}
		#endregion


		#region Timer stuff for Animations
		internal static event EventHandler AnimationUpdateEvent;

		private static Timer _timer;
		private static int _anistep;

		// NOTE: Remove suppression for Release cfg.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility",
		"CA1601:DoNotUseTimersThatPreventPowerStateChanges",
		Justification = "Because animations at or greater than 1 second ain't gonna cut it.")]
		internal static void Animate(bool animate)
		{
			if (animate)
			{
				if (_timer == null)
				{
					_timer = new Timer();
					_timer.Interval = 250;
					_timer.Tick += AnimateStep;
				}

				if (!_timer.Enabled)
					_timer.Start();
			}
			else if (_timer != null)
			{
				_timer.Stop();
				_anistep = 0;
			}
		}

		private static void AnimateStep(object sender, EventArgs e)
		{
			_anistep = ++_anistep % 8;

			if (AnimationUpdateEvent != null)
				AnimationUpdateEvent(null, null);
		}

		internal static bool IsAnimated
		{
			get { return (_timer != null && _timer.Enabled); }
		}

		internal static int AniStep
		{
			get { return _anistep; }
		}
		#endregion
	}
}
