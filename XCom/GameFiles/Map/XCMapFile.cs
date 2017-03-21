using System;
using System.Collections.Generic;
using System.IO;

using XCom.Interfaces.Base;
using XCom.Services;


namespace XCom
{
	public class XCMapFile
		:
		IMap_Base
	{
		private readonly string _blankPath;
		private readonly string[] _deps;

		public static readonly string MapExt = ".MAP";

		private RouteFile _routeFile;


		public XCMapFile(
				string baseName,
				string basePath,
				string blankPath,
				List<TileBase> tiles,
				string[] depList,
				RouteFile routeFile)
			:
			base(baseName, tiles)
		{
			BaseName = baseName;
			BasePath = basePath;

			_blankPath = blankPath;

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
			using (var input = new BufferedStream(str))
			{
				var rows   = input.ReadByte();
				var cols   = input.ReadByte();
				var height = input.ReadByte();
	
				MapSize = new MapSize(rows, cols, height);
				MapData = new MapTileList(rows, cols, height);
	
				for (int h = 0; h != height; ++h)
					for (int r = 0; r != rows; ++r)
						for (int c = 0; c != cols; ++c)
						{
							int q1 = input.ReadByte();
							int q2 = input.ReadByte();
							int q3 = input.ReadByte();
							int q4 = input.ReadByte();
	
							this[r, c, h] = CreateTile(tiles, q1, q2, q3, q4);
						}
	
				if (input.Position < input.Length)
					RouteFile.ExtraHeight = (byte)input.ReadByte(); // <- NON-STANDARD <-| See also Save() below_
	
//				input.Close(); // NOTE: the 'using' block closes the stream.
			}
		}

		private void SetupRoutes(RouteFile file)
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

		public string BaseName
		{ get; private set; }

		public string BasePath
		{ get; private set; }

		public string[] Dependencies
		{
			get { return _deps; }
		}

		public string GetDependencyName(TileBase tile)
		{
			int id = -1;

			foreach (var tile0 in Tiles)
			{
				if (tile0.Id == 0)
					++id;

				if (tile0 == tile)
					break;
			}

			if (id != -1 && id < _deps.Length)
				return _deps[id];

			return null;
		}

		public RouteFile RouteFile
		{
			get { return _routeFile; }
		}

		public RouteNode AddRouteNode(MapLocation loc)
		{
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
	
//				bw.Flush();
//				bw.Close(); // NOTE: the 'using' block flushes & closes the stream.
			}
		}

		/// <summary>
		/// Saves the .MAP file and the .RMP file.
		/// </summary>
		public override void Save()
		{
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

				fs.WriteByte(RouteFile.ExtraHeight); // <- NON-STANDARD <-| See also ReadMapFile() above^

//				fs.Close(); // NOTE: the 'using' block closes the stream.
			}
			MapChanged = false;
		}

		public override void ResizeTo(
				int newR,
				int newC,
				int newH,
				bool wrtCeiling)
		{
			var tileList = MapResizeService.ResizeMap(
													newR,
													newC,
													newH,
													MapSize,
													MapData,
													wrtCeiling);
			if (tileList != null)
			{
				if (wrtCeiling && newH != MapSize.Height) // update Routes
				{
					var d = newH - MapSize.Height;
					foreach (RouteNode node in RouteFile)
					{
						if (newH < MapSize.Height)
							node.Height = (byte)(node.Height + d);
						else
							node.Height += (byte)d;
					}
				}

				if (   newC < MapSize.Cols // delete route-nodes outside the new bounds
					|| newR < MapSize.Rows
					|| newH < MapSize.Height)
				{
					RouteFile.CheckNodeBounds(newC, newR, newH);
				}

				MapData = tileList;
				MapSize = new MapSize(newR, newC, newH);
				CurrentHeight = (byte)(MapSize.Height - 1);
				MapChanged = true;
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
				XCTile a, b, c, d;
				a =
				b =
				c =
				d = null;

				if (q1 != 0 && q1 != 1)
					a = (XCTile)tiles[q1 - 2];

				if (q2 != 0 && q2 != 1)
					b = (XCTile)tiles[q2 - 2];

				if (q3 != 0 && q3 != 1)
					c = (XCTile)tiles[q3 - 2];

				if (q4 != 0 && q4 != 1)
					d = (XCTile)tiles[q4 - 2];

				return new XCMapTile(a, b, c, d);
			}
			catch
			{
				//Console.WriteLine("Error in Map::createTile, indexes: {0},{1},{2},{3} length: {4}",q1,q2,q3,q4,tiles.Length);
				return XCMapTile.BlankTile;
			}
		}

		public void Hq2x()
		{
			foreach (string st in _deps) // instead i would want to make an image of the whole map and run that through hq2x
				foreach (var image in GameInfo.GetPckFile(st))
					image.Hq2x();

			PckImage.Width  *= 2;
			PckImage.Height *= 2;
		}
	}
}
