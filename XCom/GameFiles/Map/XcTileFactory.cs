using System;
using System.IO;

using XCom.Interfaces;


namespace XCom.GameFiles.Map
{
	public class XCTileFactory
	{
		internal XCTile[] CreateTiles(
				string baseName,
				string dir,
				PckSpriteCollection pckPack)
		{
			int diff = (baseName == "XBASES05") ? 3 : 0; // TODO: wtf.

			const int Total = 62;

			using (var bs = new BufferedStream(File.OpenRead(dir + baseName + ".MCD")))
			{
				var tiles = new XCTile[(((int) bs.Length) / Total) - diff];

				for (int i = 0; i != tiles.Length; ++i)
				{
					var info = new byte[Total];
					bs.Read(info, 0, Total);
					var record = McdRecordFactory.Create(info);

					var tile = new XCTile(i, pckPack, record); //, tiles);			// NOTE: Tiles is not used.
					tile.Dead = GetDeadTile(baseName, i, record, tiles);
//					tile.Alternate = GetAlternateTile(baseName, i, record, tiles);	// NOTE: the Alternate tile is not used.
																					// TODO: possibly count it for MapInfoForm
					tiles[i] = tile;
				}

				return tiles;
			}
		}

/*		private XCTile GetAlternateTile(
				string baseName,
				int index,
				McdRecord record,
				XCTile[] tiles)
		{
			if (record.UfoDoor || record.HumanDoor || record.Alt_MCD != 0)
			{
				if (tiles.Length < record.Alt_MCD)
				{
					OnHandleWarning(String.Format(
											System.Globalization.CultureInfo.CurrentCulture,
											"In the MCD file {0}, the tile entry {1} has an invalid alternate tile (id {2} of {3} tiles).",
											baseName,
											index,
											record.Alt_MCD,
											tiles.Length));
				}
				else
					return tiles[record.Alt_MCD];
			}
			return null;
		} */

		private XCTile GetDeadTile(
				string baseName,
				int index,
				McdRecord record,
				XCTile[] tiles)
		{
			try
			{
				if (record.DieTile != 0)
					return tiles[record.DieTile];
			}
			catch
			{
				OnHandleWarning(String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"In the MCD file {3}, the tile entry {0} has an invalid dead tile (# {1} of {2} tiles).",
										index,
										record.Alt_MCD,
										tiles.Length,
										baseName));	// you've got to be kidding me ... yah you were, haha you got me yep.
			}										// I'll remove the rest of this shenanigan later.
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
