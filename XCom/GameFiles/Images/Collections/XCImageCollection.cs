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
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private string _path;
		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}

		private Palette _pal;
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

		private int _scale = 1;

		private IXCImageFile _file;
		public IXCImageFile IXCFile
		{
			get { return _file; }
			set { _file = value; }
		}

		public void Hq2x()
		{
			foreach (XCImage image in this)
				image.HQ2X();

			_scale *= 2;
		}

		public new XCImage this[int id]
		{
			get { return (id > -1 && id < Count) ? base[id]
												 : null; }
			set
			{
				if (id > -1 && id < Count)
					base[id] = value;
				else
				{
					value.FileId = Count;
					Add(value);
				}
			}
		}

		public void Remove(int id)
		{
			RemoveAt(id);
		}
	}
}
