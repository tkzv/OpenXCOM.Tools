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

		public event MethodInvoker MapChanged;
		private void OnMapChanged()
		{
			MethodInvoker handler = MapChanged;
			if (handler != null)
				handler();
		}


		private IMainShowAllManager _showAllManager;

		private IContainer components = null; // quahhaha

		private TilePanel allTiles;
		private TilePanel[] panels;

		private McdViewerForm McdInfoForm;

		private Hashtable _brushes;

		public TileView()
		{
			InitializeComponent();

			tcTileTypes.Selected += OnTabSelected;

			allTiles    = new TilePanel(TileType.All);
			var ground  = new TilePanel(TileType.Ground);
			var wWalls  = new TilePanel(TileType.WestWall);
			var nWalls  = new TilePanel(TileType.NorthWall);
			var objects = new TilePanel(TileType.Object);

			panels = new[]
			{
				allTiles,
				ground,
				wWalls,
				nWalls,
				objects
			};

			AddPanel(allTiles, tpAll);

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
			if (tile != null && tile.Info is McdRecord)
			{
				f.Text = BuildTitleString(tile.TileListId, tile.Id);

				if (McdInfoForm != null)
					McdInfoForm.UpdateData((McdRecord)tile.Info);
			}
			else
			{
				f.Text = "Tile View";

				if (McdInfoForm != null)
					McdInfoForm.UpdateData(null);
			}

			OnSelectedTileTypeChanged(tile);
		}

		public void Initialize(IMainShowAllManager showAllManager)
		{
			_showAllManager = showAllManager;
		}

		public override void LoadDefaultSettings()
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
								OnBrushChanged,
								false,
								null);
			}
			VolutarSettingService.LoadDefaultSettings(Settings);

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
			if (tile != null && tile.Info is McdRecord)
			{
				f.Text = BuildTitleString(tile.TileListId, tile.Id);

				if (McdInfoForm != null)
					McdInfoForm.UpdateData((McdRecord)tile.Info);
			}
			else
			{
				f.Text = "Tile View";

				if (McdInfoForm != null)
					McdInfoForm.UpdateData(null);
			}
		}

		private void OnOptionsClick(object sender, EventArgs e)
		{
			var f = new OptionsForm("TileViewOptions", Settings);
			f.Text = "Tile View Options";
			f.Show();
		}

		public override IMapBase Map
		{
			set
			{
				base.Map = value;
				Tiles = (value != null) ? value.Tiles
										: null;
			}
		}

		private System.Collections.Generic.IList<TileBase> Tiles
		{
			set
			{
				for (int i = 0; i != panels.Length; ++i)
					panels[i].SetTiles(value);

				OnResize(null);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TileBase SelectedTile
		{
			get { return panels[tcTileTypes.SelectedIndex].SelectedTile; }
			set
			{
				allTiles.SelectedTile = value;
				tcTileTypes.SelectedIndex = 0;

				Refresh();
			}
		}

		private void OnMcdInfoClick(object sender, EventArgs e)
		{
			if (McdInfoForm == null)
			{
				McdInfoForm = new McdViewerForm();
				McdInfoForm.Closing += OnMcdInfoClosing;

				var f = FindForm();

				var tile = SelectedTile;
				if (tile != null && tile.Info is McdRecord)
				{
					f.Text = BuildTitleString(tile.TileListId, tile.Id);
					McdInfoForm.UpdateData((McdRecord)tile.Info);
				}
				else
				{
					f.Text = "Tile View";
					McdInfoForm.UpdateData(null);
				}
			}
			McdInfoForm.Show();
		}

		private void OnMcdInfoClosing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			McdInfoForm.Hide();
		}

		private void OnVolutarMcdEditorClick(object sender, EventArgs e)
		{
			if ((Map as XCMapFile) != null)
			{
				var service = new VolutarSettingService(Settings);
				var path = service.FullPath;	// this will invoke a box for the user to input the
												// executable's path if it doesn't exist in Settings.
				if (!String.IsNullOrEmpty(path))
				{
					string dir = path.Substring(0, path.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
					Directory.SetCurrentDirectory(dir); // change to MCDEdit dir so that accessing MCDEdit.txt doesn't cause probs.

					Process.Start(new ProcessStartInfo(path));

					Directory.SetCurrentDirectory(SharedSpace.Instance.GetString(SharedSpace.AppDir)); // change back to app dir
				}
			}
		}

		private string BuildTitleString(int id, int mcd)
		{
			var dep = GetDepLabel();
			return "Tile View : id " + id + " - mcd " + mcd + " - " + (dep ?? "unknown");
		}

		private string GetDepLabel()
		{
			var tile = SelectedTile;
			if (tile != null)
			{
				var file = Map as XCMapFile;
				if (file != null)
					return file.GetDepLabel(tile);
			}
			return null;
		}

		private void OnEditPckClick(object sender, EventArgs e) // NOTE: The dropdown button has been disabled (Visible=FALSE).
		{
			var dep = GetDepLabel();
			if (dep != null)
			{
				var imageInfo = GameInfo.ImageInfo[dep];
				if (imageInfo != null)
				{
					var pfe = imageInfo.BasePath + imageInfo.BaseName + ".PCK"; // pfe=PathFileExt

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
							var pckFile = imageInfo.GetPckPack();
							editor.SelectedPalette = pckFile.Pal.Label;
							editor.LoadPckFile(pfe, pckFile.Bpp);

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
								GameInfo.ClearPckCache(imageInfo.BasePath, imageInfo.BaseName);

								OnMapChanged();
							}
						}

						_showAllManager.RestoreAll();
					}
				}
			}
		}
	}

	internal delegate void SelectedTileTypeChangedEventHandler(TileBase tile);
}
