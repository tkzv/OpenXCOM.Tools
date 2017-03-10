using System;
using System.Collections.Generic;
using System.IO;


namespace XCom
{
	public class ImageInfo
		:
		FileDesc
	{
		private readonly Dictionary<string, ImageDescriptor> _images;


		public ImageInfo(string inFile, VarCollection vars)
			:
			base(inFile)
		{
			_images = new Dictionary<string, ImageDescriptor>();
			Load(inFile, vars);
		}


		public ImageDescriptor this[string name]
		{
			get
			{
				var key = name.ToUpper();
				return (_images.ContainsKey(key)) ? _images[key]
												  : null;

			}
			set { _images[name.ToUpper()] = value; }
		}

		public void Load(string inFile, VarCollection vars)
		{
			using (var sr = new StreamReader(File.OpenRead(inFile)))
			{
				vars = new VarCollection(sr, vars);

				KeyVal keyVal;
				while ((keyVal = vars.ReadLine()) != null)
				{
					var img = new ImageDescriptor(keyVal.Keyword.ToUpper(), keyVal.Rest);
					_images[keyVal.Keyword.ToUpper()] = img;
				}

				sr.Close();
			}
		}

		public override void Save(string outFile)
		{
			using (var sw = new StreamWriter(outFile))
			{
				var a = new List<string>(_images.Keys);
				a.Sort();
				var vars = new Dictionary<string, Variable>();

				foreach (string st in a)
				{
					if (_images[st] != null)
					{
						var id = _images[st];
						if (!vars.ContainsKey(id.BasePath))
							vars[id.BasePath] = new Variable(id.BaseName + ":", id.BasePath);
						else
							vars[id.BasePath].Inc(id.BaseName + ":");
					}
				}

				foreach (string basePath in vars.Keys)
					vars[basePath].Write(sw);

				sw.Flush();
				sw.Close();
			}
		}

		public ImagesAccessor Images
		{
			get { return new ImagesAccessor(_images); }
		}

		/// <summary>
		/// Helps making sure images are accessed with upper case keys.
		/// </summary>
		public class ImagesAccessor
		{
			private readonly Dictionary<string, ImageDescriptor> _images;

			public ImagesAccessor(Dictionary<string, ImageDescriptor> images)
			{
				_images = images;
			}

			public IEnumerable<string> Keys
			{
				get { return _images.Keys; }
			}
			public IEnumerable<ImageDescriptor> ImageDescriptors
			{
				get { return _images.Values; }
			}

			public void Remove(string toString)
			{
				_images.Remove(toString.ToUpper());
			}

			public ImageDescriptor this[string imageSet]
			{
				get { return _images[imageSet.ToUpper()]; }
			}
		}
	}
}
