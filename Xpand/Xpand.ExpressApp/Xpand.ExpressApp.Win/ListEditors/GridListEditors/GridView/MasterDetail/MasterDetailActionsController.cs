using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraGrid;
using Fasterflect;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail {
    public abstract class MasterDetailActionsController : ViewController<ListView> {
        readonly Dictionary<string, BoolList> _enabledBoolLists = new Dictionary<string, BoolList>();
        readonly Dictionary<string, BoolList> _activeBoolLists = new Dictionary<string, BoolList>();
        bool _disposing;

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.Disposing += FrameOnDisposing;
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.Disposing -= FrameOnDisposing;
            _disposing = true;
        }

        void SubscribeToActionStateResultChange(ActionBase actionBase) {
            if (!IsExcluded(actionBase)) {
                actionBase.Enabled.ResultValueChanged += (sender, args) => {
                    if (CanClone())
                        CloneBoolList(actionBase.Id, actionBase.Enabled, _enabledBoolLists);
                };
                actionBase.Active.ResultValueChanged += (sender, args) => {
                    if (CanClone())
                        CloneBoolList(actionBase.Id, actionBase.Active, _activeBoolLists);
                };                
            }
        }

        bool CanClone() {
            return !_disposing && GridListEditor != null && GridListEditor.Grid != null && GridListEditor.Grid.FocusedView!=null && ((IMasterDetailColumnView)GridListEditor.Grid.FocusedView).Window == null;
        }

        protected virtual IEnumerable<ActionBase> GetActions(Frame frame){
            return frame.Actions();
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            _activeBoolLists.Clear();
            _enabledBoolLists.Clear();
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (GridListEditor != null && GridListEditor.ColumnView is IMasterDetailColumnView) {
                var gridView = ((IMasterDetailColumnView)GridListEditor.ColumnView);
                if (gridView.MasterFrame == null && HasRules && SynchronizeActions()) {
                    foreach (var action in GetActions(Frame)) {
                        SubscribeToActionStateResultChange(action);
                        PushExecutionToNestedFrame(action);
                    }
                    if (gridView.MasterFrame == null) {
                        CloneActionState(Frame, _activeBoolLists, _enabledBoolLists);
                        gridView.GridControl.FocusedViewChanged += OnFocusedViewChanged;
                    }
                }
            }

        }

        protected virtual bool SynchronizeActions() {
            return Frame.GetController<MasterDetailViewController>().FilterRules(View.CurrentObject, Frame).Any();
        }

        void OnFocusedViewChanged(object sender, ViewFocusEventArgs e) {
            if (GridListEditor != null) {
                Frame frame = Frame;
                var activeBoolLists = new Dictionary<string, BoolList>();
                var enableBoolLists = new Dictionary<string, BoolList>();
                if (GridListEditor.Grid.MainView != e.View) {
                    frame = ((IMasterDetailColumnView)GridListEditor.Grid.FocusedView).Window;
                }
                CloneActionState(frame, activeBoolLists, enableBoolLists);
                if (GridListEditor.Grid.MainView == e.View) {
                    activeBoolLists = _activeBoolLists;
                    enableBoolLists = _enabledBoolLists;
                }
                foreach (var action in GetActions(Frame)) {
                    action.CallMethod("UpdateState");
                    SyncStates(action.Id, action.Active, activeBoolLists);
                    SyncStates(action.Id, action.Enabled, enableBoolLists);
                }
            }
        }

        void SyncStates(string id, BoolList boolList, Dictionary<string, BoolList> boolLists) {
            if (boolLists.ContainsKey(id)) {
                var list = boolLists[id];
                boolList.BeginUpdate();
                if (list.GetKeys().FirstOrDefault() != null) {
                    boolList.Clear();
                    foreach (var key in list.GetKeys()) {
                        boolList.SetItemValue(key, list[key]);
                    }
                }
                boolList.EndUpdate();
            }
        }

        void CloneActionState(Frame frame, Dictionary<string, BoolList> active, Dictionary<string, BoolList> enable) {
            foreach (var action in GetActions(frame)) {
                CloneBoolList(action.Id, action.Active, active);
                CloneBoolList(action.Id, action.Enabled, enable);
            }
        }

        void CloneBoolList(string id, BoolList boolList, Dictionary<string, BoolList> boolLists) {
            boolLists[id] = new BoolList();
            boolLists[id].BeginUpdate();
            foreach (var key in boolList.GetKeys()) {
                boolLists[id].SetItemValue(key, boolList[key]);
            }
            boolLists[id].EndUpdate();
        }

        void PushExecutionToNestedFrame(ActionBase actionBase) {
            actionBase.Executing +=(sender, args) =>PushExecutionToNestedFrameCore((ActionBase)sender, () => args.Cancel = true);
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

        void PushExecutionToNestedFrameCore(ActionBase action, Action cancelAction) {
            var xpandXafGridView = GridListEditor != null ? (IMasterDetailColumnView)GridListEditor.Grid.FocusedView : null;
            if (xpandXafGridView != null && xpandXafGridView.MasterFrame != null) {
                var controller = Controller(action.Controller, xpandXafGridView);
                if (controller != action.Controller) {
                    cancelAction.Invoke();
                    (controller.Actions[action.Id]).DoExecute();
                }
            }
        }

        Controller Controller(Controller sender, IMasterDetailColumnView xpandXafGridView) {
            return xpandXafGridView.Window.Controllers.Cast<Controller>().FirstOrDefault(controller1 => sender.GetType() == controller1.GetType());
        }

        protected virtual bool IsExcluded(ActionBase action) {
            return false;
        }
    }
}