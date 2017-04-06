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
		public MainView MainView
		{
			get { return _mainView; }
		}

		private static MainViewPanel _instance;
		public static MainViewPanel Instance
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
			ImageUpdateEvent += OnImageUpdate; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			_scrollBarHori = new HScrollBar();
			_scrollBarVert = new VScrollBar();

			_scrollBarHori.Scroll += OnHorizontalScroll;
			_scrollBarHori.Dock = DockStyle.Bottom;

			_scrollBarVert.Scroll += OnVerticalScroll;
			_scrollBarVert.Dock = DockStyle.Right;

			Controls.AddRange(new Control[]{ _scrollBarVert, _scrollBarHori });

			var mapView = new MainView();
			if (_mainView != null)
			{
				mapView.Map = _mainView.Map;
				Controls.Remove(_mainView);
			}
			_mainView = mapView;

			_mainView.Location = new Point(0, 0);
			_mainView.BorderStyle = BorderStyle.Fixed3D;

			_scrollBarVert.Minimum = 0;
			_scrollBarVert.Value = _scrollBarVert.Minimum;

			_mainView.Width = ClientSize.Width - _scrollBarVert.Width - 1;

			Controls.Add(_mainView);
		}


		public void OnCut(object sender, EventArgs e)
		{
			_mainView.Copy();
			_mainView.ClearSelection();
		}

		public void OnCopy(object sender, EventArgs e)
		{
			_mainView.Copy();
		}

		public void OnPaste(object sender, EventArgs e)
		{
			_mainView.Paste();
		}

		public void OnFill(object sender, EventArgs e)
		{
			_mainView.Fill();
		}

		public IMapBase BaseMap
		{
			get { return _mainView.Map; }
		}

		private void OnImageUpdate(object sender, EventArgs e)
		{
			_mainView.Refresh();
		}

		public void OnResize()
		{
			OnResize(EventArgs.Empty);
		}

		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			if (Globals.AutoPckImageScale)
				SetupMapSize();

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

		public void SetMap(IMapBase baseMap)
		{
			_mainView.Map = baseMap;
//			_mapView.Select();				// TODO: Select the *panel*

//			Select();						// doesn't work right.
//			MainViewPanel.Instance.Select();	// doesn't work.
//			MainViewPanel.Instance.Focus();	// doesn't work.

//			var controls = _mapView.Controls.Find("tscPanel", true);
//			if (controls.GetLength(0) != 0)
//				controls[0].Select();		// doesn't work.

//			Focus();
//			Select();						// doesn't work.

			//LeftToolStripPanel


			OnResize(null);
		}

		public void SetupMapSize()
		{
			if (Globals.AutoPckImageScale)
			{
				var size = _mainView.GetMapSize(1);

				var wP = Width  / (double)size.Width;
				var hP = Height / (double)size.Height;

				Globals.PckImageScale = (wP > hP) ? hP : wP;

				if (Globals.PckImageScale > Globals.MaxPckImageScale)
					Globals.PckImageScale = Globals.MaxPckImageScale;

				if (Globals.PckImageScale < Globals.MinPckImageScale)
					Globals.PckImageScale = Globals.MinPckImageScale;
			}

			_mainView.SetupMapSize();
		}

		public void ForceResize()
		{
			OnResize(null);
		}


		/*** Timer stuff ***/
		public static event EventHandler ImageUpdateEvent;

		private static Timer _timer;
		private static int _current;

		// NOTE: Remove suppression for Release cfg.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility",
		"CA1601:DoNotUseTimersThatPreventPowerStateChanges",
		Justification = "Because animations at or greater than 1 second ain't gonna cut it.")]
		public static void Start()
		{
			if (_timer == null)
			{
				_timer = new Timer();
				_timer.Interval = 250;
				_timer.Tick += Animate;
			}

			if (!_timer.Enabled)
				_timer.Start();
		}

		// NOTE: Remove suppression for Release cfg.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility",
		"CA1601:DoNotUseTimersThatPreventPowerStateChanges",
		Justification = "Because animations at or greater than 1 second ain't gonna cut it.")]
		public static void Stop()
		{
			if (_timer != null)
				_timer.Stop();
		}

		public static bool IsAnimated
		{
			get { return (_timer != null && _timer.Enabled); }
		}

/*		public static int Interval
		{
			get { return _timer.Interval; }
			set { _timer.Interval = value; }
		} */

		private static void Animate(object sender, EventArgs e)
		{
			_current = (_current + 1) % 8;

			if (ImageUpdateEvent != null)
				ImageUpdateEvent(null, null);
		}

		public static int Current
		{
			get { return _current; }
		}
	}
}
