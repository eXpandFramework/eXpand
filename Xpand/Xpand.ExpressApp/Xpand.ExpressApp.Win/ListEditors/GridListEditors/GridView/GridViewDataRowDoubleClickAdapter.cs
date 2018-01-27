using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Win.Editors.Grid.Internal;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView{
	public class GridViewDataRowDoubleClickAdapter: IGridViewDataRowDoubleClickAdapter {

		public int DoubleClickTime = SystemInformation.DoubleClickTime;
		private int mouseUpTime;
		private int mouseDownTime;
		private bool isDoubleClicking = false;
		private DevExpress.XtraGrid.Views.Grid.GridView gridView;
		private GridControl grid;
		private void grid_DoubleClick(object sender, EventArgs e) {
#if DebugTest
			Log += "-grid_DoubleClick";
#endif
			isDoubleClicking = false;
			if (DataRowDoubleClick != null) {
				if (gridView.FocusedRowHandle >= 0) {
					DXMouseEventArgs args = DXMouseEventArgs.GetMouseArgs(grid, e);
					if (args.Button == MouseButtons.Left) {
						GridHitInfo hitInfo = gridView.CalcHitInfo(args.Location);
						if (hitInfo.InRow) {
							args.Handled = true;
							DataRowDoubleClick(this, e);
						}
					}
				}
			}
		}
		private void gridView_MouseDown(object sender, MouseEventArgs e) {
#if DebugTest
			Log += "-gridView_MouseDown";
#endif
			isDoubleClicking = false;
			DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
			GridHitInfo hi = view.CalcHitInfo(new Point(e.X, e.Y));
			if ((hi != null) && (hi.RowHandle >= 0)) {
				mouseDownTime = System.Environment.TickCount;
			}
			else {
				mouseDownTime = 0;
			}
		}
		private void gridView_MouseUp(object sender, MouseEventArgs e) {
#if DebugTest
			Log += "-gridView_MouseUp";
#endif
			mouseUpTime = System.Environment.TickCount;
			if (isDoubleClicking) {
				if (e.Button == MouseButtons.Left) {
					GridHitInfo hitInfo = gridView.CalcHitInfo(e.Location);
					if (hitInfo.InRow && (e is DXMouseEventArgs)) {
						((DXMouseEventArgs)e).Handled = true;
					}
				}
				isDoubleClicking = false;
			}
		}
		private void Editor_MouseDown(object sender, MouseEventArgs e) {
#if DebugTest
			Log += "-Editor_MouseDown";
#endif
			if (e.Button == MouseButtons.Left) {
				Int32 currentTime = System.Environment.TickCount;
				if ((mouseDownTime <= mouseUpTime) && (mouseUpTime <= currentTime) && (currentTime - mouseDownTime < DoubleClickTime)) {
					isDoubleClicking = true;
					RepositoryItemPopupBase repositoryItemPopupBase = null;
					if (gridView.FocusedColumn != null) {
						repositoryItemPopupBase = gridView.FocusedColumn.ColumnEdit as RepositoryItemPopupBase;
					}
					if (gridView.ActiveEditor.IsModified && gridView.OptionsBehavior.EditorShowMode == EditorShowMode.MouseDown && gridView.OptionsView.ShowButtonMode == ShowButtonModeEnum.ShowAlways
						|| (repositoryItemPopupBase != null) && (!repositoryItemPopupBase.ReadOnly) && (repositoryItemPopupBase.ShowDropDown == ShowDropDown.DoubleClick) && (gridView.FocusedColumn != null) && (gridView.FocusedColumn.OptionsColumn.AllowEdit)) {
					}
					else {
						if (DataRowDoubleClick != null) {
							if (gridView.ActiveEditor != null) {
								gridView.ActiveEditor.DoValidate();
								gridView.PostEditor();
								gridView.CloseEditor();
							}
							if (e is DXMouseEventArgs) {
								((DXMouseEventArgs)e).Handled = true;
							}
							DataRowDoubleClick(this, e);
						}
					}
					mouseDownTime = 0;
				}
				else {
#if DebugTest
					Log += string.Format("({0},{1},{2},{3})", mouseDownTime, mouseUpTime, currentTime, DoubleClickTime);
#endif
				}
			}
		}
		private void Editor_MouseUp(object sender, MouseEventArgs e) {
#if DebugTest
			Log += "-Editor_MouseUp";
#endif
			mouseUpTime = System.Environment.TickCount;
		}
		private void GridView_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e) {
#if DebugTest
		Log += "-GridView_CustomRowCellEditForEditing";
#endif
			DevExpress.XtraGrid.Views.Grid.GridView gridView = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
			if ((e.RepositoryItem != null)
				&& (gridView.FocusedColumn != null)
				&& (gridView.FocusedRowHandle != GridControl.AutoFilterRowHandle)
				&& (gridView.FocusedRowHandle != GridControl.NewItemRowHandle)
				) {
				e.RepositoryItem.MouseDown += new MouseEventHandler(Editor_MouseDown);
				e.RepositoryItem.MouseUp += new MouseEventHandler(Editor_MouseUp);
			}
		}
		public GridViewDataRowDoubleClickAdapter(GridControl grid, DevExpress.XtraGrid.Views.Grid.GridView gridView) {
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(grid, "grid");
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(gridView, "gridView");
			this.gridView = gridView;
			this.grid = grid;
			grid.DoubleClick += grid_DoubleClick;
			gridView.MouseUp += new MouseEventHandler(gridView_MouseUp);
			gridView.MouseDown += new MouseEventHandler(gridView_MouseDown);
			gridView.CustomRowCellEditForEditing += GridView_CustomRowCellEditForEditing;
		}
		public void Dispose() {
			DataRowDoubleClick = null;
			if (grid != null) {
				grid.DoubleClick -= grid_DoubleClick;
				grid = null;
			}
			if (gridView != null) {
				gridView.MouseUp -= new MouseEventHandler(gridView_MouseUp);
				gridView.MouseDown -= new MouseEventHandler(gridView_MouseDown);
				gridView.CustomRowCellEditForEditing -= GridView_CustomRowCellEditForEditing;
				gridView = null;
			}
		}
		public event EventHandler<EventArgs> DataRowDoubleClick;
#if DebugTest
		public string Log = "";
#endif
	}
}
