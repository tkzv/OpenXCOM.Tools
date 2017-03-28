using System;


namespace XCom.Interfaces.Base
{
	/// <summary>
	/// Provides all the necessary information to animate a tilepart.
	/// </summary>
	public class TileBase
	{
		private IMcdInfo _info;
		/// <summary>
		/// The Info object that has additional flags and information about this tile.
		/// </summary>
		public IMcdInfo Info
		{
			get { return _info; }
			set { _info = value; }
		}

		private XCImage[] _images;
		/// <summary>
		/// Gets the image-array used to animate this tile.
		/// </summary>
//		public System.Collections.ObjectModel.Collection<XCImage> Images
		public XCImage[] Images
		{
			get { return _images; }
			set { _images = value; }
		}
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
		/// This is the ID unique to this TileBase after it has been loaded.
		/// </summary>
		public int Id
		{ get; protected set; }

		/// <summary>
		/// This is the ID by which the Map knows this tile by.
		/// </summary>
		public int TileListId
		{ get; set; }


		/// <summary>
		/// Instantiates a blank tile.
		/// </summary>
		/// <param name="id"></param>
		public TileBase(int id)
		{
			Id = id;
			TileListId = -1;
//			_info = null;
		}
	}
}
