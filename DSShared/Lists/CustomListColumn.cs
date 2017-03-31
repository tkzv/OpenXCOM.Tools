using System;
using System.Windows.Forms;


namespace DSShared.Lists
{
	/// <summary>
	/// Delegate for use in the CustomListColumn.WidthChanged and LeftChanged events.
	/// </summary>
	/// <param name="columnChanged">Column that is changed</param>
	/// <param name="changeAmount">How much it has changed</param>
	internal delegate void CustomListColumChangedEventHandler(CustomListColumn columnChanged, int changeAmount);

	/// <summary>
	/// Delegate for use when a column is clicked on.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	internal delegate void ColClickEventHandler(object sender, RowClickEventArgs e);

	/// <summary>
	/// Delegate for use when a key is pressed on a row in this column.
	/// </summary>
	/// <param name="row"></param>
	/// <param name="col"></param>
	/// <param name="e"></param>
	internal delegate void KeyPressEventHandler(ObjRow row, CustomListColumn col, KeyPressEventArgs e);


	/// <summary>
	/// Class representing a column in a CustomList control.
	/// </summary>
	public sealed class CustomListColumn
	{
		/// <summary>
		/// Gets or sets the property this column holds.
		/// </summary>
		/// <value>The property.</value>
		public ObjProperty Property
		{ get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{ get; set; }

		/// <summary>
		/// Gets or sets the index. This value is the position in the column list.
		/// </summary>
		/// <value>The index.</value>
		public int Index
		{ get; set; }

		private int _width = 50;
		private int _left;


		/// <summary>
		/// Minimum width of a column
		/// </summary>
		public const int MinWidth = 20;

		/// <summary>
		/// Fired when a column's width changes.
		/// </summary>
		internal event CustomListColumChangedEventHandler WidthChanged;

		/// <summary>
		/// Fired when a column's left parameter changes.
		/// </summary>
		internal event CustomListColumChangedEventHandler LeftChanged;

		/// <summary>
		/// Fired when a row has been clicked on under this column.
		/// </summary>
		private event ColClickEventHandler OnClick;

		/// <summary>
		/// Fired when a row gets keyboard events under this column.
		/// </summary>
		private event KeyPressEventHandler KeyPress;


		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.Lists.CustomListColumn"/> class.
		/// </summary>
		/// <param name="title">Column title</param>
		/// <param name="property">ObjProperty that will reflect on the objects contained in the list</param>
		public CustomListColumn(string title, ObjProperty property)
		{
			Title = title;
			Property = property;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.Lists.CustomListColumn"/> class.
		/// </summary>
		/// <param name="title">Column title</param>
		/// <param name="property">PropertyInfo that will reflect on the objects contained in the list</param>
		public CustomListColumn(string title, System.Reflection.PropertyInfo property)
		{
			Title = title;
			Property = new ObjProperty(property);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.Lists.CustomListColumn"/> class.
		/// </summary>
		/// <param name="title">Column title</param>
		public CustomListColumn(string title)
			:
			this(title, (ObjProperty)null)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.Lists.CustomListColumn"/> class.
		/// </summary>
		public CustomListColumn()
			:
			this(String.Empty, (ObjProperty)null)
		{}


		/// <summary>
		/// Equality test. Based on GetHashCode() between objects of this type.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is CustomListColumn)
				return GetHashCode() == obj.GetHashCode();

			return false;
		}

		/// <summary>
		/// Raises the KeyPress event for the specified row.
		/// </summary>
		/// <param name="row">Row to raise event with</param>
		/// <param name="e">Args</param>
		public void FireKeyPress(ObjRow row, KeyPressEventArgs e)
		{
			if (KeyPress != null)
				KeyPress(row, this, e);
		}

		/// <summary>
		/// Gets the hash code of the title of this column.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return Title.GetHashCode(); // TODO: fix: Non-readonly field referenced ....
		}

		/// <summary>
		/// Raises the OnClick event for the specified row.
		/// </summary>
		/// <param name="row">Row to raise event with</param>
		public void FireClick(ObjRow row)
		{
			if (OnClick != null)
				OnClick(this, new RowClickEventArgs(row, this));
		}

		/// <summary>
		/// Resizes the title width based on the size of the input font.
		/// </summary>
		/// <param name="font"></param>
		public void ResizeTitle(System.Drawing.Font font)
		{
			Width = Math.Max(
						MinWidth,
						TextRenderer.MeasureText(Title, font).Width) + 2;
		}


		/// <summary>
		/// Gets or sets the left in screen coordinates.
		/// </summary>
		/// <value>The left.</value>
		public int Left
		{
			get { return _left; }
			set
			{
				int diff = _left - value;

				_left = value;

				if (LeftChanged != null)
					LeftChanged(this, diff);
			}
		}

		/// <summary>
		/// Gets or sets the width of the column in screen coordinates.
		/// </summary>
		/// <value>The width.</value>
		public int Width
		{
			get { return _width; }
			set
			{
				if (value >= MinWidth)
				{
					int diff = _width - value;

					_width = value;

					if (WidthChanged != null)
						WidthChanged(this, diff);
				}
			}
		}
	}

	/// <summary>
	/// Args class that holds a row and a column.
	/// </summary>
	internal sealed class RowClickEventArgs
		:
			EventArgs
	{
		private readonly ObjRow _row;
		private readonly CustomListColumn _col;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.Lists.RowClickEventArgs"/> class.
		/// </summary>
		/// <param name="row">the row that was clicked on</param>
		/// <param name="col">the column that was clicked under</param>
		public RowClickEventArgs(ObjRow row, CustomListColumn col)
		{
			_row = row;
			_col = col;
		}

		/// <summary>
		/// Gets the row.
		/// </summary>
		/// <value>The row.</value>
		public ObjRow Row
		{
			get { return _row; }
		}

		/// <summary>
		/// Gets the column.
		/// </summary>
		/// <value>The column.</value>
		public CustomListColumn Column
		{
			get { return _col; }
		}
	}
}
