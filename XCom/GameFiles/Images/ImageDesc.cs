using System;
using System.Collections.Generic;
using System.IO;


namespace XCom
{
	public sealed class ImageDesc
	{
		#region Fields & Properties
		private readonly Dictionary<string, ImageDescriptor> _imagesDictionary;

		private readonly string _path;
		public string Path
		{
			get { return _path; }
		}

		public ImageDescriptor this[string name]
		{
			get
			{
				string key = name.ToUpperInvariant();
				return (_imagesDictionary.ContainsKey(key)) ? _imagesDictionary[key]
															: null;
			}
			set { _imagesDictionary[name.ToUpperInvariant()] = value; }
		}

		public ImagesAccessor Images
		{
			get { return new ImagesAccessor(_imagesDictionary); }
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="pfe"></param>
		/// <param name="vars"></param>
		internal ImageDesc(string pfe, Varidia vars)
		{
			_path = pfe;
			_imagesDictionary = new Dictionary<string, ImageDescriptor>();

			Load(pfe, vars);
		}
		#endregion


		#region Methods
		private void Load(string pfe, Varidia vars)
		{
			using (var sr = new StreamReader(File.OpenRead(pfe)))
			{
				vars = new Varidia(sr, vars);

				KeyvalPair keyVal;
				while ((keyVal = vars.ReadLine()) != null)
				{
					var image = new ImageDescriptor(keyVal.Keyword.ToUpperInvariant(), keyVal.Value);
					_imagesDictionary[keyVal.Keyword.ToUpperInvariant()] = image;
				}
			}
		}

		public void Save(string pfe)
		{
			using (var sw = new StreamWriter(pfe))
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



		/// <summary>
		/// Ensures images are accessed with uppercase keys.
		/// good lord ....
		/// </summary>
		public sealed class ImagesAccessor
		{
			#region Fields
			private readonly Dictionary<string, ImageDescriptor> _imagesDictionary;
			#endregion


			#region Properties
			public ImageDescriptor this[string terrain]
			{
				get { return _imagesDictionary[terrain.ToUpperInvariant()]; }
			}

			public IEnumerable<string> Keys
			{
				get { return _imagesDictionary.Keys; }
			}

//			public IEnumerable<ImageDescriptor> ImageDescriptors
//			{
//				get { return _images.Values; }
//			}
			#endregion


			#region cTor
			/// <summary>
			/// cTor.
			/// </summary>
			/// <param name="imagesDictionary"></param>
			public ImagesAccessor(Dictionary<string, ImageDescriptor> imagesDictionary)
			{
				_imagesDictionary = imagesDictionary;
			}
			#endregion


			#region Methods
			public void Remove(string key)
			{
				_imagesDictionary.Remove(key.ToUpperInvariant());
			}
			#endregion
		}
	}
}
