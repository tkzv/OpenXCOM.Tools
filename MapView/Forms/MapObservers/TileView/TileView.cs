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


		private ShowHideManager _showAllManager;

		private IContainer components = null; // quahhaha

		private TilePanel _allTiles;
		private TilePanel[] _panels;

		private McdViewerForm _mcdInfoForm;

		private Hashtable _brushes;


		internal TileView()
		{
			InitializeComponent();

			tcTileTypes.Selected += OnPageSelected;

			_allTiles   = new TilePanel(TileType.All);
			var ground  = new TilePanel(TileType.Ground);
			var wWalls  = new TilePanel(TileType.WestWall);
			var nWalls  = new TilePanel(TileType.NorthWall);
			var objects = new TilePanel(TileType.Object);

			_panels = new[]
			{
				_allTiles,
				ground,
				wWalls,
				nWalls,
				objects
			};

			AddPanel(_allTiles, tpAll);

			AddPanel(ground,  tpFloors);
			AddPanel(wWalls,  tpWestwalls);
			AddPanel(nWalls,  tpNorthwalls);
			AddPanel(objects, tpObjects);
		}


		private void AddPanel(TilePanel panel, Control page)
		{
			panel.Dock = DockStyle.Fill;
			panel.PanelSelectedTileChanged += OnPanelTileChanged;

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
		/// Loads default settings for TileView screen.
		/// </summary>
		public override void LoadControl0Settings() // TODO: access that internal
		{
			_brushes = new Hashtable();

			foreach (string type in Enum.GetNames(typeof(SpecialType)))
			{
				_brushes[type] = new SolidBrush(TilePanel.TileColors[(int)Enum.Parse(typeof(SpecialType), type)]);
				Settings.AddSetting(
								type,
								((SolidBrush)_brushes[type]).Color,
								"Color of specified tile type",
								"TileView",
								OnBrushChanged);
			}
			VolutarSettingService.LoadSettings(Settings);

			TilePanel.SetBrushes(_brushes);
		}

		private void OnBrushChanged(object sender, string key, object val)
		{
			((SolidBrush)_brushes[key]).Color = (Color)val;
			Refresh();
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
