using System;
using System.Collections.Generic;

using XCom.Interfaces;


namespace XCom
{
	public class XCImageCollection
		:
			List<XCImage>
	{
		public string Name
		{ get; set; }

		public string Path
		{ get; set; }

		public XCImageFile IXCFile
		{ get; set; }

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

		public void HQ2X()
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
