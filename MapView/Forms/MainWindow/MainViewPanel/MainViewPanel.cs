using System;
using System.Drawing;
using System.Windows.Forms;

using XCom.Interfaces.Base;


namespace MapView
{
	internal sealed class MainViewPanel
		:
			Panel
	{
		private MainView _mainView;
		internal MainView MainView
		{
			get { return _mainView; }
		}

		private static MainViewPanel _instance;
		internal static MainViewPanel Instance
		{
			get
			{
				if (_instance == null)
					_instance = new MainViewPanel();

				return _instance;
			}
		}

		private readonly HScrollBar _scrollBarHori;
		private readonly VScrollBar _scrollBarVert;


		private MainViewPanel()
		{
			AnimationUpdateEvent += OnAnimationUpdate; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			_scrollBarHori = new HScrollBar();
			_scrollBarVert = new VScrollBar();

			_scrollBarHori.Scroll += OnHorizontalScroll;
			_scrollBarHori.Dock = DockStyle.Bottom;

			_scrollBarVert.Scroll += OnVerticalScroll;
			_scrollBarVert.Dock = DockStyle.Right;

			Controls.AddRange(new Control[]{ _scrollBarVert, _scrollBarHori });

			var mainView = new MainView();
			if (_mainView != null)
			{
				mainView.Map = _mainView.Map;
				Controls.Remove(_mainView);
			}
			_mainView = mainView;

			_mainView.Location = new Point(0, 0);
			_mainView.BorderStyle = BorderStyle.Fixed3D;

			_scrollBarVert.Minimum = 0;
			_scrollBarVert.Value = _scrollBarVert.Minimum;

			_mainView.Width = ClientSize.Width - _scrollBarVert.Width - 1;

			Controls.Add(_mainView);
		}


		internal void OnCut(object sender, EventArgs e)
		{
			_mainView.Copy();
			_mainView.ClearSelection();
		}

		internal void OnCopy(object sender, EventArgs e)
		{
			_mainView.Copy();
		}

		internal void OnPaste(object sender, EventArgs e)
		{
			_mainView.Paste();
		}

		internal void OnFill(object sender, EventArgs e)
		{
			_mainView.Fill();
		}

		internal IMapBase BaseMap
		{
			get { return _mainView.Map; }
		}

		private void OnAnimationUpdate(object sender, EventArgs e)
		{
			_mainView.Refresh();
		}

		internal void OnResize()
		{
			OnResize(EventArgs.Empty);
		}

		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			if (Globals.AutoPckImageScale)
				SetMapSize();

			_scrollBarVert.Value  = _scrollBarVert.Minimum;
			_scrollBarHori.Value = _scrollBarHori.Minimum;

			OnVerticalScroll(null, null);
			OnHorizontalScroll(null, null);

//			int h = 0;
//			int w = 0;

			_scrollBarVert.Visible = (_mainView.Height > ClientSize.Height);
			if (_scrollBarVert.Visible)
			{
				_scrollBarVert.Maximum = _mainView.Height - ClientSize.Height + _scrollBarHori.Height;
//				w = _scrollBarVert.Width;
			}
			else
				_scrollBarHori.Width = ClientSize.Width;

			_scrollBarHori.Visible = (_mainView.Width > ClientSize.Width);
			if (_scrollBarHori.Visible)
			{
				_scrollBarHori.Maximum = Math.Max(
											_mainView.Width - ClientSize.Width + _scrollBarVert.Width,
											_scrollBarHori.Minimum);
//				h = _scrollBarHori.Height;
			}
			else
				_scrollBarVert.Height = ClientSize.Height;

//			_mapView.Viewable = new Size(Width - w, Height - h);
			_mainView.Refresh();
		}

		private void OnVerticalScroll(object sender, ScrollEventArgs e)
		{
			_mainView.Location = new Point(
										_mainView.Left,
										-(_scrollBarVert.Value) + 1);
			_mainView.Refresh();
		}

		private void OnHorizontalScroll(object sender, ScrollEventArgs e)
		{
			_mainView.Location = new Point(
										-(_scrollBarHori.Value),
										_mainView.Top);
			_mainView.Refresh();
		}

		internal void SetMap(IMapBase baseMap)
		{
			_mainView.Map = baseMap;
//			_mapView.Select();					// TODO: Select the *panel*

//			Select();							// doesn't work right.
//			MainViewPanel.Instance.Select();	// doesn't work.
//			MainViewPanel.Instance.Focus();		// doesn't work.

//			var controls = _mapView.Controls.Find("tscPanel", true);
//			if (controls.GetLength(0) != 0)
//				controls[0].Select();			// doesn't work.

//			Focus();
//			Select();							// doesn't work.

			//LeftToolStripPanel


			OnResize(null);
		}

		internal void SetMapSize()
		{
			if (Globals.AutoPckImageScale)
			{
				var size = _mainView.GetMapSize(1.0);

				var width  = Width  / (double)size.Width;
				var height = Height / (double)size.Height;

				Globals.PckImageScale = (width > height) ? height : width;
				Globals.PckImageScale.Clamp(
										Globals.MinPckImageScale,
										Globals.MaxPckImageScale);
			}

			_mainView.SetMapSize();
		}

		internal void ForceResize()
		{
			OnResize(null);
		}


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
