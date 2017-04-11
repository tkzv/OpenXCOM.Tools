using System;
using System.IO;


namespace XCom.GameFiles.Map
{
	public sealed class XCTileFactory
	{
		internal XCTile[] CreateTiles(
				string file,
				string dir,
				PckSpriteCollection pckPack)
		{
//			int diff = (file == "XBASES05") ? 3 : 0; // TODO: wtf.

			const int Total = 62; // there are 62 entries in each MCD file.

			using (var bs = new BufferedStream(File.OpenRead(dir + file + ".MCD")))
			{
//				var tiles = new XCTile[(((int)bs.Length) / Total) - diff];
				var tiles = new XCTile[(int)bs.Length / Total]; // TODO: Error if this don't work out right.

				for (int id = 0; id != tiles.Length; ++id)
				{
					var bindata = new byte[Total];
					bs.Read(bindata, 0, Total);
					var record = McdRecordFactory.CreateRecord(bindata);

					var tile = new XCTile(id, pckPack, record); //, tiles); // NOTE: Tiles is not used.

					tiles[id] = tile;
				}

				for (int id = 0; id != tiles.Length; ++id)
				{
					tiles[id].Dead = GetDeadTile(file, id, tiles[id].Record, tiles);
					tiles[id].Alternate = GetAlternateTile(file, id, tiles[id].Record, tiles); // TODO: check if the Alternate gets counted by MapInfoForm
				}

				return tiles;
			}
		}

		private XCTile GetDeadTile(
				string file,
				int id,
				McdRecord record,
				XCTile[] tiles)
		{
			if (record.DieTile != 0)
			{
				if (record.DieTile < tiles.Length)
					return tiles[record.DieTile];

				HandleWarning(String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"In the MCD file {0}, the tile entry {1} has an invalid alternate tile (id {2} of {3} records).",
										file,
										id,
										record.Alt_MCD,
										tiles.Length));
			}
			return null;
		}

		private XCTile GetAlternateTile(
				string file,
				int id,
				McdRecord record,
				XCTile[] tiles)
		{
			if (record.Alt_MCD != 0) // || record.HumanDoor || record.UfoDoor
			{
				if (record.Alt_MCD < tiles.Length)
					return tiles[record.Alt_MCD];

				HandleWarning(String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"In the MCD file {0}, the tile entry {1} has an invalid alternate tile (id {2} of {3} records).",
										file,
										id,
										record.Alt_MCD,
										tiles.Length));
			}
			return null;
		}


		public event Action<string> WarningEvent;

		private void HandleWarning(string warning)
		{
			Action<string> handler = WarningEvent;
			if (handler != null)
			{
				handler(warning);
			}
			else
			{
				throw new ApplicationException(warning);
			}
		}
	}
}
