using System;


namespace DSShared
{
	/// <summary>
	/// </summary>
	public interface IFilter<T>
	{
		/// <summary>
		/// </summary>
		bool FilterObj(T o);
	}
}
