using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces.Base;
using XCom.Services;


namespace XCom
{
	public sealed class XCMapFile
		:
			IMapBase
	{
//		private readonly string _blankPath;
		private readonly string[] _deps;

		public static readonly string MapExt = ".MAP";

		private RouteNodeCollection _routeFile;


		internal XCMapFile(
				string baseName,
				string basePath,
				string blankPath,
				List<TileBase> tiles,
				string[] depList,
				RouteNodeCollection routeFile)
			:
				base(baseName, tiles)
		{
			BaseName = baseName;
			BasePath = basePath;

//			_blankPath = blankPath;

			_deps = depList;

			_routeFile = routeFile;

			var filePath = basePath + baseName + MapExt;
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException(filePath);
			}

			for (int i = 0; i != tiles.Count; ++i)
				tiles[i].TileListId = i;

			ReadMapFile(File.OpenRead(filePath), tiles);

			SetupRoutes(routeFile);

			if (!string.IsNullOrEmpty(blankPath)) // TODO: investigate saving/loading of the Blanks.
			{
				if (File.Exists(blankPath + baseName + BlankFile.BlankExt))
				{
					try
					{
						BlankFile.LoadBlank(baseName, blankPath, this);
					}
					catch
					{
						for (int h = 0; h != MapSize.Height; ++h)
							for (int r = 0; r != MapSize.Rows; ++r)
								for (int c = 0; c != MapSize.Cols; ++c)
									this[r, c, h].DrawAbove = true;
						throw;
					}
				}
				else
				{
					CalculateDrawAbove();
					BlankFile.SaveBlank(BaseName, blankPath, this);
				}
			}
			// TODO: throw something here or at least inform the user.
		}


		private void ReadMapFile(Stream str, List<TileBase> tiles)
		{
			using (var bs = new BufferedStream(str))
			{
				int rows   = bs.ReadByte();
				int cols   = bs.ReadByte();
				int height = bs.ReadByte();
	
				MapTiles = new MapTileList(rows, cols, height);
				MapSize  = new MapSize(rows, cols, height);

				for (int h = 0; h != height; ++h)
					for (int r = 0; r != rows; ++r)
						for (int c = 0; c != cols; ++c)
						{
							int q1 = bs.ReadByte();
							int q2 = bs.ReadByte();
							int q3 = bs.ReadByte();
							int q4 = bs.ReadByte();

							this[r, c, h] = CreateTile(tiles, q1, q2, q3, q4);
						}

//				if (bs.Position < bs.Length)
//					RouteFile.ExtraHeight = (byte)bs.ReadByte(); // <- NON-STANDARD <-| See also Save() below_
			}
		}

		private void SetupRoutes(RouteNodeCollection file)
		{
			if (file.ExtraHeight != 0) // remove ExtraHeight for editing - see Save() below_
				foreach (RouteNode node in file)
					node.Height -= file.ExtraHeight;

			foreach (RouteNode node in file)
			{
				var baseTile = this[node.Row, node.Col, node.Height];
				if (baseTile != null)
					((XCMapTile)baseTile).Node = node;
			}
		}

		private void CalculateDrawAbove()
		{
			for (int h = MapSize.Height - 1; h > -1; --h)
				for (int r = 0; r < MapSize.Rows - 2; ++r)
					for (int c = 0; c < MapSize.Cols - 2; ++c)
						if (this[r, c, h] != null && h - 1 > -1)		// TODO: should probably be "h-1 > 0"
						{
							var tile = (XCMapTile)this[r, c, h - 1];	// TODO: ... because "h-1"

							if (   tile != null							// TODO: doesn't happen anyway.
								&& tile.Ground != null										// top
								&& ((XCMapTile)this[r + 1, c,     h - 1]).Ground != null	// south
								&& ((XCMapTile)this[r + 2, c,     h - 1]).Ground != null
								&& ((XCMapTile)this[r + 1, c + 1, h - 1]).Ground != null	// southeast
								&& ((XCMapTile)this[r + 2, c + 1, h - 1]).Ground != null
								&& ((XCMapTile)this[r + 2, c + 2, h - 1]).Ground != null
								&& ((XCMapTile)this[r,     c + 1, h - 1]).Ground != null	// east
								&& ((XCMapTile)this[r,     c + 2, h - 1]).Ground != null
								&& ((XCMapTile)this[r + 1, c + 2, h - 1]).Ground != null)
							{
								this[r, c, h].DrawAbove = false;
							}
						}
		}

