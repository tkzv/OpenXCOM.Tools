using System;
using System.IO;


namespace DSShared.FileSystems
{
	/// <summary>
	/// </summary>
	public interface IFileBackupManager
	{
		/// <summary>
		/// Backups a file.
		/// </summary>
		void Backup(string filePath);
	}

	/// <summary>
	/// </summary>
	public class FileBackupManager
		:
		IFileBackupManager
	{
		/// <summary>
		/// Backups a file.
		/// </summary>
		public void Backup(string filePath)
		{
			var dir = Path.GetDirectoryName(filePath);
			dir = Path.Combine(dir, "Backups");

			Directory.CreateDirectory(dir);

			var name = Path.GetFileName(filePath);

			var finalPath = Path.Combine(dir, name + Path.GetRandomFileName());
			while (File.Exists(finalPath))
				finalPath = Path.Combine(dir, name + Path.GetRandomFileName());

			File.Copy(filePath, finalPath);
		}
	}
}
