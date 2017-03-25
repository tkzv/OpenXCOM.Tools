using System;

using DSShared.Interfaces;
using DSShared.Loadable;


namespace XCom.Interfaces.Base
{
	public class IMapDesc
		:
		IAssemblyLoadable,
		IOpenSave
	{
		private const string _ext = ".notused";

		private const string _fileFilter = "no description";
		/// <summary>
		/// See: AssemblyLoadable.ExplorerDescription
		/// </summary>
		public virtual string ExplorerDescription
		{
			get { return _fileFilter; }
		}

		private string _name;
		public string Name
		{
			get { return _name; }
		}


		public IMapDesc(string name)
		{
			_name = name;
		}


		public override string ToString()
		{
			return _name;
		}

		public virtual void Unload()
		{}

		public virtual string FileFilter
		{
			get
			{
				return string.Format(
								System.Globalization.CultureInfo.CurrentCulture,
								"*{0} - {1}|*{0}",
								_ext, _fileFilter);
			}
		}

		/// <summary>
		/// See: AssemblyLoadable.RegisterFile
		/// </summary>
		/// <returns></returns>
		public virtual bool RegisterFile()
		{
			return (GetType() != typeof(IMapDesc));
		}
	}
}
