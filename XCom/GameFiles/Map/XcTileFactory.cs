using System;
using System.IO;
using XCom.Interfaces;


namespace XCom.GameFiles.Map
{
	public class XcTileFactory
		:
		IWarningNotifier
	{
		public XCTile[] CreateTiles(
				string baseName,
				string directory,
				PckFile pckFile)
		{
			int diff = (baseName == "XBASES05") ? 3 : 0; // TODO: wtf.

			const int TOTAL = 62;

			using (var file = new BufferedStream(File.OpenRead(directory + baseName + ".MCD")))
			{
				var tiles = new XCTile[(((int) file.Length) / TOTAL) - diff];

				for (int i = 0; i != tiles.Length; ++i)
				{
					var info = new byte[TOTAL];
					file.Read(info, 0, TOTAL);
					var mcdEntry = McdEntryFactory.Create(info);

					var dead = GetDeadValue(baseName, i, mcdEntry, tiles);
					var alternate = GetAlternate(baseName, i, mcdEntry, tiles);
					var tile = new XCTile(i, pckFile, mcdEntry, tiles);
					tile.Dead = dead;
					tile.Alternate = alternate;
					tiles[i] = tile;
				}

//				file.Close(); // NOTE: the 'using' block closes the stream.

				return tiles;
			}
		}

		private XCTile GetAlternate(string baseName, int index, McdEntry info, XCTile[] tiles)
		{
			if (info.UfoDoor || info.HumanDoor || info.Alt_MCD != 0)
			{
				if (tiles.Length < info.Alt_MCD)
				{
					OnHandleWarning(string.Format(
						"In the MCD file {3}, the tile entry {0} have an invalid alternative tile (# {1} of {2} tiles)",
						index, info.Alt_MCD, tiles.Length, baseName));
					return null;
				}
				return tiles[info.Alt_MCD];
			}
			return null;
		}

		private XCTile GetDeadValue(string baseName, int index, McdEntry info, XCTile[] tiles)
		{
			try
			{
				if (info.DieTile != 0)
				{
					return tiles[(info).DieTile];
				}
			}
			catch
			{
				OnHandleWarning(string.Format(
					"In the MCD file {3}, the tile entry {0} have an invalid dead tile (# {1} of {2} tiles)",
					index, info.Alt_MCD, tiles.Length, baseName));
			}
			return null;
		}

		public event Action<string> HandleWarning;

		protected virtual void OnHandleWarning(string message)
		{
			Action<string> handler = HandleWarning;
			if (handler != null)
			{
				handler(message);
			}
			else
			{
				throw new ApplicationException(message);
			}
		}
	}
}
