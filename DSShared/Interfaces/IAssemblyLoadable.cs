using System;


namespace DSShared.Loadable
{
	/// <summary>
	/// Top-level interface for user-created addons. The classes that will be
	/// actively searched for at runtime will be defined by the program's author.
	/// </summary>
	public interface IAssemblyLoadable
	{
		/// <summary>
		/// Unload() will be called immediately if this returns false.
		/// </summary>
		/// <returns></returns>
		bool RegisterFile();

		/// <summary>
		/// Called when this object needs to detach itself from the system.
		/// Usually with program shutdown. Any exceptions raised will not be
		/// caught. You buy it you break it.
		/// </summary>
		void Unload();
	}
}
