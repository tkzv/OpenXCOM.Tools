using System;


namespace XCom.Interfaces.Base
{
	/// <summary>
	/// Provides all the necessary information to animate a tilepart.
	/// </summary>
	public class TileBase
	{
		/// <summary>
		/// The object that has information about the IG mechanics of this tile.
		/// </summary>
		public McdRecord Record
		{ get; protected set; }

		/// <summary>
		/// Gets the image-array used to animate this tile.
		/// </summary>
//		public System.Collections.ObjectModel.Collection<XCImage> Images
		public XCImage[] Images
		{ get; set; }

		/// <summary>
		/// Gets an image at the specified animation frame.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public XCImage this[int i]
		{
			get { return Images[i]; }
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
		internal TileBase(int id)
		{
			Id = id;
			TileListId = -1;
		}
	}
}
