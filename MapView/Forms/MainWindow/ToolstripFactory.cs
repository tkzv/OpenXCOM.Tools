using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using MapView.Properties;


namespace MapView.Forms.MainWindow
{
	internal sealed class ToolstripFactory
	{
		#region Properties (static)
		internal static ToolstripFactory Instance
		{ get; set; }
		#endregion


		#region Fields
		private readonly MainViewUnderlay _mainViewUnderlay;
		private readonly List<ToolStripButton> _pasters = new List<ToolStripButton>();


		private readonly ToolStripTextBox _tstbSearch = new ToolStripTextBox();

		private ToolStripButton _tsbAutoZoom = new ToolStripButton();
		private ToolStripButton _tsbZoomIn   = new ToolStripButton();
		private ToolStripButton _tsbZoomOut  = new ToolStripButton();

		private ToolStripButton _tsbUp       = new ToolStripButton();
		private ToolStripButton _tsbDown     = new ToolStripButton();
		private ToolStripButton _tsbCut      = new ToolStripButton();
		private ToolStripButton _tsbCopy     = new ToolStripButton();
		private ToolStripButton _tsbPaste    = new ToolStripButton();
		private ToolStripButton _tsbFill     = new ToolStripButton();
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="panel"></param>
		internal ToolstripFactory(MainViewUnderlay panel)
		{
			Instance = this;
			_mainViewUnderlay = panel;
		}
		#endregion


		#region Methods
		/// <summary>
		/// Adds a textfield for search to the specified toolstrip.
		/// </summary>
		/// <param name="toolStrip"></param>
		internal void CreateToolstripSearchObjects(ToolStrip toolStrip)
		{
			var tsbClearSearch = new ToolStripButton();

			var tsItems = new ToolStripItem[]
			{
				_tstbSearch,
				tsbClearSearch
			};
			toolStrip.Items.AddRange(tsItems);

			// Search textfield
			_tstbSearch.Name             = "tstbSearch";
			_tstbSearch.Text             = "search";
			_tstbSearch.KeyPress        += OnSearchKeyPress;

			// ClearSearch btn
			tsbClearSearch.Name         = "tsbClearSearch";
			tsbClearSearch.DisplayStyle = ToolStripItemDisplayStyle.Text;
			tsbClearSearch.Text         = "C"; // TODO: use a 'delete' image
			tsbClearSearch.ToolTipText  = "clear search-start highlight";
			tsbClearSearch.Click       += OnClearSearchedClick;
		}


		/// <summary>
		/// Adds buttons for zooming the map-scale to the specified toolstrip.
		/// </summary>
		/// <param name="toolStrip"></param>
		internal void CreateToolstripZoomObjects(ToolStrip toolStrip)
		{
			var tsItems = new ToolStripItem[]
			{
				new ToolStripSeparator(),
				_tsbAutoZoom,
				_tsbZoomIn,
				_tsbZoomOut
			};
			toolStrip.Items.AddRange(tsItems);

			// AutoZoom btn
			_tsbAutoZoom.Name         = "tsbAutoZoom";
			_tsbAutoZoom.DisplayStyle = ToolStripItemDisplayStyle.Image;
			_tsbAutoZoom.Image        = Resources._11_Search_16;
			_tsbAutoZoom.Checked      = true;
			_tsbAutoZoom.ToolTipText  = "auto zoom";
			_tsbAutoZoom.Click       += XCMainWindow.Instance.OnAutoScaleClick;

			// ZoomIn btn
			_tsbZoomIn.Name           = "tsbZoomIn";
			_tsbZoomIn.DisplayStyle   = ToolStripItemDisplayStyle.Image;
			_tsbZoomIn.Image          = Resources._12_Zoom_in_16;
			_tsbZoomIn.ToolTipText    = "zoom In";
			_tsbZoomIn.Click         += XCMainWindow.Instance.OnZoomInClick;

			// ZoomOut btn
			_tsbZoomOut.Name          = "tsbZoomOut";
			_tsbZoomOut.DisplayStyle  = ToolStripItemDisplayStyle.Image;
			_tsbZoomOut.Image         = Resources._13_Zoom_out_16;
			_tsbZoomOut.ToolTipText   = "zoom Out";
			_tsbZoomOut.Click        += XCMainWindow.Instance.OnZoomOutClick;
		}


