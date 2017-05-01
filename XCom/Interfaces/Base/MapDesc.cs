using System;

//using DSShared.Interfaces;
//using DSShared.Loadable;


namespace XCom.Interfaces.Base
{
	public class MapDesc
//		: IAssemblyLoadable, IDialogFilter
	{
		#region Properties
		public string Label
		{ get; internal protected set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor. Instantiated only as the parent of XCMapDesc.
		/// </summary>
		/// <param name="label"></param>
		internal protected MapDesc(string label)
		{
			Label = label;
		}
		#endregion


		#region Methods
		public override string ToString() // isUsed yes/no
		{
			return Label;
		}
		#endregion


//		private const string _ext = ".default";

/*		private const string _brief = "Default Brief";
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
		} */

/*		/// <summary>
		/// See: AssemblyLoadable.RegisterFile
		/// </summary>
		/// <returns></returns>
		public virtual bool RegisterFile()
		{
			return (GetType() != typeof(MapDesc));
		}

		/// <summary>
		/// See: AssemblyLoadable.Unload
		/// </summary>
		public virtual void Unload()
		{} */
	}
}
