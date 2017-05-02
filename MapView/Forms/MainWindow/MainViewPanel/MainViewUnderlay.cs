using System;
using System.Drawing;
using System.Windows.Forms;

using XCom.Interfaces.Base;


namespace MapView
{
	internal sealed class MainViewUnderlay
		:
			Panel
	{
		#region Fields & Properties
		private MainViewOverlay _mainViewOverlay;
		internal MainViewOverlay MainViewOverlay
		{
			get { return _mainViewOverlay; }
		}

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

		internal XCMapBase MapBase
		{
			get { return _mainViewOverlay.MapBase; }
		}

		private readonly HScrollBar _scrollBarHori;
		private readonly VScrollBar _scrollBarVert;
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		private MainViewUnderlay()
		{
			AnimationUpdateEvent += OnAnimationUpdate; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			_scrollBarHori = new HScrollBar();
			_scrollBarVert = new VScrollBar();

			_scrollBarHori.Scroll += OnHorizontalScroll;
			_scrollBarHori.Dock = DockStyle.Bottom;

			_scrollBarVert.Scroll += OnVerticalScroll;
			_scrollBarVert.Dock = DockStyle.Right;

			Controls.AddRange(new Control[]{ _scrollBarVert, _scrollBarHori });

			var mainView = new MainViewOverlay();
			if (_mainViewOverlay != null)
			{
				mainView.MapBase = _mainViewOverlay.MapBase;
				Controls.Remove(_mainViewOverlay);
			}
			_mainViewOverlay = mainView;

			_mainViewOverlay.Location = new Point(0, 0);
			_mainViewOverlay.BorderStyle = BorderStyle.Fixed3D;

			_scrollBarVert.Minimum = 0;
			_scrollBarVert.Value = _scrollBarVert.Minimum;

			_mainViewOverlay.Width = ClientSize.Width - _scrollBarVert.Width - 1;

			Controls.Add(_mainViewOverlay);
		}
		#endregion


		#region EventCalls
		internal void OnCut(object sender, EventArgs e)
		{
			_mainViewOverlay.Copy();
			_mainViewOverlay.ClearSelection();
		}

		internal void OnCopy(object sender, EventArgs e)
		{
			_mainViewOverlay.Copy();
		}

		internal void OnPaste(object sender, EventArgs e)
		{
			_mainViewOverlay.Paste();
		}

		internal void OnFill(object sender, EventArgs e)
		{
			_mainViewOverlay.Fill();
		}

		private void OnAnimationUpdate(object sender, EventArgs e)
		{
			_mainViewOverlay.Refresh();
		}

		/// <summary>
		/// Forces an OnResize event for this Panel.
		/// </summary>
		internal void ForceResize()
		{
			OnResize(EventArgs.Empty);
		}

		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			if (Globals.AutoPckImageScale)
				SetPanelSize();

			_scrollBarVert.Value = _scrollBarVert.Minimum;
			_scrollBarHori.Value = _scrollBarHori.Minimum;

			OnVerticalScroll(null, null);
			OnHorizontalScroll(null, null);

//			int h = 0;
//			int w = 0;

			_scrollBarVert.Visible = (_mainViewOverlay.Height > ClientSize.Height);
			if (_scrollBarVert.Visible)
			{
				_scrollBarVert.Maximum = _mainViewOverlay.Height - ClientSize.Height + _scrollBarHori.Height;
//				w = _scrollBarVert.Width;
			}
			else
				_scrollBarHori.Width = ClientSize.Width;

			_scrollBarHori.Visible = (_mainViewOverlay.Width > ClientSize.Width);
			if (_scrollBarHori.Visible)
			{
				_scrollBarHori.Maximum = Math.Max(
											_mainViewOverlay.Width - ClientSize.Width + _scrollBarVert.Width,
											_scrollBarHori.Minimum);
//				h = _scrollBarHori.Height;
			}
			else
				_scrollBarVert.Height = ClientSize.Height;

//			_mapView.Viewable = new Size(Width - w, Height - h);
			_mainViewOverlay.Refresh();
		}

		private void OnVerticalScroll(object sender, ScrollEventArgs e)
		{
			_mainViewOverlay.Location = new Point(
											_mainViewOverlay.Left,
											-(_scrollBarVert.Value) + 1);
			_mainViewOverlay.Refresh();
		}

		private void OnHorizontalScroll(object sender, ScrollEventArgs e)
		{
			_mainViewOverlay.Location = new Point(
											-(_scrollBarHori.Value),
											_mainViewOverlay.Top);
			_mainViewOverlay.Refresh();
		}
		#endregion


		#region Methods
		internal void SetMapBase(XCMapBase mapBase)
		{
			_mainViewOverlay.MapBase = mapBase; // TODO: Select the *panel*

			OnResize(null);
		}

		/// <summary>
		/// Sets the scale-factor and the Panel-size.
		/// </summary>
		internal void SetPanelSize()
		{
			if (Globals.AutoPckImageScale)
			{
				var size = _mainViewOverlay.GetPanelSizeRequired(1.0);

				var width  = Width  / (double)size.Width;
				var height = Height / (double)size.Height;

				Globals.PckImageScale = (width > height) ? height
														 : width;
				Globals.PckImageScale.Clamp(
										Globals.MinPckImageScale,
										Globals.MaxPckImageScale);
			}
			_mainViewOverlay.SetPanelSize();
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
