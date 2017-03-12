using System.Collections.Generic;

using System.Windows.Forms;


namespace MapView.Forms.MainWindow
{
	public interface IMainWindowsShowAllManager
	{
		void HideAll();
		void RestoreAll();
	}

	public class MainWindowsShowAllManager
		:
		IMainWindowsShowAllManager
	{
		private readonly IEnumerable<Form> _allForms;
		private readonly IEnumerable<MenuItem> _allItems;

		private List<Form> _forms;
		private List<MenuItem> _items;


		public MainWindowsShowAllManager(
				IEnumerable<Form> allForms,
				IEnumerable<MenuItem> allItems)
		{
			_allForms = allForms;
			_allItems = allItems;
		}


		public void HideAll()
		{
			_items = new List<MenuItem>();
			foreach (var i in _allItems)
				if (i.Checked)
					_items.Add(i);

			_forms = new List<Form>();
			foreach (var f in _allForms)
				if (f.Visible)
				{
					f.Close();
					_forms.Add(f);
				}
		}

		public void RestoreAll()
		{
			foreach (var f in _forms)
			{
				f.Show();
				f.WindowState = FormWindowState.Normal;
			}

			foreach (var i in _items)
				i.Checked = true;
		}
	}
}
