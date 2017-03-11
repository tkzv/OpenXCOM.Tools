using System;
using System.Collections.Generic;

using DSShared.Interfaces;
using DSShared.Loadable;


namespace XCom.Interfaces
{
	/// <summary>
	/// Class that contains all information needed to read/save image files and
	/// collections.
	/// This class should not be instantiated directly. Objects from derived
	/// classes will be created and tracked on startup.
	/// </summary>
	public class IXCImageFile
		:
		IAssemblyLoadable,
		IOpenSave
	{
		protected Palette defPal = Palette.UFOBattle;
//		protected Palette defPal = Palette.TFTDBattle;

		protected System.Drawing.Size imageSize;

		protected xcFileOptions fileOptions = new xcFileOptions();

		protected string author		= "Author";
		protected string desc		= "Description";
		protected string expDesc	= "Bad Description";
		protected string ext		= ".bad";

		protected string singleFile = null;

		public enum Filter
		{
			Save,
			Open,
			Custom,
			Bmp
		};


		#region AssemblyLoadable implementation

		public virtual string FileFilter
		{
			get
			{
				if (singleFile != null)
					return singleFile + " - " + expDesc + "|" + singleFile;

				return "*" + ext + " - " + expDesc + "|*" + ext;
			}
		}

		/// <summary>
		/// See: AssemblyLoadable.RegisterFile
		/// </summary>
		/// <returns></returns>
		public virtual bool RegisterFile()
		{
//			Console.WriteLine("{0} registered: {1}", this.GetType(), GetType() != typeof(IXCFile));
			xConsole.AddLine(string.Format(
										"{0} registered: {1}",
										this.GetType(),
										GetType() != typeof(IXCImageFile)));

			return GetType() != typeof(IXCImageFile);
		}

		/// <summary>
		/// See: AssemblyLoadable.ExplorerDescription
		/// </summary>
		public virtual string ExplorerDescription
		{
			get { return expDesc; }
		}
		#endregion


		/// <summary>
		/// Read only access to the file extension.
		/// </summary>
		public virtual string FileExtension
		{
			get { return ext; }
		}

		public void Unload()
		{}

		/// <summary>
		/// It is not recommended to instantiate objects of this type directly.
		/// See PckView.xcProfile for a generic implementation that does not
		/// throw runtime exceptions
		/// </summary>
		/// <param name="width">default width</param>
		/// <param name="height">default height</param>
		public IXCImageFile(int width, int height)
		{
			imageSize = new System.Drawing.Size(width,height);
			expDesc = this.GetType().ToString();
		}

		/// <summary>
		/// Creates an object of this class with width and height of 0.
		/// </summary>
		public IXCImageFile()
			:
			this(0, 0)
		{}

		/// <summary>
		/// Gets who wrote this class.
		/// </summary>
		public string Author
		{
			get { return author; }
		}

		/// <summary>
		/// Short description of this class.
		/// </summary>
		public string Description
		{
			get { return desc; }
		}

		/// <summary>
		/// Loads a file and return a collection of images.
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="file"></param>
		/// <param name="imgWid"></param>
		/// <param name="imgHei"></param>
		/// <param name="pal"></param>
		/// <returns></returns>
		protected virtual XCImageCollection LoadFileOverride(
				string directory,
				string file,
				int imgWid,
				int imgHei,
				Palette pal)
		{
			throw new Exception("Override not yet implemented: IXCFile::LoadFileOverride(...)");
		}

		/// <summary>
		/// Calls LoadFile with ImageSize.Width and ImageSize.Height.
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="file"></param>
		/// <returns></returns>
		public XCImageCollection LoadFile(string directory, string file)
		{
			return LoadFile(
						directory,
						file,
						ImageSize.Width,
						ImageSize.Height);
		}

		/// <summary>
		/// Method that calls the overloaded load function in order to do some
		/// similar functionality across all instances of this class.
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="file"></param>
		/// <param name="imgWid"></param>
		/// <param name="imgHei"></param>
		/// <returns></returns>
		public XCImageCollection LoadFile(
				string directory,
				string file,
				int imgWid,
				int imgHei)
		{
			return LoadFile(
						directory,
						file,
						imgWid,
						imgHei,
						defPal);
		}

		public XCImageCollection LoadFile(
				string directory,
				string file,
				int imgWid,
				int imgHei,
				Palette pal)
		{
			XCImageCollection ixc = LoadFileOverride(
												directory,
												file,
												imgWid,
												imgHei,
												pal);

			if (ixc != null)
			{
				ixc.IXCFile = this;
				ixc.Path = directory;
				ixc.Name = file;

				if (ixc.Pal == null)
					ixc.Pal = defPal;
			}

			return ixc;
		}

		/// <summary>
		/// flags that tell the system where each mod should be displayed
		/// </summary>
		public xcFileOptions FileOptions
		{
			get { return fileOptions; }
		}

		/// <summary>
		/// Saves a collection
		/// </summary>
		/// <param name="directory">directory to save to</param>
		/// <param name="file">file</param>
		/// <param name="images">images to save in this format</param>
		public virtual void SaveCollection(string directory, string file, XCImageCollection images)
		{
			throw new Exception("Override not yet implemented: IXCFile::SaveCollection(...)");
		}

		/// <summary>
		/// Defines the initial coloring of the sprites when loaded
		/// </summary>
		public virtual Palette DefaultPalette
		{
			get { return defPal; }
		}

		/// <summary>
		/// Image size that will be loaded
		/// </summary>
		public System.Drawing.Size ImageSize
		{
			get { return imageSize; }
		}

		/// <summary>
		/// The complete file.extension that this object will open. If null,
		/// then this object will open files based on the FileExtension property.
		/// </summary>
		public virtual string SingleFileName
		{
			get { return singleFile; }
		}
	}


		public class xcFileOptions
	{
		private Dictionary<IXCImageFile.Filter, bool> filters;
		private int bitDepth = 8;
		private int space = 1;

		public xcFileOptions()
			:
			this(true, true, true, true)
		{}

		public xcFileOptions(
				bool save,
				bool bmp,
				bool open,
				bool custom)
		{
			filters = new Dictionary<IXCImageFile.Filter, bool>();
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
			filters[IXCImageFile.Filter.Bmp]	= bmp;
			filters[IXCImageFile.Filter.Custom]	= custom;
			filters[IXCImageFile.Filter.Open]	= open;
			filters[IXCImageFile.Filter.Save]	= save;
		}

		public bool this[IXCImageFile.Filter filter]
		{
			get { return filters[filter]; }
		}

		public int BitDepth
		{
			get { return bitDepth; }
			set { bitDepth = value; }
		}

		public int Space
		{
			get { return space; }
			set { space = value; }
		}
	}
}