		/// <summary>
		/// Adds buttons for Up,Down,Cut,Copy,Paste and Fill to the specified
		/// toolstrip in MainView.
		/// </summary>
		/// <param name="toolStrip"></param>
		internal void CreateToolstripEditorObjects(ToolStrip toolStrip)
		{
			var tsItems = new ToolStripItem[]
			{
				new ToolStripSeparator(), // NOTE: c# cant figure out how to use 1 separator 4 times.
				_tsbUp,
				_tsbDown,
				new ToolStripSeparator(),
				_tsbCut,
				_tsbCopy,
				_tsbPaste,
				new ToolStripSeparator(),
				_tsbFill,
				new ToolStripSeparator()
			};
			toolStrip.Items.AddRange(tsItems);

			// LevelUp btn
			_tsbUp.Name            = "tsbUp";
			_tsbUp.ImageScaling    = ToolStripItemImageScaling.None;
			_tsbUp.DisplayStyle    = ToolStripItemDisplayStyle.Image;
			_tsbUp.ToolTipText     = "level up";
			_tsbUp.Click          += OnUpClick;

			// LevelDown btn
			_tsbDown.Name          = "tsbDown";
			_tsbDown.ImageScaling  = ToolStripItemImageScaling.None;
			_tsbDown.DisplayStyle  = ToolStripItemDisplayStyle.Image;
			_tsbDown.ToolTipText   = "level down";
			_tsbDown.Click        += OnDownClick;

			// Cut btn
			_tsbCut.Name           = "tsbCut";
			_tsbCut.ImageScaling   = ToolStripItemImageScaling.None;
			_tsbCut.DisplayStyle   = ToolStripItemDisplayStyle.Image;
			_tsbCut.ToolTipText    = "cut";
			_tsbCut.Click         += (sender, e) =>
									{
										EnablePasteButton();
										_mainViewUnderlay.OnCut(sender, e);
									};

			// Copy btn
			_tsbCopy.Name          = "tsbCopy";
			_tsbCopy.ImageScaling  = ToolStripItemImageScaling.None;
			_tsbCopy.DisplayStyle  = ToolStripItemDisplayStyle.Image;
			_tsbCopy.ToolTipText   = "copy";
			_tsbCopy.Click        += (sender, e) =>
									{
										EnablePasteButton();
										_mainViewUnderlay.OnCopy(sender, e);
									};

			// Paste btn
			_tsbPaste.Name         = "tsbPaste";
			_tsbPaste.ImageScaling = ToolStripItemImageScaling.None;
			_tsbPaste.DisplayStyle = ToolStripItemDisplayStyle.Image;
			_tsbPaste.ToolTipText  = "paste";
			_tsbPaste.Enabled      = false;
			_tsbPaste.Click       += _mainViewUnderlay.OnPaste;
			_pasters.Add(_tsbPaste);

			// Fill btn
			_tsbFill.Name          = "tsbFill";
			_tsbFill.DisplayStyle  = ToolStripItemDisplayStyle.Text;
			_tsbFill.Text          = "F";
			_tsbFill.ToolTipText   = "fill";
			_tsbFill.Click        += _mainViewUnderlay.OnFillSelectedTiles;


			// Images ->
			_tsbUp.Image    = Resources.up;
			_tsbDown.Image  = Resources.down;
			_tsbCut.Image   = Resources.cut;
			_tsbCopy.Image  = Resources.copy;
			_tsbPaste.Image = Resources.paste;
		}

