using System;
using System.IO;


namespace DSShared
{
	/// <summary>
	/// Class to help pass around file paths.
	/// </summary>
	public class PathInfo
	{
		#region Fields (static)
		// path-keys
		public const string SettingsFile = "MV_SettingsFile";	// -> MVSettings.cfg

		public const string MapViewers   = "MV_ViewersFile";	// -> MapViewers.yml
		public const string MapTilesets  = "MV_TilesetsFile";	// -> MapConfig.yml TODO: change to MapTilesets.yml

		// TODO: key MapResources

		// YAML files
		public const string ConfigViewers    = "MapViewers.yml";
		public const string ConfigViewersOld = "MapViewers.old";

		public const string ConfigResources    = "MapDirectory.yml"; // TODO: change to MapResources.yml
//		public const string ConfigResourcesOld = "MapDirectory.old"; // TODO: change to MapResources.old

		public const string ConfigTilesets    = "MapConfig.yml"; // TODO: change to MapTilesets.yml
		public const string ConfigTilesetsOld = "MapConfig.old"; // TODO: change to MapTilesets.old
		#endregion


		#region Properties
		private readonly string _file;
		private readonly string _extension;

		private readonly string _path;
		/// <summary>
		/// Directory path.
		/// </summary>
		public string DirectoryPath
		{
			get { return _path; }
		}

		/// <summary>
		/// Gets the fullpath.
		/// kL_question: Can a file or directory end with "."
		/// </summary>
		public string FullPath
		{
			get
			{
				string fullpath = Path.Combine(_path, _file);
				if (_extension.Length != 0)
					fullpath += "." + _extension;

				return fullpath;
			}
		}
		#endregion


		#region cTor
		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.PathInfo"/> class.
		/// </summary>
		/// <param name="path">the path</param>
		/// <param name="file">the file</param>
		/// <param name="extension">the extension</param>
		public PathInfo(
				string path,
				string file,
				string extension)
		{
			_path      = path;
			_file      = file;
			_extension = extension;
		}
		#endregion


		#region Methods
		/// <summary>
		/// Checks if the file exists.
		/// </summary>
		/// <returns></returns>
		public bool FileExists()
		{
			return File.Exists(FullPath);
		}

		/// <summary>
		/// Creates the directory if it does not exist.
		/// </summary>
		public void CreateDirectory()
		{
			Directory.CreateDirectory(_path);
		}
		#endregion
	}
}

//		/// <summary>
//		/// Initializes a new instance of the <see cref="T:DSShared.PathInfo"/> class.
//		/// </summary>
//		/// <param name="fullPath">the full path</param>
//		public PathInfo(string fullPath)
//			:
//				this(fullPath, true)
//		{}

//		/// <summary>
//		/// Initializes a new instance of the <see cref="T:DSShared.PathInfo"/> class.
//		/// </summary>
//		/// <param name="fullPath">the full path</param>
//		/// <param name="parseFile">if set to <c>true</c> the path will be broken down into
//		/// filename and extension parts. You should pass false if the path string does not
//		/// describe a file location</param>
//		public PathInfo(string fullPath, bool parseFile)
//		{
//			if (parseFile && fullPath.IndexOf(".", StringComparison.Ordinal) > 0)
//			{
//				_path = fullPath.Substring(0, fullPath.LastIndexOf(@"\", StringComparison.Ordinal));
//				_file = fullPath.Substring(fullPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
//				_file = _file.Substring(0, _file.LastIndexOf(".", StringComparison.Ordinal));
//				_ext  = fullPath.Substring(fullPath.LastIndexOf(".", StringComparison.Ordinal) + 1);
//			}
//			else
//			{
//				_path = fullPath;
//				_file = String.Empty;
//				_ext  = String.Empty;
//			}
//		}

//		/// <summary>
//		/// Extension part of the path.
//		/// </summary>
//		public string Ext
//		{
//			get { return _ext; }
//			set { _ext = value; }
//		}

//		/// <summary>
//		/// Filename part of the path without extension.
//		/// </summary>
//		public string File
//		{
//			get { return _file; }
//			set { _file = value; }
//		}

//		/// <summary>
//		/// String representation of this path with the supplied extension added
//		/// on instead of the one the object was constructed with.
//		/// </summary>
//		/// <param name="ext">the extension that will replace the current one</param>
//		/// <returns></returns>
//		public string ToString(string ext)
//		{
//			return _path + @"\" + _file + "." + ext;
//		}
