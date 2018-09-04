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
		private readonly List<ToolStripButton> _pasteButtons = new List<ToolStripButton>();
		private readonly MainViewUnderlay _mainViewUnderlay;

		private readonly ToolStripTextBox tstbSearch = new ToolStripTextBox();
		private ToolStripButton tsbClearSearch       = new ToolStripButton();

		private ToolStripButton tsbAutoZoom = new ToolStripButton();
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
		/// Adds a textfield for search and up/down search-direction buttons to
		/// the specified toolstrip.
		/// </summary>
		/// <param name="toolStrip"></param>
		internal void CreateToolstripSearchObjects(ToolStrip toolStrip)
		{
			var tsItems = new ToolStripItem[]
			{
				tstbSearch,
				tsbClearSearch
			};
			toolStrip.Items.AddRange(tsItems);

			// Search textfield
			tstbSearch.Name             = "tstbSearch";
			tstbSearch.Text             = "search";
			tstbSearch.KeyPress        += OnSearchKeyPress;

			// ClearSearch btn
			tsbClearSearch.Name         = "tsbClearSearch";
			tsbClearSearch.DisplayStyle = ToolStripItemDisplayStyle.Text;
			tsbClearSearch.Text         = "C"; // TODO: use a 'delete' image
			tsbClearSearch.ToolTipText  = "clear search start highlight";
			tsbClearSearch.Click       += OnClearSearchedClick;
		}


		/// <summary>
		/// Adds a textfield for search and up/down search-direction buttons to
		/// the specified toolstrip.
		/// </summary>
		/// <param name="toolStrip"></param>
		internal void CreateToolstripZoomObjects(ToolStrip toolStrip)
		{
			var tssDivider1 = new ToolStripSeparator();
			var tsbZoomIn   = new ToolStripButton();
			var tsbZoomOut  = new ToolStripButton();

			var tsItems = new ToolStripItem[]
			{
				tssDivider1,
				tsbAutoZoom,
				tsbZoomIn,
				tsbZoomOut
			};
			toolStrip.Items.AddRange(tsItems);

			// AutoZoom btn
			tsbAutoZoom.Name         = "tsbAutoZoom";
			tsbAutoZoom.DisplayStyle = ToolStripItemDisplayStyle.Image;
			tsbAutoZoom.Image        = Resources._11_Search_16;
			tsbAutoZoom.Checked      = true;
			tsbAutoZoom.ToolTipText  = "auto zoom";
			tsbAutoZoom.Click       += XCMainWindow.Instance.OnAutoScaleClick;

			// ZoomIn btn
			tsbZoomIn.Name           = "tsbZoomIn";
			tsbZoomIn.DisplayStyle   = ToolStripItemDisplayStyle.Image;
			tsbZoomIn.Image          = Resources._12_Zoom_in_16;
			tsbZoomIn.ToolTipText    = "zoom In";
			tsbZoomIn.Click         += XCMainWindow.Instance.OnZoomInClick;

			// ZoomOut btn
			tsbZoomOut.Name          = "tsbZoomOut";
			tsbZoomOut.DisplayStyle  = ToolStripItemDisplayStyle.Image;
			tsbZoomOut.Image         = Resources._13_Zoom_out_16;
			tsbZoomOut.ToolTipText   = "zoom Out";
			tsbZoomOut.Click        += XCMainWindow.Instance.OnZoomOutClick;
		}


		/// <summary>
		/// Adds buttons for Up,Down,Cut,Copy,Paste and Fill to the specified
		/// toolstrip as well as sets some properties for the toolstrip.
		/// </summary>
		/// <param name="toolStrip"></param>
		internal void CreateToolstripEditorObjects(ToolStrip toolStrip)
		{
			var tssDivider1 = new ToolStripSeparator(); // NOTE: c# cant figure out how to use 1 separator 3 times.
			var tsbUp       = new ToolStripButton();
			var tsbDown     = new ToolStripButton();
			var tssDivider2 = new ToolStripSeparator();
			var tsbCut      = new ToolStripButton();
			var tsbCopy     = new ToolStripButton();
			var tsbPaste    = new ToolStripButton();
			var tssDivider3 = new ToolStripSeparator();
			var tsbFill     = new ToolStripButton();
			var tssDivider4 = new ToolStripSeparator();

			var tsItems = new ToolStripItem[]
			{
				tssDivider1,
				tsbUp,
				tsbDown,
				tssDivider2,
				tsbCut,
				tsbCopy,
				tsbPaste,
				tssDivider3,
				tsbFill,
				tssDivider4
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
			_pasteButtons.Add(tsbPaste);

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
			tsbAutoZoom.Checked = check;
		}

		/// <summary>
		/// Toggles the Autozoom button checked/unchecked.
		/// </summary>
		/// <returns>true if checked</returns>
		internal bool ToggleAutozoomChecked()
		{
			return (tsbAutoZoom.Checked = !tsbAutoZoom.Checked);
		}

		/// <summary>
		/// Enables the paste-button in any viewer after cut or copy is
		/// clicked or Ctrl+X / Ctrl+C is pressed on the keyboard.
		/// </summary>
		internal void EnablePasteButton()
		{
			foreach (var tsb in _pasteButtons)
				tsb.Enabled = true;
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
				XCMainWindow.Instance.Search(tstbSearch.Text);
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
			tstbSearch.Focus();
		}
		#endregion
	}
}
