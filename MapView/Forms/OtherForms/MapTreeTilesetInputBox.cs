using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces;


namespace MapView
{
	internal sealed partial class MapTreeTilesetInputBox
		:
			Form
	{
		/// <summary>
		/// The possible box-types.
		/// </summary>
		internal enum BoxType
		{
			AddTileset,
			EditTileset
		}


		#region Fields (static)
		private const string AddTileset  = "Add Tileset";
		private const string EditTileset = "Edit Tileset";
		#endregion


		#region Properties
		private BoxType InputBoxType
		{ get; set; }

		/// <summary>
		/// Gets/Sets the group-label.
		/// </summary>
		private string Group
		{
			get { return lblGroupCurrent.Text; }
			set { lblGroupCurrent.Text = value; }
		}

		/// <summary>
		/// Gets/Sets the category-label.
		/// </summary>
		private string Category
		{
			get { return lblCategoryCurrent.Text; }
			set { lblCategoryCurrent.Text = value; }
		}

		/// <summary>
		/// Gets/Sets the tileset-label.
		/// </summary>
		private string Tileset
		{
			get { return tbTileset.Text; }
			set { tbTileset.Text = value; }
		}

		/// <summary>
		/// Sets the value of an edited tileset-label, used only for
		/// 'EditTileset' to store a new label until user clicks Accept.
		/// </summary>
		private string TilesetEdited
		{ get; set; }

		/// <summary>
		/// Gets/Sets the basepath. Calls ListTerrains() which also sets the
		/// Descriptor.
		/// </summary>
		private string _basepath;
		private string BasePath
		{
			get { return _basepath; }
			set
			{
				_basepath = value;
				lblPathCurrent.Text = Path.Combine(_basepath, "MAPS");

				ListTerrains();
			}
		}

		private Descriptor Descriptor
		{ get; set; }

		private TileGroup TileGroup
		{ get; set; }

		private bool Inited
		{ get; set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="boxType"></param>
		/// <param name="labelGroup"></param>
		/// <param name="labelCategory"></param>
		/// <param name="labelTileset"></param>
		internal MapTreeTilesetInputBox(
				BoxType boxType,
				string labelGroup,
				string labelCategory,
				string labelTileset)
		{
			InitializeComponent();

			Group    = labelGroup;
			Category = labelCategory;
			Tileset  = labelTileset;

			Inited = true;	// don't let setting 'Tileset' run OnTilesetTextChanged() until
							// after 'BasePath' is initialized. Else ListTerrains() will throwup.

			TileGroup = ResourceInfo.TileGroupInfo.TileGroups[Group] as TileGroup;

			switch (InputBoxType = boxType)
			{
				case BoxType.AddTileset:
					Text = AddTileset;

					lblHeaderTileset.Visible  =
					lblTilesetCurrent.Visible = false;

					btnCreateMap.Enabled = false;

					string keyBaseDir = null;
					switch (TileGroup.GroupType)
					{
						case TileGroup.GameType.Ufo:
							keyBaseDir = SharedSpace.ResourcesDirectoryUfo;
							break;
						case TileGroup.GameType.Tftd:
							keyBaseDir = SharedSpace.ResourcesDirectoryTftd;
							break;
					}
					BasePath = SharedSpace.Instance.GetShare(keyBaseDir);
					break;

				case BoxType.EditTileset:
				{
					Text = EditTileset;
					lblTilesetCurrent.Text = Tileset;

					btnCreateMap.Visible     =
					btnFindTileset.Visible   =
					btnFindDirectory.Visible = false;

					BasePath = TileGroup.Categories[Category][Tileset].BasePath;

					break;
				}
			}

			tbTileset.Select();
		}
		#endregion


		#region Eventcalls
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			int lbWidth = gbTerrains.Width / 2 - pSpacer.Width * 2 / 3; // not sure why 2/3 works.
			lbTerrainsAllocated.Width =
			lbTerrainsAvailable.Width = lbWidth;

			lblAllocated.Left = lbTerrainsAllocated.Right - lblAllocated.Width - 5;
			lblAvailable.Left = lbTerrainsAvailable.Left;
		}

		/// <summary>
		/// Opens a find file dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFindTilesetClick(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Filter = "Map Files (*.map)|*.map|All Files (*.*)|*.*";
				ofd.Title  = "Select a Map File";
				ofd.InitialDirectory = Path.Combine(BasePath, "MAPS");
				if (!Directory.Exists(ofd.InitialDirectory))
					ofd.InitialDirectory = BasePath;

				if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					string pfeMap = ofd.FileName;

					Tileset = Path.GetFileNameWithoutExtension(pfeMap);

					string basepath = Path.GetDirectoryName(pfeMap);
					int pos = basepath.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase);
					BasePath = (pos != -1) ? basepath.Substring(0, pos)
										   : basepath;
				}
			}
		}

		/// <summary>
		/// Opens a find directory dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFindDirectoryClick(object sender, EventArgs e)
		{
			using (var fbd = new FolderBrowserDialog())
			{
				fbd.SelectedPath = BasePath;
				fbd.Description = String.Format(
							System.Globalization.CultureInfo.CurrentCulture,
							"Browse to a resource folder. A resource"
								+ " folder shall have the subfolders"
								+ " MAPS, ROUTES, and TERRAIN.");

				if (fbd.ShowDialog() == DialogResult.OK)
				{
					BasePath = fbd.SelectedPath;
				}
			}
		}

		/// <summary>
		/// Ensures that tileset-labels will be UpperCASE, and refreshes the
		/// terrains-lists.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTilesetTextChanged(object sender, EventArgs e)
		{
			if (Inited)
			{
				switch (InputBoxType)
				{
					case BoxType.AddTileset:
						ListTerrains();
						btnCreateMap.Enabled = (Descriptor == null
											&& !String.IsNullOrEmpty(Tileset));
						break;
					case BoxType.EditTileset:
						TilesetEdited = Tileset;
						break;
				}
			}
		}

		/// <summary>
		/// Lists the allocated and available terrains in their list-boxes. This
		/// function also sets the current Descriptor, which is essential to
		/// listing the terrains as well as to the proper functioning of various
		/// control-buttons.
		/// </summary>
		private void ListTerrains()
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("ListTerrains");

			btnMoveUp.Enabled    =
			btnMoveDown.Enabled  =
			btnMoveRight.Enabled =
			btnMoveLeft.Enabled  = false;

			lbTerrainsAllocated.Items.Clear();
			lbTerrainsAvailable.Items.Clear();

