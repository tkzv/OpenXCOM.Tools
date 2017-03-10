using System;
using System.Collections;

using XCom;
using XCom.Interfaces.Base;


namespace MapView
{
	public class MapInfoForm
		:
		System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblDimensions;
		private System.Windows.Forms.Label lblPckFiles;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblPckImages;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lblMcd;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label lblFilled;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ProgressBar pBar;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupAnalyze;
		private System.Windows.Forms.GroupBox groupInfo;

		private IMap_Base _map;
		private int slotsUsed = 0;

		public MapInfoForm()
		{
			InitializeComponent();
		}

		public IMap_Base Map
		{
			set
			{
				_map = value;
				startAnalyzing();
			}
		}

		private void startAnalyzing()
		{
			groupAnalyze.Visible = true;
			var imgHash = new Hashtable();
			var mcdHash = new Hashtable();
			groupInfo.Text = "Map: " + _map.Name;
			lblDimensions.Text = _map.MapSize.Rows + "," + _map.MapSize.Cols + "," + _map.MapSize.Height;

			lblPckFiles.Text = String.Empty;
			bool one = true;
			int totalImages = 0;
			int totalMcd = 0;

			var mapFile = _map as XCMapFile;
			if (mapFile != null)
			{
				foreach (string st in mapFile.Dependencies)
				{
					if (one)
						one = false;
					else
						lblPckFiles.Text += ",";

					totalImages += GameInfo.ImageInfo[st].GetPckFile().Count;
					totalMcd += GameInfo.ImageInfo[st].GetMcdFile().Count;
					lblPckFiles.Text += st;
				}
			}

			pBar.Maximum = _map.MapSize.Rows * _map.MapSize.Cols * _map.MapSize.Height;
			pBar.Value = 0;

			for (int h = 0; h < _map.MapSize.Height; h++)
				for (int r = 0; r < _map.MapSize.Rows; r++)
					for (int c = 0; c < _map.MapSize.Cols; c++)
					{
						var tile = (XCMapTile)_map[r, c, h];
						if (!tile.Blank)
						{
							if (tile.Ground != null)
							{
								count(imgHash, mcdHash, tile.Ground);
								var tilePart = tile.Ground as XCTile;
								if (tilePart != null)
									count(imgHash, mcdHash, tilePart.Dead);
								slotsUsed++;
							}

							if (tile.West != null)
							{
								count(imgHash, mcdHash, tile.West);
								var tilePart = tile.West as XCTile;
								if (tilePart != null)
									count(imgHash, mcdHash, tilePart.Dead);
								slotsUsed++;
							}

							if (tile.North != null)
							{
								count(imgHash, mcdHash, tile.North);
								var tilePart = tile.North as XCTile;
								if (tilePart != null)
									count(imgHash, mcdHash, tilePart.Dead);
								slotsUsed++;
							}

							if (tile.Content != null)
							{
								count(imgHash, mcdHash, tile.Content);
								var tilePart = tile.Content as XCTile;
								if (tilePart != null)
									count(imgHash, mcdHash, tilePart.Dead);
								slotsUsed++;
							}

							pBar.Value = (r + 1) * (c + 1) * (h + 1);
							pBar.Refresh();
						}
					}

			var pct = Math.Round(100 * ((mcdHash.Keys.Count * 1.0) / totalMcd), 2);
			lblMcd.Text = mcdHash.Keys.Count + "/" + totalMcd + " - " + pct + "%";

			pct = Math.Round(100 * ((imgHash.Keys.Count * 1.0) / totalImages), 2);
			lblPckImages.Text = imgHash.Keys.Count + "/" + totalImages + " - " + pct + "%";

			pct = Math.Round(100 * ((slotsUsed * 1.0) / (pBar.Maximum * 4)), 2);
			lblFilled.Text = slotsUsed + "/" + (pBar.Maximum * 4) + " - " + pct + "%";

			groupAnalyze.Visible = false;
		}

