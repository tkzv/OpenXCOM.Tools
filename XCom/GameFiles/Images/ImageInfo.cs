using System;
using System.Collections.Generic;
using System.IO;


namespace XCom
{
	public sealed class ImageInfo
		:
			FileDesc
	{
		#region FileDesc requirements

		public override void Save(string outFile)
		{
			using (var sw = new StreamWriter(outFile))
			{
				var keys = new List<string>(_imagesDictionary.Keys);
				keys.Sort();
				var vars = new Dictionary<string, Variable>();

				foreach (string key in keys)
				{
					if (_imagesDictionary[key] != null)
					{
						var image = _imagesDictionary[key];
						if (!vars.ContainsKey(image.Path))
							vars[image.Path] = new Variable(image.Label + ":", image.Path);
						else
							vars[image.Path].Add(image.Label + ":");
					}
				}

				foreach (string basePath in vars.Keys)
					vars[basePath].Write(sw);
			}
		}
		#endregion


		private readonly Dictionary<string, ImageDescriptor> _imagesDictionary;


		internal ImageInfo(string inFile, Varidia vars)
			:
				base(inFile)
		{
			_imagesDictionary = new Dictionary<string, ImageDescriptor>();
			Load(inFile, vars);
		}


		public ImageDescriptor this[string name]
		{
			get
			{
				var key = name.ToUpperInvariant();
				return (_imagesDictionary.ContainsKey(key)) ? _imagesDictionary[key]
															: null;
			}
			set { _imagesDictionary[name.ToUpperInvariant()] = value; }
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
					_imagesDictionary[keyVal.Keyword.ToUpperInvariant()] = img;
				}
			}
		}

		public ImagesAccessor Images
		{
			get { return new ImagesAccessor(_imagesDictionary); }
		}

		/// <summary>
		/// Ensures images are accessed with uppercase keys.
		/// </summary>
		public class ImagesAccessor
		{
			public ImagesAccessor(Dictionary<string, ImageDescriptor> images)
			{
				_images = images;
			}


			private readonly Dictionary<string, ImageDescriptor> _images;

			public IEnumerable<string> Keys
			{
				get { return _images.Keys; }
			}

			public IEnumerable<ImageDescriptor> ImageDescriptors
			{
				get { return _images.Values; }
			}

			public ImageDescriptor this[string imageSet]
			{
				get { return _images[imageSet.ToUpperInvariant()]; }
			}

			public void Remove(string key)
			{
				_images.Remove(key.ToUpperInvariant());
			}
		}
	}
}
