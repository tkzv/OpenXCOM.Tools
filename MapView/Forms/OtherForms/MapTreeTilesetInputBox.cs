using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		internal string Tileset
		{
			get { return tbTileset.Text; }
			private set { tbTileset.Text = value; }
		}

		/// <summary>
		/// Stores the original tileset-label, used only for 'EditTileset' to
		/// check if the label has changed when user clicks Accept.
		/// </summary>
		private string TilesetOriginal
		{ get; set; }

		/// <summary>
		/// Stores the original terrain-list of a tileset, used only for
		/// 'EditTileset' to check if the terrains have changed when user clicks
		/// Accept.
		/// </summary>
		private List<string> TerrainsOriginal
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

					TilesetOriginal = String.Copy(Tileset);

					var tileset = TileGroup.Categories[Category][Tileset];

					TerrainsOriginal = new List<string>();
					foreach (string terrain in tileset.Terrains)
						TerrainsOriginal.Add(String.Copy(terrain));

					BasePath = tileset.BasePath;
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
//					case BoxType.EditTileset:
//						break;
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

					if ((Descriptor == null || !Descriptor.Terrains.Contains(terrain))
						&& terrain != "BLANKS")
					{
						lbTerrainsAvailable.Items.Add(terrain);
					}
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

						Descriptor = new Descriptor(
												Tileset,
												new List<string>(),
												BasePath,
												TileGroup.Pal);
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
			//LogFile.WriteLine("");
			//LogFile.WriteLine("OnTilesetLabelKeyUp");

			if (InputBoxType == BoxType.AddTileset && e.KeyCode == Keys.Enter)
				OnCreateTilesetClick(null, EventArgs.Empty);
		}

		/// <summary>
		/// If this inputbox is type AddTileset, the accept click must check to
		/// see if a descriptor has been created already with the CreateMap
		/// button first.
		/// 
		/// If this inputbox is type EditTileset, the accept click will create a
		/// descriptor if the tileset-label changed and delete the old
		/// descriptor, and add the new one to the current tilegroup/category.
		/// If the tileset-label didn't change, nothing more need be done since
		/// any terrains that were changed have already been changed by changes
		/// to the Allocated/Available listboxes.
		///
		/// TODO: Check if the rest of the code stays happy if a tileset doesn't
		/// have any terrains listed. -> not only is it unhappy, but this has to
		/// end up with writing a new valid MapFile to disk, perhaps just a
		/// basic 10x10x1 map with flooring only.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAcceptClick(object sender, EventArgs e)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("OnAcceptClick");

			Tileset = Tileset.Trim();

			switch (InputBoxType)
			{
				case BoxType.AddTileset:			// NOTE: The tileset-label should already have been
					DialogResult = DialogResult.OK;	// checked for validity by here by the Create button.
					LogFile.WriteLine(". AddTileset");
					break;

				case BoxType.EditTileset:
					LogFile.WriteLine(". EditTileset");

					if (!String.IsNullOrEmpty(Tileset))
					{
						if (ValidateTilesetLabel(Tileset))
						{
							if (lbTerrainsAllocated.Items.Count != 0)
							{
								LogFile.WriteLine(". Tileset= " + Tileset);
								foreach (string t in TerrainsOriginal)
									LogFile.WriteLine(". terrain original= " + t);
								foreach (string t in Descriptor.Terrains)
									LogFile.WriteLine(". terrain= " + t);

								if (Tileset == TilesetOriginal) // label didn't change; check if terrains changed ->
								{
									LogFile.WriteLine(". . label did *not* change");

									if (!Descriptor.Terrains.SequenceEqual(TerrainsOriginal))
									{
										DialogResult = DialogResult.OK;
									}
									else
										ShowInfoDialog("No changes were made.");

									// NOTE: Save of Map-file is *not* required.
								}
								else // label changed; rewrite the descriptor ->
								{
									LogFile.WriteLine(". . change label");

									// NOTE: user cannot edit a Map-label to be another already existing file.
									// There are other ways to do that: either let the user delete the target-
									// Map-file from his/her disk, or better click Edit on *that* tileset.
									// NOTE: however, if while editing a tileset the user browses to another
									// tileset and edits that tileset's terrains, the changes are effective.

									if (!TilesetFileExists(Tileset))
									{
										var category = TileGroup.Categories[Category];
										if (!category.ContainsKey(Tileset)) // safety. If the Map-file does not exist, then the current category cannot contain its key. ... ideally.
										{
											LogFile.WriteLine(". tileset Created");

											try
											{
												string pfeMap    = GetFullPath(Tileset);
												string pfeMapPre = GetFullPath(TilesetOriginal);

												LogFile.WriteLine(". . fileMapPre= " + pfeMapPre);
												LogFile.WriteLine(". . fileMap= " + pfeMap);

//												using (var fs = File.Create(fileMap)) // create the file and drop the handle.
//												{}

												File.Move(pfeMapPre, pfeMap);	// NOTE: This has to happen now because if the MapTree
																				// is clicked it will try to load the .MAP file etc.

												if (File.Exists(pfeMap))		// NOTE: do *not* alter the descriptor if File.Move() went bork.
												{								// This is likely redundant: File.Move() should throw.
													Descriptor = new Descriptor(
																			Tileset,
																			category[TilesetOriginal].Terrains,
																			BasePath,
																			TileGroup.Pal);
													TileGroup.AddTileset(Descriptor, Category);			// NOTE: This could be done on return to XCMainWindow.OnEditTilesetClick()
																										// but then 'Descriptor' would have to be internal.
													TileGroup.DeleteTileset(TilesetOriginal, Category);	// NOTE: This could be done on return to XCMainWindow.OnEditTilesetClick()
																										// but then 'TilesetOriginal' would have to be internal.

													DialogResult = DialogResult.OK;
												}
											}
											catch (Exception ex)
											{
												ShowErrorDialog(ex.ToString());
											}
										}
										else
											ShowErrorDialog("The Map tileset already exists.");
									}
									else
										ShowErrorDialog("The Map file already exists on disk. The Tileset Editor is"
														+ " not sophisticated enough to deal with this eventuality."
														+ " Either edit that Map directly if it's already in a Group,"
														+ " or use Add Tileset to make it editable, or as a last"
														+ " resort delete it from your disk." + Environment.NewLine + Environment.NewLine
														+ GetFullPath(Tileset));
														// TODO: Ask user if he/she wants to overwrite the Map-file.
								}
							}
							else
								ShowErrorDialog("The Map must have at least one terrain allocated.");
						}
						else
							ShowErrorDialog("The Map label contains illegal characters.");
					}
					else
						ShowErrorDialog("The Map label cannot be blank.");

					break;
			}
		}
		#endregion


		#region Methods
		/// <summary>
		/// Checks that the tileset-label can be a legal filename (Windows).
		/// NOTE: Check that 'Tileset' is not null or empty before call.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		private static bool ValidateTilesetLabel(string label)
		{
			var invalid = new List<char>();

			char[] invalidChars = Path.GetInvalidFileNameChars();
			for (int i = 0; i != invalidChars.Length; ++i)
				invalid.Add(invalidChars[i]);

			invalid.Add(' '); // no spaces also.
			invalid.Add('.'); // and not dots.
			// TODO: hell i should just check for alpha-numeric and underscore. Old-school style. guaranteed.

			invalidChars = invalid.ToArray();

			return (label.IndexOfAny(invalidChars) == -1);
		}
