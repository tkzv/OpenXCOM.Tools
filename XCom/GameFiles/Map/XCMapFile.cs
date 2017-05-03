using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces.Base;
using XCom.Services;


namespace XCom
{
	public sealed class XCMapFile
		:
			XCMapBase
	{
		#region Fields & Properties
		public static readonly string MapExt = ".MAP";

		private readonly string _file = String.Empty;
		private readonly string _path = String.Empty;

		private readonly string[] _deps;
		public string[] Dependencies
		{
			get { return _deps; }
		}

		private RouteNodeCollection _routeFile;
		public RouteNodeCollection RouteFile
		{
			get { return _routeFile; }
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="path"></param>
		/// <param name="blankPath"></param>
		/// <param name="tiles"></param>
		/// <param name="dependencies"></param>
		/// <param name="routeFile"></param>
		internal XCMapFile(
				string file,
				string path,
				string blankPath,
				List<TileBase> tiles,
				string[] dependencies,
				RouteNodeCollection routeFile)
			:
				base(file, tiles)
		{
			_file = file;
			_path = path;

			_deps = dependencies;

			_routeFile = routeFile;

			var pfe = path + file + MapExt;
			if (!File.Exists(pfe))
			{
				throw new FileNotFoundException(pfe);
			}

			for (int i = 0; i != tiles.Count; ++i)
				tiles[i].TileListId = i;

			ReadMapFile(File.OpenRead(pfe), tiles);

			SetupRoutes(routeFile);

			if (!String.IsNullOrEmpty(blankPath)) // TODO: investigate saving/loading of the Blanks.
			{
				if (File.Exists(blankPath + file + BlankFile.BlankExt))
				{
					try
					{
						BlankFile.LoadBlank(file, blankPath, this);
					}
					catch
					{
						for (int l = 0; l != MapSize.Levs; ++l)
							for (int r = 0; r != MapSize.Rows; ++r)
								for (int c = 0; c != MapSize.Cols; ++c)
									this[r, c, l].DrawAbove = true;
						throw;
					}
				}
				else
				{
					CalculateDrawAbove();
					BlankFile.SaveBlank(_file, blankPath, this);
				}
			}
			// TODO: throw something here or at least inform the user.
		}
		#endregion


		#region Methods
		private void ReadMapFile(Stream str, List<TileBase> tiles)
		{
			using (var bs = new BufferedStream(str))
			{
				int rows = bs.ReadByte();
				int cols = bs.ReadByte();
				int levs = bs.ReadByte();
	
				MapTiles = new MapTileList(rows, cols, levs);
				MapSize  = new MapSize(rows, cols, levs);

				for (int l = 0; l != levs; ++l)
					for (int r = 0; r != rows; ++r)
						for (int c = 0; c != cols; ++c)
						{
							int q1 = bs.ReadByte();
							int q2 = bs.ReadByte();
							int q3 = bs.ReadByte();
							int q4 = bs.ReadByte();

							this[r, c, l] = CreateTile(tiles, q1, q2, q3, q4);
						}

//				if (bs.Position < bs.Length)
//					RouteFile.ExtraHeight = (byte)bs.ReadByte(); // <- NON-STANDARD <-| See also Save() below_
			}
		}

		private void SetupRoutes(RouteNodeCollection file)
		{
//			if (file.ExtraHeight != 0) // remove ExtraHeight for editing - see Save() below_
//				foreach (RouteNode node in file)
//					node.Lev -= file.ExtraHeight;

			foreach (RouteNode node in file)
			{
				var baseTile = this[node.Row, node.Col, node.Lev];
				if (baseTile != null)
					((XCMapTile)baseTile).Node = node;
			}
		}

		private void CalculateDrawAbove()
		{
			for (int l = MapSize.Levs - 1; l > -1; --l)
				for (int r = 0; r < MapSize.Rows - 2; ++r)
					for (int c = 0; c < MapSize.Cols - 2; ++c)
						if (this[r, c, l] != null && l - 1 > -1)		// TODO: should probably be "l-1 > 0"
						{
							var tile = (XCMapTile)this[r, c, l - 1];	// TODO: ... because "l-1"

							if (   tile != null							// TODO: doesn't happen anyway.
								&& tile.Ground != null										// top
								&& ((XCMapTile)this[r + 1, c,     l - 1]).Ground != null	// south
								&& ((XCMapTile)this[r + 2, c,     l - 1]).Ground != null
								&& ((XCMapTile)this[r + 1, c + 1, l - 1]).Ground != null	// southeast
								&& ((XCMapTile)this[r + 2, c + 1, l - 1]).Ground != null
								&& ((XCMapTile)this[r + 2, c + 2, l - 1]).Ground != null
								&& ((XCMapTile)this[r,     c + 1, l - 1]).Ground != null	// east
								&& ((XCMapTile)this[r,     c + 2, l - 1]).Ground != null
								&& ((XCMapTile)this[r + 1, c + 2, l - 1]).Ground != null)
							{
								this[r, c, l].DrawAbove = false;
							}
						}
		}

		public string GetDepLabel(TileBase tile)
		{
			int id = -1;

			foreach (var tile1 in Tiles)
			{
				if (tile1.Id == 0)
					++id;

				if (tile1 == tile)
					break;
			}

			if (id != -1 && id < _deps.Length)
				return _deps[id];

			return null;
		}

		public RouteNode AddRouteNode(MapLocation location)
		{
			MapChanged = true;

			var node = RouteFile.Add(
								(byte)location.Row,
								(byte)location.Col,		// TODO:
								(byte)location.Lev);	// The screwy XCMapBase.LevelDown() will add an extra pip to 'Lev'
														// in its LevelChangedEventArgs, which will get passed into here
														// through QuadrantPanel(sic).OnLevelChanged_Observer(), so I'll
														// have to start tests by saving .RMP files to find out if that
														// is the correct way to do things.
														//
														// Note that XCMapBase.LevelUp() does not have this vagary ....

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

				for (int l = 0; l != levs; ++l)
					for (int r = 0; r != rows; ++r)
						for (int c = 0; c != cols; ++c)
							bw.Write((int)0);
			}
		}

		/// <summary>
		/// Saves the .MAP file and the .RMP file.
		/// </summary>
		public override void Save()
		{
			MapChanged = false;

			using (var fs = File.Create(_path + _file + MapExt))
			{
//				if (RouteFile.ExtraHeight != 0) // add ExtraHeight to save - see SetupRoutes() above^
//					foreach (RouteNode node in RouteFile)
//						node.Lev += RouteFile.ExtraHeight;

				RouteFile.Save(); // <- saves the .RMP file

				fs.WriteByte((byte)MapSize.Rows); // http://www.ufopaedia.org/index.php/MAPS
				fs.WriteByte((byte)MapSize.Cols); // - says this header is "height, width and depth (in that order)"
				fs.WriteByte((byte)MapSize.Levs);

				for (int l = 0; l != MapSize.Levs; ++l)
					for (int r = 0; r != MapSize.Rows; ++r)
						for (int c = 0; c != MapSize.Cols; ++c)
						{
							var tile = this[r, c, l] as XCMapTile;

							if (tile.Ground == null)
								fs.WriteByte(0);
							else
								fs.WriteByte((byte)(tile.Ground.TileListId + 2)); // why "+2" -> reserved for the 2 Blank tiles.

							if (tile.West == null)
								fs.WriteByte(0);
							else
								fs.WriteByte((byte)(tile.West.TileListId + 2));

							if (tile.North == null)
								fs.WriteByte(0);
							else
								fs.WriteByte((byte)(tile.North.TileListId + 2));

							if (tile.Content == null)
								fs.WriteByte(0);
							else
								fs.WriteByte((byte)(tile.Content.TileListId + 2));
						}

//				fs.WriteByte(RouteFile.ExtraHeight); // <- NON-STANDARD <-| See also ReadMapFile() above^
			}
		}

		public override void MapResize(
				int r,
				int c,
				int l,
				bool toCeiling)
		{
			var tileList = MapResizeService.ResizeMapDimensions(
															r, c, l,
															MapSize,
															MapTiles,
															toCeiling);
			if (tileList != null)
			{
				MapChanged = true;

				if (toCeiling && l != MapSize.Levs) // update Routes
				{
					int delta = l - MapSize.Levs;
					foreach (RouteNode node in RouteFile)
					{
//						if (hPost < MapSize.Height)
//							node.Height = node.Height + delta;
//						else
						node.Lev += delta;
					}
				}

				if (   c < MapSize.Cols // delete route-nodes outside the new bounds
					|| r < MapSize.Rows
					|| l < MapSize.Levs)
				{
					RouteFile.CheckNodeBounds(c, r, l);
				}

				MapTiles = tileList;
				MapSize  = new MapSize(r, c, l);

				Level = MapSize.Levs - 1;
			}
		}

		private XCMapTile CreateTile(
				IList<TileBase> tiles,
				int q1,
				int q2,
				int q3,
				int q4)
		{
			try
			{
				var a = (q1 > 1) ? (XCTile)tiles[q1 - 2]
								 : null;
	
				var b = (q2 > 1) ? (XCTile)tiles[q2 - 2]
								 : null;
	
				var c = (q3 > 1) ? (XCTile)tiles[q3 - 2]
								 : null;
	
				var d = (q4 > 1) ? (XCTile)tiles[q4 - 2]
								 : null;
	
				return new XCMapTile(a, b, c, d);
			}
			catch (Exception ex)
			{
				// and/or use AdZerg()
				// TODO: Settle a graceful way to handle exceptions throughout.
				// Unfortunately it would take a long time to force each one to
				// be raised for investigation.
				XConsole.AdZerg("XCMapFile.CreateTile() Invalid value(s) in .MAP file: " + _file);
				System.Windows.Forms.MessageBox.Show(
												"XCMapFile.CreateTile()" + Environment.NewLine
													+ "Invalid value(s) in .MAP file: " + _file + Environment.NewLine
													+ "indices: " + q1 + "," + q2 + "," + q3 + "," + q4 + Environment.NewLine
													+ "length: " + tiles.Count + Environment.NewLine
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
