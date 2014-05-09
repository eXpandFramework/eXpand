using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.FilterEditor;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraPrinting;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail;
using Xpand.Persistent.Base.Xpo;
using NewItemRowPosition = DevExpress.ExpressApp.NewItemRowPosition;
using Fasterflect;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView {
    public static class Extensions{
        public static RepositoryItem CreateDefaultFilterControlRepositoryItem(this DataColumnInfoFilterColumn column, DevExpress.XtraGrid.Views.Grid.GridView gridView, ITypeInfo objectTypeInfo, RepositoryEditorsFactory repositoryFactory) {
            if (gridView != null && column != null) {
                if (column.Parent == null || !column.Parent.IsList) {
                    IMemberInfo memberInfo = objectTypeInfo.FindMember(column.FullName);
                    if (memberInfo != null) {
                        GridColumn gridColumn = gridView.Columns[memberInfo.BindingName];
                        if (gridColumn != null) {
                            return new GridFilterColumn(gridColumn).ColumnEditor;
                        }
                    }
                }
                if (repositoryFactory != null && column.ColumnType != null) {
                    return repositoryFactory.CreateStandaloneRepositoryItem(column.ColumnType);
                }
            }
            return null;
        }

    }
    public class InternalXafWinFilterTreeNodeModel : WinFilterTreeNodeModelBase {
        protected override void OnCreateCustomRepositoryItem(CreateCustomRepositoryItemEventArgs args) {
            base.OnCreateCustomRepositoryItem(args);
            if (CreateCustomRepositoryItem != null) {
                CreateCustomRepositoryItem(this, args);
            }
        }

        public event EventHandler<CreateCustomRepositoryItemEventArgs> CreateCustomRepositoryItem;
    }


    public abstract class GridListEditorBase : ColumnsListEditor, IControlOrderProvider, IDXPopupMenuHolder,
                                               IComplexListEditor, ILookupListEditor, IHtmlFormattingSupport,
                                               ISupportNewItemRowPosition, IFocusedElementCaptionProvider,
                                               IGridListEditorTestable, ISupportFooter, ILookupEditProvider,
                                               ISupportAppearanceCustomization, ISupportEnabledCustomization,
                                               IExportable, ISupportFilter, ISupportUpdate {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static Int32 PageRowCountForServerMode = 100;
		public const string DragEnterCustomCodeId = "DragEnter";
		public const string DragDropCustomCodeId = "DragDrop";
		private RepositoryEditorsFactory repositoryFactory;
		private bool readOnlyEditors = false;
		private GridControl grid;
		private IColumnView gridView;
		private int mouseDownTime;
		private int mouseUpTime;
		private bool focusedChangedRaised;
		private bool selectedChangedRaised;
		private bool isForceSelectRow;
		private int prevFocusedRowHandle;
		private CollectionSourceBase collectionSource;
		private RepositoryItem activeEditor;
		private ActionsDXPopupMenu popupMenu;
		private Boolean processSelectedItemBySingleClick;
		private Boolean scrollOnMouseMove;
		private Boolean trackMousePosition;
		private TimeLatch moveRowFocusSpeedLimiter;
		private bool selectedItemExecuting;
		private NewItemRowPosition newItemRowPosition = NewItemRowPosition.None;
		private XafApplication application;
		private ColumnFilterMode lookupColumnFilterMode = ColumnFilterMode.Value;
		private IDisposable criteriaSessionScope;
		private InternalXafWinFilterTreeNodeModel model;
		private Boolean isGridDataSourceChanging;
		private AppearanceFocusedCellMode appearanceFocusedCellMode = AppearanceFocusedCellMode.Smart;
		private BaseEdit GetEditor(Object sender) {
			if(sender is BaseEdit) {
				return (BaseEdit)sender;
			}
			if(sender is RepositoryItem) {
				return ((RepositoryItem)sender).OwnerEdit;
			}
			return null;
		}
		private void SetNewItemRow() {
			if(gridView == null) {
				return;
			}
			gridView.InitNewRow -= new InitNewRowEventHandler(gridView_InitNewRow);
			gridView.CancelNewRow -= new EventHandler(gridView_CancelNewRow);
			if(gridView.DataController is XafCurrencyDataController) {
				((XafCurrencyDataController)gridView.DataController).NewItemRowObjectCustomAdding -= new HandledEventHandler(gridView_DataController_NewItemRowObjectAdding);
			}
			gridView.OptionsView.NewItemRowPosition = (DevExpress.XtraGrid.Views.Grid.NewItemRowPosition)Enum.Parse(typeof(DevExpress.XtraGrid.Views.Grid.NewItemRowPosition), newItemRowPosition.ToString());
			if(newItemRowPosition != NewItemRowPosition.None) {
				gridView.InitNewRow += new InitNewRowEventHandler(gridView_InitNewRow);
				gridView.CancelNewRow += new EventHandler(gridView_CancelNewRow);
				if(gridView.DataController is XafCurrencyDataController) {
					((XafCurrencyDataController)gridView.DataController).NewItemRowObjectCustomAdding += new HandledEventHandler(gridView_DataController_NewItemRowObjectAdding);
				}
			}
		}
		private void SubscribeToDataControllerEvents() {
			if(gridView.DataController != null) {
				gridView.DataController.BeforeListChanged += new ListChangedEventHandler(DataController_BeforeListChanged);
				gridView.DataController.ListChanged += new ListChangedEventHandler(DataController_ListChanged);
			}
		}
		private void UnsubscribeFromDataControllerEvents() {
			if(gridView.DataController != null) {
				gridView.DataController.BeforeListChanged -= new ListChangedEventHandler(DataController_BeforeListChanged);
				gridView.DataController.ListChanged -= new ListChangedEventHandler(DataController_ListChanged);
			}
		}
		private void SubscribeGridViewEvents() {
			gridView.BeforeLeaveRow += new RowAllowEventHandler(gridView_BeforeLeaveRow);
			gridView.FocusedRowChanged += new FocusedRowChangedEventHandler(gridView_FocusedRowChanged);
			gridView.ColumnFilterChanged += new EventHandler(gridView_ColumnFilterChanged);
			gridView.SelectionChanged += new SelectionChangedEventHandler(gridView_SelectionChanged);
			gridView.ShowingEditor += new CancelEventHandler(gridView_EditorShowing);
			gridView.ShownEditor += new EventHandler(gridView_ShownEditor);
			gridView.HiddenEditor += new EventHandler(gridView_HiddenEditor);
			gridView.MouseDown += new MouseEventHandler(gridView_MouseDown);
			gridView.MouseUp += new MouseEventHandler(gridView_MouseUp);
			gridView.Click += new EventHandler(gridView_Click);
			gridView.MouseMove += new MouseEventHandler(gridView_MouseMove);
			gridView.MouseWheel += new MouseEventHandler(gridView_MouseWheel);
			gridView.ShowCustomizationForm += new EventHandler(gridView_ShowCustomizationForm);
			gridView.HideCustomizationForm += new EventHandler(gridView_HideCustomizationForm);
			gridView.RowCellStyle += new RowCellStyleEventHandler(gridView_RowCellStyle);
			gridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(gridView_ShowGridMenu);
			gridView.ColumnChanged += new EventHandler(gridView_ColumnChanged);
			gridView.FocusedRowLoaded += new RowEventHandler(gridView_FocusedRowLoaded);
			gridView.FilterEditorPopup += new EventHandler(gridView_FilterEditorPopup);
			gridView.FilterEditorClosed += new EventHandler(gridView_FilterEditorClosed);
			gridView.CalcPreviewText += new CalcPreviewTextEventHandler(gridView_CalcPreviewText);
			if(FilterColumnsMode == FilterColumnsMode.AllProperties) {
				gridView.CreateCustomFilterColumnCollection += new EventHandler<CreateCustomFilterColumnCollectionEventArgs>(gridview_CreateCustomFilterColumnCollection);
				gridView.CustomiseFilterFromFilterBuilder += new EventHandler<CustomiseFilterFromFilterBuilderEventArgs>(gridView_CustomiseFilterFromFilterBuilder);
			}
			if(AllowEdit) {
				gridView.ValidateRow += new ValidateRowEventHandler(gridView_ValidateRow);
			}
			SubscribeToDataControllerEvents();
			gridView.CustomRowCellEdit += new CustomRowCellEditEventHandler(gridView_CustomRowCellEdit);
		}
		private ITypeInfo FindListElementTypeInfo(IBoundProperty listProperty) {
			if((listProperty != null) && listProperty.IsList && (listProperty.Type != null) && !string.IsNullOrEmpty(listProperty.Name)) {
				IMemberInfo mi = ObjectTypeInfo.FindMember(listProperty.Name); 
				if(mi != null && mi.IsList && (mi.ListElementTypeInfo != null)) {
					return mi.ListElementTypeInfo;
				}
			}
			return null;
		}
		private void gridView_CalcPreviewText(object sender, CalcPreviewTextEventArgs e) {
			if(gridView.Columns[gridView.PreviewFieldName] != null) {
				e.PreviewText = gridView.GetRowCellDisplayText(e.RowHandle, gridView.Columns[gridView.PreviewFieldName]);
			}
		}
		protected virtual void CustomizeGridMenu(DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e) {
			if(e.MenuType == GridMenuType.Summary) {
				XafGridColumn xafGridColumn = e.HitInfo.Column as XafGridColumn;
				if(xafGridColumn != null) {
					e.Allow = xafGridColumn.AllowSummaryChange;
				}
			}
		}
		void IGridListEditorTestable.CustomizeGridMenu(DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e) {
			CustomizeGridMenu(e);
		}
		private void UnsubscribeGridViewEvents() {
			gridView.FocusedRowChanged -= new FocusedRowChangedEventHandler(gridView_FocusedRowChanged);
			gridView.ColumnFilterChanged -= new EventHandler(gridView_ColumnFilterChanged);
			gridView.SelectionChanged -= new SelectionChangedEventHandler(gridView_SelectionChanged);
			gridView.ShowingEditor -= new CancelEventHandler(gridView_EditorShowing);
			gridView.ShownEditor -= new EventHandler(gridView_ShownEditor);
			gridView.HiddenEditor -= new EventHandler(gridView_HiddenEditor);
			gridView.MouseDown -= new MouseEventHandler(gridView_MouseDown);
			gridView.MouseUp -= new MouseEventHandler(gridView_MouseUp);
			gridView.Click -= new EventHandler(gridView_Click);
			gridView.MouseMove -= new MouseEventHandler(gridView_MouseMove);
			gridView.MouseWheel -= new MouseEventHandler(gridView_MouseWheel);
			gridView.ShowCustomizationForm -= new EventHandler(gridView_ShowCustomizationForm);
			gridView.HideCustomizationForm -= new EventHandler(gridView_HideCustomizationForm);
			gridView.ColumnChanged -= new EventHandler(gridView_ColumnChanged);
			gridView.RowCellStyle -= new RowCellStyleEventHandler(gridView_RowCellStyle);
			gridView.ValidateRow -= new ValidateRowEventHandler(gridView_ValidateRow);
			gridView.BeforeLeaveRow -= new RowAllowEventHandler(gridView_BeforeLeaveRow);
			gridView.PopupMenuShowing -= new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(gridView_ShowGridMenu);
			gridView.FocusedRowLoaded -= new RowEventHandler(gridView_FocusedRowLoaded);
			gridView.FilterEditorPopup -= new EventHandler(gridView_FilterEditorPopup);
			gridView.FilterEditorClosed -= new EventHandler(gridView_FilterEditorClosed);
			gridView.CalcPreviewText -= new CalcPreviewTextEventHandler(gridView_CalcPreviewText);
			gridView.CreateCustomFilterColumnCollection -= new EventHandler<CreateCustomFilterColumnCollectionEventArgs>(gridview_CreateCustomFilterColumnCollection);
			UnsubscribeFromDataControllerEvents();
			gridView.CustomRowCellEdit -= new CustomRowCellEditEventHandler(gridView_CustomRowCellEdit);
		}
		private void SetGridViewOptions() {
			gridView.OptionsBehavior.EditorShowMode = EditorShowMode.MouseDownFocused;
			gridView.OptionsBehavior.Editable = true;
			gridView.OptionsBehavior.AllowIncrementalSearch = !AllowEdit || ReadOnlyEditors;
			gridView.OptionsBehavior.AutoPopulateColumns = false;
			gridView.OptionsBehavior.FocusLeaveOnTab = true;
			gridView.OptionsBehavior.CacheValuesOnRowUpdating = CacheRowValuesMode.Disabled;
			gridView.OptionsSelection.MultiSelect = true;
			gridView.OptionsSelection.EnableAppearanceFocusedCell = true;
			gridView.OptionsNavigation.AutoFocusNewRow = true;
			gridView.OptionsNavigation.AutoMoveRowFocus = true;
			gridView.OptionsView.ShowDetailButtons = false;
			gridView.OptionsDetail.EnableMasterViewMode = false;
			gridView.OptionsView.ShowIndicator = true;
			gridView.OptionsFilter.DefaultFilterEditorView = FilterEditorViewMode.VisualAndText;
			gridView.OptionsFilter.FilterEditorAggregateEditing = FilterControlAllowAggregateEditing.AggregateWithCondition;
			gridView.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
			gridView.FocusRectStyle = DrawFocusRectStyle.RowFocus;
			gridView.OptionsView.ShowButtonMode = ShowButtonModeEnum.ShowOnlyInEditor;
			ApplyHtmlFormatting();
			gridView.OptionsMenu.ShowGroupSummaryEditorItem = true;
		}
		private void SetupGridView() {
			if(gridView == null) {
				throw new ArgumentNullException("gridView");
			}
			gridView.ErrorMessages = ErrorMessages;
			SubscribeGridViewEvents();
			SetNewItemRow();
		}
        private IColumnView CreateGridView() {
			gridView = CreateGridViewCore();
			return gridView;
		}
		private void SaveFocusedRowHandle(int focusedRowHandle) {
			prevFocusedRowHandle = focusedRowHandle;
		}
		private void RestoreFocusedRow() {
			int dataRowCount = gridView.DataRowCount;
			if(prevFocusedRowHandle >= dataRowCount) {
				SelectRowByHandle(dataRowCount - 1);
			}
			else {
				SelectRowByHandle(prevFocusedRowHandle);
			}
		}
		private void SelectRowByHandle(int rowHandle) {
			if(gridView.IsValidRowHandle(rowHandle)) {
				XtraGridUtils.SelectRowByHandle((DevExpress.XtraGrid.Views.Base.ColumnView)gridView, rowHandle);
			}
		}
		private void gridView_DataController_NewItemRowObjectAdding(object sender, HandledEventArgs e) {
			e.Handled = OnNewObjectAdding() != null;
		}
		private void gridView_InitNewRow(object sender, InitNewRowEventArgs e) {
			OnNewObjectCreated();
		}
		private void gridView_CancelNewRow(object sender, EventArgs e) {
			OnNewObjectCanceled();
		}
		private void gridView_ShowGridMenu(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e) {
			CustomizeGridMenu(e);
		}
		private void gridView_RowCellStyle(object sender, RowCellStyleEventArgs e) {
			if(e.RowHandle != GridControl.AutoFilterRowHandle) {
				string propertyName = e.Column is XafGridColumn ? ((XafGridColumn)e.Column).PropertyName : e.Column.FieldName;
				OnCustomizeAppearance(propertyName, new GridViewRowCellStyleEventArgsAppearanceAdapter((DevExpress.XtraGrid.Views.Grid.GridView)sender, e), e.RowHandle);
			}
		}
		private void gridView_MouseWheel(object sender, MouseEventArgs e) {
			moveRowFocusSpeedLimiter.Reset();
		}
		private void gridView_HideCustomizationForm(object sender, EventArgs e) {
			OnEndCustomization();
		}
		private void gridView_ShowCustomizationForm(object sender, EventArgs e) {
			OnBeginCustomization();
		}
		private void gridView_FilterEditorPopup(object sender, EventArgs e) {
			if(collectionSource != null && collectionSource.ObjectSpace != null) {
				criteriaSessionScope = collectionSource.ObjectSpace.CreateParseCriteriaScope();
			}
			OnBeginCustomization();
		}
		private void gridView_FilterEditorClosed(object sender, EventArgs e) {
			OnEndCustomization();
			if(criteriaSessionScope != null) {
				criteriaSessionScope.Dispose();
			}
		}
		private void OnBeginCustomization() {
			if(BeginCustomization != null) {
				BeginCustomization(this, EventArgs.Empty);
			}
		}
		private void OnEndCustomization() {
			if(EndCustomization != null) {
				EndCustomization(this, EventArgs.Empty);
			}
		}
		private bool IsGroupRowHandle(int handle) {
			return handle < 0;
		}
		private void grid_HandleCreated(object sender, EventArgs e) {
			AssignDataSourceToControl(DataSource);
		}
		private void grid_KeyDown(object sender, KeyEventArgs e) {
			ProcessGridKeyDown(e);
		}
		private void SubmitActiveEditorChanges() {
			if((GridView.ActiveEditor != null) && GridView.ActiveEditor.IsModified) {
				GridView.PostEditor();
				GridView.UpdateCurrentRow();
			}
		}
		private void grid_DoubleClick(object sender, EventArgs e) {
			ProcessMouseClick(e);
		}
		private void gridView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			isForceSelectRow = e.Action == CollectionChangeAction.Add;
			if(!isGridDataSourceChanging) {
				OnSelectionChanged();
			}
		}
		private void gridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
			if(GridView.DataController.IsUpdateLocked) {
				return;
			}
			OnFocusedObjectChanged();
		}
		private void gridView_FocusedRowLoaded(object sender, RowEventArgs e) {
			if(IsAsyncServerMode()) {
				OnFocusedObjectChanged();
				OnSelectionChanged(); 
			}
		}
		private void gridView_ColumnFilterChanged(object sender, EventArgs e) {
			if(!GridView.IsLoading) {
				OnFocusedObjectChanged();
			}
		}
		private void gridView_Click(Object sender, EventArgs e) {
			if(processSelectedItemBySingleClick) {
				ProcessMouseClick(e);
			}
		}
		private Boolean IsLastVisibleRow(GridHitInfo hitInfo, RowVisibleState rowVisibleState) {
			Boolean result = false;
			if(hitInfo.RowHandle >= 0) {
				if(rowVisibleState == RowVisibleState.Partially) {
					result = true;
				}
				else if(rowVisibleState == RowVisibleState.Visible) {
					if(hitInfo.RowHandle < gridView.RowCount - 1) {
						RowVisibleState nextRowVisibleState = gridView.IsRowVisible(hitInfo.RowHandle + 1);
						if(nextRowVisibleState == RowVisibleState.Hidden) {
							result = true;
						}
					}
				}
			}
			return result;
		}
		private void SelectRowForMousePositionTracking(Int32 rowHandle) {
			if(rowHandle != GridControl.AutoFilterRowHandle) {
				XtraGridUtils.SelectRowByHandle((DevExpress.XtraGrid.Views.Base.ColumnView) gridView, rowHandle, (gridView.FocusedRowHandle != GridControl.AutoFilterRowHandle));
			}
		}
		private void gridView_MouseMove(object sender, MouseEventArgs e) {
			if(trackMousePosition || scrollOnMouseMove) {
				GridHitInfo hitInfo = gridView.CalcHitInfo(e.Location);
				if(hitInfo.InRow) {
					Boolean isTimeIntervalExpired = moveRowFocusSpeedLimiter.IsTimeIntervalExpired;
					if(isTimeIntervalExpired) {
						moveRowFocusSpeedLimiter.ResetLastEventTime();
					}
					RowVisibleState rowVisibleState = gridView.IsRowVisible(hitInfo.RowHandle);
					if(hitInfo.RowHandle == gridView.TopRowIndex) {
						if(scrollOnMouseMove && isTimeIntervalExpired && (gridView.TopRowIndex != 0)) {
							if(trackMousePosition) {
								SelectRowForMousePositionTracking(gridView.TopRowIndex - 1);
							}
							gridView.TopRowIndex--;
						}
						else if(trackMousePosition && (gridView.FocusedRowHandle != gridView.TopRowIndex)) {
							SelectRowForMousePositionTracking(gridView.TopRowIndex);
						}
					}
					else if(IsLastVisibleRow(hitInfo, rowVisibleState)) {
						if(scrollOnMouseMove && isTimeIntervalExpired && (hitInfo.RowHandle < gridView.RowCount - 1)) {
							gridView.TopRowIndex++;
							if(trackMousePosition) {
								if(rowVisibleState == RowVisibleState.Partially) {
									SelectRowForMousePositionTracking(hitInfo.RowHandle);
								}
								else {
									SelectRowForMousePositionTracking(hitInfo.RowHandle + 1);
								}
							}
						}
						else if(trackMousePosition && (gridView.FocusedRowHandle != hitInfo.RowHandle)) {
							if(rowVisibleState == RowVisibleState.Visible) {
								SelectRowForMousePositionTracking(hitInfo.RowHandle);
							}
							else if(rowVisibleState == RowVisibleState.Partially) {
							    gridView.SetPropertyValue("SkipMakeRowVisible", true);
								try {
									SelectRowForMousePositionTracking(hitInfo.RowHandle);
								}
								finally {
									gridView.SetPropertyValue("SkipMakeRowVisible", false);
								}
							}
						}
					}
					else {
						if(trackMousePosition && (gridView.FocusedRowHandle != hitInfo.RowHandle)) {
							SelectRowForMousePositionTracking(hitInfo.RowHandle);
						}
					}
				}
			}
		}
		private void gridView_ValidateRow(object sender, ValidateRowEventArgs e) {
			if(e.Valid) {
				ValidateObjectEventArgs ea = new ValidateObjectEventArgs(FocusedObject, true);
				OnValidateObject(ea);
				e.Valid = ea.Valid;
				e.ErrorText = ea.ErrorText;
			}
		}
		private void gridView_BeforeLeaveRow(object sender, RowAllowEventArgs e) {
			if(e.Allow) {
				if(!((ISupportNewItemRow)gridView).IsNewItemRowCancelling) {
					e.Allow = OnFocusedObjectChanging();
				}
			}
		}
		private void gridView_EditorShowing(object sender, CancelEventArgs e) {
			activeEditor = null;
			string propertyName = gridView.FocusedColumn is XafGridColumn ? ((XafGridColumn)gridView.FocusedColumn).PropertyName : gridView.FocusedColumn.FieldName;
			OnCustomizeEnabled(propertyName, new GridViewCancelEventArgsAppearanceAdapter((DevExpress.XtraGrid.Views.Grid.GridView)sender, e), gridView.FocusedRowHandle);
			if(e.Cancel) {
				return;
			}
			OnCustomizeAppearance(propertyName, new GridViewCancelEventArgsAppearanceAdapter((DevExpress.XtraGrid.Views.Grid.GridView)sender, e), gridView.FocusedRowHandle);
			if(!e.Cancel) {
				RepositoryItem edit = gridView.FocusedColumn.ColumnEdit;
				if(edit != null) {
					OnCustomizeAppearance(propertyName, new GridViewCancelEventArgsAppearanceAdapterWithReset((DevExpress.XtraGrid.Views.Grid.GridView)sender, e, edit.Appearance), gridView.FocusedRowHandle);
					edit.MouseDown += new MouseEventHandler(Editor_MouseDown);
					edit.MouseUp += new MouseEventHandler(Editor_MouseUp);
					RepositoryItemButtonEdit buttonEdit = edit as RepositoryItemButtonEdit;
					if(buttonEdit != null) {
						buttonEdit.ButtonPressed += new ButtonPressedEventHandler(ButtonEdit_ButtonPressed);
					}
					RepositoryItemBaseSpinEdit spinEdit = edit as RepositoryItemBaseSpinEdit;
					if(spinEdit != null) {
						spinEdit.Spin += new SpinEventHandler(SpinEdit_Spin);
					}
					edit.KeyDown += new KeyEventHandler(Editor_KeyDown);
					activeEditor = edit;
				}
			}
		}
		private void gridView_ShownEditor(object sender, EventArgs e) {
			if(popupMenu != null) {
				popupMenu.ResetPopupContextMenuSite();
			}
			LookupEdit editor = gridView.ActiveEditor as LookupEdit;
			if(editor != null) {
				OnLookupEditCreated(editor);
			}
		}
		private void gridView_HiddenEditor(object sender, EventArgs e) {
			if(popupMenu != null) {
				popupMenu.SetupPopupContextMenuSite();
			}
			LookupEdit editor = gridView.ActiveEditor as LookupEdit;
			if(editor != null) {
				OnLookupEditCreated(editor);
			}
			if(activeEditor != null) {
				activeEditor.KeyDown -= new KeyEventHandler(Editor_KeyDown);
				activeEditor.MouseDown -= new MouseEventHandler(Editor_MouseDown);
				activeEditor.MouseUp -= new MouseEventHandler(Editor_MouseUp);
				RepositoryItemButtonEdit buttonEdit = activeEditor as RepositoryItemButtonEdit;
				if(buttonEdit != null) {
					buttonEdit.ButtonPressed -= new ButtonPressedEventHandler(ButtonEdit_ButtonPressed);
				}
				RepositoryItemBaseSpinEdit spinEdit = activeEditor as RepositoryItemBaseSpinEdit;
				if(spinEdit != null) {
					spinEdit.Spin -= new SpinEventHandler(SpinEdit_Spin);
				}
				activeEditor.Appearance.Reset();
				activeEditor = null;
			}
		}
		private void gridView_MouseDown(object sender, MouseEventArgs e) {
			DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
			GridHitInfo hi = view.CalcHitInfo(new Point(e.X, e.Y));
			if(hi.RowHandle >= 0) {
				mouseDownTime = System.Environment.TickCount;
			}
			else {
				mouseDownTime = 0;
			}
		}
		private void gridView_MouseUp(object sender, MouseEventArgs e) {
			mouseUpTime = System.Environment.TickCount;
		}
		private void UpdateAppearanceFocusedCell() {
			if(gridView != null) {
				switch(AppearanceFocusedCellMode) {
					case AppearanceFocusedCellMode.Smart:
						gridView.OptionsSelection.EnableAppearanceFocusedCell = GridView.VisibleColumns.Count > 1;
						break;
					case AppearanceFocusedCellMode.Enabled:
						gridView.OptionsSelection.EnableAppearanceFocusedCell = true;
						break;
					case AppearanceFocusedCellMode.Disabled:
						gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
						break;
				}
			}
		}
		private void gridView_ColumnChanged(object sender, EventArgs e) {
			UpdateAppearanceFocusedCell();
		}
		private void Editor_MouseDown(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				Int32 currentTime = System.Environment.TickCount;
				if((mouseDownTime <= mouseUpTime) && (mouseUpTime <= currentTime) && (currentTime - mouseDownTime < SystemInformation.DoubleClickTime)) {
					OnProcessSelectedItem();
					mouseDownTime = 0;
				}
			}
		}
		private void Editor_MouseUp(object sender, MouseEventArgs e) {
			mouseUpTime = System.Environment.TickCount;
		}
		private void Editor_KeyDown(object sender, KeyEventArgs e) {
			ProcessEditorKeyDown(e);
		}
		private void SpinEdit_Spin(object sender, SpinEventArgs e) {
			mouseDownTime = 0;
		}
		private void ButtonEdit_ButtonPressed(object sender, ButtonPressedEventArgs e) {
			mouseDownTime = 0;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool RestoreSelectedRowByHandle = true;
		private void DataController_BeforeListChanged(object sender, ListChangedEventArgs e) {
			if(e.ListChangedType == ListChangedType.ItemChanged || (e.ListChangedType == ListChangedType.ItemDeleted && prevFocusedRowHandle != 0)) {
				SaveFocusedRowHandle(gridView.FocusedRowHandle);
			}
			if(e.ListChangedType == ListChangedType.Reset && gridView.IsServerMode && RestoreSelectedRowByHandle) {
				SaveFocusedRowHandle(gridView.FocusedRowHandle);
			}
		}
		private void DataController_ListChanged(object sender, ListChangedEventArgs e) {
			if(e.ListChangedType == ListChangedType.ItemAdded) {
				if((grid != null) && (grid.FindForm() != null) && !grid.ContainsFocus) {
					IList dataSource = ListHelper.GetList(((BaseGridController)sender).DataSource);
					if(dataSource != null && dataSource.Count == 1) {
						IEditableObject obj = dataSource[e.NewIndex] as IEditableObject;
						if(obj != null) {
							obj.EndEdit();
						}
					}
				}
			}
			if(e.ListChangedType == ListChangedType.ItemDeleted && gridView.FocusedRowHandle != BaseListSourceDataController.NewItemRow) {
				RestoreFocusedRow();
				OnFocusedObjectChanged();
			}
			if(e.ListChangedType == ListChangedType.Reset) {
				if(gridView.IsServerMode && RestoreSelectedRowByHandle) {
					RestoreFocusedRow();
				}
				if(gridView.SelectedRowsCount == 0) {
					XtraGridUtils.SelectFocusedRow((DevExpress.XtraGrid.Views.Base.ColumnView) gridView);
				}
				OnFocusedObjectChanged(); 
			}
		}
		private void SetTag() {
			if(grid != null) {
				grid.Tag = EasyTestTagHelper.FormatTestTable(Name);
			}
		}
		private void repositoryItem_EditValueChanging(object sender, ChangingEventArgs e) {
			BaseEdit editor = GetEditor(sender);
			if((editor != null) && (editor.InplaceType == InplaceType.Grid) ) {
				OnObjectChanged();
			}
		}
		private void grid_VisibleChanged(object sender, EventArgs e) {
			AssignDataSourceToControl(DataSource);
			if(grid.Visible) {
				GridColumn defaultColumn = GetDefaultColumn();
				if(defaultColumn != null)
					gridView.FocusedColumn = defaultColumn;
			}
		}
		private void grid_Paint(object sender, PaintEventArgs e) {
			grid.Paint -= new PaintEventHandler(grid_Paint);
			UpdateAppearanceFocusedCell();
		}
		private void grid_ParentChanged(object sender, EventArgs e) {
			if(grid.Parent != null) {
				grid.ForceInitialize();
			}
		}
		private GridColumn GetDefaultColumn() {
			GridColumn result = null;
			if(Model != null) {
				ITypeInfo classType = Model.ModelClass.TypeInfo;
				if(classType != null) {
					IMemberInfo defaultMember = classType.DefaultMember;
					if(defaultMember != null) {
						result = GridView.Columns[defaultMember.Name];
					}
				}
			}
			return result == null || !result.Visible ? null : result;
		}
		private void RemoveColumnInfo(IXafGridColumn column) {
			if(column is XafGridColumn) {
				IModelColumn columnInfo = Model.Columns[((XafGridColumn)column).Model.Id];
				if(columnInfo != null) {
					columnInfo.Remove();
				}
			}
		}
		private void UpdateAllowEditGridViewAndColumnsOptions() {
			if(gridView != null) {
				gridView.BeginUpdate();
				foreach(GridColumn column in gridView.Columns) {
					column.OptionsColumn.AllowEdit = column.ColumnEdit != null && IsDataShownOnDropDownWindow(column.ColumnEdit) ? true : AllowEdit;
					if(column.ColumnEdit != null) {
						column.ColumnEdit.ReadOnly = !AllowEdit || ReadOnlyEditors;
						if(column is XafGridColumn) {
							column.ColumnEdit.ReadOnly |= !((XafGridColumn)column).Model.AllowEdit;
						}
					}
					if(AllowEdit) {
						gridView.ValidateRow += new ValidateRowEventHandler(gridView_ValidateRow);
					}
					else {
						gridView.ValidateRow -= new ValidateRowEventHandler(gridView_ValidateRow);
					}
					gridView.OptionsBehavior.AllowIncrementalSearch = !AllowEdit || ReadOnlyEditors;
				}
				gridView.EndUpdate();
			}
		}
		internal bool IsAsyncServerMode() {
			CollectionSource source = CollectionSource as CollectionSource;
			return ((source != null) && source.IsServerMode && source.IsAsyncServerMode);
		}
		private bool IsReplacedColumnByAsyncServerMode(string propertyName) {
			IMemberInfo memberInfo = ObjectTypeInfo.FindMember(propertyName);
			return IsAsyncServerMode()
				&& (memberInfo != null)
				&& (memberInfo.MemberType != typeof(Type))
				&& (GetDisplayablePropertyName(propertyName) != memberInfo.BindingName)
				&& SimpleTypes.IsClass(memberInfo.MemberType);
		}
		private IMemberInfo FindMemberInfoForColumn(IModelColumn columnInfo) {
			if(IsReplacedColumnByAsyncServerMode(columnInfo.PropertyName)) {
				return ObjectTypeInfo.FindMember(GetDisplayablePropertyName(columnInfo.PropertyName));
			}
			return ObjectTypeInfo.FindMember(columnInfo.PropertyName);
		}
		protected virtual void OnCustomizeAppearance(string name, IAppearanceBase item, int rowHandle) {
			if(CustomizeAppearance != null) {
				CustomizeAppearanceEventArgs args = null;
				if(!IsAsyncServerMode()) {
					args = new CustomizeAppearanceEventArgs(name, item, DevExpress.ExpressApp.Win.Core.XtraGridUtils.GetRow((DevExpress.XtraGrid.Views.Base.ColumnView) gridView, rowHandle));
				}
				else {
					if(!gridView.IsRowLoaded(rowHandle)) {
						return; 
					}
					args = new CustomizeAppearanceEventArgs(name, item,
						rowHandle, new AsyncServerModeContextDescriptor((DevExpress.XtraGrid.Views.Base.ColumnView) gridView, CollectionSource.ObjectSpace, CollectionSource.ObjectTypeInfo.Type));
				}
				CustomizeAppearance(this, args);
			}
		}
		protected virtual void OnCustomizeEnabled(string name, IAppearanceBase item, int rowHandle) {
			if(CustomizeEnabled != null && CollectionSource != null && !IsAsyncServerMode()) {
				if(gridView.FocusedRowHandle != GridControl.AutoFilterRowHandle && gridView.FocusedColumn.UnboundType == UnboundColumnType.Bound) {
					object rowObj = DevExpress.ExpressApp.Win.Core.XtraGridUtils.GetRow(CollectionSource, (DevExpress.XtraGrid.Views.Base.ColumnView) gridView, gridView.FocusedRowHandle);
					if(rowObj != null) {
						CustomizeEnabledEventArgs args = new CustomizeEnabledEventArgs(name, item, rowObj, CollectionSource, CollectionSource.ObjectSpace);
						CustomizeEnabled(this, args);
					}
				}
			}
		}
		protected virtual GridControl CreateGridControl() {
			return new GridControl();
		}

        protected abstract IColumnView CreateGridViewCore();
		protected static CriteriaOperator CustomiseFilterFromFilterBuilder(IEnumerable columns, FilterColumnsMode filterColumnsMode, CriteriaOperator criteria) {
			CriteriaOperator result = criteria;
			if(filterColumnsMode == FilterColumnsMode.AllProperties) {
				DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(columns, "GridView");
				List<string> existingLookupFieldNames = new List<string>();
				foreach(GridColumn col in columns) {
					if(!string.IsNullOrEmpty(col.FieldName) && col.FieldName.EndsWith("!")) {
						existingLookupFieldNames.Add(col.FieldName);
					}
				}
				if(existingLookupFieldNames.Count > 0) {
					result = CriteriaOperator.Clone(criteria);
					new PatchXpoSpecificFieldNameProcessor(existingLookupFieldNames).Process(result);
				}
			}
			return result;
		}
		private void gridView_CustomiseFilterFromFilterBuilder(object sender, CustomiseFilterFromFilterBuilderEventArgs e) {
			e.Criteria = CustomiseFilterFromFilterBuilder(GridView.Columns, FilterColumnsMode, e.Criteria);
		}
		private void gridview_CreateCustomFilterColumnCollection(object sender, CreateCustomFilterColumnCollectionEventArgs e) {
			if(FilterColumnsMode == FilterColumnsMode.AllProperties) {
				if(model == null) {
					model = new InternalXafWinFilterTreeNodeModel();
					model.CreateCustomRepositoryItem += new EventHandler<CreateCustomRepositoryItemEventArgs>(Model_CreateCustomRepositoryItem);
					model.SourceControl = DevExpress.ExpressApp.Editors.CriteriaPropertyEditorHelper.CreateFilterControlDataSource(ObjectType, application != null ? application.ObjectSpaceProvider : null);
				}
				e.FilterColumnCollection = (FilterColumnCollection)model.FilterProperties;
			}
		}
		private void Model_CreateCustomRepositoryItem(object sender, CreateCustomRepositoryItemEventArgs e) {
			if(CreateCustomFilterEditorRepositoryItem != null) {
				CreateCustomFilterEditorRepositoryItem(this, e);
			}
			if(e.RepositoryItem == null) {
                e.RepositoryItem = e.Column.CreateDefaultFilterControlRepositoryItem((DevExpress.XtraGrid.Views.Grid.GridView) GridView,ObjectTypeInfo,RepositoryFactory);
			}
		}
		protected virtual void ProcessMouseClick(EventArgs e) {
			if(!selectedItemExecuting) {
				if(GridView.FocusedRowHandle >= 0) {
					DXMouseEventArgs args = DXMouseEventArgs.GetMouseArgs(grid, e);
					if(args.Button == MouseButtons.Left) {
						GridHitInfo hitInfo = GridView.CalcHitInfo(args.Location);
						if(hitInfo.InRow) {
							args.Handled = true;
							OnProcessSelectedItem();
						}
					}
				}
			}
		}
		protected virtual void ProcessGridKeyDown(KeyEventArgs e) {
			if(FocusedObject != null && e.KeyCode == Keys.Enter) {
				if(GridView.ActiveEditor == null && !ReadOnlyEditors) {
					OnProcessSelectedItem();
					e.SuppressKeyPress = true;
					e.Handled = true;
				}
				else {
					if(ReadOnlyEditors && GridView.ActiveEditor == null) {
						if(gridView.IsLastColumnFocused) {
							gridView.UpdateCurrentRow();
							e.Handled = true;
						}
						else {
							GridView.FocusedColumn =
								GridView.GetVisibleColumn(1 + gridView.VisibleColumns.IndexOf(GridView.FocusedColumn));
							e.Handled = true;
						}
					}
					else {
						PopupBaseEdit popupEdit = GridView.ActiveEditor as PopupBaseEdit;
						if((popupEdit == null) || (!popupEdit.IsPopupOpen)) {
							SubmitActiveEditorChanges();
							e.Handled = true;
						}
					}
				}
			}
		}
		protected virtual void ProcessEditorKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter) {
				SubmitActiveEditorChanges();
			}
		}
		protected virtual void OnColumnCreated(GridColumn column, IModelColumn columnInfo) {
			if(ColumnCreated != null) {
				ColumnCreatedEventArgs args = new ColumnCreatedEventArgs(column, columnInfo);
				ColumnCreated(this, args);
			}
		}
		protected override void OnFocusedObjectChanged() {
			base.OnFocusedObjectChanged();
			focusedChangedRaised = true;
		}
		protected override void OnSelectionChanged() {
			base.OnSelectionChanged();
			selectedChangedRaised = true;
			if(GridView.SelectedRowsCount == 0 && isForceSelectRow) {
				XtraGridUtils.SelectFocusedRow((DevExpress.XtraGrid.Views.Base.ColumnView) GridView);
			}
		}
		protected virtual void OnGridDataSourceChanging() {
			if(GridDataSourceChanging != null) {
				GridDataSourceChanging(this, EventArgs.Empty);
			}
		}
		protected virtual void SubscribeToGridEvents() {
			grid.HandleCreated += new EventHandler(grid_HandleCreated);
			grid.KeyDown += new KeyEventHandler(grid_KeyDown);
			grid.DoubleClick += new EventHandler(grid_DoubleClick);
			grid.ParentChanged += new EventHandler(grid_ParentChanged);
			grid.Paint += new PaintEventHandler(grid_Paint);
			grid.VisibleChanged += new EventHandler(grid_VisibleChanged);
		}
		protected virtual void UnsubscribeFromGridEvents() {
			grid.VisibleChanged -= new EventHandler(grid_VisibleChanged);
			grid.KeyDown -= new KeyEventHandler(grid_KeyDown);
			grid.HandleCreated -= new EventHandler(grid_HandleCreated);
			grid.DoubleClick -= new EventHandler(grid_DoubleClick);
			grid.ParentChanged -= new EventHandler(grid_ParentChanged);
			grid.Paint -= new PaintEventHandler(grid_Paint);
		}
		protected void OnPrintableChanged() {
			if(PrintableChanged != null) {
				PrintableChanged(this, new PrintableChangedEventArgs(Printable));
			}
		}
		protected override object CreateControlsCore() {
			if(grid == null) {
				grid = CreateGridControl();
				((System.ComponentModel.ISupportInitialize)(grid)).BeginInit();
				try {
					grid.MinimumSize = new Size(100, 75);
					grid.Dock = DockStyle.Fill;
					grid.AllowDrop = true;
					SubscribeToGridEvents();
					grid.Height = 100;
					grid.TabStop = true;
					grid.MainView = (BaseView) CreateGridView();
					SetupGridView();
					SetGridViewOptions();
					ApplyModel();
					SetTag();
				}
				finally {
					((System.ComponentModel.ISupportInitialize)(grid)).EndInit();
					grid.ForceInitialize();
				}
				OnPrintableChanged();
			}
			return grid;
		}

		public override void ApplyModel() {
			Grid.BeginUpdate();
			try {
				base.ApplyModel();
				UpdateAppearanceFocusedCell();
			}
			finally {
				Grid.EndUpdate();
			}
		}
		protected override void OnProcessSelectedItem() {
			selectedItemExecuting = true;
			try {
				if((gridView != null) && (gridView.ActiveEditor != null)) {
					BindingHelper.EndCurrentEdit(Grid);
				}
				base.OnProcessSelectedItem();
			}
			finally {
				selectedItemExecuting = false;
			}
		}
		protected internal bool IsDataShownOnDropDownWindow(RepositoryItem repositoryItem) {
			return DXPropertyEditor.RepositoryItemsTypesWithMandatoryButtons.Contains(repositoryItem.GetType());
		}
		protected override void AssignDataSourceToControl(Object dataSource) {
			if(grid != null && grid.DataSource != dataSource) {
				if(grid.IsHandleCreated && grid.Visible) {
					focusedChangedRaised = false;
					selectedChangedRaised = false;
					grid.BeginUpdate();
					isGridDataSourceChanging = true;
					OnGridDataSourceChanging();
					try {
						UnsubscribeFromDataControllerEvents();
						gridView.CallMethod("CancelCurrentRowEdit");
						grid.DataSource = dataSource;
						SubscribeToDataControllerEvents();
					}
					finally {
						isGridDataSourceChanging = false;
						if(gridView.FocusedRowHandle > 0) {
							gridView.FocusedRowHandle = 0;
						}
						XtraGridUtils.SelectFocusedRow((DevExpress.XtraGrid.Views.Base.ColumnView) gridView);
						grid.EndUpdate();
					}
					if(!selectedChangedRaised) {
						OnSelectionChanged();
					}
					if(!focusedChangedRaised) {
						OnFocusedObjectChanged();
					}
				}
				else {
					grid.DataSource = null;
				}
			}
		}
		protected override void OnProtectedContentTextChanged() {
			base.OnProtectedContentTextChanged();
			repositoryFactory.ProtectedContentText = ProtectedContentText;
		}
		protected override void OnAllowEditChanged() {
			UpdateAllowEditGridViewAndColumnsOptions();
			base.OnAllowEditChanged();
		}
		protected override void OnErrorMessagesChanged() {
			base.OnErrorMessagesChanged();
			if(grid != null && gridView != null) {
				grid.Refresh();
				gridView.LayoutChanged();
			}
		}
		public GridListEditorBase(IModelListView model)
			: base(model) {
			FilterColumnsMode = FilterColumnsMode.AllProperties;
			popupMenu = new ActionsDXPopupMenu();
			moveRowFocusSpeedLimiter = new TimeLatch();
			moveRowFocusSpeedLimiter.TimeoutInMilliseconds = 100;
			prevFocusedRowHandle = GridControl.InvalidRowHandle;
		}
		public override void Dispose() {
			BreakLinksToControls();
			ColumnCreated = null;
			GridDataSourceChanging = null;
			base.Dispose();
		}
		public override void BreakLinksToControls() {
			base.BreakLinksToControls();
			if(popupMenu != null) {
				popupMenu.Dispose();
				popupMenu = null;
			}
			if(gridView != null) {
				UnsubscribeGridViewEvents();
				gridView.CancelNewRow -= new EventHandler(gridView_CancelNewRow);
				gridView.InitNewRow -= new InitNewRowEventHandler(gridView_InitNewRow);
				if(gridView.DataController is XafCurrencyDataController) {
					((XafCurrencyDataController)gridView.DataController).NewItemRowObjectCustomAdding -= new HandledEventHandler(gridView_DataController_NewItemRowObjectAdding);
				}
				gridView.Dispose();
				gridView = null;
			}
			if(grid != null) {
				grid.DataSource = null;
				UnsubscribeFromGridEvents();
				grid.RepositoryItems.Clear();
				grid.Dispose();
				grid = null;
				OnPrintableChanged();
			}
		}
		public string FindColumnPropertyName(GridColumn column) {
			if(column is XafGridColumn) {
				return ((XafGridColumn)column).PropertyName;
			}
			else {
				return null;
			}
		}

		protected override ColumnWrapper AddColumnCore(IModelColumn columnInfo) {
            var column = CreateGridColumn();
			GridView.Columns.Add((GridColumn) column);
			IMemberInfo memberInfo = null;
			memberInfo = FindMemberInfoForColumn(columnInfo);
			if(memberInfo != null) {
				column.FieldName = memberInfo.BindingName;
				if(memberInfo.MemberType.IsEnum) {
					column.SortMode = ColumnSortMode.Value;
				}
				else if(!SimpleTypes.IsSimpleType(memberInfo.MemberType)) {
					column.SortMode = ColumnSortMode.DisplayText;
					column.OptionsColumn.AllowSort = DefaultBoolean.True;
					column.OptionsColumn.AllowGroup = DefaultBoolean.True;
				}
				if(SimpleTypes.IsClass(memberInfo.MemberType) || memberInfo.MemberType.IsInterface) {
					column.FilterMode = ColumnFilterMode.DisplayText;
				}
				else {
					column.FilterMode = ColumnFilterMode.Value;
				}
			}
			else {
				column.FieldName = columnInfo.PropertyName;
			}
			column.ApplyModel(columnInfo);
			if(memberInfo != null) {
				if(repositoryFactory != null) {
					RepositoryItem repositoryItem = null;
					if(IsReplacedColumnByAsyncServerMode(columnInfo.PropertyName)) {
						MemberEditorInfoCalculator calculator = new MemberEditorInfoCalculator();
						Type editorType = calculator.GetEditorType(Model.ModelClass.FindMember(memberInfo.Name));
						IInplaceEditSupport propertyEditor = Activator.CreateInstance(editorType, ObjectType, columnInfo) as IInplaceEditSupport;
						repositoryItem = propertyEditor != null ? ((IInplaceEditSupport)propertyEditor).CreateRepositoryItem() : null;
					}
					else {
						repositoryItem = repositoryFactory.CreateRepositoryItem(false, columnInfo, ObjectType);
					}
					if(repositoryItem != null) {
						grid.RepositoryItems.Add(repositoryItem);
						repositoryItem.EditValueChanging += new ChangingEventHandler(repositoryItem_EditValueChanging);
						column.ColumnEdit = repositoryItem;
						column.OptionsColumn.AllowEdit = IsDataShownOnDropDownWindow(repositoryItem) ? true : AllowEdit;
						column.AppearanceCell.Options.UseTextOptions = true;
						column.AppearanceCell.TextOptions.HAlignment = WinAlignmentProvider.GetAlignment(memberInfo.MemberType);
						repositoryItem.ReadOnly |= !AllowEdit || ReadOnlyEditors;
						column.OptionsColumn.TabStop = !repositoryItem.ReadOnly;
						if(Model.UseServerMode || ((columnInfo.ModelMember != null) && columnInfo.ModelMember.Type.IsInterface && !typeof(IComparable).IsAssignableFrom(memberInfo.MemberType))) {
							column.FieldNameSortGroup = new ObjectEditorHelperBase(XafTypesInfo.Instance.FindTypeInfo(columnInfo.ModelMember.Type), columnInfo).GetFullDisplayMemberName(columnInfo.PropertyName);
						}
						if(repositoryItem is ILookupEditRepositoryItem) {
							column.FilterMode = LookupColumnFilterMode;
							if(LookupColumnFilterMode == ColumnFilterMode.Value) {
								column.OptionsFilter.AutoFilterCondition = AutoFilterCondition.Equals;
								column.OptionsFilter.FilterBySortField = DefaultBoolean.False;
							}
							else {
								column.OptionsFilter.FilterBySortField = DefaultBoolean.True;
							}
						}
						if(repositoryItem is RepositoryItemMemoExEdit) {
							column.OptionsColumn.AllowGroup = column.OptionsColumn.AllowSort = DefaultBoolean.True;
						}
						if((repositoryItem is RepositoryItemPictureEdit) && (((RepositoryItemPictureEdit)repositoryItem).CustomHeight > 0)) {
							GridView.OptionsView.RowAutoHeight = true;
						}
						if(repositoryItem is RepositoryItemRtfEditEx) {
							column.FilterMode = ColumnFilterMode.DisplayText;
						}
						if(!column.DisplayFormat.IsEquals(repositoryItem.DisplayFormat)) {
							column.DisplayFormat.FormatType = repositoryItem.DisplayFormat.FormatType;
							column.DisplayFormat.Format = repositoryItem.DisplayFormat.Format;
							column.DisplayFormat.FormatString = repositoryItem.DisplayFormat.FormatString;
						}
					}
				}
				if((column.ColumnEdit == null) && !typeof(IList).IsAssignableFrom(memberInfo.MemberType)) {
					column.OptionsColumn.AllowEdit = false;
					column.OptionsColumn.TabStop = false;
					column.FieldName = GetDisplayablePropertyName(columnInfo.PropertyName);
				}
			}
			OnColumnCreated((GridColumn) column, columnInfo);
			if(!grid.IsLoading && gridView.DataController.Columns.GetColumnIndex(column.FieldName) == -1) {
				gridView.DataController.RePopulateColumns();
			}
            return CreateGridColumnWrapper(column); ;
		}

        protected abstract void ApplyModel(IXafGridColumn column, IModelColumn columnInfo);

        protected abstract ColumnWrapper CreateGridColumnWrapper(IXafGridColumn column);
        protected abstract IXafGridColumn CreateGridColumn();
		public override void RemoveColumn(ColumnWrapper column) {
            IXafGridColumn gridColumn = ((XpandGridColumnWrapper)column).Column;
            if (GridView != null && GridView.Columns.Contains((GridColumn)gridColumn)) {
                RemoveColumnInfo(gridColumn);
                GridView.Columns.Remove((GridColumn)gridColumn);
            } else {
                throw new ArgumentException(string.Format(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.GridColumnDoesNotExist), column.PropertyName), "PropertyName");
            }
		}
		public override void Refresh() {
			if(grid != null) {
				SaveFocusedRowHandle(gridView.FocusedRowHandle);
				if(IsAsyncServerMode()) {
					CollectionSource.ResetCollection();
				}
				grid.RefreshDataSource();
				RestoreFocusedRow();
			}
		}
		public override IList GetSelectedObjects() {
			ArrayList selectedObjects = new ArrayList();
			if(GridView != null) {
				int[] selectedRows = GridView.GetSelectedRows();
				if((selectedRows != null) && (selectedRows.Length > 0)) {
					foreach(int rowHandle in selectedRows) {
						if(!IsGroupRowHandle(rowHandle)) {
							object obj = XtraGridUtils.GetRow(CollectionSource, (DevExpress.XtraGrid.Views.Base.ColumnView) GridView, rowHandle);
							if(obj != null) {
								selectedObjects.Add(obj);
							}
						}
					}
				}
			}
			return (object[])selectedObjects.ToArray(typeof(object));
		}
		protected override bool HasProtectedContent(string propertyName) {
			return !(ObjectTypeInfo.FindMember(propertyName) == null || DataManipulationRight.CanRead(ObjectType, propertyName, null, collectionSource, collectionSource != null ? collectionSource.ObjectSpace : null));
		}
		public override void StartIncrementalSearch(string searchString) {
			GridColumn defaultColumn = GetDefaultColumn();
			if(defaultColumn != null) {
				GridView.FocusedColumn = defaultColumn;
			}
			GridView.StartIncrementalSearch(searchString);
		}
		public override string[] RequiredProperties {
			get {
				List<string> result = new List<string>();
				if(Model != null) {
					foreach(IModelColumn columnInfo in Model.Columns) {
						if((columnInfo.Index > -1) || (columnInfo.GroupIndex > -1)) {
							IMemberInfo memberInfo = FindMemberInfoForColumn(columnInfo);
							if(memberInfo != null) {
								result.Add(memberInfo.BindingName);
							}
						}
					}
				}
				return result.ToArray();
			}
		}
		public override IContextMenuTemplate ContextMenuTemplate {
			get { return popupMenu; }
		}
		public override string Name {
			get { return base.Name; }
			set {
				base.Name = value;
				SetTag();
			}
		}
		public override object FocusedObject {
			get {
				object result = null;
				if(GridView != null) {
					result = XtraGridUtils.GetFocusedRowObject(CollectionSource, (DevExpress.XtraGrid.Views.Base.ColumnView) GridView);
				}
				return result;
			}
			set {
				if(value != null && value != DBNull.Value && gridView != null && DataSource != null) {
					int dataSourceIndex = List.IndexOf(value);
					if(dataSourceIndex >= 0 && object.ReferenceEquals(value, List[dataSourceIndex])) {
						XtraGridUtils.SelectRowByHandle((DevExpress.XtraGrid.Views.Base.ColumnView) gridView, gridView.GetRowHandle(dataSourceIndex));
						if(XtraGridUtils.HasValidRowHandle((DevExpress.XtraGrid.Views.Base.ColumnView) gridView)) {
							gridView.SetRowExpanded(gridView.FocusedRowHandle, true, true);
						}
					}
				}
			}
		}
		public override SelectionType SelectionType {
			get { return SelectionType.Full; }
		}
		public RepositoryEditorsFactory RepositoryFactory {
			get { return repositoryFactory; }
			set { repositoryFactory = value; }
		}
		public GridControl Grid {
			get { return grid; }
		}
		public IColumnView GridView {
			get { return gridView; }
		}
		public NewItemRowPosition NewItemRowPosition {
			get { return newItemRowPosition; }
			set {
				if(newItemRowPosition != value) {
					newItemRowPosition = value;
					SetNewItemRow();
				}
			}
		}
		public Boolean ScrollOnMouseMove {
			get { return scrollOnMouseMove; }
			set { scrollOnMouseMove = value; }
		}
		public bool ReadOnlyEditors {
			get { return readOnlyEditors; }
			set {
				if(readOnlyEditors != value) {
					readOnlyEditors = value;
					AllowEdit = !readOnlyEditors;
				}
			}
		}
		public ColumnFilterMode LookupColumnFilterMode {
			get { return lookupColumnFilterMode; }
			set { lookupColumnFilterMode = value; }
		}
		public override IList<ColumnWrapper> Columns {
			get {
                var result = new List<ColumnWrapper>();
                if (GridView != null) {
                    var columnWrappers = GridView.Columns.OfType<IXafGridColumn>().Select(column => new XpandGridColumnWrapper(column));
                    result.AddRange(columnWrappers);
                }
                return result;
			}
		}
		public AppearanceFocusedCellMode AppearanceFocusedCellMode {
			get {
				return appearanceFocusedCellMode;
			}
			set {
				if(appearanceFocusedCellMode != value) {
					appearanceFocusedCellMode = value;
					UpdateAppearanceFocusedCell();
				}
			}
		}
		public override Boolean IsServerModeSupported {
			get { return true; }
		}
		[DefaultValue(FilterColumnsMode.AllProperties)]
		public FilterColumnsMode FilterColumnsMode { get; set; }
		public event EventHandler GridDataSourceChanging;
		public event EventHandler<ColumnCreatedEventArgs> ColumnCreated;
		#region IDXPopupMenuHolder Members
		public Control PopupSite {
			get { return Grid; }
		}
		public virtual bool CanShowPopupMenu(Point position) {
			GridHitTest hitTest = gridView.CalcHitInfo(grid.PointToClient(position)).HitTest;
			return
				 ((hitTest == GridHitTest.Row)
				 || (hitTest == GridHitTest.RowCell)
				 || (hitTest == GridHitTest.EmptyRow)
				 || (hitTest == GridHitTest.None));
		}
		public void SetMenuManager(IDXMenuManager manager) {
			if(grid != null) {
				grid.MenuManager = manager;
			}
		}
		#endregion
		#region IControlOrderProvider Members
		public int GetIndexByObject(Object obj) {
			int index = -1;
			if((DataSource != null) && (gridView != null)) {
				int dataSourceIndex = List.IndexOf(obj);
				index = gridView.GetRowHandle(dataSourceIndex);
				if(index == GridControl.InvalidRowHandle) {
					index = -1;
				}
			}
			return index;
		}
		public Object GetObjectByIndex(int index) {
			if((gridView != null) && (gridView.DataController != null)) {
				return gridView.GetRow(index);
			}
			return null;
		}
		public IList GetOrderedObjects() {
			List<Object> list = new List<Object>();
			if(gridView != null) {
				if(gridView.IsServerMode) {
					Int32 focusedRowVisibleIndex = gridView.GetVisibleIndex(gridView.FocusedRowHandle);
					Int32 topRowIndex = focusedRowVisibleIndex - PageRowCountForServerMode / 2;
					if(topRowIndex < 0) {
						topRowIndex = 0;
					}
					Int32 bottomRowIndex = topRowIndex + PageRowCountForServerMode - 1;
					if(bottomRowIndex > gridView.RowCount - 1) {
						bottomRowIndex = gridView.RowCount - 1;
					}
					for(Int32 i = topRowIndex; i <= bottomRowIndex; i++) {
						Int32 rowHandle = gridView.GetVisibleRowHandle(i);
						if(gridView.IsDataRow(rowHandle)) {
							if(gridView.IsRowLoaded(rowHandle)) {
								list.Add(gridView.GetRow(rowHandle));
							}
						}
					}
				}
				else {
					for(Int32 i = 0; i < gridView.DataRowCount; i++) {
						Int32 rowHandle = i;
						if(gridView.IsRowLoaded(rowHandle)) {
							list.Add(gridView.GetRow(rowHandle));
						}
					}
				}
			}
			return list;
		}
		#endregion
		#region IComplexListEditor Members
		public virtual void Setup(CollectionSourceBase collectionSource, XafApplication application) {
			this.collectionSource = collectionSource;
			this.application = application;
			repositoryFactory = new RepositoryEditorsFactory(application, collectionSource.ObjectSpace);
		}
		#endregion
        public CollectionSourceBase CollectionSource {
			get { return collectionSource; }
		}
		#region ILookupListEditor Members
		public Boolean ProcessSelectedItemBySingleClick {
			get { return processSelectedItemBySingleClick; }
			set { processSelectedItemBySingleClick = value; }
		}
		public Boolean TrackMousePosition {
			get { return trackMousePosition; }
			set { trackMousePosition = value; }
		}
		public event EventHandler BeginCustomization;
		public event EventHandler EndCustomization;
		#endregion
		#region ISupportAppearanceCustomization Members
		public event EventHandler<CustomizeAppearanceEventArgs> CustomizeAppearance;
		#endregion
		#region ISupportAppearanceCustomization2 Members
		public event EventHandler<CustomizeEnabledEventArgs> CustomizeEnabled;
		#endregion
		#region IHtmlFormattingSupport Members
		private bool htmlFormattingEnabled;
		public void SetHtmlFormattingEnabled(bool htmlFormattingEnabled) {
			this.htmlFormattingEnabled = htmlFormattingEnabled;
			if(this.GridView != null) {
				ApplyHtmlFormatting();
			}
		}
		private void ApplyHtmlFormatting() {
			this.gridView.OptionsView.AllowHtmlDrawHeaders = htmlFormattingEnabled;
			this.gridView.Appearance.HeaderPanel.TextOptions.WordWrap = htmlFormattingEnabled ? WordWrap.Wrap : WordWrap.Default;
		}
		#endregion
		#region IFocusedElementCaptionProvider Members
		object IFocusedElementCaptionProvider.FocusedElementCaption {
			get {
				if(GridView != null) {
					return GridView.GetFocusedDisplayText();
				}
				return null;
			}
		}
		#endregion
		#region ISupportFooter Members
		bool ISupportFooter.IsFooterVisible {
			get {
				return GridView.OptionsView.ShowFooter;
			}
			set {
				GridView.OptionsView.ShowFooter = value;
			}
		}
		#endregion
		#region ILookupEditProvider Members
		private event EventHandler<LookupEditProviderEventArgs> lookupEditCreated;
		private event EventHandler<LookupEditProviderEventArgs> lookupEditHide;
		protected void OnLookupEditCreated(LookupEdit control) {
			if(lookupEditCreated != null) {
				lookupEditCreated(this, new LookupEditProviderEventArgs(control));
			}
		}
		protected void OnLookupEditHide(LookupEdit control) {
			if(lookupEditHide != null) {
				lookupEditHide(this, new LookupEditProviderEventArgs(control));
			}
		}
		event EventHandler<LookupEditProviderEventArgs> ILookupEditProvider.ControlCreated {
			add { lookupEditCreated += value; }
			remove { lookupEditCreated -= value; }
		}
		event EventHandler<LookupEditProviderEventArgs> ILookupEditProvider.HideControl {
			add { lookupEditHide += value; }
			remove { lookupEditHide -= value; }
		}
		#endregion
		public void BeginUpdate() {
			LockSelectionEvents();
		}
		public void EndUpdate() {
			UnlockSelectionEvents();
		}
		public IPrintable Printable {
			get { return Grid; }
		}
		public List<ExportTarget> SupportedExportFormats {
			get {
				if(Printable == null) {
					return new List<ExportTarget>();
				}
				else {
					return new List<ExportTarget>(){
				ExportTarget.Csv,
				ExportTarget.Html,
				ExportTarget.Image,
				ExportTarget.Mht,
				ExportTarget.Pdf,
				ExportTarget.Rtf,
				ExportTarget.Text,
				ExportTarget.Xls,
				ExportTarget.Xlsx
				};
				}
			}
		}
		public void OnExporting() {
			if(Grid != null) {
				Grid.MainView.ClearDocument(); 
			}
		}
		public event EventHandler<PrintableChangedEventArgs> PrintableChanged;
		public event EventHandler<CreateCustomRepositoryItemEventArgs> CreateCustomFilterEditorRepositoryItem;
