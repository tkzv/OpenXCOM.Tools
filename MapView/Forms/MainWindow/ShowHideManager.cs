using System.Collections.Generic;
using System.Windows.Forms;


namespace MapView.Forms.MainWindow
{
	internal sealed class ShowHideManager
	{
		private readonly IEnumerable<Form> _allForms;
		private readonly IEnumerable<MenuItem> _allItems;

		private List<Form> _forms;
		private List<MenuItem> _items;


		internal ShowHideManager(
				IEnumerable<Form> allForms,
				IEnumerable<MenuItem> allItems)
		{
			_allForms = allForms;
			_allItems = allItems;
		}


		/// <summary>
		/// Hides the viewers when opening the PckView in TileView.
		/// </summary>
		internal void HideViewers()
		{
			_items = new List<MenuItem>();
			foreach (var it in _allItems)
				if (it.Checked)
					_items.Add(it);

			_forms = new List<Form>();
			foreach (var f in _allForms)
				if (f.Visible)
				{
					f.Close(); // TODO: just use Hide()
					_forms.Add(f);
				}
		}

		/// <summary>
		/// Shows the viewers when closing the PckView in TileView.
		/// </summary>
		internal void RestoreViewers()
		{
			foreach (var f in _forms)
			{
				f.Show();
				f.WindowState = FormWindowState.Normal;
			}

			foreach (var it in _items)
				it.Checked = true;
		}
	}
}
