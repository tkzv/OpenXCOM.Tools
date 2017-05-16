using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;


namespace MapView
{
	/// <summary>
	/// Displays the About box.
	/// </summary>
	internal sealed partial class About
		:
			Form
	{
		internal About()
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

			lblVersion.Text += Environment.NewLine + Environment.NewLine
							+ String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"{0:yyyy MMM d}  {0:HH}:{0:mm}:{0:ss} {0:zzz}",
										DateTime.Now);
		}

		private Point _locBase;
		private Point _loc;
		private Size _size;
		private double _lastPoint;

		private void OnShown(object sender, EventArgs e)
		{
			string before = String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"{0:n0}", GC.GetTotalMemory(false));
//			string after  = String.Format(
//										System.Globalization.CultureInfo.CurrentCulture,
//										"{0:n0}", GC.GetTotalMemory(true));

//			Text += " - " + before + " \u2192 " + after + " bytes"; // '\u2192' = right arrow.
			Text += " - " + before + " bytes allocated";

			_size = new Size(Width, Height);

			_locBase =
			_loc     = Location;

			MoveWindow();
		}

		private void OnTick(object sender, EventArgs e)
		{
			MoveWindow();
		}

		private void MoveWindow()
		{
			_loc = GetLocationStep(_lastPoint += 0.035);

			bool IsInsideBounds = false;
			foreach (var screen in Screen.AllScreens)
			{
				IsInsideBounds = screen.Bounds.Contains(_loc)
							  && screen.Bounds.Contains(_loc + _size);

				if (IsInsideBounds)
					break;
			}

			if (!IsInsideBounds)
				_loc = _locBase;

			Location = _loc;
		}

		private void OnLocationChanged(object sender, EventArgs e)
		{
			var locPre = new Size(GetLocationStep(_lastPoint));
			_loc += new Size(Location - locPre);
		}

		private Point GetLocationStep(double delta)
		{
			var loc = Location;
			loc.X = (int)(_loc.X + (Math.Sin(delta) * 50));
			loc.Y = (int)(_loc.Y + (Math.Cos(delta) * 50));
			return loc;
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
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