		private void count(IDictionary img, IDictionary mcd, TileBase tile)
		{
			if (tile != null)
			{
				foreach (PckImage pi in tile.Images)
					img[pi.StaticID] = true;

				mcd[tile.Info.ID] = true;
			}
		}


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
			this.label1 = new System.Windows.Forms.Label();
			this.lblDimensions = new System.Windows.Forms.Label();
			this.lblPckFiles = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lblPckImages = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lblMcd = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.lblFilled = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.pBar = new System.Windows.Forms.ProgressBar();
			this.groupAnalyze = new System.Windows.Forms.GroupBox();
			this.groupInfo = new System.Windows.Forms.GroupBox();
			this.groupAnalyze.SuspendLayout();
			this.groupInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(115, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Dimensions (r,c,h): ";
			// 
			// lblDimensions
			// 
			this.lblDimensions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.lblDimensions.Location = new System.Drawing.Point(125, 20);
			this.lblDimensions.Name = "lblDimensions";
			this.lblDimensions.Size = new System.Drawing.Size(359, 15);
			this.lblDimensions.TabIndex = 1;
			// 
			// lblPckFiles
			// 
			this.lblPckFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.lblPckFiles.Location = new System.Drawing.Point(125, 35);
			this.lblPckFiles.Name = "lblPckFiles";
			this.lblPckFiles.Size = new System.Drawing.Size(359, 15);
			this.lblPckFiles.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 35);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(115, 15);
			this.label3.TabIndex = 2;
			this.label3.Text = "Pck files used:";
			// 
			// lblPckImages
			// 
			this.lblPckImages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.lblPckImages.Location = new System.Drawing.Point(125, 50);
			this.lblPckImages.Name = "lblPckImages";
			this.lblPckImages.Size = new System.Drawing.Size(359, 15);
			this.lblPckImages.TabIndex = 5;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(10, 50);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(115, 15);
			this.label4.TabIndex = 4;
			this.label4.Text = "Pck images used:";
			// 
			// lblMcd
			// 
			this.lblMcd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.lblMcd.Location = new System.Drawing.Point(125, 65);
			this.lblMcd.Name = "lblMcd";
			this.lblMcd.Size = new System.Drawing.Size(359, 15);
			this.lblMcd.TabIndex = 7;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(10, 65);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(115, 15);
			this.label6.TabIndex = 6;
			this.label6.Text = "Mcd entries used:";
			// 
			// lblFilled
			// 
			this.lblFilled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.lblFilled.Location = new System.Drawing.Point(125, 80);
			this.lblFilled.Name = "lblFilled";
			this.lblFilled.Size = new System.Drawing.Size(359, 15);
			this.lblFilled.TabIndex = 9;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(10, 80);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(115, 15);
			this.label8.TabIndex = 8;
			this.label8.Text = "% of map filled:";
			// 
			// pBar
			// 
			this.pBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pBar.Location = new System.Drawing.Point(3, 15);
			this.pBar.Name = "pBar";
			this.pBar.Size = new System.Drawing.Size(486, 26);
			this.pBar.TabIndex = 1;
			// 
			// groupAnalyze
			// 
			this.groupAnalyze.Controls.Add(this.pBar);
			this.groupAnalyze.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.groupAnalyze.Location = new System.Drawing.Point(0, 102);
			this.groupAnalyze.Name = "groupAnalyze";
			this.groupAnalyze.Size = new System.Drawing.Size(492, 44);
			this.groupAnalyze.TabIndex = 11;
			this.groupAnalyze.TabStop = false;
			this.groupAnalyze.Text = "Analyzing";
			// 
			// groupInfo
			// 
			this.groupInfo.Controls.Add(this.lblPckImages);
			this.groupInfo.Controls.Add(this.label3);
			this.groupInfo.Controls.Add(this.lblPckFiles);
			this.groupInfo.Controls.Add(this.label1);
			this.groupInfo.Controls.Add(this.lblMcd);
			this.groupInfo.Controls.Add(this.label4);
			this.groupInfo.Controls.Add(this.label6);
			this.groupInfo.Controls.Add(this.lblFilled);
			this.groupInfo.Controls.Add(this.label8);
			this.groupInfo.Controls.Add(this.lblDimensions);
			this.groupInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupInfo.Location = new System.Drawing.Point(0, 0);
			this.groupInfo.Name = "groupInfo";
			this.groupInfo.Size = new System.Drawing.Size(492, 102);
			this.groupInfo.TabIndex = 12;
			this.groupInfo.TabStop = false;
			this.groupInfo.Text = "Map";
			// 
			// MapInfoForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(492, 146);
			this.Controls.Add(this.groupInfo);
			this.Controls.Add(this.groupAnalyze);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "MapInfoForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Map Info";
			this.groupAnalyze.ResumeLayout(false);
			this.groupInfo.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
	}
}
