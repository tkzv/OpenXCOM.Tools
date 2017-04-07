using System;


namespace XCom.Interfaces.Base
{
	public interface IMcdInfo
	{
		int Id
		{ get; }

		sbyte TileOffset
		{ get; }

		TileType TileType
		{ get; }

		SpecialType TargetType
		{ get; }

		bool HumanDoor
		{ get; }

		bool UfoDoor
		{ get; }
	}
}