using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace DSShared.Lists
{
	/// <summary>
	/// Delegate for when the control wants to be refreshed.
	/// </summary>
	public delegate void RefreshEventHandler();

	/// <summary>
	/// Delegate for when the mouse moves over a row.
	/// </summary>
	/// <param name="mouseY">y-coordinate of the mouse</param>
	/// <param name="overCol">Column that the y-coordinate is under</param>
	public delegate void MouseOverEventHandler(int mouseY, CustomListColumn overCol);


	/// <summary>
	/// Class that manages a collection of CustomListColumn objects in a CustomList control.
	/// </summary>
	public class CustomListColumnCollection
		:
			IEnumerable
	{
		private List<CustomListColumn> _list;

		private Dictionary<string, CustomListColumn> _dictColumns;

		private  const int PadX = 2;
		internal const int PadY = 2;

		private const int Threshhold = 5;

		private int _widthTable;
		/// <summary>
		/// Gets the width of the table.
		/// </summary>
		/// <value>the width of the table</value>
		public int TableWidth
		{
			get { return _widthTable; }
		}

		private int _heightHeader = 14;

		/// <summary>
		/// Gets/Sets the x-offset value. Tweaking this will move where the
		/// control is drawn.
		/// </summary>
		/// <value></value>
		public int OffX
		{ get; set; }

		/// <summary>
		/// Gets/Sets the y-offset value. Tweaking this will move where the
		/// control is drawn.
		/// </summary>
		/// <value></value>
		public int OffY
		{ get; set; }

		private BorderStyle styleBorder = BorderStyle.FixedSingle;
		private Border3DStyle styleBorder3D = Border3DStyle.Etched;

		private SolidBrush _brushFore;
		private SolidBrush _brushHeader;

		/// <summary>
		/// Gets/Sets the font.
		/// </summary>
		/// <value>the font</value>
		public Font Font
		{ get; set; }

		private CustomListColumn movingCol;
		private CustomListColumn overCol;
		private CustomListColumn overThreshhold;

		/// <summary>
		/// Gets/Sets the parent control of this object.
		/// </summary>
		/// <value>the parent</value>
		public CustomList Parent
		{ get; set; }

		/// <summary>
		/// Gets/Sets the width.
		/// </summary>
		/// <value>the width</value>
		public int Width
		{ get; set; }

		/// <summary>
		/// Gets/Sets the height.
		/// </summary>
		/// <value>the height</value>
		public int Height
		{ get; set; }


		/// <summary>
		/// Event for when this collection wants to be refreshed.
		/// </summary>
		public event RefreshEventHandler RefreshEvent;

		/// <summary>
		/// Event for when the mouse moves over a column.
		/// </summary>
		public event MouseOverEventHandler MouseOverEvent;

		/// <summary>
		/// Event for when a row gets clicked.
		/// </summary>
		public event MouseEventHandler MouseEvent;


		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.Lists.CustomListColumnCollection"/> class.
		/// </summary>
		public CustomListColumnCollection()
		{
//			SetStyle(
//				  ControlStyles.SupportsTransparentBackColor
//				| ControlStyles.DoubleBuffer
//				| ControlStyles.AllPaintingInWmPaint
//				| ControlStyles.UserPaint
//				| ControlStyles.ResizeRedraw, true);

			_brushHeader = new SolidBrush(Color.LightGray);
			_brushFore = new SolidBrush(Color.Black);

			_list = new List<CustomListColumn>();
			_dictColumns = new Dictionary<string, CustomListColumn>();
		}


		/// <summary>
		/// Gets/Sets the <see cref="T:DSShared.Lists.CustomListColumn"/> with the specified idx.
		/// </summary>
		/// <value></value>
		public CustomListColumn this[int idx]
		{
			get { return _list[idx]; }
			set { _list[idx] = value; }
		}

		/// <summary>
		/// Adds the specified column to the collection.
		/// </summary>
		/// <param name="column">the column</param>
		public void Add(CustomListColumn column)
		{
			if (!_list.Contains(column))
			{
				_dictColumns[column.Title] = column;

				column.Left = _widthTable;
				column.Index = _list.Count;

				column.LeftChangedEvent += OnLeftChanged;
				column.WidthChangedEvent += OnWidthChanged;
				_widthTable += column.Width;

				_list.Add(column);
			}
		}

		/// <summary>
		/// Gets the column with the specified name.
		/// </summary>
		/// <param name="name">the name</param>
		/// <returns></returns>
		public CustomListColumn GetColumn(string name)
		{
			return _dictColumns[name];
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// a <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		/// <summary>
		/// Gets the number of columns in the collection.
		/// </summary>
		/// <value>the count</value>
		public int Count
		{
			get { return _list.Count; }
		}

		private bool _lockLeftSide;

		private void OnLeftChanged(CustomListColumn col, int amount)
		{
			if (!_lockLeftSide && col.Index != 0
				&& this[col.Index - 1].Width - amount >= CustomListColumn.MinWidth)
			{
				_lockLeftSide = true;

				this[col.Index - 1].Width -= amount;

				int startX = this[col.Index - 1].Width + this[col.Index - 1].Left;
				for (int i = col.Index; i < Count; ++i)
				{
					this[i].Left = startX;
					startX += this[i].Width;
				}

				if (RefreshEvent != null)
					RefreshEvent();

				_lockLeftSide = false;
			}
		}

		private void OnWidthChanged(CustomListColumn col, int amount)
		{
			_widthTable -= amount;

			if (!_lockLeftSide)
			{
				_lockLeftSide = true;

				for (int i = col.Index + 1; i < Count; ++i)
					this[i].Left -= amount;

				if (RefreshEvent != null)
					RefreshEvent();

				_lockLeftSide = false;
			}
		}

		/// <summary>
		/// Gets/Sets the height of the header.
		/// </summary>
		/// <value>the height of the header</value>
		[Browsable(true)]
		[Category("Appearance")]
		[DefaultValue(14)] // NOTE: This does not set the default value; it sets a value only for the designer.
		[Description("Height of the row with column headers")]
		public int HeaderHeight
		{
			get { return OffY + _heightHeader + PadY * 2; }
			set { _heightHeader = value; }
		}

		/// <summary>
		/// Gets/Sets the border style.
		/// </summary>
		/// <value>the border style</value>
		[Browsable(true)]
		[Category("Appearance")]
		[DefaultValue(BorderStyle.FixedSingle)] // NOTE: This does not set the default value; it sets a value only for the designer.
		[Description("Displays a border around the control")]
		public BorderStyle BorderStyle
		{
			get { return styleBorder; }
			set
			{
				switch (styleBorder = value)
				{
					case BorderStyle.None:
						OffX =
						OffY = 0;
						break;

					case BorderStyle.FixedSingle:
						OffX =
						OffY = 1;
						break;

					default:
						OffX =
						OffY = 2;
						break;
				}
			}
		}

		/// <summary>
		/// Gets/Sets the border3D style.
		/// </summary>
		/// <value>the border3D style</value>
		[Browsable(true)]
		[Category("Appearance")]
		[DefaultValue(Border3DStyle.Etched)] // NOTE: This does not set the default value; it sets a value only for the designer.
		[Description("Displays a border around the control")]
		public Border3DStyle Border3DStyle
		{
			get { return styleBorder3D; }
			set { styleBorder3D = value; }
		}

		/// <summary>
		/// Renders this control to the supplied PaintEventArgs
		/// </summary>
		/// <param name="e">the <see cref="T:System.Windows.Forms.PaintEventArgs"/> instance containing the event data</param>
		/// <param name="rowHeight">the height of the row</param>
//		/// <param name="yOffset">the y-offset</param>
		public void Render(PaintEventArgs e, int rowHeight/*, int yOffset*/)
		{
			e.Graphics.FillRectangle(
								_brushHeader,
								OffX, OffY,
								_widthTable - 1,
								_heightHeader + PadY * 2);

			int startX = 0;
			for (int i = 0; i != _list.Count; ++i)
			{
				var col = _list[i] as CustomListColumn;
				if (col == overCol)
					e.Graphics.FillRectangle(
										Brushes.LightBlue,
										col.Left, OffY,
										col.Width,
										_heightHeader + PadY * 2);

				// vertical lines
				e.Graphics.DrawLine(
								Pens.Black,
								col.Width + startX - PadX + OffX,
								OffY,
								col.Width + startX - PadX + OffX,
								HeaderHeight + rowHeight /*+ yOffset*/);
				e.Graphics.DrawString(
								col.Title,
								Font,
								_brushFore,
								new RectangleF(
											startX + OffX,
											OffY,
											startX + col.Width,
											Font.Height));
//				e.Graphics.DrawLine(
//								Pens.Red,
//								col.Left + col.Width,
//								HeaderHeight + rowHeight + yOffset,
//								col.Left + col.Width,
//								Height);

				startX += col.Width;
			}

			e.Graphics.DrawLine(
							Pens.Black,
							OffX,
							OffY + _heightHeader + PadY * 2,
							_widthTable - 1,
							OffY + _heightHeader + PadY * 2);

			switch (styleBorder)
			{
				case BorderStyle.Fixed3D:
					System.Windows.Forms.ControlPaint.DrawBorder3D(
															e.Graphics,
															0, 0,
															Width, Height,
															styleBorder3D);
					break;

				case BorderStyle.FixedSingle:
					System.Windows.Forms.ControlPaint.DrawBorder(
															e.Graphics,
															new Rectangle(0, 0, Width, Height),
															Color.Black,
															ButtonBorderStyle.Solid);
					break;
			}
		}

		/// <summary>
		/// The parent calls this when the mouse button is released.
		/// </summary>
		/// <param name="e">the <see cref="T:System.Windows.Forms.MouseEventArgs"/>
		/// instance containing the event data</param>
		public void MouseUp(MouseEventArgs e)
		{
			movingCol = null;
		}

		/// <summary>
		/// The parent calls this when the mouse button is pressed.
		/// </summary>
		/// <param name="e">the <see cref="T:System.Windows.Forms.MouseEventArgs"/>
		/// instance containing the event data</param>
		public void MouseDown(MouseEventArgs e)
		{
			if (e.Y < _heightHeader)
			{
				if (overThreshhold != null)
					movingCol = overThreshhold;
			}
			else
			{
				if (MouseEvent != null)
					MouseEvent(this, e);
			}
		}

		/// <summary>
		/// The parent calls this when the mouse is moved.
		/// </summary>
		/// <param name="e">the <see cref="T:System.Windows.Forms.MouseEventArgs"/>
		/// instance containing the event data</param>
		public void MouseMove(MouseEventArgs e)
		{
			if (movingCol != null)
			{
				if (this[movingCol.Index].Left + CustomListColumn.MinWidth < e.X)
				{
					if (movingCol.Index + 1 != Count)
						this[movingCol.Index + 1].Left = e.X;
					else
						movingCol.Width = e.X - movingCol.Left;
				}
			}
			else
			{
				if (e.Y < _heightHeader)
				{
					overThreshhold = null;

					for (int i = 0; i != Count; ++i)
						if (   e.X >= this[i].Left + this[i].Width - Threshhold
							&& e.X <= this[i].Left + this[i].Width + Threshhold)
						{
							overThreshhold = this[i];
							break;
						}

					Parent.Cursor = (overThreshhold != null) ? Cursors.VSplit
															 : Cursors.Arrow;
				}
				else
					Parent.Cursor = Cursors.Arrow;
			}

			overCol = null;
			foreach (CustomListColumn cc in this)
				if (cc.Left < e.X && cc.Left + cc.Width > e.X)
				{
					overCol = cc;
					if (RefreshEvent != null)
						RefreshEvent();

					break;
				}

			if (e.Y > _heightHeader && MouseOverEvent != null)
				MouseOverEvent(e.Y, overCol);
		}
	}
}
