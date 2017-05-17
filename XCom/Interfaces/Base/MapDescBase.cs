using System;


namespace XCom.Interfaces.Base
{
	public class MapDescBase
	{
		#region Properties
		public string Label
		{ get; internal protected set; }
		#endregion


		#region cTor
		/// <summary>
		/// cTor. Instantiated only as the parent of MapDescChild.
		/// </summary>
		/// <param name="tileset"></param>
		internal protected MapDescBase(string tileset)
		{
			Label = tileset;
		}
		#endregion


		#region Methods
		public override string ToString() // isUsed yes/no
		{
			return Label;
		}
		#endregion
	}
}
