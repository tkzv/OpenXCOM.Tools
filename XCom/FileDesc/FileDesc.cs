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


		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="path"></param>
		protected FileDesc(string path)
		{
			_path = path;
		}


		public abstract void Save(string pfe);
	}
}
