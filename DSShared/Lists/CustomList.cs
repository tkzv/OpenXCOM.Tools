using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using Microsoft.Win32;

using DSShared.Windows;


namespace DSShared.Lists
{
	/// <summary>
	/// Delegate for the CustomList.RowClick event.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="clicked"></param>
	public delegate void RowClickEventHandler(object sender, ObjRow clicked);

	/// <summary>
	/// Delegate for the CustomList.RowTextChange event.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="current"></param>
	public delegate void RowTextChangeEventHandler(object sender, ObjRow current);


	/// <summary>
	/// A custom list view control allowing you to specify a list of
	/// columns that retrieve their row information via reflection.
	/// </summary>
	public class CustomList
		:
			Control
	{
		private readonly CustomListColumnCollection _columns;
		private List<ObjRow> _items;

		/// <summary>
		/// The currently selected ObjRow.
		/// </summary>
		private ObjRow _sel;

		/// <summary>
		/// The last ObjRow clicked on.
		/// </summary>
		private ObjRow _clicked;

		private int _startY;
		private int _yOffset;
		private int _selRow = -1;

		private VScrollBar _scrollbar;

		private CustomListColumn _colOver;

		private DSShared.Windows.RegistryInfo _regInfo;

		private Type _rowType;

		private string _name = String.Empty;

		/// <summary>
		/// Occurs when a row is clicked.
		/// </summary>
		public event RowClickEventHandler RowClick;

		/// <summary>
		/// Occurs when the text is changed in a row.
		/// </summary>
		public event RowTextChangeEventHandler RowTextChange;


		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.Lists.CustomList"/> class.
		/// </summary>
		public CustomList()
		{
			_columns = new CustomListColumnCollection();
			_columns.OffX =
			_columns.OffY = 1;
			_columns.Font=Font;

			_columns.RefreshEvent += Refresh;
			_columns.MouseOverEvent += mouseOverRows;
			_columns.MouseEvent += rowClicked;
			_columns.Parent = this;

			_items = new List<ObjRow>();

			SetStyle(ControlStyles.DoubleBuffer|ControlStyles.AllPaintingInWmPaint|ControlStyles.UserPaint,true);

			_startY = _columns.HeaderHeight;

			_rowType = typeof(ObjRow);
		}


		/// <summary>
		/// Gets or sets the name of the control.
		/// </summary>
		/// <value></value>
		/// <returns>the name of the control. The default is an empty string ("")</returns>
		public new string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets or sets the type of the row.
		/// </summary>
		/// <value>the type of the row</value>
		[Browsable(false)]
		[DefaultValue(typeof(ObjRow))]
		public Type RowType
		{
			get { return _rowType; }
			set { _rowType = value; }
		}

		/// <summary>
		/// Gets the items.
		/// </summary>
		/// <value>the items</value>
		public List<ObjRow> Items
		{
			get { return _items; }
		}

		/// <summary>
		/// Gets/Sets the registry info object.
		/// </summary>
		/// <value>the registry info</value>
		[Browsable(false)]
		[DefaultValue(null)]
		public RegistryInfo RegistryInfo
		{
			get { return _regInfo; }
			set
			{
				_regInfo = value;
				_regInfo.Loading += loading;
				_regInfo.Saving += saving;
			}
		}

		private void loading(object sender, RegistrySaveLoadEventArgs e)
		{
			RegistryKey key = e.OpenRegistryKey;
//			Graphics g = Graphics.FromHwnd(Handle);
			foreach (CustomListColumn col in _columns)
			{
				try
				{
					col.Width = (int)key.GetValue("strLen" + _name + col.Index, col.Width);
				}
				catch
				{
					// NOTE: was using g.MeasureString()
					col.Width = TextRenderer.MeasureText(col.Title, Font).Width + 2;
				}
			}
		}

		private void saving(object sender, RegistrySaveLoadEventArgs e)
		{
			RegistryKey regkey = e.OpenRegistryKey;
			foreach (CustomListColumn cc in _columns)
				regkey.SetValue("strLen" + _name + cc.Index, cc.Width);
		}

		private void rowClicked(object sender, MouseEventArgs e)
		{
//			int overY = (e.Y - (_columns.HeaderHeight + _yOffset)) / (Font.Height + _columns.RowSpace * 2);
			if (_clicked != null)
				_clicked.UnClick();

			if (_sel != null && _colOver != null)
				_sel.Click(_colOver);

			_clicked = _sel;

			if (RowClick != null && _clicked != null)
				RowClick(this, _clicked);
		}

		/// <summary>
		/// Gets the preferred height of the control. This is the draw height of all the rows.
		/// </summary>
		[Browsable(false)]
		public int PreferredHeight
		{
			get { return _columns.HeaderHeight + (_items.Count + 1) * RowHeight; }
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.Resize"></see> event.
		/// </summary>
		/// <param name="e">a <see cref="T:System.EventArgs"></see> that contains the event data</param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			_columns.Width  = Width;
			_columns.Height = Height;

			if (_scrollbar != null)
			{
				_scrollbar.Value = _scrollbar.Minimum;
				_startY = _columns.HeaderHeight + _scrollbar.Value;
				if (PreferredHeight > Height)
				{
					_scrollbar.Maximum = (((_items.Count + 1) * (RowHeight + 3))) - Height;
					_scrollbar.Enabled = true;
				}
				else
					_scrollbar.Enabled = false;
			}
			Refresh();
		}

		/// <summary>
		/// Gets/Sets the vertical scroll-bar. 
		/// </summary>
		/// <value>the vertical scroll-bar</value>
		[DefaultValue(null)]
		public VScrollBar VertScroll
		{
			get { return _scrollbar; }
			set
			{
				if (_scrollbar != null)
					_scrollbar.Scroll -= new ScrollEventHandler(scroll);

				_scrollbar = value; 
				_scrollbar.Minimum = 0; 
				_scrollbar.Scroll += new ScrollEventHandler(scroll);
			}
		}

		private void scroll(object sender, ScrollEventArgs e)
		{
			_yOffset = -_scrollbar.Value;
			Refresh();
		}

		private void mouseOverRows(int mouseY, CustomListColumn curCol)
		{
			int overY = (mouseY - (_columns.HeaderHeight + _yOffset)) / (Font.Height + CustomListColumnCollection.PadY * 2);
			
			if (_sel != null)
				_sel.MouseLeave();

			if (curCol != null && overY > -1 && overY < _items.Count)
			{
				_selRow = overY;
				_sel = _items[_selRow];
				_sel.MouseOver(curCol);
			}
			else
				_sel = null;

			_colOver = curCol;
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"></see> event.
		/// </summary>
		/// <param name="e">a <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data</param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			_columns.MouseMove(e);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown"></see> event.
		/// </summary>
		/// <param name="e">a <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data</param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			_columns.MouseDown(e);
			Focus();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"></see> event.
		/// </summary>
		/// <param name="e">a <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data</param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			_columns.MouseUp(e);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.LostFocus"></see> event.
		/// </summary>
		/// <param name="e">a <see cref="T:System.EventArgs"></see> that contains the event data</param>
		protected override void OnLostFocus(EventArgs e)
		{
			if (_clicked != null)
				_clicked.UnClick();

			_clicked = null;
			Refresh();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.Paint"></see> event.
		/// </summary>
		/// <param name="e">a <see cref="T:System.Windows.Forms.PaintEventArgs"></see> that contains the event data</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			int rowHeight = 0;
			for (int i = 0; i < _items.Count; i++)
			{
				var row = (ObjRow)_items[i];
				if (rowHeight + _yOffset + row.Height > -1 && rowHeight + _yOffset < Height)
					row.Render(e, _yOffset);

				rowHeight += row.Height;
			}
			_columns.Render(e, rowHeight, _yOffset);
		}

		// if row.equals(any other row) then you start to have weird behavior

		/// <summary>
		/// Deletes the row.
		/// </summary>
		/// <param name="row">the row to delete</param>
		public virtual void DeleteRow(ObjRow row)
		{
			Object obj = null;
			for (int i = 0; i < _items.Count; i++)
			{
				if (obj != null) // move this row's object to the one above it, move obj to the end to delete
					_items[i - 1].Object = _items[i].Object;
				else if (_items[i].Equals(row)) // found it, will start moving up on the next iteration
					obj = _items[i].Object;
			}

			if (obj != null) // actually deleted something
			{
				_items[_items.Count - 1].Object = obj;
				_items[_items.Count - 1].RefreshEvent -= new RefreshEventHandler(Refresh);

				_items.Remove(_items[_items.Count - 1]);
				_startY -= Font.Height + CustomListColumnCollection.PadY * 2;

				if (refreshOnAdd)
					Refresh();
			}
		}

		private bool refreshOnAdd = true;

		/// <summary>
		/// Gets or sets a value indicating whether to refresh the list when a row is added to the collection.
		/// </summary>
		/// <value><c>true</c> if [refresh on add]; otherwise, <c>false</c></value>
		[Browsable(false)]
		[DefaultValue(true)]
		public bool RefreshOnAdd
		{
			get { return refreshOnAdd; }
			set { refreshOnAdd = value; }
		}

		/// <summary>
		/// Adds a row to the item-collection. Creates an ObjRow and calls AddItem(ObjRow row).
		/// </summary>
		/// <param name="o">the item to add</param>
		public virtual void AddItem(object o)
		{
			ConstructorInfo ci = _rowType.GetConstructor(new Type[]{ typeof(object) });
			AddItem((ObjRow)ci.Invoke(new object[]{ o })); // add row.
		}

		// GOOD GOD QUIT WASTING MY FUCKING TIME!!! oh sry.

		/// <summary>
		/// Adds an ObjRow to the collection.
		/// </summary>
		/// <param name="row">the row to add</param>
		public virtual void AddItem(ObjRow row)
		{
			row.SetTop(_startY);
//			row.SetWidth(Width);
//			row.Height = RowHeight;
			row.Height += Font.Height + CustomListColumnCollection.PadY * 2;
			row.SetColumns(_columns);
			row.RefreshEvent += new RefreshEventHandler(Refresh);
			row.SetRowIndex(_items.Count);
			_items.Add(row);
			_startY += Font.Height + CustomListColumnCollection.PadY * 2;

			if (refreshOnAdd)
				Refresh();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.KeyPress"></see> event.
		/// </summary>
		/// <param name="e">a <see cref="T:System.Windows.Forms.KeyPressEventArgs"></see> that contains the event data</param>
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (_clicked != null)
			{
				_clicked.KeyPress(e);
				if (RowTextChange != null)
					RowTextChange(this, _clicked);
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.KeyDown"></see> event.
		/// </summary>
		/// <param name="e">a <see cref="T:System.Windows.Forms.KeyEventArgs"></see> that contains the event data</param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (_clicked != null)
				_clicked.KeyDown(e);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.KeyUp"></see> event.
		/// </summary>
		/// <param name="e">a <see cref="T:System.Windows.Forms.KeyEventArgs"></see>
		/// that contains the event data</param>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (_clicked != null)
				_clicked.KeyUp(e);
		}

		/// <summary>
		/// Gets the height of a row.
		/// </summary>
		/// <value>the height of a row</value>
		public int RowHeight
		{
			get { return Font.Height + CustomListColumnCollection.PadY * 2; }
		}

		/// <summary>
		/// Clears all ObjRows from the internal collection.
		/// </summary>
		public virtual void Clear()
		{
			_items = new List<ObjRow>();
			_startY = _columns.HeaderHeight;
			Refresh();
		}

		/// <summary>
		/// Gets or sets the font of the text displayed by the control.
		/// </summary>
		/// <value></value>
		/// <returns>the <see cref="T:System.Drawing.Font"></see> to apply to the text
		/// displayed by the control. The default is the value of the
		/// <see cref="P:System.Windows.Forms.Control.DefaultFont"></see> property</returns>
		/// <PermissionSet>
		/// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
		/// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// </PermissionSet>
		public new Font Font
		{
			get { return base.Font;}
			set
			{
				base.Font = value;
				_columns.Font = value;
			}
		}

		/// <summary>
		/// Adds a column to the collection.
		/// </summary>
		/// <param name="column">the column to add</param>
		public void AddColumn(CustomListColumn column)
		{
			_columns.Add(column);
			column.ResizeTitle(Font);
		}

		/// <summary>
		/// Gets a column by the specified title.
		/// </summary>
		/// <param name="name">the name of the column to get</param>
		/// <returns></returns>
		public CustomListColumn GetColumn(string name)
		{
			return _columns.GetColumn(name);
		}

		/// <summary>
		/// Gets the collection of columns that is displayed in the control.
		/// </summary>
		/// <value></value>
		[Browsable(false)]
		public CustomListColumnCollection Columns
		{
			get { return _columns; }
		}
	}
}