//			if (TilesetFileExists())
//			{
//				var category = TileGroup.Categories[Category];
//				Descriptor = (category.ContainsKey(Tileset)) ? category[Tileset]
//															 : null;
//			}
//			else
//				Descriptor = null;


			var category = TileGroup.Categories[Category];
			Descriptor = (category.ContainsKey(Tileset)) ? category[Tileset]
														 : null;

			if (Descriptor != null)
			{
				foreach (string terrain in Descriptor.Terrains)
					lbTerrainsAllocated.Items.Add(terrain);
			}

			string terrainsPath = Path.Combine(BasePath, "TERRAIN");
			if (Directory.Exists(terrainsPath))
			{
				string terrain = String.Empty;
				string[] terrains = Directory.GetFiles(
													terrainsPath,
													"*.pck",
													SearchOption.TopDirectoryOnly);
				for (int i = 0; i != terrains.Length; ++i)
				{
					terrain = Path.GetFileNameWithoutExtension(terrains[i]).ToUpperInvariant();

					if (Descriptor == null || !Descriptor.Terrains.Contains(terrain))
						lbTerrainsAvailable.Items.Add(terrain);
				}
			}
		}

		private void OnMoveLeftClick(object sender, EventArgs e)
		{
//			if (Descriptor != null)
			Descriptor.Terrains.Add(lbTerrainsAvailable.SelectedItem as String);

			ListTerrains();
		}

		private void OnMoveRightClick(object sender, EventArgs e)
		{
			Descriptor.Terrains.Remove(lbTerrainsAllocated.SelectedItem as String);

			ListTerrains();
		}

		private void OnMoveUpClick(object sender, EventArgs e)
		{
			var terrains = Descriptor.Terrains;

			for (int id = 1; id != terrains.Count; ++id)
			{
				if (terrains[id] == lbTerrainsAllocated.SelectedItem as String)
				{
					string t = terrains[id - 1];
					terrains[id - 1] = terrains[id];
					terrains[id] = t;

					Descriptor.Terrains = terrains;

					lbTerrainsAllocated.Items.Clear();

					foreach (string terrain in terrains)
						lbTerrainsAllocated.Items.Add(terrain);

					lbTerrainsAllocated.SelectedItem = terrains[id - 1];
					break;
				}
			}
		}

		private void OnMoveDownClick(object sender, EventArgs e)
		{
			var terrains = Descriptor.Terrains;

			for (int id = 0; id != terrains.Count - 1; ++id)
			{
				if (terrains[id] == lbTerrainsAllocated.SelectedItem as String)
				{
					string t = terrains[id + 1];
					terrains[id + 1] = terrains[id];
					terrains[id] = t;

					Descriptor.Terrains = terrains;
					
					lbTerrainsAllocated.Items.Clear();

					foreach (string terrain in terrains)
						lbTerrainsAllocated.Items.Add(terrain);

					lbTerrainsAllocated.SelectedItem = terrains[id + 1];
					break;
				}
			}
		}

		private void OnAllocatedIndexChanged(object sender, EventArgs e)
		{
			if (lbTerrainsAllocated.SelectedIndex != -1)
			{
				btnMoveRight.Enabled = true;

				if (Descriptor != null && Descriptor.Terrains.Count > 1)
				{
					btnMoveUp.Enabled   = (lbTerrainsAllocated.SelectedIndex != 0);
					btnMoveDown.Enabled = (lbTerrainsAllocated.SelectedIndex != Descriptor.Terrains.Count - 1);
				}
			}
		}

		private void OnAvailableIndexChanged(object sender, EventArgs e)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("OnAvailableIndexChanged");
			LogFile.WriteLine(". descriptor " + ((Descriptor != null) ? "VALID" : "NOT Valid"));
			LogFile.WriteLine(". selected id= " + lbTerrainsAvailable.SelectedIndex);

			btnMoveLeft.Enabled = (Descriptor != null)
							   && (lbTerrainsAvailable.SelectedIndex != -1);
		}

		/// <summary>
		/// Creates a tileset as a valid Descriptor. This is allowed iff a
		/// tileset is being Added; it's disallowed if a tileset is only being
		/// Edited.
		/// NOTE: A Map's descriptor must be created/valid before terrains can
		/// be added.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCreateTilesetClick(object sender, EventArgs e)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("OnCreateMapClick");

			Tileset = tbTileset.Text.Trim();

			if (!String.IsNullOrEmpty(Tileset))
			{
				if (ValidateTilesetLabel(Tileset))
				{
//					if (TilesetFileExists())	// TODO: check to ensure that this Create function (and KeyUp-Enter events)
//					{							// cannot be called if a descriptor and/or a Map-file already exist.
//						ShowErrorDialog("The Map file already exists.");
//					}
//					else
					var category = TileGroup.Categories[Category];
					if (!category.ContainsKey(Tileset))	// safety -> TODO: the create map and tileset keyup events should
					{									// be disabled if a Descriptor w/ tileset-label already exists
						LogFile.WriteLine(". tileset Created");

						var pal = (TileGroup.GroupType == TileGroup.GameType.Ufo) ? Palette.UfoBattle
																				  : Palette.TftdBattle;
						Descriptor = new Descriptor(
												Tileset,
												new List<string>(),
												BasePath,
												pal);
						TileGroup.AddTileset(Descriptor, Category);

						btnCreateMap.Enabled = false;
						ListTerrains();
					}
				}
				else
					ShowErrorDialog("The Map label contains illegal characters.");
			}
			else
				ShowErrorDialog("The Map label cannot be blank.");
		}

		/// <summary>
		/// Calls OnCreateTilesetClick() if Enter is key-upped in the
		/// tileset-label textbox.
		/// NOTE: KeyDown event doesn't work for an Enter key. Be careful 'cause
		/// the keydown gets intercepted by the form itself.
		/// TODO: Bypass triggering OnAcceptClick() ...
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTilesetLabelKeyUp(object sender, KeyEventArgs e)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("OnTilesetLabelKeyUp");

			if (InputBoxType == BoxType.AddTileset && e.KeyCode == Keys.Enter)
				OnCreateTilesetClick(null, EventArgs.Empty);
		}

		/// <summary>
		/// If this inputbox is type AddTileset, the accept click must check to
		/// see if a descriptor has been created already with the CreateMap
		/// button first. If this inputbox is type EditTileset, the accept click
		/// will create a descriptor, delete the old descriptor, and add the new
		/// one to the current tilegroup/category.
		///
		/// TODO: Check if the rest of the code stays happy if a tileset doesn't
		/// have any terrains listed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAcceptClick(object sender, EventArgs e)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("OnAcceptClick");

			Tileset = tbTileset.Text.Trim();

			switch (InputBoxType)
			{
				case BoxType.AddTileset:			// NOTE: The tileset-label should already
					DialogResult = DialogResult.OK;	// have been checked for validity by here
					break;							// by the Create button.

				case BoxType.EditTileset:
					if (!String.IsNullOrEmpty(Tileset))
					{
						if (ValidateTilesetLabel(TilesetEdited))
						{
							// TODO: create a descriptor. Check if Map-file exists already. Delete old Descriptor.

							DialogResult = DialogResult.OK;
						}
						else
							ShowErrorDialog("The Map label contains illegal characters.");
					}
					else
						ShowErrorDialog("The Map label cannot be blank.");

					break;
			}


