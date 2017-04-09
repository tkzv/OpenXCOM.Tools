/*
using System;
using System.Drawing;
using System.Windows.Forms;


namespace DSShared.Lists
{
	/// <summary>
	/// Class that represents a row in a CustomList.
	/// </summary>
	public sealed class RowObject
		:
			IComparable
	{
		/// <summary>
		/// Raised when the control needs to refresh itself.
		/// </summary>
		internal event RefreshEventHandler RefreshEvent;

		/// <summary>
		/// The object this row displays.
		/// </summary>
		private readonly Object _object;


		/// <summary>
		/// The list of columns that specify what information of the obj is being displayed.
		/// </summary>
		private CustomListColumnCollection _columns;

		/// <summary>
		/// Row screen information.
		/// </summary>
		private int _top;
//		private int _width;

		/// <summary>
		/// Selected column.
		/// </summary>
		private CustomListColumn _colSelected;

		/// <summary>
		/// Clicked-on column.
		/// </summary>
		private CustomListColumn _colClicked;

		/// <summary>
		/// Timer to make a blinking caret when an editable cell is clicked on.
		/// </summary>
		private Timer _caretTimer;

		/// <summary>
		/// String for the blinking caret.
		/// </summary>
		private string _addStr = String.Empty;

		/// <summary>
		/// True if in edit mode.
		/// </summary>
		private bool _editMode;

		/// <summary>
		/// The edit-buffer.
		/// </summary>
		private string _editBuffer = String.Empty;

		/// <summary>
		/// Row index.
		/// </summary>
		private int _rowId;

		private static int _idCanonical;
		private readonly int _id;


		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.Lists.RowObject"/>
		/// class. Scanning of object should have taken place before this object
		/// is created.
		/// </summary>
		/// <param name="obj">the object</param>
		/// <param name="columns">the columns</param>
		private RowObject(object obj, CustomListColumnCollection columns)
		{
			_object = obj;
			_columns = columns;

			if (_caretTimer == null)
			{
				_caretTimer = new Timer();
				_caretTimer.Interval = 200;
				_caretTimer.Tick += CaretBlink;
			}

			_id = _idCanonical++;
		}

//		/// <summary>
//		/// Initializes a new instance of the <see cref="T:DSShared.Lists.RowObject"/> class.
//		/// </summary>
//		/// <param name="obj">the obj</param>
//		public RowObject(object obj)
//			:
//				this(obj, null)
//		{}


		// NOTE: not used but required by interface.
		/// <summary>
		/// Compares one row to another.
		/// </summary>
		/// <param name="other">The object to compare with.
		/// Must be an RowObject and RowObject.Object must implement IComparable</param>
		/// <returns></returns>
		public int CompareTo(object other)
		{
			var rowOther = other as RowObject;
			if (rowOther != null)
			{
				var rowThis = _object as IComparable;
				if (rowThis != null)
					return rowThis.CompareTo(rowOther._object);
			}
			return -1;
		}

		/// <summary>
		/// Sets the index of the row. This is its position in the list.
		/// </summary>
		/// <param name="id">the index of the row</param>
		internal void SetRowIndex(int id)
		{
			_rowId = id;
		}

		/// <summary>
		/// Equality test against another object. Calls Object.Equals(other.Object).
		/// </summary>
		/// <param name="obj">The other object to test against</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (_object != null)
			{
				var obj1 = obj as RowObject;
				return (obj1 != null && _object.Equals(obj1._object));
			}
			return (obj == this);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// <see cref="M:System.Object.GetHashCode"></see> is suitable for use
		/// in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>
		/// a hash code for the current <see cref="T:System.Object"></see>
		/// </returns>
		public override int GetHashCode()
		{
			if (_object == null) // FIX: "Non-readonly field referenced in GetHashCode()."
				return _id;

			return _object.GetHashCode() >> 1;
		}

		private void CaretBlink(object sender, EventArgs e)
		{
			_addStr = (_addStr.Length == 0) ? "|" : String.Empty;

			if (RefreshEvent != null)
				RefreshEvent();
		}

//		private void startTimer()
//		{
//			if (!_caretTimer.Enabled)
//				_caretTimer.Start();
//		}
//		private void stopTimer()
//		{
//			_caretTimer.Stop();
//		}

//		/// <summary>
//		/// Sets the width of the row. This is used for the drawing function and
//		/// should not be changed by the user.
//		/// </summary>
//		/// <param name="width">the width</param>
//		public void SetWidth(int width)
//		{
//			_width = width; // never used.
//		}

		/// <summary>
		/// Gets/Sets the height of the row. This is used for the drawing
		/// function and should not be changed by the user.
		/// </summary>
		/// <value>the height</value>
		internal int Height
		{ get; set; }

		/// <summary>
		/// Sets the top. This is used for the drawing function and should not
		/// be changed by the user.
		/// </summary>
		/// <param name="top">the top</param>
		internal void SetTop(int top)
		{
			_top = top;
		}

		/// <summary>
		/// Sets the column collection used to pull information from the object.
		/// </summary>
		/// <param name="collection">the columns</param>
		internal void SetColumns(CustomListColumnCollection collection)
		{
			_columns = collection;
		}

		/// <summary>
		/// Called when a mouse moves over the row.
		/// </summary>
		/// <param name="col"></param>
		internal void MouseOver(CustomListColumn col)
		{
			_colSelected = col;
		}

		/// <summary>
		/// Called when the mouse leaves the row's bounding rectangle.
		/// </summary>
		internal void MouseLeave()
		{
			_colSelected = null;
		}

		/// <summary>
		/// Called when the mouse clicks on a row.
		/// </summary>
		/// <param name="col">the column the mouse was over when the button was
		/// clicked</param>
		internal void Click(CustomListColumn col)
		{
			_addStr = "|";
			_caretTimer.Start();

			_colClicked = col;
			if (col.Property != null)
			{
				_editBuffer = _colClicked.Property.GetValue(_object).ToString();
				_editMode = true;
			}
			col.RowClick(this);
		}

		/// <summary>
		/// This method is called before another row is clicked. This is used as
		/// a 'turn off' function.
		/// </summary>
		internal void ClearClick()
		{
			_caretTimer.Stop();

			if (_colClicked != null && _colClicked.Property != null)
			{
//				try
//				{
				switch (_colClicked.Property.EditType)
				{
					case EditStrType.String:
						_colClicked.Property.SetValue(_object, _editBuffer);
						break;

					case EditStrType.Int:
						_colClicked.Property.SetValue(_object, int.Parse(_editBuffer));
						break;

					case EditStrType.Float:
						_colClicked.Property.SetValue(_object, double.Parse(_editBuffer));
						break;
				}
//				}
//				catch
//				{
					// FIX: "Empty general catch clause suppresses any error."
//				}

				_editMode = false;
			}

			_colClicked = null;
			_addStr = String.Empty;

			RowRefresh();
		}

		/// <summary>
		/// Outside access to fire the refresh event.
		/// </summary>
		private void RowRefresh()
		{
			if (RefreshEvent != null)
				RefreshEvent();
		}

		/// <summary>
		/// Called when a key is pressed on the keyboard and this row is selected.
		/// </summary>
		/// <param name="e">The <see cref="T:System.Windows.Forms.KeyPressEventArgs"/>
		/// instance containing the event data.</param>
		internal void RowKeyPress(KeyPressEventArgs e)
		{
			_colClicked.RowKeyPress(this, e);

			if (_colClicked != null && _colClicked.Property.EditType != EditStrType.None)
			{
				switch (_colClicked.Property.EditType)
				{
					case EditStrType.Custom:
						if (_colClicked.Property.KeyFunction == null)
							throw new Exception("KeyFunction was not initialized");

						_colClicked.Property.KeyFunction(this, _colClicked, e);
						break;

					default:
						if (e.KeyChar == '\b')
						{
							if (_editBuffer.Length > 0)
								_editBuffer = _editBuffer.Substring(0, _editBuffer.Length - 1);
						}
						else if (e.KeyChar > 31) // printable characters only
							_editBuffer += e.KeyChar;

						if (RefreshEvent != null)
							RefreshEvent();
						break;
				}
			}
		}

		/// <summary>
		/// This function currently does nothing and never will.
		/// </summary>
		/// <param name="e"></param>
		public void KeyDown(KeyEventArgs e)
		{}

		/// <summary>
		/// This function currently does nothing and never will.
		/// </summary>
		/// <param name="e"></param>
		public void KeyUp(KeyEventArgs e)
		{}

		/// <summary>
		/// Method that paints this row.
		/// </summary>
		/// <param name="e"></param>
//		/// <param name="yOffset"></param>
//		public void Render(PaintEventArgs e, int yOffset)
		public void Render(PaintEventArgs e)
		{
//			base.OnPaint(e);

			if (_object != null)
			{
				int startX = 0;
				var rowRect = new System.Drawing.RectangleF(
														_columns.OffX,
//														_top + yOffset + 1,
														_top + 1,
														_columns.TableWidth - 1,
														_columns.Font.Height + CustomListColumnCollection.PadY * 2 - 1);
				if (_colSelected != null)
					e.Graphics.FillRectangle(Brushes.LightGreen, rowRect);

				if (_colClicked != null)
				{
					var rect = new System.Drawing.Rectangle(
														_colClicked.Left,
//														_top + yOffset + 1,
														_top + 1,
														_colClicked.Width,
														_columns.Font.Height + CustomListColumnCollection.PadY + 1);
					e.Graphics.FillRectangle(Brushes.LightSeaGreen, rowRect);
					e.Graphics.FillRectangle(Brushes.LightSteelBlue, rect);
				}

				for (int i = 0; i != _columns.Count; ++i)
				{
					var col = _columns[i] as CustomListColumn;

					var rect = new System.Drawing.Rectangle(
														startX + _columns.OffX,
//														_top + yOffset + CustomListColumnCollection.PadY,
														_top + CustomListColumnCollection.PadY,
														col.Width - 4,
														_columns.Font.Height);

					if (_colClicked == col
						&& col.Property != null
						&& _colClicked.Property.EditType == EditStrType.Custom)
					{
						e.Graphics.DrawString(
										col.Property.GetValue(_object) + _addStr,
										_columns.Font,
										Brushes.Black,
										rect);
					}
					else if (_colClicked == col
						&& col.Property != null
						&& _colClicked.Property.EditType != EditStrType.None)
					{
						e.Graphics.DrawString(
										(_editMode) ? _editBuffer
													: col.Property.GetValue(_object) + _addStr,
										_columns.Font,
										Brushes.Black,
										rect);
					}
					else if (col.Property != null)
					{
						e.Graphics.DrawString(
										col.Property.GetValue(_object).ToString(),
										_columns.Font,
										Brushes.Black,
										rect);
					}

					startX += col.Width;
					if (_colSelected == col)
						e.Graphics.DrawRectangle(Pens.Red, rect);
				}

				e.Graphics.DrawLine(
								Pens.Black,
								_columns.OffX,
//								_top + _columns.Font.Height + CustomListColumnCollection.PadY * 2 + yOffset,
								_top + _columns.Font.Height + CustomListColumnCollection.PadY * 2,
								_columns.TableWidth - 1,
//								_top + _columns.Font.Height + CustomListColumnCollection.PadY * 2 + yOffset);
								_top + _columns.Font.Height + CustomListColumnCollection.PadY * 2);
			}
		}

//		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
//		{
//			base.OnMouseDown(e);
//		}
//		protected override void OnMouseEnter(EventArgs e)
//		{
//			base.OnMouseEnter(e);
//		}
//		protected override void OnMouseLeave(EventArgs e)
//		{
//			base.OnMouseLeave(e);
//		}
	}
}
*/
