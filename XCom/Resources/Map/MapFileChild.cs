using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using XCom.Interfaces.Base;
using XCom.Services;


namespace XCom
{
	public sealed class MapFileChild
		:
			MapFileBase
	{
		#region Fields & Properties
		public   const string MapExt  = ".MAP";
		internal const string MapsDir = @"\MAPS";

		private string FullPath
		{ get; set; }

		public List<string> Terrains
		{ get; private set; }

		public RouteNodeCollection Routes
		{ get; private set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="parts"></param>
		/// <param name="routes"></param>
		internal MapFileChild(
				Descriptor descriptor,
				List<TilepartBase> parts,
				RouteNodeCollection routes)
			:
				base(descriptor, parts)
		{
			FullPath = Path.Combine(
								Descriptor.BasePath + MapsDir,
								Descriptor.Label    + MapExt);

			Terrains = Descriptor.Terrains;

//			_pathOccult = pathOccult;

			Routes = routes;

			if (File.Exists(FullPath))
			{
				for (int i = 0; i != parts.Count; ++i)
					parts[i].TilesetId = i;

				ReadMapFile(parts);
				SetupRouteNodes(routes);
				CalculateOccultations();

//				if (!String.IsNullOrEmpty(pathOccult))
//				{
//					string pfeOccult = Path.Combine(pathOccult, file + OccultFile.OccultExt);
//					if (File.Exists(pfeOccult))
//					{
//						try
//						{
//							OccultFile.LoadOccult(file, pathOccult, this);
//						}
//						catch // TODO: send warning to user.
//						{
//							for (int lev = 0; lev != MapSize.Levs; ++lev)
//							for (int row = 0; row != MapSize.Rows; ++row)
//							for (int col = 0; col != MapSize.Cols; ++col)
//							{
//								this[row, col, lev].Occulted = false;
//							}
//							throw;
//						}
//					}
//					else
//						CalculateOccultations();
//				}
			}
			else
			{
				string error = String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"The file does not exist{0}{0}{1}",
										Environment.NewLine,
										FullPath);
				MessageBox.Show(
							error,
							"Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error,
							MessageBoxDefaultButton.Button1,
							0);
			}
		}
		#endregion


		#region Methods
		private void ReadMapFile(List<TilepartBase> parts)
		{
			using (var bs = new BufferedStream(File.OpenRead(FullPath)))
			{
				int rows = bs.ReadByte();
				int cols = bs.ReadByte();
				int levs = bs.ReadByte();

				MapTiles = new MapTileList(rows, cols, levs);
				MapSize  = new MapSize(rows, cols, levs);

				for (int lev = 0; lev != levs; ++lev)
				for (int row = 0; row != rows; ++row)
				for (int col = 0; col != cols; ++col)
				{
					int q1 = bs.ReadByte();
					int q2 = bs.ReadByte();
					int q3 = bs.ReadByte();
					int q4 = bs.ReadByte();

					this[row, col, lev] = CreateTile(parts, q1, q2, q3, q4);
				}

//				if (bs.Position < bs.Length)
//					RouteFile.ExtraHeight = (byte)bs.ReadByte(); // <- NON-STANDARD <-| See also Save() below_
			}
		}

		private XCMapTile CreateTile(
				IList<TilepartBase> parts,
				int q1,
				int q2,
				int q3,
				int q4)
		{
			try
			{
				var a = (q1 > 1) ? (Tilepart)parts[q1 - 2]
								 : null;
	
				var b = (q2 > 1) ? (Tilepart)parts[q2 - 2]
								 : null;
	
				var c = (q3 > 1) ? (Tilepart)parts[q3 - 2]
								 : null;
	
				var d = (q4 > 1) ? (Tilepart)parts[q4 - 2]
								 : null;
	
				return new XCMapTile(a, b, c, d);
			}
			catch (Exception ex)
			{
				// TODO: Settle a graceful way to handle exceptions throughout.
				// and/or use AdZerg()

//				XConsole.AdZerg("MapFileChild.CreateTile() Invalid value(s) in .MAP file: " + Descriptor.Label);
				System.Windows.Forms.MessageBox.Show(
												"MapFileChild.CreateTile()" + Environment.NewLine
													+ "Invalid value(s) in .MAP file: " + Descriptor.Label + Environment.NewLine
													+ "indices: " + q1 + "," + q2 + "," + q3 + "," + q4 + Environment.NewLine
													+ "length: " + parts.Count + Environment.NewLine
													+ ex + ":" + ex.Message,
												"Error",
												System.Windows.Forms.MessageBoxButtons.OK,
												System.Windows.Forms.MessageBoxIcon.Warning,
												System.Windows.Forms.MessageBoxDefaultButton.Button1,
												0);
//				return XCMapTile.BlankTile;
				throw;
			}
		}

		private void SetupRouteNodes(RouteNodeCollection routes)
		{
//			if (file.ExtraHeight != 0) // remove ExtraHeight for editing - see Save() below_
//				foreach (RouteNode node in routes)
//					node.Lev -= file.ExtraHeight;

			foreach (RouteNode node in routes)
			{
				var tile = this[node.Row, node.Col, node.Lev];
				if (tile != null)
					((XCMapTile)tile).Node = node;
			}
		}

		public void CalculateOccultations()
		{
			if (MapSize.Levs > 1) // NOTE: Maps shall be at least 10x10x1 ...
			{
				MapTileBase tile = null;

				for (int lev = MapSize.Levs - 1; lev != 0; --lev)
				for (int row = 0; row != MapSize.Rows - 2; ++row)
				for (int col = 0; col != MapSize.Cols - 2; ++col)
				{
					if ((tile = this[row, col, lev]) != null) // safety. The tile should always be valid.
					{
						if (   ((XCMapTile)this[row,     col,     lev - 1]).Ground != null // above

							&& ((XCMapTile)this[row + 1, col,     lev - 1]).Ground != null // south
							&& ((XCMapTile)this[row + 2, col,     lev - 1]).Ground != null

							&& ((XCMapTile)this[row,     col + 1, lev - 1]).Ground != null // east
							&& ((XCMapTile)this[row,     col + 2, lev - 1]).Ground != null

							&& ((XCMapTile)this[row + 1, col + 1, lev - 1]).Ground != null // southeast
							&& ((XCMapTile)this[row + 1, col + 2, lev - 1]).Ground != null
							&& ((XCMapTile)this[row + 2, col + 1, lev - 1]).Ground != null
							&& ((XCMapTile)this[row + 2, col + 2, lev - 1]).Ground != null)
						{
							tile.Occulted = true;
						}
						else
							tile.Occulted = false;
					}
				}
//				OccultFile.SaveOccult(_file, _pathOccult, this);
			}
			//else // TODO: inform user that the current map has only 1 level
			// and no .OTD will be created if the command is initiated from the
			// filemenu.
		}

		/// <summary>
		/// Gets the terrain-type given a tile-part.
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public string GetTerrainLabel(TilepartBase part)
		{
			int id = -1;

			foreach (var tile1 in Parts)
			{
				if (tile1.Id == 0)
					++id;

				if (tile1 == part)
					break;
			}

			if (id != -1 && id < Terrains.Count)
				return Terrains[id];

			return null;
		}

		/// <summary>
		/// Adds a route-node to the map-tile at a given location.
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public RouteNode AddRouteNode(MapLocation location)
		{
			MapChanged = true;

			var node = Routes.AddNode(
									(byte)location.Row,
									(byte)location.Col,
									(byte)location.Lev);

			return (((XCMapTile)this[node.Row, node.Col, node.Lev]).Node = node);
		}

		/// <summary>
		/// Writes a blank Map to the stream provided.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="rows"></param>
		/// <param name="cols"></param>
		/// <param name="levs"></param>
		public static void CreateMap(
				Stream str,
				byte rows,
				byte cols,
				byte levs)
		{
			using (var bw = new BinaryWriter(str))
			{
				bw.Write(rows);
				bw.Write(cols);
				bw.Write(levs);

				for (int lev = 0; lev != levs; ++lev)
				for (int row = 0; row != rows; ++row)
				for (int col = 0; col != cols; ++col)
				{
					bw.Write((int)0);
				}
			}
		}

		/// <summary>
		/// Saves the .MAP file and the .RMP file.
		/// </summary>
		public override void Save()
		{
			MapChanged = false;

			using (var fs = File.Create(FullPath))
			{
//				if (RouteFile.ExtraHeight != 0) // add ExtraHeight to save - see SetupRoutes() above^
//					foreach (RouteNode node in RouteFile)
//						node.Lev += RouteFile.ExtraHeight;

				Routes.Save(); // <- saves the .RMP file

				fs.WriteByte((byte)MapSize.Rows); // http://www.ufopaedia.org/index.php/MAPS
				fs.WriteByte((byte)MapSize.Cols); // - says this header is "height, width and depth (in that order)"
				fs.WriteByte((byte)MapSize.Levs);

				for (int lev = 0; lev != MapSize.Levs; ++lev)
				for (int row = 0; row != MapSize.Rows; ++row)
				for (int col = 0; col != MapSize.Cols; ++col)
				{
					var tile = this[row, col, lev] as XCMapTile;

					if (tile.Ground == null)
						fs.WriteByte(0);
					else
						fs.WriteByte((byte)(tile.Ground.TilesetId + 2)); // why "+2" -> reserved for the 2 Blank tiles.

					if (tile.West == null)
						fs.WriteByte(0);
					else
						fs.WriteByte((byte)(tile.West.TilesetId + 2));

					if (tile.North == null)
						fs.WriteByte(0);
					else
						fs.WriteByte((byte)(tile.North.TilesetId + 2));

					if (tile.Content == null)
						fs.WriteByte(0);
					else
						fs.WriteByte((byte)(tile.Content.TilesetId + 2));
				}

//				fs.WriteByte(RouteFile.ExtraHeight); // <- NON-STANDARD <-| See also ReadMapFile() above^
			}
		}

		/// <summary>
		/// Resizes the current Map.
		/// </summary>
		/// <param name="rows">total rows in the new Map</param>
		/// <param name="cols">total columns in the new Map</param>
		/// <param name="levs">total levels in the new Map</param>
		/// <param name="ceiling">true to add extra levels above the top level,
		/// false to add extra levels below the ground level - but only if a
		/// height difference is found for either case</param>
		public override void MapResize(
				int rows,
				int cols,
				int levs,
				bool ceiling)
		{
			var tileList = MapResizeService.ResizeMapDimensions(
															rows, cols, levs,
															MapSize,
															MapTiles,
															ceiling);
			if (tileList != null)
			{
				MapChanged = true;

				if (levs != MapSize.Levs && ceiling) // adjust route-nodes ->
				{
					int delta = levs - MapSize.Levs;	// NOTE: map levels are reversed
					foreach (RouteNode node in Routes)	// so adding levels to the ceiling needs to push the existing nodes down.
						node.Lev += delta;
				}

				if (   cols < MapSize.Cols // check for and ask if user wants to delete any route-nodes outside the new bounds
					|| rows < MapSize.Rows
					|| levs < MapSize.Levs)
				{
					Routes.CheckNodeBounds(cols, rows, levs);
				}

				MapTiles = tileList;
				MapSize  = new MapSize(rows, cols, levs);

				Level = 0; //MapSize.Levs - 1; NOTE: this might need to fire a LevelChanged event ...
			}
		}
		#endregion
	}
}

//		public void HQ2X()
//		{
//			foreach (string dep in _deps) // instead i would want to make an image of the whole map and run that through hq2x
//				foreach (var image in GameInfo.GetPckPack(dep))
//					image.HQ2X();
//
//			PckImage.Width  *= 2;
//			PckImage.Height *= 2;
//		}
