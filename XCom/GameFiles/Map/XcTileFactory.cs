using System;
using System.IO;


namespace XCom.GameFiles.Map
{
	public sealed class XCTileFactory
	{
		internal XCTilepart[] CreateTiles(
				string file,
				string dir,
				PckSpriteCollection pckPack)
		{
//			int diff = (file == "XBASES05") ? 3 : 0; // TODO: wtf.

			const int Total = 62; // there are 62 entries in each MCD file.

			using (var bs = new BufferedStream(File.OpenRead(dir + file + ".MCD")))
			{
//				var tiles = new XCTilepart[(((int)bs.Length) / Total) - diff];
				var parts = new XCTilepart[(int)bs.Length / Total]; // TODO: Error if this don't work out right.

				for (int id = 0; id != parts.Length; ++id)
				{
					var bindata = new byte[Total];
					bs.Read(bindata, 0, Total);
					var record = McdRecordFactory.CreateRecord(bindata);

					var part = new XCTilepart(id, pckPack, record); //, tiles); // NOTE: Tiles is not used.

					parts[id] = part;
				}

				for (int id = 0; id != parts.Length; ++id)
				{
					parts[id].Dead = GetDeadPart(file, id, parts[id].Record, parts);
					parts[id].Alternate = GetAlternatePart(file, id, parts[id].Record, parts); // TODO: check if the Alternate gets counted by MapInfoForm
				}

				return parts;
			}
		}

		private XCTilepart GetDeadPart(
				string file,
				int id,
				McdRecord record,
				XCTilepart[] parts)
		{
			if (record.DieTile != 0)
			{
				if (record.DieTile < parts.Length)
					return parts[record.DieTile];

				HandleWarning(String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"In the MCD file {0}, the tile entry {1} has an invalid alternate tile (id {2} of {3} records).",
										file,
										id,
										record.Alt_MCD,
										parts.Length));
			}
			return null;
		}

		private XCTilepart GetAlternatePart(
				string file,
				int id,
				McdRecord record,
				XCTilepart[] parts)
		{
			if (record.Alt_MCD != 0) // || record.HumanDoor || record.UfoDoor
			{
				if (record.Alt_MCD < parts.Length)
					return parts[record.Alt_MCD];

				HandleWarning(String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"In the MCD file {0}, the tile entry {1} has an invalid alternate tile (id {2} of {3} records).",
										file,
										id,
										record.Alt_MCD,
										parts.Length));
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
