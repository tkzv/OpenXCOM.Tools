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
		private ShowHideManager _showAllManager;

		private IContainer components = null; // quahhaha

		private TilePanel _allTiles;
		private TilePanel[] _panels;

		private McdViewerForm _mcdInfoForm;

		private Hashtable _brushes = new Hashtable();
		#endregion


		#region Properties
		public override IMapBase BaseMap
		{
			set
			{
				base.BaseMap = value;
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
		/// cTor.
		/// </summary>
		internal TileView()
		{
			InitializeComponent();

			tcTileTypes.Selected += OnPageSelected;

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

			AddPanel(_allTiles, tpAll);

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

		private void OnPageSelected(object sender, TabControlEventArgs e)
		{
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

			if (_mcdInfoForm != null)
				_mcdInfoForm.UpdateData(record);
		}

		internal void SetShowAllManager(ShowHideManager showAllManager)
		{
			_showAllManager = showAllManager;
		}

		/// <summary>
		/// These are the default colors for tiles' Special Properties.
		/// TileView will load these colors when the program loads, then any
		/// Special Property colors that were customized will be set and
		/// accessed by TilePanel and/or the Help screen later.
		/// </summary>
		internal static readonly Color[] TileColors =
		{
			Color.NavajoWhite,		//  0 - Tile
			Color.Lavender,			//  1 - StartPoint
			Color.IndianRed,		//  2 - IonBeamAccel
			Color.PaleTurquoise,	//  3 - DestroyObjective
			Color.Khaki,			//  4 - MagneticNav
			Color.MistyRose,		//  5 - AlienCryo
			Color.Aquamarine,		//  6 - AlienClon
			Color.LightSkyBlue,		//  7 - AlienLearn
			Color.Thistle,			//  8 - AlienImplant
			Color.YellowGreen,		//  9 - Unknown9
			Color.MediumPurple,		// 10 - AlienPlastics
			Color.LightCoral,		// 11 - ExamRoom
			Color.LightCyan,		// 12 - DeadTile
			Color.BurlyWood,		// 13 - EndPoint
			Color.Blue				// 14 - MustDestroy
		};

		/// <summary>
		/// Loads default settings for TileView screen.
		/// </summary>
		public override void LoadControl0Settings() // TODO: access that as internal
		{
			foreach (string specialType in Enum.GetNames(typeof(SpecialType)))
			{
				_brushes[specialType] = new SolidBrush(TileColors[(int)Enum.Parse(typeof(SpecialType), specialType)]);

				// NOTE: The colors of the brushes get overwritten by the
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
								"Color of special tile type",	// appears as a tip at the bottom of the Options screen.
								"TileView",						// this identifies what Option category the setting appears under.
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

		private void OnMcdInfoClick(object sender, EventArgs e)
		{
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

		private void OnMcdInfoClosing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			_mcdInfoForm.Hide();
		}

		private void OnVolutarMcdEditorClick(object sender, EventArgs e)
		{
			if ((BaseMap as XCMapFile) != null)
			{
				var service = new VolutarSettingService(Settings);
				var path = service.FullPath;	// this will invoke a box for the user to input the
												// executable's path if it doesn't exist in Settings.
				if (!String.IsNullOrEmpty(path))
				{
					string dir = path.Substring(0, path.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
					Directory.SetCurrentDirectory(dir); // change to MCDEdit dir so that accessing MCDEdit.txt doesn't cause probs.

					Process.Start(new ProcessStartInfo(path));

					Directory.SetCurrentDirectory(SharedSpace.Instance.GetString(SharedSpace.ApplicationDirectory)); // change back to app dir
				}
			}
		}

		private string BuildTitleString(int id, int mcd)
		{
			var dep = GetDepLabel();
			return "Tile View - id[" + id + "]  mcd[" + mcd + "]  file[" + (dep ?? "unknown") + "]";
		}

		private string GetDepLabel()
		{
			var tile = SelectedTile;
			if (tile != null)
			{
				var file = BaseMap as XCMapFile;
				if (file != null)
					return file.GetDepLabel(tile);
			}
			return null;
		}

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
						_showAllManager.HideAll();

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

						_showAllManager.RestoreAll();
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
	}
}
