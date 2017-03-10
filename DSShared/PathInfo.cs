using System;


namespace DSShared
{
	/// <summary>
	/// Class to help pass around file paths.
	/// </summary>
	public class PathInfo
	{
		private string _path = "";
		private string _file = "";
		private string _ext  = "";


		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.PathInfo"/> class.
		/// </summary>
		/// <param name="path">the path</param>
		/// <param name="file">the file</param>
		/// <param name="ext">the extension</param>
		public PathInfo(string path, string file, string ext)
		{
			_path = path;
			_file = file;
			_ext  = ext;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.PathInfo"/> class.
		/// </summary>
		/// <param name="fullPath">the full path</param>
		public PathInfo(string fullPath)
			:
			this(fullPath, true)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.PathInfo"/> class.
		/// </summary>
		/// <param name="fullPath">the full path</param>
		/// <param name="parseFile">if set to <c>true</c> the path will be broken down into
		/// filename and extension parts. You should pass false if the path string does not
		/// describe a file location</param>
		public PathInfo(string fullPath, bool parseFile)
		{
			if (parseFile && fullPath.IndexOf(".", StringComparison.Ordinal) > 0)
			{
				_path = fullPath.Substring(0, fullPath.LastIndexOf(@"\", StringComparison.Ordinal));
				_file = fullPath.Substring(fullPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
				_file = _file.Substring(0, _file.LastIndexOf(".", StringComparison.Ordinal));
				_ext  = fullPath.Substring(fullPath.LastIndexOf(".", StringComparison.Ordinal) + 1);
			}
			else
			{
				_path = fullPath;
				_file = String.Empty;
				_ext  = String.Empty;
			}
		}


		/// <summary>
		/// Extension part of the path.
		/// </summary>
		public string Ext
		{
			get { return _ext; }
			set { _ext = value; }
		}

		/// <summary>
		/// Filename part of the path without extension.
		/// </summary>
		public string File
		{
			get { return _file; }
			set { _file = value; }
		}

		/// <summary>
		/// Directory path.
		/// </summary>
		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}

		/// <summary>
		/// First checks if the directory exists then checks if file exists.
		/// </summary>
		/// <returns>System.IO.File.Exists(ToString())</returns>
		public bool Exists()
		{
			return (System.IO.Directory.Exists(_path) && System.IO.File.Exists(ToString()));
		}

		/// <summary>
		/// Calling this will create the directory if it does not exist.
		/// </summary>
		public void EnsureDirectoryExists()
		{
			if (!System.IO.Directory.Exists(_path))
				System.IO.Directory.CreateDirectory(_path);
		}

/*		/// <summary>
		/// String representation of this path with the supplied extension added
		/// on instead of the one the object was constructed with.
		/// </summary>
		/// <param name="ext">the extension that will replace the current one</param>
		/// <returns></returns>
		public string ToString(string ext)
		{
			return _path + @"\" + _file + "." + ext;
		} */

		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override string ToString()
		{
			return (_ext.Length != 0) ? _path + @"\" + _file + "." + _ext
									  : _path + @"\" + _file;
		}
	}
}
