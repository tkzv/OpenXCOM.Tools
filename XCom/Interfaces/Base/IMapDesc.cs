using System;
using System.Collections.Generic;
using System.IO;

using DSShared.Interfaces;
using DSShared.Loadable;


namespace XCom.Interfaces.Base
{
	public class IMapDesc
		:
		IAssemblyLoadable,
		IOpenSave
	{
		protected string _name;
		protected string _explorerDesc = "no Description";
		protected string _ext = ".unused";


		public IMapDesc(string name)
		{
			_name = name;
		}


		public override string ToString()
		{
			return _name;
		}

		public string Name
		{
			get { return _name; }
		}

		public virtual void Unload()
		{}

		public virtual string FileFilter
		{
			get { return "*" + _ext + " - " + _explorerDesc + "|*" + _ext; }
		}

		/// <summary>
		/// See: AssemblyLoadable.RegisterFile
		/// </summary>
		/// <returns></returns>
		public virtual bool RegisterFile()
		{
			return GetType() != typeof(IMapDesc);
		}

		/// <summary>
		/// See: AssemblyLoadable.ExplorerDescription
		/// </summary>
		public virtual string ExplorerDescription
		{
			get { return _explorerDesc; }
		}
	}
}
