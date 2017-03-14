using System;
using System.Collections.Generic;

using XCom.Interfaces;


namespace XCom
{
	public class XCImageCollection
		:
		List<XCImage>
	{
		private string _name;
		private string _path;

		private Palette _pal;

		private int _scale = 1;

		private IXCImageFile _file;


		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}

		public IXCImageFile IXCFile
		{
			get { return _file; }
			set { _file = value; }
		}

		public void Hq2x()
		{
			foreach (XCImage image in this)
				image.Hq2x();

			_scale *= 2;
		}

		public virtual Palette Pal
		{
			get { return _pal; }
			set
			{
				_pal = value;
				foreach (XCImage image in this)
					image.Image.Palette = _pal.Colors;
			}
		}

		public new XCImage this[int i]
		{
			get { return (i > -1 && i < Count) ? base[i]
											   : null; }
			set
			{
				if (i > -1 && i < Count)
					base[i] = value;
				else
				{
					value.FileId = Count;
					Add(value);
				}
			}
		}

		public void Remove(int i)
		{
			RemoveAt(i);
		}
	}
}
