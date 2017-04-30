using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using MapView.Forms.MainWindow;
using MapView.Forms.McdViewer;
using MapView.SettingServices;

using PckView;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TileViews
{
	internal sealed partial class TileView
		:
			MapObserverControl0
	{
		#region Events
		internal event SelectedTileChangedEventHandler Observer0SelectedTileChanged;
		private void OnSelectedTileChanged(TileBase tile)
		{
			var handler = Observer0SelectedTileChanged;
			if (handler != null)
				handler(tile);
		}

		/// <summary>
		/// Raised when a save is done in PckView.
		/// </summary>
		internal event MethodInvoker MapChangedEvent;
		private void OnMapChanged()
		{
			var handler = MapChangedEvent;
			if (handler != null)
				handler();
		}
		#endregion


		#region Fields
		private ShowHideManager _showHideManager;

		private IContainer components = null; // quahhaha

		private TilePanel _allTiles;
		private TilePanel[] _panels;

		private McdViewerForm _mcdInfoForm;

		private Hashtable _brushes = new Hashtable();
		#endregion


		#region Properties
		public override IMapBase MapBase
		{
			set
			{
				base.MapBase = value;
				Tiles = (value != null) ? value.Tiles
										: null;
			}
		}

		private System.Collections.Generic.IList<TileBase> Tiles
		{
			set
			{
				for (int i = 0; i != _panels.Length; ++i)
					_panels[i].SetTiles(value);

				OnResize(null);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal TileBase SelectedTile
		{
			get { return _panels[tcTileTypes.SelectedIndex].SelectedTile; }
			set
			{
				_allTiles.SelectedTile = value;
				tcTileTypes.SelectedIndex = 0;

				Refresh();
			}
		}
		#endregion



		#region cTor
		/// <summary>
		/// cTor. Instantiates the TileView viewer and its pages/panels.
		/// </summary>
		internal TileView()
		{
			InitializeComponent();

			tcTileTypes.SelectedIndexChanged += OnSelectedIndexChanged;

			_allTiles      = new TilePanel(TileType.All);
			var floors     = new TilePanel(TileType.Ground);
			var westwalls  = new TilePanel(TileType.WestWall);
			var northwalls = new TilePanel(TileType.NorthWall);
			var content    = new TilePanel(TileType.Object);

			_panels = new[]
			{
				_allTiles,
				floors,
				westwalls,
				northwalls,
				content
			};

			AddPanel(_allTiles,  tpAll);
			AddPanel(floors,     tpFloors);
			AddPanel(westwalls,  tpWestwalls);
			AddPanel(northwalls, tpNorthwalls);
			AddPanel(content,    tpObjects);
		}
		#endregion


		private void AddPanel(TilePanel panel, Control page)
		{
			panel.PanelSelectedTileChangedEvent += OnPanelTileChanged;
			page.Controls.Add(panel);
		}


		#region EventCalls
		/// <summary>
		/// Focuses the selected panel, updates the quadrant and MCD-info if
		/// applicable.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSelectedIndexChanged(object sender, EventArgs e)
		{
			GetSelectedPanel().Focus();
			var f = FindForm();

			McdRecord record = null;

			if (SelectedTile != null)
			{
				ViewerFormsManager.TopView.Control.SelectQuadrant(SelectedTile.Record.TileType);

				f.Text = BuildTitleString(SelectedTile.TileListId, SelectedTile.Id);
				record = SelectedTile.Record;
			}
			else
				f.Text = "Tile View";

			if (_mcdInfoForm != null)
				_mcdInfoForm.UpdateData(record);
		}

		private void OnPanelTileChanged(TileBase tile)
		{
			var f = FindForm();

			McdRecord record = null;

			if (tile != null)
			{
				f.Text = BuildTitleString(tile.TileListId, tile.Id);
				record = tile.Record;
			}
			else
				f.Text = "Tile View";

			if (_mcdInfoForm != null)
				_mcdInfoForm.UpdateData(record);

			OnSelectedTileChanged(tile);
		}
		#endregion

		/// <summary>
		/// Sets the ShowHideManager.
		/// </summary>
		/// <param name="showHideManager"></param>
		internal void SetShowHideManager(ShowHideManager showHideManager)
		{
			_showHideManager = showHideManager;
		}

		/// <summary>
		/// These are the default colors for tiles' Special Properties.
		/// TileView will load these colors when the program loads, then any
		/// Special Property colors that were customized will be set and
		/// accessed by TilePanel and/or the Help screen later.
		/// </summary>
		internal static readonly Color[] TileColors =
		{
									//      UFO:			TFTD:
			Color.NavajoWhite,		//  0 - Standard
			Color.Lavender,			//  1 - EntryPoint
			Color.IndianRed,		//  2 - PowerSource		IonBeamAccel
			Color.PaleTurquoise,	//  3 - Navigation		DestroyObjective
			Color.Khaki,			//  4 - Construction	MagneticNav
			Color.MistyRose,		//  5 - Food			AlienCryo
			Color.Aquamarine,		//  6 - Reproduction	AlienClon
			Color.LightSkyBlue,		//  7 - Entertainment	AlienLearn
			Color.Thistle,			//  8 - Surgery			AlienImplant
			Color.YellowGreen,		//  9 - ExaminationRoom	Unknown9
			Color.MediumPurple,		// 10 - Alloys			AlienPlastics
			Color.LightCoral,		// 11 - Habitat			ExamRoom
			Color.LightCyan,		// 12 - DeadTile
			Color.BurlyWood,		// 13 - ExitPoint
			Color.Blue				// 14 - MustDestroy
		};

		/// <summary>
		/// Loads default settings for TileView screen.
		/// </summary>
		protected internal override void LoadControl0Settings()
		{
			string desc = String.Empty;

			foreach (string specialType in Enum.GetNames(typeof(SpecialType)))
			{
				int i = (int)Enum.Parse(typeof(SpecialType), specialType);
				_brushes[specialType] = new SolidBrush(TileColors[i]);

				switch (i)
				{
					case  0: desc = "Color of (standard tile)";
						break;
					case  1: desc = "Color of (entry point)";
						break;
					case  2: desc = "Color of UFO Power Source - UFO" + Environment.NewLine
								  + "Color of Ion-beam Accelerators - TFTD";
						break;
					case  3: desc = "Color of UFO Navigation - UFO" + Environment.NewLine
								  + "Color of (destroy objective) (alias of Magnetic Navigation) - TFTD";
						break;
					case  4: desc = "Color of UFO Construction - UFO" + Environment.NewLine
								  + "Color of Magnetic Navigation (alias of Alien Sub Construction) - TFTD";
						break;
					case  5: desc = "Color of Alien Food - UFO" + Environment.NewLine
								  + "Color of Alien Cryogenics - TFTD";
						break;
					case  6: desc = "Color of Alien Reproduction - UFO" + Environment.NewLine
								  + "Color of Alien Cloning - TFTD";
						break;
					case  7: desc = "Color of Alien Entertainment - UFO" + Environment.NewLine
								  + "Color of Alien Learning Arrays - TFTD";
						break;
					case  8: desc = "Color of Alien Surgery - UFO" + Environment.NewLine
								  + "Color of Alien Implanter - TFTD";
						break;
					case  9: desc = "Color of Examination Room - UFO" + Environment.NewLine
								  + "Color of (unknown) (alias of Examination Room) - TFTD";
						break;
					case 10: desc = "Color of Alien Alloys - UFO" + Environment.NewLine
								  + "Color of Aqua Plastics - TFTD";
						break;
					case 11: desc = "Color of Alien Habitat - UFO" + Environment.NewLine
								  + "Color of Examination Room (alias of Alien Re-animation Zone) - TFTD";
						break;
					case 12: desc = "Color of (dead tile)";
						break;
					case 13: desc = "Color of (exit point)";
						break;
					case 14: desc = "Color of (must destroy tile)" + Environment.NewLine
								  + "Alien Brain - UFO" + Environment.NewLine
								  + "T'leth Power Cylinders - TFTD";
						break;
				}

				// NOTE: The colors of these brushes get overwritten by the
				// Option settings somewhere/how between here and their actual
				// use in TilePanel.OnPaint(). That is, this only sets default
				// colors and might not even be very useful other than as
				// perhaps for placeholder-key(s) for the actual values that
				// are later retrieved from Options ....
				//
				// See OnSpecialPropertyColorChanged() below_
				Settings.AddSetting(
								specialType,
								((SolidBrush)_brushes[specialType]).Color,
								desc,					// appears as a tip at the bottom of the Options screen.
								"TileBackgroundColors",	// this identifies what Option category the setting appears under.
								OnSpecialPropertyColorChanged);
			}
			TilePanel.SetSpecialPropertyColors(_brushes);

			VolutarSettingService.LoadSettings(Settings);
		}

		/// <summary>
		/// Loads a different brush/color for a tiletype's Special Property into
		/// an already existing key.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="key">a string representing the SpecialType</param>
		/// <param name="val">the brush to insert</param>
		private void OnSpecialPropertyColorChanged(object sender, string key, object val)
		{
			((SolidBrush)_brushes[key]).Color = (Color)val;
			Refresh();
		}

		/// <summary>
		/// Gets the brushes/colors for all tiletypes' Special Properties.
		/// </summary>
		/// <returns>a hashtable of the brushes</returns>
		internal Hashtable GetSpecialPropertyBrushes()
		{
			return _brushes;
		}


		private Form _foptions;
		private bool _closing;

		private void OnOptionsClick(object sender, EventArgs e)
		{
			var it = (ToolStripMenuItem)sender;
			if (!it.Checked)
			{
				it.Checked = true;

				_foptions = new OptionsForm("TileViewOptions", Settings);
				_foptions.Text = "Tile View Options";

				_foptions.Show();

				_foptions.Closing += (sender1, e1) =>
				{
					if (!_closing)
						OnOptionsClick(sender, e);

					_closing = false;
				};
			}
			else
			{
				_closing = true;

				it.Checked = false;
				_foptions.Close();
			}
		}

		/// <summary>
		/// Opens the MCD-info screen.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnMcdInfoClick(object sender, EventArgs e)
		{
			if (!tsmiMcdInfo.Checked)
			{
				tsmiMcdInfo.Checked = true;

				if (_mcdInfoForm == null)
				{
					_mcdInfoForm = new McdViewerForm();
					_mcdInfoForm.Closing += OnMcdInfoClosing;

					var f = FindForm();

					McdRecord record = null;

					var tile = SelectedTile;
					if (tile != null)
					{
						f.Text = BuildTitleString(tile.TileListId, tile.Id);
						record = tile.Record;
					}
					else
						f.Text = "Tile View";

					_mcdInfoForm.UpdateData(record);
				}
				_mcdInfoForm.Show();
			}
			else
				OnMcdInfoClosing(null, null);
		}

		/// <summary>
		/// Hides the MCD-info screen.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMcdInfoClosing(object sender, CancelEventArgs e)
		{
			tsmiMcdInfo.Checked = false;

			if (e != null)			// if (e==null) the form is hiding due to a menu-click, or a double-click on a tile
				e.Cancel = true;	// if (e!=null) the form really was closed, so cancel that.

			_mcdInfoForm.Hide();
		}

		/// <summary>
		/// Opens MCDEdit.exe or any program or file specified in Settings.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnVolutarMcdEditorClick(object sender, EventArgs e)
		{
			if ((MapBase as XCMapFile) != null)
			{
				var service = new VolutarSettingService(Settings);
				var pfe = service.FullPath;	// this will invoke a box for the user to input the
											// executable's path if it doesn't exist in Settings.
				if (File.Exists(pfe))
				{
					string dir = Path.GetDirectoryName(pfe); // change to MCDEdit dir so that accessing MCDEdit.txt doesn't cause probs.
					Directory.SetCurrentDirectory(dir);

					Process.Start(new ProcessStartInfo(pfe));

					Directory.SetCurrentDirectory(SharedSpace.Instance.GetString(SharedSpace.ApplicationDirectory)); // change back to app dir
				}
			}
		}

		/// <summary>
		/// Opens PckView with the tileset of the currently selected tile loaded.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPckEditorClick(object sender, EventArgs e)
		{
			var dep = GetDepLabel();
			if (dep != null)
			{
				var imageInfo = GameInfo.ImageInfo[dep];
				if (imageInfo != null)
				{
					string pfePck = imageInfo.Path + imageInfo.Label + PckSpriteCollection.PckExt;
					string pfeTab = pfePck.Substring(0, pfePck.Length - 4);
					pfeTab += PckSpriteCollection.TabExt;

					if (!File.Exists(pfePck))
					{
						MessageBox.Show(
									this,
									"File does not exist: " + pfePck,
									"Error",
									MessageBoxButtons.OK,
									MessageBoxIcon.Error,
									MessageBoxDefaultButton.Button1,
									0);
					}
					else if (!File.Exists(pfeTab))
					{
						MessageBox.Show(
									this,
									"File does not exist: " + pfeTab,
									"Error",
									MessageBoxButtons.OK,
									MessageBoxIcon.Error,
									MessageBoxDefaultButton.Button1,
									0);
					}
					else
					{
						_showHideManager.HideAll();

						using (var f = new PckViewForm())
						{
							var pckPack = imageInfo.GetPckPack();
							f.SelectedPalette = pckPack.Pal.Label;
							f.LoadSpriteCollectionFile(pfePck, true);

							var parent = FindForm();

							Form owner = null;
							if (parent != null)
								owner = parent.Owner;

							if (owner == null)
								owner = parent;

							f.ShowDialog(owner);
							if (f.SavedFile)
							{
								GameInfo.ImageInfo.Images[dep].ClearMcdTable();
								GameInfo.ClearPckCache(imageInfo.Path, imageInfo.Label);

								OnMapChanged();
							}
						}

						_showHideManager.RestoreAll();
					}
				}
			}
			else
			{
				MessageBox.Show(
							this,
							"Select a Tile.",
							"Notice",
							MessageBoxButtons.OK,
							MessageBoxIcon.Asterisk,
							MessageBoxDefaultButton.Button1,
							0);
			}
		}

		#region Methods
		/// <summary>
		/// Builds and returns a string that's appropriate for the currently
		/// selected tile.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="mcd"></param>
		/// <returns></returns>
		private string BuildTitleString(int id, int mcd)
		{
			var dep = GetDepLabel();
			return "Tile View - id: " + id + "  mcd: " + mcd + "  file: " + (dep ?? "unknown");
		}

		/// <summary>
		/// Gets the label of the tileset of the currently selected tile.
		/// </summary>
		/// <returns></returns>
		private string GetDepLabel()
		{
			var tile = SelectedTile;
			if (tile != null)
			{
				var mapFile = MapBase as XCMapFile;
				if (mapFile != null)
					return mapFile.GetDepLabel(tile);
			}
			return null;
		}

		/// <summary>
		/// Gets the panel of the currently displayed tabpage.
		/// </summary>
		/// <returns></returns>
		internal Panel GetSelectedPanel()
		{
			return _panels[tcTileTypes.SelectedIndex];
		}
		#endregion
	}
}
