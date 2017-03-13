using System;
using System.Drawing;


namespace MapView
{
	/// <summary>
	/// Displays the About box.
	/// </summary>
	public partial class AboutWindow
		:
		System.Windows.Forms.Form
	{
		public AboutWindow()
		{
			InitializeComponent();

			string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major + "."
					   + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor + "."
					   + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build + "."
					   + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision;

			lblVersion.Text = "MapView " + ver;
#if DEBUG
			lblVersion.Text += " kL_d";
#else
			lblVersion.Text += " kL_r";
#endif
			lblVersion.Text += Environment.NewLine + Environment.NewLine + "2017";
		}

		private Point _loc;
		private bool _moving;
		private double _lastPoint;

		private void MoveTimer_Tick(object sender, EventArgs e)
		{
			MoveWindow();
		}

		private void MoveWindow()
		{
			try
			{
				MoveTimer.Interval = 1000;
				_moving = true;
				Location = GetLocation(_lastPoint += 0.01);
			}
			finally
			{
				_moving = false;
			}
		}

		private Point GetLocation(double delta)
		{
			var loc = Location;
			loc.X = (int)(_loc.X + (Math.Sin(delta) * 50));
			loc.Y = (int)(_loc.Y + (Math.Cos(delta) * 50));
			return loc;
		}

		private void AboutWindow_LocationChanged(object sender, EventArgs e)
		{
			if (!_moving)
			{
				var locationBeforeMove = new Size(GetLocation(_lastPoint));
				var distance = new Size(Location - locationBeforeMove);
				_loc += distance;

				MoveTimer.Interval = 1000;
				MoveTimer.Enabled = true;
			}
		}

		private void AboutWindow_Shown(object sender, EventArgs e)
		{
			_loc = Location;
			MoveWindow();
		}

		private void keyClose(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == System.Windows.Forms.Keys.Escape)
			{
				Close();
			}
		}
	}
}
