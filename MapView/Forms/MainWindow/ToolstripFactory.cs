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

		private ToolStripButton _tsbZoomAuto = new ToolStripButton();
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
			tsbClearSearch.ToolTipText  = "clear search-start highlite";
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
				_tsbZoomAuto,
				_tsbZoomIn,
				_tsbZoomOut
			};
			toolStrip.Items.AddRange(tsItems);

			// AutoZoom btn
			_tsbZoomAuto.Name         = "tsbZoomAuto";
			_tsbZoomAuto.DisplayStyle = ToolStripItemDisplayStyle.Image;
			_tsbZoomAuto.Image        = Resources._11_Search_16;
			_tsbZoomAuto.Checked      = true;
			_tsbZoomAuto.ToolTipText  = "autoscale";
			_tsbZoomAuto.Click       += XCMainWindow.Instance.OnAutoScaleClick;

			// ZoomIn btn
			_tsbZoomIn.Name           = "tsbZoomIn";
			_tsbZoomIn.DisplayStyle   = ToolStripItemDisplayStyle.Image;
			_tsbZoomIn.Image          = Resources._12_Zoom_in_16;
			_tsbZoomIn.ToolTipText    = "scale In";
			_tsbZoomIn.Click         += XCMainWindow.Instance.OnZoomInClick;

			// ZoomOut btn
			_tsbZoomOut.Name          = "tsbZoomOut";
			_tsbZoomOut.DisplayStyle  = ToolStripItemDisplayStyle.Image;
			_tsbZoomOut.Image         = Resources._13_Zoom_out_16;
			_tsbZoomOut.ToolTipText   = "scale Out";
			_tsbZoomOut.Click        += XCMainWindow.Instance.OnZoomOutClick;
		}


		/// <summary>
		/// Adds buttons for Up,Down,Cut,Copy,Paste and Fill to the specified
		/// toolstrip in MainView.
		/// </summary>
		/// <param name="toolStrip"></param>
		/// <param name="tertiary">false for MainView's toolstrip, true for TopView's and TopRouteView's</param>
		internal void CreateToolstripEditorObjects(ToolStrip toolStrip, bool tertiary)
		{
			ToolStripButton tsbUp;
			ToolStripButton tsbDown;
			ToolStripButton tsbCut;
			ToolStripButton tsbCopy;
			ToolStripButton tsbPaste;
			ToolStripButton tsbFill;

			if (tertiary)
			{
				tsbUp    = new ToolStripButton(); // NOTE: MainView's toolstrip buttons are classvars because
				tsbDown  = new ToolStripButton(); // they will be disabled when the app loads and will be enabled
				tsbCut   = new ToolStripButton(); // when the user clicks and loads a Map. The tertiary viewers'
				tsbCopy  = new ToolStripButton(); // toolstrip buttons don't need to be disabled because those
				tsbPaste = new ToolStripButton(); // viewers appear to the user after user clicks and loads a
				tsbFill  = new ToolStripButton(); // Map (at which the buttons will already be enabled).
			}
			else
			{
				tsbUp    = _tsbUp;
				tsbDown  = _tsbDown;
				tsbCut   = _tsbCut;
				tsbCopy  = _tsbCopy;
				tsbPaste = _tsbPaste;
				tsbFill  = _tsbFill;
			}

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
			tsbCut.Click         += _mainViewUnderlay.OnCut;
//			tsbCut.Click         += (sender, e) => // -> example of ... lambda usage
//									{
//										EnablePasteButton();
//										_mainViewUnderlay.OnCut(sender, e);
//									};

			// Copy btn
			tsbCopy.Name          = "tsbCopy";
			tsbCopy.ImageScaling  = ToolStripItemImageScaling.None;
			tsbCopy.DisplayStyle  = ToolStripItemDisplayStyle.Image;
			tsbCopy.ToolTipText   = "copy";
			tsbCopy.Click        += _mainViewUnderlay.OnCopy;
//			tsbCopy.Click        += (sender, e) => // -> example of ... lambda usage
//									{
//										EnablePasteButton();
//										_mainViewUnderlay.OnCopy(sender, e);
//									};

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
			_tsbZoomAuto.Checked = check;
		}

		/// <summary>
		/// Toggles the Autozoom button checked/unchecked.
		/// </summary>
		/// <returns>true if checked</returns>
		internal bool ToggleAutozoomChecked()
		{
			return (_tsbZoomAuto.Checked = !_tsbZoomAuto.Checked);
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
			_tsbZoomAuto.Enabled =
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
