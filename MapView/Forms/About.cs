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
		#region cTor
		internal About()
		{
			InitializeComponent();

			var an = Assembly.GetExecutingAssembly().GetName();
			string ver = an.Version.Major + "."
					   + an.Version.Minor + "."
					   + an.Version.Build + "."
					   + an.Version.Revision;

			lblVersion.Text = "MapView " + ver;
#if DEBUG
			lblVersion.Text += " debug";
#else
			lblVersion.Text += " release";
#endif

			lblVersion.Text += Environment.NewLine + Environment.NewLine
							+ String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"{0:yyyy MMM d}  {0:HH}:{0:mm}:{0:ss} {0:zzz}",
										DateTime.Now);
		}
		#endregion


		#region Fields
		private Point _locBase;
		private Point _loc;
		private Size _size;
		private double _lastPoint;
		#endregion


		#region Eventcalls
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

		private void OnLocationChanged(object sender, EventArgs e)
		{
			var locPre = new Size(GetLocationStep(_lastPoint));
			_loc += new Size(Location - locPre);
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
		#endregion


		#region Methods
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

		private Point GetLocationStep(double delta)
		{
			var loc = Location;
			loc.X = (int)(_loc.X + (Math.Sin(delta) * 50));
			loc.Y = (int)(_loc.Y + (Math.Cos(delta) * 50));
			return loc;
		}
		#endregion
	}
}
