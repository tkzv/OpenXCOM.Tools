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
		public event SelectedTileTypeChangedEventHandler SelectedTileTypeChangedObserver;
		private void OnSelectedTileTypeChanged(TileBase tile)
		{
			var handler = SelectedTileTypeChangedObserver;
			if (handler != null)
				handler(tile);
		}

		public event MethodInvoker MapChangedEventHandler;


		private ShowHideManager _showAllManager;

		private IContainer components = null; // quahhaha

		private TilePanel _allTiles;
		private TilePanel[] _panels;

		private McdViewerForm _mcdInfoForm;

		private Hashtable _brushes;

		public TileView()
		{
			InitializeComponent();

			tcTileTypes.Selected += OnTabSelected;

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
			page.Controls.Add(panel);
			panel.SelectedTileTypeChangedPanel += OnTileChanged;
		}

		private void OnTileChanged(TileBase tile)
		{
			var f = FindForm();
			if (tile != null)
			{
				f.Text = BuildTitleString(tile.TileListId, tile.Id);

				if (_mcdInfoForm != null)
					_mcdInfoForm.UpdateData(tile.Record);
			}
			else
			{
				f.Text = "Tile View";

				if (_mcdInfoForm != null)
					_mcdInfoForm.UpdateData(null);
			}

			OnSelectedTileTypeChanged(tile);
		}

		public void Initialize(ShowHideManager showAllManager)
		{
			_showAllManager = showAllManager;
		}

		/// <summary>
		/// Loads default settings for TileView screen.
		/// </summary>
		public override void LoadControl0Settings()
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

			TilePanel.SetColors(_brushes);
		}

		private void OnBrushChanged(object sender, string key, object val)
		{
			((SolidBrush)_brushes[key]).Color = (Color)val;
			Refresh();
		}

		private void OnTabSelected(object sender, TabControlEventArgs e)
		{
			var f = FindForm();

			var tile = SelectedTile;
			if (tile != null)
			{
				f.Text = BuildTitleString(tile.TileListId, tile.Id);

				if (_mcdInfoForm != null)
					_mcdInfoForm.UpdateData(tile.Record);
			}
			else
			{
				f.Text = "Tile View";

				if (_mcdInfoForm != null)
					_mcdInfoForm.UpdateData(null);
			}
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
		public TileBase SelectedTile
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

				var tile = SelectedTile;
				if (tile != null)
				{
					f.Text = BuildTitleString(tile.TileListId, tile.Id);
					_mcdInfoForm.UpdateData(tile.Record);
				}
				else
				{
					f.Text = "Tile View";
					_mcdInfoForm.UpdateData(null);
				}
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

		private void OnPckEditorClick(object sender, EventArgs e) // NOTE: The dropdown button has been disabled (Visible=FALSE).
		{
			var dep = GetDepLabel();
			if (dep != null)
			{
				var imageInfo = GameInfo.ImageInfo[dep];
				if (imageInfo != null)
				{
					var pfe = imageInfo.Path + imageInfo.Label + ".PCK";

					if (!File.Exists(pfe))
					{
						MessageBox.Show(
									this,
									"File does not exist: " + pfe,
									"Error",
									MessageBoxButtons.OK,
									MessageBoxIcon.Exclamation,
									MessageBoxDefaultButton.Button1,
									0);
					}
					else
					{
						_showAllManager.HideAll();

						using (var editor = new PckViewForm())
						{
							var pckPack = imageInfo.GetPckPack();
							editor.SelectedPalette = pckPack.Pal.Label;
							editor.LoadPckFile(pfe, pckPack.Bpp);

							var parent = FindForm();

							Form owner = null;
							if (parent != null)
								owner = parent.Owner;

							if (owner == null)
								owner = parent;

							editor.ShowDialog(owner);
							if (editor.SavedFile)
							{
								GameInfo.ImageInfo.Images[dep].ClearMcdTable();
								GameInfo.ClearPckCache(imageInfo.Path, imageInfo.Label);

								HandleMapChanged();
							}
						}

						_showAllManager.RestoreAll();
					}
				}
			}
		}

		private void HandleMapChanged()
		{
			MethodInvoker handler = MapChangedEventHandler;
			if (handler != null)
				handler();
		}
	}

	internal delegate void SelectedTileTypeChangedEventHandler(TileBase tile);
}
