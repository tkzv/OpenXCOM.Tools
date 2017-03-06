using System;
using System.Collections.Generic;
using System.Text;


namespace XCom
{
	public abstract class FileDesc
	{
		private readonly string path;

		protected FileDesc(string path1)
		{ path = path1; }

		public abstract void Save(string outFile);

		public string Path
		{
			get { return path; }
		}
	}
}
