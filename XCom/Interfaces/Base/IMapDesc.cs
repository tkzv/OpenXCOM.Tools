using System;

using DSShared.Interfaces;
using DSShared.Loadable;


namespace XCom.Interfaces.Base
{
	public class IMapDesc // psst. This isn't an interface.
		:
			IAssemblyLoadable,
			IDialogFilter
	{
		// TODO: Dialog Filters do not appear to be implemented. cf, XCImageFile.
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

		public string Label
		{ get; protected set; }


		public IMapDesc(string label)
		{
			Label = label;
		}


		public override string ToString() // isUsed yes/no
		{
			return Label;
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
		/// See: AssemblyLoadable.Unload
		/// </summary>
		public virtual void Unload()
		{}
	}
}
