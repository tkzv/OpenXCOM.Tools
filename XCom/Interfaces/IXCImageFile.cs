using System;
using System.Collections.Generic;

using DSShared.Interfaces;
using DSShared.Loadable;


namespace XCom.Interfaces
{
	/// <summary>
	/// Class that contains all information needed to read/save image files and
	/// collections. This class should not be instantiated directly. Objects
	/// from derived classes will be created and tracked on startup.
	/// </summary>
	public class IXCImageFile
		:
			IAssemblyLoadable,
			IDialogFilter
	{
		private Palette _palDefault = Palette.UfoBattle;
		/// <summary>
		/// Defines the initial palette for the sprites.
		/// </summary>
		public Palette DefaultPalette
		{
			get { return _palDefault; }
			protected set { _palDefault = value; }
		}

		private System.Drawing.Size _imageSize;
		/// <summary>
		/// Image size that will be loaded.
		/// </summary>
		public System.Drawing.Size ImageSize
		{
			get { return _imageSize; }
			protected set { _imageSize = value; }
		}

		private XCFileOptions _fileOptions = new XCFileOptions();
		/// <summary>
		/// Flags that tell the OS where each filetype should be displayed.
		/// </summary>
		public XCFileOptions FileOptions
		{
			get { return _fileOptions; }
		}

		private string _ext = ".default";
		/// <summary>
		/// Gets/Sets the file-extension.
		/// </summary>
		public string FileExtension
		{
			get { return _ext; }
			protected set { _ext = value; }
		}

		private string _desc = "Default Description";
		/// <summary>
		/// Gets/Sets a description of this filetype.
		/// </summary>
		public string Description
		{
			get { return _desc; }
			protected set { _desc = value; }
		}

		private string _author = "Default Author";
		/// <summary>
		/// Gets/Sets who wrote this filetype.
		/// </summary>
		public string Author
		{
			get { return _author; }
			protected set { _author = value; }
		}

		private string _singleFile;
		/// <summary>
		/// The complete file.extension that this object will open. If null,
		/// then this object will open files based on the FileExtension property.
		/// </summary>
		public virtual string SingleFile
		{
			get { return _singleFile; }
			protected set { _singleFile = value; }
		}

		public enum Filter
		{
			Save,
			Open,
			Custom,
			Bmp
		};

		#region IDialogFilter (interface) implementation
		private string _brief = "Default Brief";
		/// <summary>
		/// See: IDialogFilter.Brief
		/// </summary>
		public string Brief
		{
			get { return _brief; }
			protected set { _brief = value; } // NOTE: the setter is not part of the interface.
		}

		/// <summary>
		/// See: IDialogFilter.FileFilter
		/// </summary>
		public string FileFilter
		{
			get
			{
				return (_singleFile != null) ? String.Format(
														System.Globalization.CultureInfo.CurrentCulture,
														"{0} - {1}|{0}",
														_singleFile, _brief)
											 : String.Format(
														System.Globalization.CultureInfo.CurrentCulture,
														"*{0} - {1}|{0}",
														_ext, _brief);
			}
		}
		#endregion

		#region AssemblyLoadable (interface) implementation
		/// <summary>
		/// See: AssemblyLoadable.RegisterFile
		/// </summary>
		/// <returns></returns>
		public bool RegisterFile()
		{
//			Console.WriteLine("{0} registered: {1}", this.GetType(), GetType() != typeof(IXCFile));
			XConsole.AdZerg(string.Format(
										System.Globalization.CultureInfo.InvariantCulture,
										"{0} registered: {1}",
										GetType(),
										GetType() != typeof(IXCImageFile)));

			return (GetType() != typeof(IXCImageFile));
		}

		/// <summary>
		/// See: AssemblyLoadable.Unload()
		/// </summary>
		public void Unload()
		{}
		#endregion


		/// <summary>
		/// It is not recommended to instantiate objects of this type directly.
		/// See PckView.XCProfile for a generic implementation that does not
		/// throw runtime exceptions
		/// </summary>
		/// <param name="width">default width</param>
		/// <param name="height">default height</param>
		public IXCImageFile(int width, int height)
		{
			_imageSize = new System.Drawing.Size(width, height);
			_brief = this.GetType().ToString();
		}
		/// <summary>
		/// Creates an object of this class with width and height of 0.
		/// </summary>
		public IXCImageFile()
			:
				this(0, 0)
		{}


		// From HERE TO THE END OF THIS CLASS will Crash and Burn.

		/// <summary>
		/// Calls LoadFile with ImageSize.Width and ImageSize.Height.
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="file"></param>
		/// <returns></returns>
		public XCImageCollection LoadFile(string dir, string file)
		{
			return LoadFile(
						dir,
						file,
						ImageSize.Width,
						ImageSize.Height);
		}

		/// <summary>
		/// Method that calls the overloaded load function in order to do some
		/// similar functionality across all instances of this class.
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="file"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public XCImageCollection LoadFile(
				string dir,
				string file,
				int width,
				int height)
		{
			return LoadFile(
						dir,
						file,
						width,
						height,
						_palDefault);
		}
		/// <summary>
		/// Have another LoadFile().
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="file"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="pal"></param>
		/// <returns></returns>
		public XCImageCollection LoadFile(
				string dir,
				string file,
				int width,
				int height,
				Palette pal)
		{
			var collection = LoadFileOverride(
											dir,
											file,
											width,
											height,
											pal);

			if (collection != null)
			{
				collection.IXCFile = this;
				collection.Path = dir;
				collection.Name = file;

				if (collection.Pal == null)
					collection.Pal = _palDefault;
			}

			return collection;
		}

		/// <summary>
		/// Loads a file and return a collection of images.
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="file"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="pal"></param>
		/// <returns></returns>
		private XCImageCollection LoadFileOverride(
				string dir,
				string file,
				int width,
				int height,
				Palette pal)
		{
			throw new Exception("muahahahha IXCImageFile.LoadFileOverride DOES NOTHING (except clock a few more CPU cycles)!");
		}

		/// <summary>
		/// Saves a collection
		/// </summary>
		/// <param name="dir">directory to save to</param>
		/// <param name="file">file</param>
		/// <param name="images">images to save in this format</param>
		public void SaveCollection(string dir, string file, XCImageCollection images)
		{
			throw new Exception("muahahahha IXCImageFile.SaveCollection DOES NOTHING (except clock a few more CPU cycles)!");
		}
	}


	/// <summary>
	/// class XCFileOptions
	/// </summary>
	public sealed class XCFileOptions
	{
		private readonly Dictionary<IXCImageFile.Filter, bool> _filters;

		private int _bpp = 8;
		public int BitDepth
		{
			get { return _bpp; }
			set { _bpp = value; }
		}

		private int _pad = 1;
		public int Pad
		{
			get { return _pad; }
			set { _pad = value; }
		}

		public bool this[IXCImageFile.Filter filter]
		{
			get { return _filters[filter]; }
		}


		public XCFileOptions()
			:
				this(true, true, true, true)
		{}

		public XCFileOptions(
				bool save,
				bool bmp,
				bool open,
				bool custom)
		{
			_filters = new Dictionary<IXCImageFile.Filter, bool>();

			Init(
				save,
				bmp,
				open,
				custom);
		}


		public void Init(
				bool save,
				bool bmp,
				bool open,
				bool custom)
		{
			_filters[IXCImageFile.Filter.Save]   = save;
			_filters[IXCImageFile.Filter.Bmp]    = bmp;
			_filters[IXCImageFile.Filter.Open]   = open;
			_filters[IXCImageFile.Filter.Custom] = custom;
		}
	}
}