		internal string BaseName
		{ get; private set; }

		internal string BasePath
		{ get; private set; }

		public string[] Dependencies
		{
			get { return _deps; }
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

		public RouteNodeCollection RouteFile
		{
			get { return _routeFile; }
		}

		public RouteNode AddRouteNode(MapLocation loc)
		{
			MapChanged = true;

			var node = RouteFile.Add(
								(byte)loc.Row,
								(byte)loc.Col,
								(byte)loc.Height);
			((XCMapTile)this[node.Row, node.Col, node.Height]).Node = node;

			return node;
		}

		/// <summary>
		/// Writes a blank Map to the stream provided.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="rows"></param>
		/// <param name="cols"></param>
		/// <param name="height"></param>
		public static void CreateMap(
				Stream str,
				byte rows,
				byte cols,
				byte height)
		{
			using (var bw = new BinaryWriter(str))
			{
				bw.Write(rows);
				bw.Write(cols);
				bw.Write(height);

				for (int h = 0; h != height; ++h)
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

			using (var fs = File.Create(BasePath + BaseName + MapExt))
			{
				if (RouteFile.ExtraHeight != 0) // add ExtraHeight to save - see SetupRoutes() above^
					foreach (RouteNode node in RouteFile)
						node.Height += RouteFile.ExtraHeight;

				RouteFile.Save(); // <- saves the .RMP file

				fs.WriteByte((byte)MapSize.Rows);	// http://www.ufopaedia.org/index.php/MAPS
				fs.WriteByte((byte)MapSize.Cols);	// - says this header is "height, width and depth (in that order)"
				fs.WriteByte((byte)MapSize.Height);

				for (int h = 0; h != MapSize.Height; ++h)
					for (int r = 0; r != MapSize.Rows; ++r)
						for (int c = 0; c != MapSize.Cols; ++c)
						{
							var tile = this[r, c, h] as XCMapTile;

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

		public override void ResizeTo(
				int rPost,
				int cPost,
				int hPost,
				bool toCeiling)
		{
			var tileList = MapResizeService.ResizeMap(
												rPost,
												cPost,
												hPost,
												MapSize,
												MapTiles,
												toCeiling);
			if (tileList != null)
			{
				MapChanged = true;

				if (toCeiling && hPost != MapSize.Height) // update Routes
				{
					int d = hPost - MapSize.Height;
					foreach (RouteNode node in RouteFile)
					{
//						if (hPost < MapSize.Height)
//							node.Height = node.Height + d;
//						else
						node.Height += d;
					}
				}

				if (   cPost < MapSize.Cols // delete route-nodes outside the new bounds
					|| rPost < MapSize.Rows
					|| hPost < MapSize.Height)
				{
					RouteFile.CheckNodeBounds(cPost, rPost, hPost);
				}

				MapTiles = tileList;
				MapSize  = new MapSize(rPost, cPost, hPost);

				CurrentHeight = (byte)(MapSize.Height - 1);
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
				XConsole.AdZerg("XCMapFile.CreateTile() Invalid value(s) in .MAP file: " + BaseName);
				System.Windows.Forms.MessageBox.Show(
												"XCMapFile.CreateTile()" + Environment.NewLine
													+ "Invalid value(s) in .MAP file: " + BaseName + Environment.NewLine
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

//		public void HQ2X()
//		{
//			foreach (string dep in _deps) // instead i would want to make an image of the whole map and run that through hq2x
//				foreach (var image in GameInfo.GetPckPack(dep))
//					image.HQ2X();
//
//			PckImage.Width  *= 2;
//			PckImage.Height *= 2;
//		}
	}
}
