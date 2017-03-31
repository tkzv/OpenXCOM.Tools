using System;
using System.Collections.Generic;
using System.Reflection;

using DSShared.Interfaces;


namespace DSShared.Loadable
{
	/// <summary>
	/// This class will scan an assembly for a specific type and manage a
	/// singleton list of those objects. Originally designed for user-created
	/// save/load plugins.
	/// </summary>
	/// <typeparam name="T">Objects of this type are stored in this class</typeparam>
	public sealed class LoadOfType<T> where T
		:
			IAssemblyLoadable,
			IDialogFilter,
			new()
	{
		/// <summary>
		/// Delegate for use with the OnLoad event.
		/// </summary>
		/// <param name="sender">the LoadOfType object that fired the event</param>
		/// <param name="e">Args for the event</param>
		public delegate void TypeLoadDelegate(object sender, LoadOfType<T>.TypeLoadArgs e);
		/// <summary>
		/// Event that gets called when a type has been loaded from an assembly
		/// and has returned true for registration
		/// </summary>
		public event TypeLoadDelegate OnLoad;


//		private Dictionary<int, T> filterDictionary;
//		private string openFileFilter = "";

		private readonly List<T> _allLoaded;


		/// <summary>
		/// cTor.
		/// </summary>
		public LoadOfType()
		{
//			filterDictionary = new Dictionary<int, T>();
			_allLoaded = new List<T>();
		}


		// <summary>
		// Returns a list of objects that meet the filter requirements.
		// </summary>
		// <param name="filterObj"></param>
		// <returns></returns>
		//public List<T> FilterBy(IFilter<T> filterObj)
		//{
		//	List<T> filterList = new List<T>();
		//	foreach (T obj in filterList)
		//		if (filterObj.FilterObj(obj))
		//			filterList.Add(obj);
		//	return filterList;
		//}

		/// <summary>
		/// A List of all the types that have been found so far.
		/// </summary>
		public List<T> AllLoaded
		{
			get { return _allLoaded; }
		}

		// <summary>
		// A string to use for an OpenFileDialog. The string will only be
		// created once and cached for later use. If the file list changes, use
		// CreateFilter() to build a new string.
		// </summary>
		//public string OpenFileFilter
		//{
		//	get
		//	{
		//		if (openFileFilter == "")
		//			CreateFilter();
		//		return openFileFilter;
		//	}
		//}

		// <summary>
		// Returns the object at a specific filter index.
		// </summary>
		// <param name="index"></param>
		// <returns></returns>
		//public T GetFromFilter(int index)
		//{
		//	return filterDictionary[index];
		//}

		//public string CreateFilter()
		//{
		//	return "";
		//}

		/// <summary>
		/// Forces a recreation of the filter string.
		/// </summary>
		/// <returns></returns>
		//public string CreateFilter()
		//{
		//	openFileFilter = "";
		//	bool two = true;
		//	int filterIdx = 1; //filter index starts at 1
		//	foreach (T fc in allLoaded)
		//	{
		//		if (fc.FilterIndex != -1)
		//		{
		//			if (!two)
		//				openFileFilter += "|";
		//			else
		//				two = false;

		//			openFileFilter += fc.FileFilter;
		//			fc.FilterIndex = filterIdx++;
		//			filterDictionary[fc.FilterIndex] = fc;
		//		}
		//	}
		//	return openFileFilter;
		//}

		public string CreateFilter(IFilter<T> filter, Dictionary<int, T> filterDictionary)
		{
			filterDictionary.Clear();

			string fileFilter = String.Empty;
			int filterId = 0;

			bool first = true;
			foreach (var fileType in _allLoaded)
			{
				if (filter.FilterObj(fileType))
				{
					if (first)
						first = false;
					else
						fileFilter += "|";

					fileFilter += fileType.FileFilter;
					filterDictionary.Add(++filterId, fileType); // id starts at 1
				}
			}
			return fileFilter;
		}

		//public string CreateFilter(IFilter<T> filter)
		//{
		//	string fileFilter = String.Empty;
		//	bool two = false;
		//	int filterIdx = 1; // filter index starts at 1

		//	List<T> filterList = allLoaded;

		//	if (filter != null)
		//	{
		//		filterList = new List<T>();
		//		foreach(T obj in allLoaded)
		//			if(filter.FilterObj(obj))
		//	}

		//	foreach (T fc in filterList)
		//	{}
		//	return fileFilter;
		//}

		/// <summary>
		/// Adds an object to this list and recreates the filter string.
		/// </summary>
		/// <param name="fc"></param>
		public void Add(T fc)
		{
			//Console.WriteLine("Adding file: " + fc.Brief);
			_allLoaded.Add(fc);
//			CreateFilter();
		}
		
		/// <summary>
		/// Scans an assembly for matching types. When a type is found it is
		/// created using the default constructor and stored in a list. Objects
		/// are only added to the internal list if they return true for
		/// registration.
		/// </summary>
		/// <param name="ass"></param>
		/// <returns>a list of objects of type T. The list contains all
		/// registered and unregistered objects</returns>
		public List<T> LoadFrom(Assembly ass)
		{
			// Get creatable objects from the assembly
			var objList = new List<T>();
			foreach (Type type in ass.GetTypes())
			{
				if (type.IsClass && !type.IsAbstract && typeof(T).IsAssignableFrom(type))
				{
					// if a class has no default constructor, it will fail this
					// this is why the new() constraint is placed on the LoadOfType def'n
					var ctorInfo = type.GetConstructor(new Type[]{});
					if (ctorInfo == null)
					{
						Console.Error.WriteLine("Error loading type: {0} -> No default constructor specified", type);
					}
					else
					{
						try
						{
							var fileType = (T)ctorInfo.Invoke(new object[]{});
							objList.Add(fileType);
	
							if (fileType.RegisterFile())
							{
								_allLoaded.Add(fileType);

								if (OnLoad != null)
									OnLoad(this, new TypeLoadArgs(fileType));
							}
						}
						catch(Exception ex)
						{
							Console.Error.WriteLine(
												"Error loading type: {0} -> {1}:{2}",
												type,
												ex.Message,
												ex.InnerException.Message);
						}
					}
				}
			}

//			CreateFilter();
			return objList;
		}

		/// <summary>
		/// Args class to pass on to a load event signifying that this object
		/// was successfully created from an assembly and registered properly.
		/// </summary>
		public sealed class TypeLoadArgs
			:
				EventArgs
		{
			private readonly T _obj;


			/// <summary>
			/// cTor.
			/// </summary>
			/// <param name="obj">object that has just been created and registered</param>
			public TypeLoadArgs(T obj)
				:
					base()
			{
				_obj = obj;
			}


			/// <summary>
			/// Object that has just been created and registered.
			/// </summary>
			public T LoadedObj
			{
				get { return _obj; }
			}
		}
	}
}