		/// <summary>
		/// Adds buttons for Up,Down,Cut,Copy,Paste and Fill to the specified
		/// toolstrip on TopView and TopRouteView.
		/// </summary>
		/// <param name="toolStrip"></param>
		internal void CreateToolstripEditorObjects2(ToolStrip toolStrip)
		{
			var tsbUp    = new ToolStripButton();
			var tsbDown  = new ToolStripButton();
			var tsbCut   = new ToolStripButton();
			var tsbCopy  = new ToolStripButton();
			var tsbPaste = new ToolStripButton();
			var tsbFill  = new ToolStripButton();

			var tsItems = new ToolStripItem[]
			{
				new ToolStripSeparator(), // NOTE: c# cant figure out how to use 1 separator 4 times.
				tsbUp,
				tsbDown,
				new ToolStripSeparator(),
				tsbCut,
				tsbCopy,
				tsbPaste,
				new ToolStripSeparator(),
				tsbFill,
				new ToolStripSeparator()
			};
			toolStrip.Items.AddRange(tsItems);

			// LevelUp btn
			tsbUp.Name            = "tsbUp";
			tsbUp.ImageScaling    = ToolStripItemImageScaling.None;
			tsbUp.DisplayStyle    = ToolStripItemDisplayStyle.Image;
			tsbUp.ToolTipText     = "level up";
			tsbUp.Click          += OnUpClick;

			// LevelDown btn
			tsbDown.Name          = "tsbDown";
			tsbDown.ImageScaling  = ToolStripItemImageScaling.None;
			tsbDown.DisplayStyle  = ToolStripItemDisplayStyle.Image;
			tsbDown.ToolTipText   = "level down";
			tsbDown.Click        += OnDownClick;

			// Cut btn
			tsbCut.Name           = "tsbCut";
			tsbCut.ImageScaling   = ToolStripItemImageScaling.None;
			tsbCut.DisplayStyle   = ToolStripItemDisplayStyle.Image;
			tsbCut.ToolTipText    = "cut";
			tsbCut.Click         += (sender, e) =>
									{
										EnablePasteButton();
										_mainViewUnderlay.OnCut(sender, e);
									};

			// Copy btn
			tsbCopy.Name          = "tsbCopy";
			tsbCopy.ImageScaling  = ToolStripItemImageScaling.None;
			tsbCopy.DisplayStyle  = ToolStripItemDisplayStyle.Image;
			tsbCopy.ToolTipText   = "copy";
			tsbCopy.Click        += (sender, e) =>
									{
										EnablePasteButton();
										_mainViewUnderlay.OnCopy(sender, e);
									};

			// Paste btn
			tsbPaste.Name         = "tsbPaste";
			tsbPaste.ImageScaling = ToolStripItemImageScaling.None;
			tsbPaste.DisplayStyle = ToolStripItemDisplayStyle.Image;
			tsbPaste.ToolTipText  = "paste";
			tsbPaste.Enabled      = false;
			tsbPaste.Click       += _mainViewUnderlay.OnPaste;
			_pasters.Add(tsbPaste);

			// Fill btn
			tsbFill.Name          = "tsbFill";
			tsbFill.DisplayStyle  = ToolStripItemDisplayStyle.Text;
			tsbFill.Text          = "F";
			tsbFill.ToolTipText   = "fill";
			tsbFill.Click        += _mainViewUnderlay.OnFillSelectedTiles;


			// Images ->
			tsbUp.Image    = Resources.up;
			tsbDown.Image  = Resources.down;
			tsbCut.Image   = Resources.cut;
			tsbCopy.Image  = Resources.copy;
			tsbPaste.Image = Resources.paste;
		}


		/// <summary>
		/// Sets the Autozoom button checked/unchecked.
		/// </summary>
		internal void SetAutozoomChecked(bool check)
		{
			_tsbAutoZoom.Checked = check;
		}

		/// <summary>
		/// Toggles the Autozoom button checked/unchecked.
		/// </summary>
		/// <returns>true if checked</returns>
		internal bool ToggleAutozoomChecked()
		{
			return (_tsbAutoZoom.Checked = !_tsbAutoZoom.Checked);
		}

		/// <summary>
		/// Enables the paste-button in any viewer after cut or copy is
		/// clicked or Ctrl+X / Ctrl+C is pressed on the keyboard.
		/// </summary>
		internal void EnablePasteButton()
		{
			foreach (var tsb in _pasters)
				tsb.Enabled = true;
		}

		/// <summary>
		/// Enables or disables toolstrip objects.
		/// </summary>
		/// <param name="enable"></param>
		internal void EnableToolStrip(bool enable)
		{
			_tsbAutoZoom.Enabled =
			_tsbZoomIn  .Enabled =
			_tsbZoomOut .Enabled =
			_tsbUp      .Enabled =
			_tsbDown    .Enabled =
			_tsbCut     .Enabled =
			_tsbCopy    .Enabled =
			_tsbPaste   .Enabled =
			_tsbFill    .Enabled = enable;
		}
		#endregion


		#region Eventcalls (editstrip)
		private void OnUpClick(object sender, EventArgs e)
		{
			if (_mainViewUnderlay.MainViewOverlay.MapBase != null)
				_mainViewUnderlay.MainViewOverlay.MapBase.LevelUp();
		}

		private void OnDownClick(object sender, EventArgs e)
		{
			if (_mainViewUnderlay.MainViewOverlay.MapBase != null)
				_mainViewUnderlay.MainViewOverlay.MapBase.LevelDown();
		}
		#endregion


		#region Eventcalls (searchstrip)
		/// <summary>
		/// Handler for pressing the Enter-key when the search-textbox is focused.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnSearchKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter)
			{
				XCMainWindow.Instance.Search(_tstbSearch.Text);
				e.Handled = true;
			}
		}

		/// <summary>
		/// Clears the searched, found, and highlighted Treenode.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnClearSearchedClick(object sender, EventArgs e)
		{
			XCMainWindow.Instance.ClearSearched();
			_tstbSearch.Focus();
		}
		#endregion
	}
}
