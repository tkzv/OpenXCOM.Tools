using System;

using DSShared.Interfaces;
using DSShared.Loadable;


namespace XCom.Interfaces.Base
{
	public class IMapDesc
		:
		IAssemblyLoadable,
		IDialogFilter
	{
		// TODO: Dialog Filters do not appear to be implemented. cf, IXCImageFile.
		private const string _ext = ".default";

		private const string _brief = "Default Brief";
		/// <summary>
		/// See: IDialogFilter.Brief
		/// </summary>
		public virtual string Brief // needed only to satisfy base interface.
		{
			get { return _brief; }
		}
		/// <summary>
		/// See: IDialogFilter.FileFilter
		/// </summary>
		public string FileFilter	// needed only to satisfy base interface and _PckView.
		{							// see LoadOfType.CreateFilter()
			get
			{
				return string.Format(
								System.Globalization.CultureInfo.CurrentCulture,
								"*{0} - {1}|*{0}",
								_ext, _brief);
			}
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

		/// <summary>
		/// See: AssemblyLoadable.RegisterFile
		/// </summary>
		/// <returns></returns>
		public virtual bool RegisterFile()
		{
			return (GetType() != typeof(IMapDesc));
		}

		/// <summary>
		/// See: AssemblyLoadable.RegisterFile
		/// </summary>
		public virtual void Unload()
		{}
	}
}