/*			switch (InputBoxType)
			{
				case BoxType.AddGroup:
				case BoxType.EditGroup:
					if (String.IsNullOrEmpty(tbInput.Text))
					{
						ShowErrorDialog("A group label has not been specified.");
					}
					else if (!tbInput.Text.StartsWith("ufo",  StringComparison.OrdinalIgnoreCase)
						&&   !tbInput.Text.StartsWith("tftd", StringComparison.OrdinalIgnoreCase))
					{
						
						ShowErrorDialog("The group label needs to start with UFO or TFTD.");
					}
//					else if (ResourceInfo.TileGroupInfo.TileGroups.ContainsKey(tbInput.Text))
					else
					{
						bool bork = false;
						foreach (var labelGroup in ResourceInfo.TileGroupInfo.TileGroups.Keys) // check if group-label already exists
						{
							if (String.Equals(labelGroup, tbInput.Text, StringComparison.OrdinalIgnoreCase))
							{
								bork = true;
								break;
							}
						}

						if (bork)
							ShowErrorDialog("The group label already exists.");
						else
							DialogResult = DialogResult.OK;
					}
					break;

				case BoxType.AddCategory:
				case BoxType.EditCategory:
					if (String.IsNullOrEmpty(tbInput.Text))
					{
						ShowErrorDialog("A category label has not been specified.");
					}
					else
					{
						bool bork = false;

						var tilegroup = ResourceInfo.TileGroupInfo.TileGroups[ParentGroup];
						foreach (var labelCategory in tilegroup.Categories.Keys)
						{
							if (String.Equals(labelCategory, tbInput.Text, StringComparison.OrdinalIgnoreCase))
							{
								bork = true;
								break;
							}
						}

						if (bork)
							ShowErrorDialog("The category label already exists.");
						else
							DialogResult = DialogResult.OK;
					}
					break;
			} */
		}
		#endregion


		#region Methods
		/// <summary>
		/// Checks that the tileset-label can be a legal filename (Windows).
		/// NOTE: Check that 'Tileset' is not null or empty before call.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		private bool ValidateTilesetLabel(string label)
		{
			var invalid = new List<char>();

			char[] invalidChars = Path.GetInvalidFileNameChars();
			for (int i = 0; i != invalidChars.Length; ++i)
				invalid.Add(invalidChars[i]);

			invalid.Add(' '); // no spaces also.
			invalid.Add('.'); // and not dots.
			// TODO: hell i should just check for alpha-numeric and underscore.

			invalidChars = invalid.ToArray();

			return (label.IndexOfAny(invalidChars) == -1);
		}
			// https://stackoverflow.com/questions/62771/how-do-i-check-if-a-given-string-is-a-legal-valid-file-name-under-windows#answer-62888
