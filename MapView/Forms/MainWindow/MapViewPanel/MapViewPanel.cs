using System;
using System.Drawing;
using System.Windows.Forms;

using XCom.Interfaces.Base;


namespace MapView
{
	internal sealed class MapViewPanel
		:
			Panel
	{
		private MapView _mapView;
		public MapView MapView
		{
			get { return _mapView; }
		}

		private readonly HScrollBar _scrollBarHori;
		private readonly VScrollBar _scrollBarVert;

		private static MapViewPanel _instance;
		public static MapViewPanel Instance
		{
			get
			{
				if (_instance == null)
					_instance = new MapViewPanel();

					return _instance;
			}
		}


		private MapViewPanel()
		{
			ImageUpdateEvent += OnImageUpdate; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			_scrollBarHori = new HScrollBar();
			_scrollBarVert = new VScrollBar();

			_scrollBarHori.Scroll += OnHorizontalScroll;
			_scrollBarHori.Dock = DockStyle.Bottom;

			_scrollBarVert.Scroll += OnVerticalScroll;
			_scrollBarVert.Dock = DockStyle.Right;

			Controls.AddRange(new Control[]{ _scrollBarVert, _scrollBarHori });

			var mapView = new MapView();
			if (_mapView != null)
			{
				mapView.Map = _mapView.Map;
				Controls.Remove(_mapView);
			}

			_mapView = mapView;

			_mapView.Location = new Point(0, 0);
			_mapView.BorderStyle = BorderStyle.Fixed3D;

			_scrollBarVert.Minimum = 0;
			_scrollBarVert.Value = _scrollBarVert.Minimum;

			_mapView.Width = ClientSize.Width - _scrollBarVert.Width - 1;

			Controls.Add(_mapView);
		}


		public void OnCut(object sender, EventArgs e)
		{
			_mapView.Copy();
			_mapView.ClearSelection();
		}

		public void OnCopy(object sender, EventArgs e)
		{
			_mapView.Copy();
		}

		public void OnPaste(object sender, EventArgs e)
		{
			_mapView.Paste();
		}

		public IMapBase BaseMap
		{
			get { return _mapView.Map; }
		}

		private void OnImageUpdate(object sender, EventArgs e)
		{
			_mapView.Refresh();
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

			_scrollBarVert.Visible = (_mapView.Height > ClientSize.Height);
			if (_scrollBarVert.Visible)
			{
				_scrollBarVert.Maximum = _mapView.Height - ClientSize.Height + _scrollBarHori.Height;
//				w = _scrollBarVert.Width;
			}
			else
				_scrollBarHori.Width = ClientSize.Width;

			_scrollBarHori.Visible = (_mapView.Width > ClientSize.Width);
			if (_scrollBarHori.Visible)
			{
				_scrollBarHori.Maximum = Math.Max(
									_mapView.Width - ClientSize.Width + _scrollBarVert.Width,
									_scrollBarHori.Minimum);
//				h = _scrollBarHori.Height;
			}
			else
				_scrollBarVert.Height = ClientSize.Height;

//			_mapView.Viewable = new Size(Width - w, Height - h);
			_mapView.Refresh();
		}

		private void OnVerticalScroll(object sender, ScrollEventArgs e)
		{
			_mapView.Location = new Point(
										_mapView.Left,
										-(_scrollBarVert.Value) + 1);
			_mapView.Refresh();
		}

		private void OnHorizontalScroll(object sender, ScrollEventArgs e)
		{
			_mapView.Location = new Point(
										-(_scrollBarHori.Value),
										_mapView.Top);
			_mapView.Refresh();
		}

		public void SetMap(IMapBase baseMap)
		{
			_mapView.Map = baseMap;
			_mapView.Focus();

			OnResize(null);
		}

		public void SetupMapSize()
		{
			if (Globals.AutoPckImageScale)
			{
				var size = _mapView.GetMapSize(1);

				var wP = Width  / (double)size.Width;
				var hP = Height / (double)size.Height;

				Globals.PckImageScale = (wP > hP) ? hP : wP;

				if (Globals.PckImageScale > Globals.MaxPckImageScale)
					Globals.PckImageScale = Globals.MaxPckImageScale;

				if (Globals.PckImageScale < Globals.MinPckImageScale)
					Globals.PckImageScale = Globals.MinPckImageScale;
			}

			_mapView.SetupMapSize();
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
