using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;


namespace MapView
{
	/// <summary>
	/// Displays the About box.
	/// </summary>
	internal sealed partial class AboutWindow
		:
			Form
	{
		public AboutWindow()
		{
			InitializeComponent();

			string ver = Assembly.GetExecutingAssembly().GetName().Version.Major + "."
					   + Assembly.GetExecutingAssembly().GetName().Version.Minor + "."
					   + Assembly.GetExecutingAssembly().GetName().Version.Build + "."
					   + Assembly.GetExecutingAssembly().GetName().Version.Revision;

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

		private void keyClose(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
				case Keys.Enter:
					Close();
					break;
			}
		}
	}
}
