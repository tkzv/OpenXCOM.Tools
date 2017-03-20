using System;


namespace DSShared.Interfaces
{
	/// <summary>
	/// Interface to define methods to properly display information in open/save
	/// dialog boxes.
	/// </summary>
	public interface IOpenSave
	{
		/// <summary>
		/// Short description to use in the open/save file dialog.
		/// </summary>
		string ExplorerDescription
		{ get; }

		/// <summary>
		/// A string in the format of "Description|*.ext" that will be added to
		/// the open/save file dialogs.
		/// </summary>
		string FileFilter
		{ get; }
	}
}
