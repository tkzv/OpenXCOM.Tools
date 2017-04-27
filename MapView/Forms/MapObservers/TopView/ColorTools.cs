using System.Drawing;


namespace MapView.Forms.MapObservers.TopViews
{
	// Warning CA1001: Implement IDisposable on 'SolidPenBrush' because it
	// creates members of the following IDisposable types: 'Pen', 'SolidBrush'.
	internal sealed class ColorTools
	{
		#region Fields
		private readonly Pen _pen;
		private readonly Pen _penLight;
		private readonly SolidBrush _brush;
		private readonly SolidBrush _brushLight;
		#endregion

		#region Properties
		internal Pen Pen
		{
			get { return _pen; }
		}

		internal Pen LightPen
		{
			get { return _penLight; }
		}

		internal Brush Brush
		{
			get { return _brush; }
		}

		internal Brush LightBrush
		{
			get { return _brushLight; }
		}
		#endregion


		#region cTors
		/// <summary>
		/// cTors.
		/// </summary>
		/// <param name="pen"></param>
		internal ColorTools(Pen pen)
		{
			_pen      = pen;
			_penLight = new Pen(Color.FromArgb(70, pen.Color), pen.Width);

			_brush      = new SolidBrush(pen.Color);
			_brushLight = new SolidBrush(Color.FromArgb(70, pen.Color));
		}
		internal ColorTools(SolidBrush brush, float width)
		{
			_pen       = new Pen(brush.Color);
			_pen.Width = width;
			_penLight  = new Pen(Color.FromArgb(50, brush.Color), width);

			_brush      = brush;
			_brushLight = new SolidBrush(Color.FromArgb(50, brush.Color));
		}
		#endregion


/*		// MS example of IDisposable:
		// https://msdn.microsoft.com/en-us/library/ms182172.aspx
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// dispose managed resources
				<resource>.Close();
			}
			// free native resources
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		} */
	}
}
