using System;


namespace XCom
{
	public abstract class FileDesc
	{
		private readonly string _path;
		public string Path
		{
			get { return _path; }
		}


		protected FileDesc(string path)
		{
			_path = path;
		}


		public abstract void Save(string outFile);
	}
}
