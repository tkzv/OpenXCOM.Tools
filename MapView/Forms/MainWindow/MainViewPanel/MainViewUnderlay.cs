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
				XCom.LogFile.WriteLine("MainViewUnderlay.MapBase set");

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

		internal int GetVBarWidth()
		{
			return _scrollBarV.Width;
		}

		internal int GetHBarHeight()
		{
			return _scrollBarH.Height;
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		private MainViewUnderlay()
		{
			AnimationUpdateEvent += OnAnimationUpdate; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			_scrollBarH = new HScrollBar();
			_scrollBarV = new VScrollBar();

			_scrollBarH.Dock = DockStyle.Bottom;
			_scrollBarH.Scroll += OnHorizontalScroll;

			_scrollBarV.Dock = DockStyle.Right;
			_scrollBarV.Scroll += OnVerticalScroll;

			Controls.AddRange(new Control[]{ _scrollBarV, _scrollBarH });


//			var mainViewOverlay = new MainViewOverlay(); // what's this for.
//			if (_mainViewOverlay != null)
//			{
//				mainViewOverlay.MapBase = _mainViewOverlay.MapBase;
//				Controls.Remove(_mainViewOverlay);
//			}
//			_mainViewOverlay = mainViewOverlay;

			_mainViewOverlay = new MainViewOverlay();
			_mainViewOverlay.SetMainViewUnderlay(this);

			Controls.Add(_mainViewOverlay);


			XCom.LogFile.WriteLine("");
			XCom.LogFile.WriteLine("MainViewUnderlay cTor");
			XCom.LogFile.WriteLine(". underlay.Width= " + Width);
			XCom.LogFile.WriteLine(". underlayHeight= " + Height);

			XCom.LogFile.WriteLine(". underlay client.Width= " + ClientSize.Width);
			XCom.LogFile.WriteLine(". underlay client.Height= " + ClientSize.Height);

			XCom.LogFile.WriteLine(". overlay.Width= " + _mainViewOverlay.Width);
			XCom.LogFile.WriteLine(". overlay.Height= " + _mainViewOverlay.Height);

			XCom.LogFile.WriteLine(". overlay client.Width= " + _mainViewOverlay.ClientSize.Width);
			XCom.LogFile.WriteLine(". overlay client.Height= " + _mainViewOverlay.ClientSize.Height);
		}
		#endregion


		#region EventCalls
		/// <summary>
		/// Forces an OnResize event for this Panel. Grants access for
		/// XCMainWindow to place a call.
		/// </summary>
		internal void FireResize()
		{
			OnResize(EventArgs.Empty);
		}

		protected override void OnResize(EventArgs eventargs)
		{
			XCom.LogFile.WriteLine("");
			XCom.LogFile.WriteLine("MainViewUnderlay.OnResize");

			XCom.LogFile.WriteLine("underlay.Width= " + Width);
			XCom.LogFile.WriteLine("underlay.Height= " + Height);

			XCom.LogFile.WriteLine("underlay client.Width= " + ClientSize.Width);
			XCom.LogFile.WriteLine("underlay client.Height= " + ClientSize.Height);

			XCom.LogFile.WriteLine("overlay.Width= " + MainViewOverlay.Width);
			XCom.LogFile.WriteLine("overlay.Height= " + MainViewOverlay.Height);

			XCom.LogFile.WriteLine("overlay client.Width= " + MainViewOverlay.ClientSize.Width);
			XCom.LogFile.WriteLine("overlay client.Height= " + MainViewOverlay.ClientSize.Height);



			base.OnResize(eventargs);

			_scrollBarV.Value =
			_scrollBarH.Value = 0;

			OnVerticalScroll(null, null);
			OnHorizontalScroll(null, null);

			_scrollBarV.Visible = (MainViewOverlay.Height > ClientSize.Height);
			if (_scrollBarV.Visible)
			{
				_scrollBarV.Maximum = Math.Max(
											MainViewOverlay.Height - ClientSize.Height + _scrollBarH.Height,
											0);
			}
			else
				_scrollBarH.Width = ClientSize.Width;

			_scrollBarH.Visible = (MainViewOverlay.Width > ClientSize.Width);
			if (_scrollBarH.Visible)
			{
				_scrollBarH.Maximum = Math.Max(
											MainViewOverlay.Width - ClientSize.Width + _scrollBarV.Width,
											0);
			}
			else
				_scrollBarV.Height = ClientSize.Height;


			if (Globals.AutoScale)
			{
				SetScale();
				SetOverlaySize();
			}

			MainViewOverlay.Refresh();
			XCom.LogFile.WriteLine("MainViewUnderlay.OnResize EXIT");
		}

		/// <summary>
		/// Sets the scale-factor if AutoScale.
		/// </summary>
		internal void SetScale()
		{
			XCom.LogFile.WriteLine("");
			XCom.LogFile.WriteLine("MainViewUnderlay.SetScale");

			var normal = GetOverlaySizeRequired(1);
			Globals.Scale = Math.Min(
								(double)Width  / normal.Width,
								(double)Height / normal.Height);
			Globals.Scale.Clamp(
							Globals.ScaleMinimum,
							Globals.ScaleMaximum);

			XCom.LogFile.WriteLine(". scale set to= " + Globals.Scale);
		}

		/// <summary>
		/// Gets the required x/y size in pixels for the current MapBase as a
		/// lozenge. Also sets the 'Origin' point and the half-width/height vals.
		/// </summary>
		/// <param name="scale">the current scaling factor</param>
		/// <returns></returns>
		internal Size GetOverlaySizeRequired(double scale)
		{
			XCom.LogFile.WriteLine("");
			XCom.LogFile.WriteLine("MainViewUnderlay.GetOverlaySizeRequired");

			if (MapBase != null)
			{
				XCom.LogFile.WriteLine(". scale= " + Globals.Scale);

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
				int height =  MapBase.MapSize.Levs * _halfHeight * 3
						   + (MapBase.MapSize.Rows + MapBase.MapSize.Cols) * _halfHeight;

				XCom.LogFile.WriteLine(". width= " + width);
				XCom.LogFile.WriteLine(". height= " + height);

				return new Size(width, height);
			}

			XCom.LogFile.WriteLine(". RET size empty.");
			return Size.Empty;
		}

		/// <summary>
		/// Sets this Panel to the size of the current Map including scaling.
		/// </summary>
		internal void SetOverlaySize()
		{
			XCom.LogFile.WriteLine("");
			XCom.LogFile.WriteLine("MainViewUnderlay.SetOverlaySize");

			if (MapBase != null)
			{
				XCom.LogFile.WriteLine(". scale= " + Globals.Scale);
				var sized = GetOverlaySizeRequired(Globals.Scale);

				MainViewOverlay.Width  = sized.Width  + _halfWidth; // don't cover half a lozenge at bottom or right sides.
				MainViewOverlay.Height = sized.Height + _halfHeight;

				XCom.LogFile.WriteLine(". Width= " + Width);
				XCom.LogFile.WriteLine(". Height= " + Height);

				XCMainWindow.Instance.StatusBarPrintScale();
			}
		}


		private void OnVerticalScroll(object sender, ScrollEventArgs e)
		{
			//XCom.LogFile.WriteLine("OnVerticalScroll overlay.Left= " + MainViewOverlay.Left);
			MainViewOverlay.Location = new Point(
												MainViewOverlay.Left,
												1 - _scrollBarV.Value);
			MainViewOverlay.Refresh();
		}

		private void OnHorizontalScroll(object sender, ScrollEventArgs e)
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

/*		public static int Interval
		{
			get { return _timer.Interval; }
			set { _timer.Interval = value; }
		} */

		internal static int AniStep
		{
			get { return _anistep; }
		}
		#endregion
	}
}
