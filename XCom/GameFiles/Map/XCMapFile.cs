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

			for (int i = 0; i < tiles.Count; i++)
				tiles[i].MapId = i;

			ReadMap(File.OpenRead(filePath), tiles);

			SetupRoutes(routeFile);

			if (File.Exists(blankPath + baseName + BlankFile.BlankExt))
			{
				try
				{
					BlankFile.LoadBlanks(baseName, blankPath, this);
				}
				catch
				{
					for (int h = 0; h < MapSize.Height; h++)
						for (int r = 0; r < MapSize.Rows; r++)
							for (int c = 0; c < MapSize.Cols; c++)
								this[r, c, h].DrawAbove = true;
					throw;
				}
			}
			else if (!string.IsNullOrEmpty(blankPath))
			{
				CalcDrawAbove();
				SaveBlanks();
			}
		}


		public string BaseName { get; private set; }
		public string BasePath { get; private set; }

		public void Hq2x()
		{
			// instead i would want to make an image of the whole map and run that through hq2x
			foreach (string s in _deps)
				foreach (PckImage pi in GameInfo.GetPckFile(s))
					pi.Hq2x();

			PckImage.Width  *= 2;
			PckImage.Height *= 2;
		}

		public RouteNode AddRmp(MapLocation loc)
		{
			RouteNode re = RouteFile.AddEntry(
									(byte)loc.Row,
									(byte)loc.Col,
									(byte)loc.Height);
			((XCMapTile)this[re.Row, re.Col, re.Height]).Node = re;
			return re;
		}

		public string[] Dependencies
		{
			get { return _deps; }
		}

		public void SaveBlanks()
		{
			BlankFile.SaveBlanks(BaseName, _blankPath, this);
		}

		public void CalcDrawAbove()
		{
			for (int h = MapSize.Height - 1; h >= 0; h--)
				for (int row = 0; row < MapSize.Rows - 2; row++)
					for (int col = 0; col < MapSize.Cols -2; col++)
						if (this[row, col, h] != null && h - 1 > -1)
						{
							var mapTileHl1 = (XCMapTile)this[row, col, h - 1];

							if (   mapTileHl1 != null
								&& mapTileHl1.Ground != null									// top
								&& ((XCMapTile)this[row + 1, col,     h - 1]).Ground != null	// south
								&& ((XCMapTile)this[row + 2, col,     h - 1]).Ground != null
								&& ((XCMapTile)this[row + 1, col + 1, h - 1]).Ground != null	// southeast
								&& ((XCMapTile)this[row + 2, col + 1, h - 1]).Ground != null
								&& ((XCMapTile)this[row + 2, col + 2, h - 1]).Ground != null
								&& ((XCMapTile)this[row,     col + 1, h - 1]).Ground != null	// east
								&& ((XCMapTile)this[row,     col + 2, h - 1]).Ground != null
								&& ((XCMapTile)this[row + 1, col + 2, h - 1]).Ground != null)
							{
								this[row, col, h].DrawAbove = false;
							}
						}
		}

		/// <summary>
		/// Writes a blank Map to the stream provided.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="rows"></param>
		/// <param name="cols"></param>
		/// <param name="height"></param>
		public static void NewMap(
				Stream str,
				byte rows,
				byte cols,
				byte height)
		{
			var bw = new BinaryWriter(str);
			bw.Write(rows);
			bw.Write(cols);
			bw.Write(height);

			for (int h = 0; h < height; h++)
				for (int r = 0; r < rows; r++)
					for (int c = 0; c < cols; c++)
						bw.Write((int)0);

			bw.Flush();
			bw.Close();
		}

		public override void Save()
		{
			using (var fs = File.Create(BasePath + BaseName + MapExt))
			{
				if (RouteFile.ExtraHeight != 0) // add ExtraHeight to save - see SetupRoutes() below_
					foreach (RouteNode route in RouteFile)
						route.Height += RouteFile.ExtraHeight;

				RouteFile.Save();
				fs.WriteByte((byte)MapSize.Rows);
				fs.WriteByte((byte)MapSize.Cols);
				fs.WriteByte((byte)MapSize.Height);

				for (int h = 0; h < MapSize.Height; h++)
					for (int r = 0; r < MapSize.Rows; r++)
						for (int c = 0; c < MapSize.Cols; c++)
						{
							var tile = (XCMapTile)this[r, c, h];

							if (tile.Ground == null)
								fs.WriteByte(0);
							else
								fs.WriteByte((byte)(tile.Ground.MapId + 2));

							if (tile.West == null)
								fs.WriteByte(0);
							else
								fs.WriteByte((byte)(tile.West.MapId + 2));

							if (tile.North == null)
								fs.WriteByte(0);
							else
								fs.WriteByte((byte)(tile.North.MapId + 2));

							if (tile.Content == null)
								fs.WriteByte(0);
							else
								fs.WriteByte((byte)(tile.Content.MapId + 2));
						}

				fs.WriteByte(RouteFile.ExtraHeight);
//				fs.Close(); // NOTE: the 'using' block flushes & closes the stream.
			}
			MapChanged = false;
		}

		private void ReadMap(Stream str, List<TileBase> tiles)
		{
			var input = new BufferedStream(str);
			var rows   = input.ReadByte();
			var cols   = input.ReadByte();
			var height = input.ReadByte();

			MapSize = new MapSize(rows, cols, height);
			MapData = new MapTileList(rows, cols, height);

			for (int h = 0; h < height; h++)
				for (int r = 0; r < rows; r++)
					for (int c = 0; c < cols; c++)
					{
						int q1 = input.ReadByte();
						int q2 = input.ReadByte();
						int q3 = input.ReadByte();
						int q4 = input.ReadByte();

						this[r, c, h] = createTile(tiles, q1, q2, q3, q4);
					}

			if (input.Position < input.Length)
				RouteFile.ExtraHeight = (byte) input.ReadByte();

			input.Close();
		}

		public RouteFile RouteFile
		{
			get { return _routeFile; }
		}

		public override void ResizeTo(
				int newR,
				int newC,
				int newH,
				bool wrtCeiling)
		{
			var newMap = MapResizeService.ResizeMap(
												newR,
												newC,
												newH,
												MapSize,
												MapData,
												wrtCeiling);
			if (newMap != null)
			{
				if (wrtCeiling && newH != MapSize.Height) // update Routes
				{
					var d = newH - MapSize.Height;
					foreach (RouteNode rmp in RouteFile)
					{
						if (newH < MapSize.Height)
							rmp.Height = (byte)(rmp.Height + d);
						else
							rmp.Height += (byte)d;
					}
				}

				if (   newC < MapSize.Cols // delete route-nodes outside the new bounds
					|| newR < MapSize.Rows
					|| newH < MapSize.Height)
				{
					RouteFile.CheckRouteEntries(newC, newR, newH);
				}

				MapData = newMap;
				MapSize = new MapSize(newR, newC, newH);
				CurrentHeight = (byte)(MapSize.Height - 1);
				MapChanged = true;
			}
		}

		private XCMapTile createTile(
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

		public string GetDependencyName(TileBase tile)
		{
			int id = -1;

			foreach (var t in Tiles)
			{
				if (t.Id == 0)
					++id;

				if (t == tile)
					break;
			}

			if (id != -1 && id < _deps.Length)
				return _deps[id];

			return null;
		}

		private void SetupRoutes(RouteFile file)
		{
			if (file.ExtraHeight != 0) // remove ExtraHeight for editing - see Save() above^
				foreach (RouteNode node in file)
					node.Height -= file.ExtraHeight;

			foreach (RouteNode node in file)
			{
				var baseTile = this[node.Row, node.Col, node.Height];
				if (baseTile != null)
					((XCMapTile)baseTile).Node = node;
			}
		}
	}
}
