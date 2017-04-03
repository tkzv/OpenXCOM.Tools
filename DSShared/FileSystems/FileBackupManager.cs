using System;
using System.IO;


namespace DSShared.FileSystems
{
	/// <summary>
	/// </summary>
	public static class FileBackupManager
	{
		/// <summary>
		/// Backups a file.
		/// </summary>
		public static void Backup(string pfe) // pfe=path+file+ext
		{
			string dir = Path.GetDirectoryName(pfe);
			dir = Path.Combine(dir, "backups");

			Directory.CreateDirectory(dir);

			string file = Path.GetFileName(pfe);

			string pfeOut = Path.Combine(dir, file + Path.GetRandomFileName());
			while (File.Exists(pfeOut))
				pfeOut = Path.Combine(dir, file + Path.GetRandomFileName());

			File.Copy(pfe, pfeOut);
		}
	}
}
