using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using ListView = DevExpress.ExpressApp.ListView;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView {
    public class OpenObjectController : DevExpress.ExpressApp.Win.SystemModule.OpenObjectController {
        OpenObjectImplementation _openObjectImplementation;

        public OpenObjectController() {
            TypeOfView = typeof(ObjectView);
        }

        void openObjectImplementation_ObjectToOpenChanged(Object sender, ObjectToOpenChangedEventArgs e) {
            SetObjectToOpen(e.ObjectToOpen);
        }

        protected override void UpdateActionState(Object objectToOpen) {
            if (ShouldUseCustomImplementation(View as ListView))
                _openObjectImplementation.UpdateOpenObjectActionState(objectToOpen);
            else
                base.UpdateActionState(objectToOpen);
        }

        protected override void OnActivated() {
            if (ShouldUseCustomImplementation(View as ListView)) {
                _openObjectImplementation = new OpenObjectFromListView(this);
                _openObjectImplementation.ObjectToOpenChanged += openObjectImplementation_ObjectToOpenChanged;
                _openObjectImplementation.OnControllerActivated();
            } else {
                base.OnActivated();
            }
        }

        bool ShouldUseCustomImplementation(ListView listView) {
            return listView != null && listView.Editor is GridView.GridListEditorBase;
        }

        protected override void OnDeactivated() {
            var listView = View as ListView;
            if (listView != null) {
                if (listView.Editor is GridView.GridListEditorBase) {
                    _openObjectImplementation.OnControllerDeactivated();
                    _openObjectImplementation.ObjectToOpenChanged -= openObjectImplementation_ObjectToOpenChanged;
                    _openObjectImplementation = null;
                }
            } else {
                base.OnDeactivated();
            }
        }
    }

    internal sealed class OpenObjectFromListView : OpenObjectImplementation {
        const string HasReadPermissionToTargetObjectEnabledKey = "HasReadPermissionToTargetObject";
        ControlCursorHelper _cursorHelper;
        GridControl _grid;
        DevExpress.XtraGrid.Views.Grid.GridView _gridView;
        ListView _listView;

        public OpenObjectFromListView(OpenObjectController controller)
            : base(controller) {
        }

        ColumnsListEditor GridListEditor {
            get {
                var columnsListEditor = _listView.Editor as ColumnsListEditor;
                return !(columnsListEditor is GridListEditor) && !(columnsListEditor is IColumnViewEditor)
                           ? null
                           : columnsListEditor;
            }
        }

        void listView_ControlsCreated(object sender, EventArgs e) {
            UnsubscribeFromGrid();
            if (GridListEditor != null) {
                _cursorHelper = new ControlCursorHelper(GridListEditor.GridView().GridControl);
                _grid = GridListEditor.GridView().GridControl;
                _gridView = GridListEditor.GridView();
                SubscribeToGrid();
            }
        }

        void SubscribeToGrid() {
            _grid.MouseDown += grid_MouseDown;
            _grid.MouseMove += grid_MouseMove;
            _gridView.FocusedRowChanged += gridView_FocusedRowChanged;
            _gridView.FocusedColumnChanged += gridView_FocusedColumnChanged;
            _gridView.ShownEditor += gridView_ShownEditor;
            _gridView.DataSourceChanged += gridView_DataSourceChanged;
        }

        void UnsubscribeFromGrid() {
            if (_grid != null) {
                _grid.MouseDown -= grid_MouseDown;
                _grid.MouseMove -= grid_MouseMove;
                _gridView.FocusedRowChanged -= gridView_FocusedRowChanged;
                _gridView.FocusedColumnChanged -= gridView_FocusedColumnChanged;
                _gridView.ShownEditor -= gridView_ShownEditor;
                _gridView.DataSourceChanged -= gridView_DataSourceChanged;
            }
        }

        void grid_MouseDown(object sender, MouseEventArgs eventArgs) {
            if (eventArgs.Button == MouseButtons.Left && NativeMethods.IsCtrlShiftPressed()) {
                OnObjectToOpenChanged(FindObjectToOpen(eventArgs.X, eventArgs.Y));
                if (CanExecuteOpenObjectAction()) {
                    var dxArgs = eventArgs as DXMouseEventArgs;
                    if (dxArgs != null) {
                        dxArgs.Handled = true;
                    }
                    ExecuteOpenObjectAction();
                }
            }
        }

        void grid_MouseMove(object sender, MouseEventArgs eventArgs) {
            if (NativeMethods.IsCtrlShiftPressed() && FindObjectToOpen(eventArgs.X, eventArgs.Y) != null) {
                _cursorHelper.ChangeControlCursor(Cursors.Hand);
            } else {
                _cursorHelper.Restore();
            }
        }

        void gridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
            OnObjectToOpenChanged(FindObjectToOpen(GridListEditor.GridView().FocusedColumn,
                                                   GridListEditor.GridView().FocusedRowHandle));
        }

        void gridView_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e) {
            OnObjectToOpenChanged(FindObjectToOpen(GridListEditor.GridView().FocusedColumn,
                                                   GridListEditor.GridView().FocusedRowHandle));
        }

        void gridView_ShownEditor(object sender, EventArgs e) {
            GridListEditor.GridView().ActiveEditor.EditValueChanged += ActiveEditor_EditValueChanged;
            GridListEditor.GridView().ActiveEditor.VisibleChanged += ActiveEditor_VisibleChanged;
        }

        void ActiveEditor_VisibleChanged(object sender, EventArgs e) {
            var editor = sender as BaseEdit;
            if (editor != null && !editor.Visible) {
                editor.EditValueChanged -= ActiveEditor_EditValueChanged;
            }
        }

        void ActiveEditor_EditValueChanged(object sender, EventArgs e) {
            OnObjectToOpenChanged(FindObjectToOpen(GridListEditor.GridView().FocusedColumn,
                                                   GridListEditor.GridView().FocusedRowHandle));
        }

        void gridView_DataSourceChanged(object sender, EventArgs e) {
            OnObjectToOpenChanged(FindObjectToOpen(GridListEditor.GridView().FocusedColumn,
                                                   GridListEditor.GridView().FocusedRowHandle));
        }

        Object FindObjectToOpen(int mouseX, int mouseY) {
            if (GridListEditor != null && GridListEditor.GridView() != null) {
                GridHitInfo gridHitInfo = GridListEditor.GridView().CalcHitInfo(new Point(mouseX, mouseY));
                return FindObjectToOpen(gridHitInfo.Column, gridHitInfo.RowHandle);
            }
            return null;
        }

        Object FindObjectToOpen(GridColumn column, int rowHandle) {
            Object result = null;
            if (column != null && GridListEditor != null && GridListEditor.GridView() != null) {
                Object currObject = XtraGridUtils.GetRow(GridListEditor.GridView(), rowHandle);
                ITypeInfo typeInfo = currObject != null
                                         ? XafTypesInfo.Instance.FindTypeInfo(currObject.GetType())
                                         : _listView.ObjectTypeInfo;
                IMemberInfo memberInfo = typeInfo.FindMember(column.FieldName);
                Object lastObject = null;
                if (GridListEditor.GridView().ActiveEditor != null) {
                    lastObject = GridListEditor.GridView().ActiveEditor.EditValue;
                } else if (currObject != null && memberInfo != null) {
                    lastObject = FindLastObject(currObject, memberInfo);
                }
                if (memberInfo != null && (IsDetailViewExists(lastObject) &&
                                           DataManipulationRight.CanRead(typeInfo.Type, memberInfo.Name, currObject,
                                                                         LinkToListViewController.FindCollectionSource(
                                                                             Controller.Frame),
                                                                         ObjectSpace))) {
                    result = lastObject;
                }
            }
            return result;
        }

        public override void OnControllerActivated() {
            base.OnControllerActivated();
            _listView = (ListView)Controller.View;
            OpenObjectAction.Active[
                DevExpress.ExpressApp.Win.SystemModule.OpenObjectController.ActiveKeyHasReadPermissionToTargetType] =
                DataManipulationRight.CanRead(_listView.ObjectTypeInfo.Type, null, null, _listView.CollectionSource,
                                              ObjectSpace);
            bool hasObjectRefControl = false;
            if (_listView.Model != null) {
                if (
                    _listView.Model.Columns.Select(
                        columnInfo => _listView.ObjectTypeInfo.FindMember(columnInfo.PropertyName)).Any(
                            findMember => findMember != null && IsDetailViewExists(findMember.MemberType))) {
                    hasObjectRefControl = true;
                }
            }
            OpenObjectAction.Active[ViewContainsObjectEditorActiveKey] = hasObjectRefControl;
            _listView.ControlsCreated += listView_ControlsCreated;
        }

        public override void OnControllerDeactivated() {
            OpenObjectAction.Active.RemoveItem(
                DevExpress.ExpressApp.Win.SystemModule.OpenObjectController.ActiveKeyHasReadPermissionToTargetType);
            OpenObjectAction.Enabled.RemoveItem(HasReadPermissionToTargetObjectEnabledKey);
            _listView.ControlsCreated -= listView_ControlsCreated;
            UnsubscribeFromGrid();
            _listView = null;
            _cursorHelper = null;
            _grid = null;
            _gridView = null;
            base.OnControllerDeactivated();
        }

        public override void UpdateOpenObjectActionState(object objectToOpen) {
            base.UpdateOpenObjectActionState(objectToOpen);
            if (objectToOpen != null) {
                OpenObjectAction.Enabled[HasReadPermissionToTargetObjectEnabledKey] =
                    DataManipulationRight.CanRead(objectToOpen.GetType(), null, objectToOpen,
                                                  LinkToListViewController.FindCollectionSource(Controller.Frame),
                                                  ObjectSpace);
            }
        }
    }


    internal sealed class ObjectToOpenChangedEventArgs : EventArgs {
        readonly Object _objectToOpen;

        public ObjectToOpenChangedEventArgs(Object objectToOpen) {
            _objectToOpen = objectToOpen;
        }

        public Object ObjectToOpen {
            get { return _objectToOpen; }
        }
    }

    internal abstract class OpenObjectImplementation {
        protected const string ViewContainsObjectEditorActiveKey = "HasObjectRefPropertyEditor";
        const string HasObjectToOpenEnabledKey = "HasObjectForOpening";
        readonly OpenObjectController _controller;

        protected OpenObjectImplementation(OpenObjectController controller) {
            _controller = controller;
        }

        protected OpenObjectController Controller {
            get { return _controller; }
        }

        protected SimpleAction OpenObjectAction {
            get { return _controller.OpenObjectAction; }
        }

        protected IObjectSpace ObjectSpace {
            get { return _controller.View.ObjectSpace; }
        }

        protected void OnObjectToOpenChanged(Object objectToOpen) {
            var e = new ObjectToOpenChangedEventArgs(objectToOpen);
            if (ObjectToOpenChanged != null) {
                ObjectToOpenChanged(this, e);
            }
        }

        protected Boolean IsDetailViewExists(Object targetObject) {
            return targetObject != null && IsDetailViewExists(targetObject.GetType());
        }

        protected Boolean IsDetailViewExists(Type targetType) {
            return !String.IsNullOrEmpty(_controller.Application.FindDetailViewId(targetType));
        }

        protected Boolean CanExecuteOpenObjectAction() {
            return OpenObjectAction.Active && OpenObjectAction.Enabled;
        }

        protected void ExecuteOpenObjectAction() {
            OpenObjectAction.DoExecute();
        }

        public virtual void OnControllerActivated() {
            OpenObjectAction.Enabled[HasObjectToOpenEnabledKey] = false;
        }

        public virtual void OnControllerDeactivated() {
        }

        public virtual void UpdateOpenObjectActionState(Object objectToOpen) {
            OpenObjectAction.Enabled[HasObjectToOpenEnabledKey] = objectToOpen != null;
        }

        public static Object FindLastObject(Object sourceObject, IMemberInfo memberInfo) {
            Object lastObject = null;
            Object currObject = sourceObject;
            IList<IMemberInfo> path = memberInfo.GetPath();
            foreach (IMemberInfo t in path) {
                if (SimpleTypes.IsSimpleType(t.MemberType)) break;
                currObject = t.GetValue(currObject);
                lastObject = currObject;
            }
            return lastObject;
        }

        public event EventHandler<ObjectToOpenChangedEventArgs> ObjectToOpenChanged;
    }


}