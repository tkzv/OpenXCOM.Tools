using System;
using System.Collections;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces.Base;


namespace MapView
{
	public class MapInfoForm
		:
		Form
	{
		public MapInfoForm()
		{
			InitializeComponent();
		}


		public void Analyze(IMap_Base baseMap)
		{
			groupInfo.Text = "Map: " + baseMap.Name;

			lbl2Dimensions.Text = baseMap.MapSize.Cols + ","
								+ baseMap.MapSize.Rows + ","
								+ baseMap.MapSize.Height;

			lbl2PckFiles.Text = String.Empty;

			int recordsTotal = 0;
			int spritesTotal = 0;

			bool first = true;
			var mapFile = baseMap as XCMapFile;
			if (mapFile != null)
			{
				foreach (string dep in mapFile.Dependencies)
				{
					if (first)
						first = false;
					else
						lbl2PckFiles.Text += ";";

					lbl2PckFiles.Text += dep;

					recordsTotal += GameInfo.ImageInfo[dep].GetMcdFile().Count;
					spritesTotal += GameInfo.ImageInfo[dep].GetPckFile().Count;
				}
			}

			Refresh();
			groupAnalyze.Visible = true;

			int parts = 0;

			var recordsTable = new Hashtable();
			var spritesTable = new Hashtable();

			pBar.Maximum = baseMap.MapSize.Cols * baseMap.MapSize.Rows * baseMap.MapSize.Height;
			pBar.Value = 0;

			for (int c = 0; c != baseMap.MapSize.Cols; ++c)
			{
				for (int r = 0; r != baseMap.MapSize.Rows; ++r)
				{
					for (int h = 0; h != baseMap.MapSize.Height; ++h)
					{
						var tile = baseMap[r, c, h] as XCMapTile;
						if (!tile.Blank)
						{
							if (tile.Ground != null)
							{
								++parts;
								Count(tile.Ground, recordsTable, spritesTable);

								var tilePart = tile.Ground as XCTile;
								if (tilePart != null)
									Count(tilePart.Dead, recordsTable, spritesTable);
							}

							if (tile.West != null)
							{
								++parts;
								Count(tile.West, recordsTable, spritesTable);

								var tilePart = tile.West as XCTile;
								if (tilePart != null)
									Count(tilePart.Dead, recordsTable, spritesTable);
							}

							if (tile.North != null)
							{
								++parts;
								Count(tile.North, recordsTable, spritesTable);

								var tilePart = tile.North as XCTile;
								if (tilePart != null)
									Count(tilePart.Dead, recordsTable, spritesTable);
							}

							if (tile.Content != null)
							{
								++parts;
								Count(tile.Content, recordsTable, spritesTable);

								var tilePart = tile.Content as XCTile;
								if (tilePart != null)
									Count(tilePart.Dead, recordsTable, spritesTable);
							}

							++pBar.Value;
							pBar.Refresh();
//							System.Threading.Thread.Sleep(1); // for testing.
						}
					}
				}
			}

			var pct = Math.Round(100.0 * (double)recordsTable.Keys.Count / (double)recordsTotal, 2);
			lbl2McdRecords.Text = recordsTable.Keys.Count + "/" + recordsTotal + " - " + pct + "%";

			pct = Math.Round(100.0 * (double)parts / (pBar.Maximum * 4), 2);
			lbl2PartsFilled.Text = parts + "/" + (pBar.Maximum * 4) + " - " + pct + "%";

			pct = Math.Round(100.0 * (double)spritesTable.Keys.Count / (double)spritesTotal, 2);
			lbl2PckImages.Text = spritesTable.Keys.Count + "/" + spritesTotal + " - " + pct + "%";


			groupAnalyze.Text = String.Empty; // hide the progress bar and show the Cancel btn.
			pBar.Visible = false;
			btnCancel.Visible = true;

			Refresh();
		}

		private void Count(
				TileBase tile,
				IDictionary recordsTable,
				IDictionary spritesTable)
		{
			if (tile != null)
			{
				recordsTable[tile.Info.Id] = true;

				var images = tile.Images;
				foreach (PckImage image in images)
					spritesTable[image.StaticId] = true;
			}
		}

		private void btnClose(object sender, MouseEventArgs e)
		{
			Close();
		}

		private void keyClose(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
		}


		private GroupBox groupInfo;
		private Label lbl1Dimensions;
		private Label lbl2Dimensions;
		private Label lbl1PckFiles;
		private Label lbl2PckFiles;
		private Label lbl1PckImages;
		private Label lbl2PckImages;
		private Label lbl1McdRecords;
		private Label lbl2McdRecords;
		private Label lbl1PartsFilled;
		private Label lbl2PartsFilled;
		private GroupBox groupAnalyze;
		private ProgressBar pBar;
		private Button btnCancel;

		private System.ComponentModel.Container components = null;


		#region Windows Form Designer generated code

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lbl1Dimensions = new System.Windows.Forms.Label();
			this.lbl2Dimensions = new System.Windows.Forms.Label();
			this.lbl2PckFiles = new System.Windows.Forms.Label();
			this.lbl1PckFiles = new System.Windows.Forms.Label();
			this.lbl2PckImages = new System.Windows.Forms.Label();
			this.lbl1PckImages = new System.Windows.Forms.Label();
			this.lbl2McdRecords = new System.Windows.Forms.Label();
			this.lbl1McdRecords = new System.Windows.Forms.Label();
			this.lbl2PartsFilled = new System.Windows.Forms.Label();
			this.lbl1PartsFilled = new System.Windows.Forms.Label();
			this.pBar = new System.Windows.Forms.ProgressBar();
			this.groupAnalyze = new System.Windows.Forms.GroupBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupInfo = new System.Windows.Forms.GroupBox();
			this.groupAnalyze.SuspendLayout();
			this.groupInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// lbl1Dimensions
			// 
			this.lbl1Dimensions.Location = new System.Drawing.Point(10, 20);
			this.lbl1Dimensions.Name = "lbl1Dimensions";
			this.lbl1Dimensions.Size = new System.Drawing.Size(115, 15);
			this.lbl1Dimensions.TabIndex = 0;
			this.lbl1Dimensions.Text = "dimensions (c,r,h)";
			// 
			// lbl2Dimensions
			// 
			this.lbl2Dimensions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl2Dimensions.Location = new System.Drawing.Point(130, 20);
			this.lbl2Dimensions.Name = "lbl2Dimensions";
			this.lbl2Dimensions.Size = new System.Drawing.Size(356, 15);
			this.lbl2Dimensions.TabIndex = 1;
			// 
			// lbl2PckFiles
			// 
			this.lbl2PckFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl2PckFiles.Location = new System.Drawing.Point(130, 35);
			this.lbl2PckFiles.Name = "lbl2PckFiles";
			this.lbl2PckFiles.Size = new System.Drawing.Size(356, 15);
			this.lbl2PckFiles.TabIndex = 3;
			// 
			// lbl1PckFiles
			// 
			this.lbl1PckFiles.Location = new System.Drawing.Point(10, 35);
			this.lbl1PckFiles.Name = "lbl1PckFiles";
			this.lbl1PckFiles.Size = new System.Drawing.Size(115, 15);
			this.lbl1PckFiles.TabIndex = 2;
			this.lbl1PckFiles.Text = "PCK Sets";
			// 
			// lbl2PckImages
			// 
			this.lbl2PckImages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl2PckImages.Location = new System.Drawing.Point(130, 50);
			this.lbl2PckImages.Name = "lbl2PckImages";
			this.lbl2PckImages.Size = new System.Drawing.Size(356, 15);
			this.lbl2PckImages.TabIndex = 5;
			// 
			// lbl1PckImages
			// 
			this.lbl1PckImages.Location = new System.Drawing.Point(10, 50);
			this.lbl1PckImages.Name = "lbl1PckImages";
			this.lbl1PckImages.Size = new System.Drawing.Size(115, 15);
			this.lbl1PckImages.TabIndex = 4;
			this.lbl1PckImages.Text = "sprites";
			// 
			// lbl2McdRecords
			// 
			this.lbl2McdRecords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl2McdRecords.Location = new System.Drawing.Point(130, 65);
			this.lbl2McdRecords.Name = "lbl2McdRecords";
			this.lbl2McdRecords.Size = new System.Drawing.Size(356, 15);
			this.lbl2McdRecords.TabIndex = 7;
			// 
			// lbl1McdRecords
			// 
			this.lbl1McdRecords.Location = new System.Drawing.Point(10, 65);
			this.lbl1McdRecords.Name = "lbl1McdRecords";
			this.lbl1McdRecords.Size = new System.Drawing.Size(115, 15);
			this.lbl1McdRecords.TabIndex = 6;
			this.lbl1McdRecords.Text = "MCD Records";
			// 
			// lbl2PartsFilled
			// 
			this.lbl2PartsFilled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl2PartsFilled.Location = new System.Drawing.Point(130, 80);
			this.lbl2PartsFilled.Name = "lbl2PartsFilled";
			this.lbl2PartsFilled.Size = new System.Drawing.Size(356, 15);
			this.lbl2PartsFilled.TabIndex = 9;
			// 
			// lbl1PartsFilled
			// 
			this.lbl1PartsFilled.Location = new System.Drawing.Point(10, 80);
			this.lbl1PartsFilled.Name = "lbl1PartsFilled";
			this.lbl1PartsFilled.Size = new System.Drawing.Size(115, 15);
			this.lbl1PartsFilled.TabIndex = 8;
			this.lbl1PartsFilled.Text = "tile parts";
			// 
			// pBar
			// 
			this.pBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pBar.Location = new System.Drawing.Point(3, 15);
			this.pBar.Name = "pBar";
			this.pBar.Size = new System.Drawing.Size(488, 26);
			this.pBar.TabIndex = 1;
			// 
			// groupAnalyze
			// 
			this.groupAnalyze.Controls.Add(this.btnCancel);
			this.groupAnalyze.Controls.Add(this.pBar);
			this.groupAnalyze.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.groupAnalyze.Location = new System.Drawing.Point(0, 102);
			this.groupAnalyze.Name = "groupAnalyze";
			this.groupAnalyze.Size = new System.Drawing.Size(494, 44);
			this.groupAnalyze.TabIndex = 11;
			this.groupAnalyze.TabStop = false;
			this.groupAnalyze.Text = "Analyzing";
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(205, 10);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(85, 30);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Visible = false;
			this.btnCancel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnClose);
			// 
			// groupInfo
			// 
			this.groupInfo.Controls.Add(this.lbl2PckImages);
			this.groupInfo.Controls.Add(this.lbl1PckFiles);
			this.groupInfo.Controls.Add(this.lbl2PckFiles);
			this.groupInfo.Controls.Add(this.lbl1Dimensions);
			this.groupInfo.Controls.Add(this.lbl2McdRecords);
			this.groupInfo.Controls.Add(this.lbl1PckImages);
			this.groupInfo.Controls.Add(this.lbl1McdRecords);
			this.groupInfo.Controls.Add(this.lbl2PartsFilled);
			this.groupInfo.Controls.Add(this.lbl1PartsFilled);
			this.groupInfo.Controls.Add(this.lbl2Dimensions);
			this.groupInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupInfo.Location = new System.Drawing.Point(0, 0);
			this.groupInfo.Name = "groupInfo";
			this.groupInfo.Size = new System.Drawing.Size(494, 102);
			this.groupInfo.TabIndex = 12;
			this.groupInfo.TabStop = false;
			this.groupInfo.Text = "Map";
			// 
			// MapInfoForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(494, 146);
			this.Controls.Add(this.groupInfo);
			this.Controls.Add(this.groupAnalyze);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MapInfoForm";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "- Map Info";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyClose);
			this.groupAnalyze.ResumeLayout(false);
			this.groupInfo.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
