using System;
using System.IO;

using XCom.Interfaces;


namespace XCom.GameFiles.Map
{
	public class XCTileFactory
		:
			IWarningNotifier
	{
		internal XCTile[] CreateTiles(
				string baseName,
				string directory,
				PckSpriteCollection pckPack)
		{
			int diff = (baseName == "XBASES05") ? 3 : 0; // TODO: wtf.

			const int Total = 62;

			using (var bs = new BufferedStream(File.OpenRead(directory + baseName + ".MCD")))
			{
				var tiles = new XCTile[(((int) bs.Length) / Total) - diff];

				for (int i = 0; i != tiles.Length; ++i)
				{
					var info = new byte[Total];
					bs.Read(info, 0, Total);
					var record = McdEntryFactory.Create(info);

					var tile = new XCTile(i, pckPack, record, tiles);
					tile.Dead = GetDeadValue(baseName, i, record, tiles);
					tile.Alternate = GetAlternate(baseName, i, record, tiles);

					tiles[i] = tile;
				}

				return tiles;
			}
		}

		private XCTile GetAlternate(
				string baseName,
				int index,
				McdEntry info,
				XCTile[] tiles)
		{
			if (info.UfoDoor || info.HumanDoor || info.Alt_MCD != 0)
			{
				if (tiles.Length < info.Alt_MCD)
				{
					OnHandleWarning(string.Format(
							"In the MCD file {3}, the tile entry {0} has an invalid alternative tile (# {1} of {2} tiles).",
							index,
							info.Alt_MCD,
							tiles.Length,
							baseName));
				}
				else
					return tiles[info.Alt_MCD];
			}
			return null;
		}

		private XCTile GetDeadValue(
				string baseName,
				int index,
				McdEntry info,
				XCTile[] tiles)
		{
			try
			{
				if (info.DieTile != 0)
					return tiles[(info).DieTile];
			}
			catch
			{
				OnHandleWarning(string.Format(
						"In the MCD file {3}, the tile entry {0} has an invalid dead tile (# {1} of {2} tiles).",
						index,
						info.Alt_MCD,
						tiles.Length,
						baseName));
			}
			return null;
		}

		public event Action<string> HandleWarning;

		private void OnHandleWarning(string message)
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
