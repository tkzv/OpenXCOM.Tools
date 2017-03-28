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


		public ImageInfo(string inFile, Varidia vars)
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
				var key = name.ToUpperInvariant();
				return (_images.ContainsKey(key)) ? _images[key]
												  : null;
			}
			set { _images[name.ToUpperInvariant()] = value; }
		}

		public void Load(string inFile, Varidia vars)
		{
			using (var sr = new StreamReader(File.OpenRead(inFile)))
			{
				vars = new Varidia(sr, vars);

				KeyvalPair keyVal;
				while ((keyVal = vars.ReadLine()) != null)
				{
					var img = new ImageDescriptor(keyVal.Keyword.ToUpperInvariant(), keyVal.Value);
					_images[keyVal.Keyword.ToUpperInvariant()] = img;
				}
			}
		}

		public override void Save(string outFile)
		{
			using (var sw = new StreamWriter(outFile))
			{
				var keys = new List<string>(_images.Keys);
				keys.Sort();
				var vars = new Dictionary<string, Variable>();

				foreach (string st in keys)
				{
					if (_images[st] != null)
					{
						var image = _images[st];
						if (!vars.ContainsKey(image.BasePath))
							vars[image.BasePath] = new Variable(image.BaseName + ":", image.BasePath);
						else
							vars[image.BasePath].Add(image.BaseName + ":");
					}
				}

				foreach (string basePath in vars.Keys)
					vars[basePath].Write(sw);
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

			public void Remove(string st)
			{
				_images.Remove(st.ToUpperInvariant());
			}

			public ImageDescriptor this[string imageSet]
			{
				get { return _images[imageSet.ToUpperInvariant()]; }
			}
		}
	}
}
