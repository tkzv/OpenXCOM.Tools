using System;


namespace XCom.Interfaces.Base
{
	/// <summary>
	/// Provides all the necessary information to animate a tilepart.
	/// </summary>
	public class TileBase
	{
		protected XCImage[] _images;

		private IInfo _info;


		public TileBase(int id)
		{
			Id = id;
			MapId = -1;
			_info = null;
		}


		/// <summary>
		/// This is the ID unique to this TileBase after it has been loaded.
		/// </summary>
		public int Id
		{ get; protected set; }

		/// <summary>
		/// This is the ID by which the map knows this tile by.
		/// </summary>
		public int MapId
		{ get; set; }

		/// <summary>
		/// Gets an image at the specified animation frame.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public XCImage this[int i]
		{
			get { return _images[i]; }
			set { _images[i] = value; }
		}

		/// <summary>
		/// Gets the image array used to animate this tile.
		/// </summary>
//		public System.Collections.ObjectModel.Collection<XCImage> Images
		public XCImage[] Images
		{
			get { return _images; }
			set { _images = value; }
		}

		/// <summary>
		/// The Info object that has additional flags and information about this tile.
		/// </summary>
		public IInfo Info
		{
			get { return _info; }
			set { _info = value; }
		}
	}
}