#if DebugTest
		public CollectionSourceBase DebugTest_CollectionSource {
			get { return CollectionSource; }
		}
#endif
		bool ISupportFilter.FilterEnabled {
			get {
				return GridView.ActiveFilterEnabled;
			}
			set {
				GridView.ActiveFilterEnabled = value;
			}
		}
		string ISupportFilter.Filter {
			get {
				string result = string.Empty;
				if(CollectionSource != null && !ReferenceEquals(GridView.ActiveFilterCriteria, null)) {
					result = CriteriaOperator.ToString(GridView.ActiveFilterCriteria);
				}
				return result;
			}
			set {
				if (CollectionSource != null) {
					SetActiveFilterCriteria();
				}
				else {
					GridView.ActiveFilterString = value;
				}
			}
		}
		protected override void OnControlsCreated() {
			base.OnControlsCreated();
			if (CollectionSource != null) {
				SetActiveFilterCriteria();
			}
			if (Model != null) {
				((ISupportFilter)this).FilterEnabled = Model.FilterEnabled;
			}
		}
		protected virtual void SetActiveFilterCriteria() {
			IObjectSpace objectSpace = CollectionSource.ObjectSpace;
			ITypeInfo typeInfo = Model.ModelClass.TypeInfo;
			CriteriaOperator criteriaOperator = objectSpace.ParseCriteria(Model.Filter);
			if (IsAsyncServerMode()) {
				new AsyncServerModeCriteriaProccessor(typeInfo).Process(criteriaOperator);
			}
			FilterWithObjectsProcessor criteriaProcessor = new FilterWithObjectsProcessor(objectSpace, typeInfo, IsAsyncServerMode());
			criteriaProcessor.Process(criteriaOperator, FilterWithObjectsProcessorMode.StringToObject);
			EnumPropertyValueCriteriaProcessor enumParametersProcessor = new EnumPropertyValueCriteriaProcessor(CollectionSource.ObjectTypeInfo);
			enumParametersProcessor.Process(criteriaOperator);
			GridView.ActiveFilterCriteria = criteriaOperator;
		}
		#region Obsolete 12.2
		void gridView_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e) {
#pragma warning disable 0618
			RepositoryItem result = GetCustomRepositoryItem(e.RowHandle, e.Column.FieldName);
#pragma warning restore 0618
			if(result != null) {
				e.RepositoryItem = result;
			}
		}
		[Obsolete("Use the GridView.CustomRowCellEdit event instead.")]
		protected virtual RepositoryItem GetCustomRepositoryItem(int rowHandle, string fieldName) {
			return null;
		}
		#endregion


    }
}