//		// https://stackoverflow.com/questions/62771/how-do-i-check-if-a-given-string-is-a-legal-valid-file-name-under-windows#answer-62888
//		You may use any character in the current code page (Unicode/ANSI above 127), except:
//
//		< > : " / \ | ? *
//		Characters whose integer representations are 0-31 (less than ASCII space)
//		Any other character that the target file system does not allow (say, trailing periods or spaces)
//		Any of the DOS names: CON, PRN, AUX, NUL, COM1, COM2, COM3, COM4,
//		COM5, COM6, COM7, COM8, COM9, LPT1, LPT2, LPT3, LPT4, LPT5, LPT6,
//		LPT7, LPT8, LPT9 (and avoid AUX.txt, etc)
//		The file name is all periods
//
//		Some optional things to check:
//
//		File paths (including the file name) may not have more than 260 characters (that don't use the \?\ prefix)
//		Unicode file paths (including the file name) with more than 32,000 characters when using \?\
//		(note that prefix may expand directory components and cause it to overflow the 32,000 limit)
//
//		also: https://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
//
//		also: https://stackoverflow.com/questions/422090/in-c-sharp-check-that-filename-is-possibly-valid-not-that-it-exists
//
//		Naming Files, Paths, and Namespaces
//		https://msdn.microsoft.com/en-us/library/aa365247(VS.85).aspx


		/// <summary>
		/// Gets the fullpath for a Map-file.
		/// </summary>
		/// <param name="labelMap"></param>
		/// <returns></returns>
		private string GetFullPath(string labelMap)
		{
			string pfeLabel = Path.Combine(BasePath, "MAPS");
			return Path.Combine(pfeLabel, labelMap + ".MAP");
		}

		/// <summary>
		/// Checks if a Map-file w/ 'label' exists in the current 'BasePath'
		/// directory.
		/// </summary>
		/// <param name="labelMap">the name w/out extension of a Map-file to check for</param>
		/// <returns></returns>
		private bool TilesetFileExists(string labelMap)
		{
			LogFile.WriteLine("");
			LogFile.WriteLine("TilesetFileExists");

			string pfeMap = null;
			if (!String.IsNullOrEmpty(labelMap))
			{
				pfeMap = GetFullPath(labelMap);
				LogFile.WriteLine(". pfeMap= " + pfeMap);
			}

			return (pfeMap != null && File.Exists(pfeMap));
		}

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

		/// <summary>
		/// Wrapper for MessageBox.Show().
		/// </summary>
		/// <param name="info">the info string to show</param>
		private void ShowInfoDialog(string info)
		{
			MessageBox.Show(
						this,
						info,
						"Notice",
						MessageBoxButtons.OK,
						MessageBoxIcon.Information,
						MessageBoxDefaultButton.Button1,
						0);
		}
		#endregion
	}
}