/*			You may use any character in the current code page (Unicode/ANSI above 127), except:

			< > : " / \ | ? *
			Characters whose integer representations are 0-31 (less than ASCII space)
			Any other character that the target file system does not allow (say, trailing periods or spaces)
			Any of the DOS names: CON, PRN, AUX, NUL, COM1, COM2, COM3, COM4,
			COM5, COM6, COM7, COM8, COM9, LPT1, LPT2, LPT3, LPT4, LPT5, LPT6,
			LPT7, LPT8, LPT9 (and avoid AUX.txt, etc)
			The file name is all periods

			Some optional things to check:

			File paths (including the file name) may not have more than 260 characters (that don't use the \?\ prefix)
			Unicode file paths (including the file name) with more than 32,000 characters when using \?\
			(note that prefix may expand directory components and cause it to overflow the 32,000 limit)
*//*
			Naming Files, Paths, and Namespaces
			https://msdn.microsoft.com/en-us/library/aa365247(VS.85).aspx
*/

//		private bool TilesetFileExists()
//		{
//			LogFile.WriteLine("");
//			LogFile.WriteLine("TilesetFileExists");
//
//			string pfeMap = null;
//			if (!String.IsNullOrEmpty(Tileset))
//			{
//				pfeMap = Path.Combine(BasePath, "MAPS");
//				pfeMap = Path.Combine(pfeMap, Tileset + ".MAP");
//
//				LogFile.WriteLine(". pfeMap= " + pfeMap);
//			}
//
//			return (pfeMap != null && File.Exists(pfeMap));
//		}

		/// <summary>
		/// Wrapper for MessageBox.Show().
		/// </summary>
		/// <param name="error">the error string to show</param>
		private void ShowErrorDialog(string error)
		{
			MessageBox.Show(
						this,
						error,
						"Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error,
						MessageBoxDefaultButton.Button1,
						0);
		}
		#endregion
	}
}
