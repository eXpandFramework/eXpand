using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraGrid;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail {
    public abstract class MasterDetailActionsController : ViewController<ListView> {
        readonly Dictionary<string, BoolList> _enabledBoolLists = new Dictionary<string, BoolList>();
        readonly Dictionary<string, BoolList> _activeBoolLists = new Dictionary<string, BoolList>();
        readonly Dictionary<DevExpress.XtraGrid.Views.Base.ColumnView, Dictionary<string, BoolList>> _activeChildBoolLists
            = new Dictionary<DevExpress.XtraGrid.Views.Base.ColumnView, Dictionary<string, BoolList>>();
        readonly Dictionary<DevExpress.XtraGrid.Views.Base.ColumnView, Dictionary<string, BoolList>> _enableChildBoolLists
            = new Dictionary<DevExpress.XtraGrid.Views.Base.ColumnView, Dictionary<string, BoolList>>();

        void SynchronizesActionStates() {
            bool _disposing = false;
            Frame.Disposing += (sender, args) => _disposing = true;
            foreach (var controller in Frame.Controllers.Values.OfType<ViewController>()) {
                foreach (var action in controller.Actions) {
                    if (!IsExcluded(action)) {
                        ActionBase action1 = action;
                        action.Enabled.Changed += (sender, args) => {
                            if (!(_disposing))
                                SyncState(action1, (BoolList)sender, simpleAction => simpleAction.Enabled, _enableChildBoolLists);
                        };
                        action.Active.Changed += (sender, args) => {
                            if (!_disposing)
                                SyncState(action1, (BoolList)sender, simpleAction => simpleAction.Active, _activeChildBoolLists);
                        };
                    }
                }
            }
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (GridListEditor != null && GridListEditor.GridView is IMasterDetailColumnView) {
                var gridView = ((IMasterDetailColumnView)GridListEditor.GridView);
                var synchronizeActions = SynchronizeActions();
                if (gridView.MasterFrame == null && HasRules) {
                    if (synchronizeActions) {
                        SynchronizesActionStates();
                        PushExecutionToNestedFrame();
                        if (HasRules && gridView.MasterFrame == null) {
                            StoreStates();
                            GridListEditor.Grid.FocusedViewChanged += MasterGridOnFocusedViewChanged;
                        }
                    }
                } else if (gridView.MasterFrame != null && synchronizeActions)
                    gridView.GridControl.FocusedViewChanged += ChildGridControlOnFocusedViewChanged;
            }

        }

        protected virtual bool SynchronizeActions() {
            return Frame.GetController<MasterDetailViewController>().SynchronizeActions();
        }

        void PushExecutionToNestedFrame() {
            foreach (var controller in Frame.Controllers.Values.OfType<ViewController>()) {
                foreach (var action in controller.Actions) {
                    if (action is SimpleAction) {
                        Controller controller1 = controller;
                        action.Executing +=
                            (sender, args) =>
                            PushExecutionToNestedFrameCore(controller1, (ActionBase)sender, () => args.Cancel = true);
                    }
                }
            }
        }

        public Dictionary<DevExpress.XtraGrid.Views.Base.ColumnView, Dictionary<string, BoolList>> ActiveChildBoolLists {
            get { return _activeChildBoolLists; }
        }

        public Dictionary<DevExpress.XtraGrid.Views.Base.ColumnView, Dictionary<string, BoolList>> EnableChildBoolLists {
            get { return _enableChildBoolLists; }
        }

        void ChildGridControlOnFocusedViewChanged(object sender, ViewFocusEventArgs viewFocusEventArgs) {
            if (Frame != null) {
                foreach (var controller in Frame.Controllers.Values.OfType<ViewController>()) {
                    foreach (var action in controller.Actions) {
                        if (viewFocusEventArgs.View != GridListEditor.GridView.GridControl.MainView && _activeChildBoolLists.Any()) {
                            var gridView = (DevExpress.XtraGrid.Views.Base.ColumnView)viewFocusEventArgs.View;
                            RestoreStates(action, action.Active, GetChildBoolList(_activeChildBoolLists, gridView));
                            RestoreStates(action, action.Enabled, GetChildBoolList(_enableChildBoolLists, gridView));
                        }
                    }
                }
            }

        }

        Dictionary<string, BoolList> GetChildBoolList(Dictionary<DevExpress.XtraGrid.Views.Base.ColumnView, Dictionary<string, BoolList>> dictionary, DevExpress.XtraGrid.Views.Base.ColumnView gridView) {
            if (!dictionary.ContainsKey(gridView))
                dictionary[gridView] = new Dictionary<string, BoolList>();
            return dictionary[gridView];
        }

        void StoreStates() {
            foreach (var controller in Frame.Controllers.Values.OfType<ViewController>()) {
                foreach (var action in controller.Actions) {
                    StoreStatesCore(action, _activeBoolLists, action.Active);
                    StoreStatesCore(action, _enabledBoolLists, action.Enabled);
                }
            }
        }

        void MasterGridOnFocusedViewChanged(object sender, ViewFocusEventArgs viewFocusEventArgs) {
            if (GridListEditor != null && GridListEditor.Grid.MainView == viewFocusEventArgs.View) {
                foreach (var controller in Frame.Controllers.Values.OfType<ViewController>()) {
                    foreach (ActionBase action in controller.Actions) {
                        RestoreStates(action, action.Active, _activeBoolLists);
                        RestoreStates(action, action.Enabled, _enabledBoolLists);
                    }
                }
            }
        }

        void RestoreStates(ActionBase action, BoolList boolList, Dictionary<string, BoolList> boolLists) {
            if (boolLists.ContainsKey(action.Id)) {
                BoolList enabledBoolList = boolLists[action.Id];
                if (enabledBoolList.GetKeys().FirstOrDefault() != null) {
                    boolList.Clear();
                    foreach (var key in enabledBoolList.GetKeys()) {
                        boolList.SetItemValue(key, enabledBoolList[key]);
                    }
                }
            }
        }

        public virtual bool HasRules {
            get {
                if (GridListEditor == null)
                    return false;
                var masterDetailViewController = Frame.GetController<MasterDetailViewController>();
                return masterDetailViewController != null && masterDetailViewController.IsMasterDetail();
            }
        }

        IColumnViewEditor GridListEditor {
            get { return View != null ? (View).Editor as IColumnViewEditor : null; }
        }

        void PushExecutionToNestedFrameCore(Controller sender, ActionBase actionBase, Action cancelAction) {
            var xpandXafGridView = GridListEditor != null ? (IMasterDetailColumnView)GridListEditor.Grid.FocusedView : null;
            if (xpandXafGridView != null && xpandXafGridView.MasterFrame != null) {
                var controller = Controller(sender, xpandXafGridView);
                if (controller != sender) {
                    cancelAction.Invoke();
                    ((SimpleAction)controller.Actions[actionBase.Id]).DoExecute();
                }
            }
        }

        Controller Controller(Controller sender, IMasterDetailColumnView xpandXafGridView) {
            return xpandXafGridView.Window.Controllers.Cast<Controller>().FirstOrDefault(controller1 => sender.GetType() == controller1.GetType());
        }

        protected virtual bool IsExcluded(ActionBase action) {
            return false;
        }

        void SyncState(ActionBase actionBase, BoolList sender, Func<ActionBase, BoolList> func, Dictionary<DevExpress.XtraGrid.Views.Base.ColumnView, Dictionary<string, BoolList>> childBoolLists) {
            if (View != null) {
                var gridControl = (View.Editor.Control as GridControl);
                if (gridControl == null)
                    return;
                var xpandXafGridView = gridControl.MainView as IMasterDetailColumnView;
                if (xpandXafGridView == null)
                    return;

                Frame masterFrame = xpandXafGridView.MasterFrame;
                if (masterFrame != null) {
                    BoolList boolList = GetBoolList(masterFrame, actionBase, func);
                    if (sender.GetKeys().FirstOrDefault() != null) {
                        var activeChildBool = new BoolList();
                        var focusedView = (DevExpress.XtraGrid.Views.Base.ColumnView)GridListEditor.Grid.FocusedView;
                        var childBoolList = GetChildBoolList(childBoolLists, focusedView);
                        childBoolList[actionBase.Id] = activeChildBool;
                        boolList.Clear();
                        foreach (var key in sender.GetKeys()) {
                            boolList.SetItemValue(key, sender[key]);
                            activeChildBool.SetItemValue(key, sender[key]);
                        }
                    }
                }
            }
        }

        void StoreStatesCore(ActionBase actionBase, Dictionary<string, BoolList> boolLists, BoolList boolList) {
            boolLists[actionBase.Id] = new BoolList();
            foreach (var key in boolList.GetKeys()) {
                boolLists[actionBase.Id].SetItemValue(key, boolList[key]);
            }
        }

        BoolList GetBoolList(Frame masterFrame, ActionBase sender, Func<ActionBase, BoolList> func) {
            foreach (var controller in masterFrame.Controllers.Values.OfType<ViewController>()) {
                foreach (var action in controller.Actions) {
                    if (action.Id == sender.Id)
                        return func.Invoke(action);
                }
            }
            throw new NotImplementedException();
        }
    }
}