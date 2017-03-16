using System.Drawing;


namespace MapView.Forms.MapObservers.TopViews
{
	// Warning CA1001: Implement IDisposable on 'SolidPenBrush' because it
	// creates members of the following IDisposable types: 'Pen', 'SolidBrush'.
	public class SolidPenBrush
	{
		private readonly Pen _pen;
		private readonly Pen _penLight;
		private readonly SolidBrush _brush;
		private readonly SolidBrush _brushLight;


		public SolidPenBrush(Pen pen)
		{
			_pen      = pen;
			_penLight = new Pen(Color.FromArgb(70, pen.Color), pen.Width);

			_brush      = new SolidBrush(pen.Color);
			_brushLight = new SolidBrush(Color.FromArgb(70, pen.Color));
		}

		public SolidPenBrush(SolidBrush brush, float width)
		{
			_pen       = new Pen(brush.Color);
			_pen.Width = width;
			_penLight  = new Pen(Color.FromArgb(50, brush.Color), width);

			_brush      = brush;
			_brushLight = new SolidBrush(Color.FromArgb(50, brush.Color));
		}


		public Pen Pen
		{
			get { return _pen; }
		}

		public Pen LightPen
		{
			get { return _penLight; }
		}

		public Brush Brush
		{
			get { return _brush; }
		}

		public Brush LightBrush
		{
			get { return _brushLight; }
		}

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
