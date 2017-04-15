using System.Collections.Generic;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TopViews
{
	internal enum ContentType
	{
		Content,
		EastWall,
		SouthWall,
		NorthWall,
		WestWall,
		NorthwestSoutheast,
		NortheastSouthwest,
		NorthWallWindow,
		WestWallWindow,
		Ground,
		NorthFence,
		WestFence,
		NorthwestCorner,
		NortheastCorner,
		SouthwestCorner,
		SoutheastCorner
	}

	/// <summary>
	/// Methods for determining how walls and objects are drawn for TopView and
	/// RouteView.
	/// </summary>
	internal static class ContentTypeService
	{
		internal static ContentType GetContentType(TileBase tile)
		{
			var record = tile.Record;
			if (record != null)
			{
				var loftList = record.GetLoftList();

				var allButGround = new List<byte>(loftList);
				allButGround.RemoveAt(0);

				if (AllLoftWith(allButGround, new[]{ 0 }))
					return ContentType.Ground;

				if (AllLoftWith(loftList, new[]{ 24, 26 }))
					return ContentType.EastWall;

				if (AllLoftWith(loftList, new[]{ 23, 25 }))
					return ContentType.SouthWall;

				if (AllLoftWith(loftList, new[]{ 8, 10, 12, 14, 38 })
					&& AnyLoftWith(loftList, new[]{ 38 }))
				{
					return ContentType.NorthWallWindow;
				}

				if (AllLoftWith(loftList, new[]{ 8, 10, 12, 14, 38, 0, 39, 77 })
					&& AnyLoftWith(loftList, new[]{ 0 }))
				{
					return ContentType.NorthFence;
				}

				if (AllLoftWith(loftList, new[]{ 8, 10, 12, 14, 16, 18, 20, 21 }))
					return ContentType.NorthWall;

				if (AllLoftWith(loftList, new[]{ 7, 9, 11, 13, 37 })
					&& AnyLoftWith(loftList, new[]{ 37 }))
				{
					return ContentType.WestWallWindow;
				}

				if (AllLoftWith(loftList, new[]{ 7, 9, 11, 13, 37,0, 39, 76 })
					&& AnyLoftWith(loftList, new[]{ 0 }))
				{
					return ContentType.WestFence;
				}

				if (AllLoftWith(loftList, new[]{ 7, 9, 11, 13, 15, 17, 19, 22 }))
					return ContentType.WestWall;

				if (AllLoftWith(loftList, new[]{ 35 }))
					return ContentType.NorthwestSoutheast;

				if (AllLoftWith(loftList, new[]{ 36 }))
					return ContentType.NortheastSouthwest;

				if (AllLoftWith(loftList, new[]{ 39, 40, 41, 103 }))
					return ContentType.NorthwestCorner;

				if (AllLoftWith(loftList, new[]{ 100 }))
					return ContentType.NortheastCorner;

				if (AllLoftWith(loftList, new[]{ 106 }))
					return ContentType.SouthwestCorner;

				if (AllLoftWith(loftList, new[]{ 109 }))
					return ContentType.SoutheastCorner;
			}
			return ContentType.Content;
		}

		private static bool AllLoftWith(IEnumerable<byte> loftList, int[] templates)
		{
			foreach (var loft in loftList)
			{
				var valid = false;
				foreach (var template in templates)
				{
					if (loft == template)
					{
						valid = true;
						break;
					}
				}

				if (!valid)
					return false;
			}
			return true;
		}

		private static bool AnyLoftWith(IEnumerable<byte> loftList, int[] templates)
		{
			foreach (var loft in loftList)
				foreach (var template in templates)
					if (loft == template)
						return true;

			return false;
		}

		internal static bool IsDoor(TileBase tile)
		{
			var record = tile.Record;
			if (record != null
				&& (record.HumanDoor || record.UfoDoor))
			{
				return true;
			}
			return false;
		}
	}
}
