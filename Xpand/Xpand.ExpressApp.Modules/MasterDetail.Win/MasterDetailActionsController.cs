using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.ExpressApp.Win.ListEditors;

namespace Xpand.ExpressApp.MasterDetail.Win {
    public class MasterDetailActionsController : ViewController<ListView> {
        readonly Dictionary<string, BoolList> _enabledBoolLists = new Dictionary<string, BoolList>();
        readonly Dictionary<string, BoolList> _activeBoolLists = new Dictionary<string, BoolList>();
        readonly Dictionary<GridView, Dictionary<string, BoolList>> _activeChildBoolLists = new Dictionary<GridView, Dictionary<string, BoolList>>();
        readonly Dictionary<GridView, Dictionary<string, BoolList>> _enableChildBoolLists = new Dictionary<GridView, Dictionary<string, BoolList>>();
        protected override void OnActivated() {
            base.OnActivated();
            if (IsMasterDetail) {
                foreach (var controller in Frame.Controllers.Values.OfType<ViewController>()) {
                    foreach (var action in controller.Actions) {
                        if (action is SimpleAction) {
                            Controller controller1 = controller;
                            action.Executing += (sender, args) => PushExecutionToNestedFrame(controller1, (ActionBase)sender, () => args.Cancel = true);
                        }
                    }
                }
            }
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (IsMasterDetail) {
                if (GridListEditor.GridView.MasterFrame == null) {
                    StoreStates();
                    GridListEditor.Grid.FocusedViewChanged += MasterGridOnFocusedViewChanged;
                } else {
                    GridListEditor.GridView.GridControl.FocusedViewChanged += ChildGridControlOnFocusedViewChanged;

                }
            }
        }



        public Dictionary<GridView, Dictionary<string, BoolList>> ActiveChildBoolLists {
            get { return _activeChildBoolLists; }
        }

        public Dictionary<GridView, Dictionary<string, BoolList>> EnableChildBoolLists {
            get { return _enableChildBoolLists; }
        }

        void ChildGridControlOnFocusedViewChanged(object sender, ViewFocusEventArgs viewFocusEventArgs) {
            if (Frame != null) {
                foreach (var controller in Frame.Controllers.Values.OfType<ViewController>()) {
                    foreach (var action in controller.Actions) {
                        if (viewFocusEventArgs.View != GridListEditor.GridView.GridControl.MainView && _activeChildBoolLists.Count() > 0) {
                            var gridView = (GridView)viewFocusEventArgs.View;
                            RestoreStates(action, action.Active, GetChildBoolList(_activeChildBoolLists, gridView));
                            RestoreStates(action, action.Enabled, GetChildBoolList(_enableChildBoolLists, gridView));
                        }
                    }
                }
            }

        }

        Dictionary<string, BoolList> GetChildBoolList(Dictionary<GridView, Dictionary<string, BoolList>> dictionary, GridView gridView) {
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

        public virtual bool IsMasterDetail {
            get {
                return false;
                return GridListEditor != null &&
                       Frame.GetController<MasterDetailRuleController>().MasterDetailRules.FirstOrDefault() != null;
            }
        }

        XpandGridListEditor GridListEditor {
            get { return View != null ? (View).Editor as XpandGridListEditor : null; }
        }

        void PushExecutionToNestedFrame(Controller sender, ActionBase actionBase, Action cancelAction) {
            var xpandXafGridView = GridListEditor != null ? (XpandXafGridView)GridListEditor.Grid.FocusedView : null;
            if (xpandXafGridView != null && xpandXafGridView.MasterFrame != null) {
                Controller controller = xpandXafGridView.Window.GetController(sender.GetType());
                if (controller != sender) {
                    cancelAction.Invoke();
                    ((SimpleAction)controller.Actions[actionBase.Id]).DoExecute();
                }
            }
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            bool _disposing = false;
            Frame.Disposing += (sender, args) => _disposing = true;
            foreach (var controller in Frame.Controllers.Values.OfType<ViewController>()) {
                //                if (controller is ViewController) {
                foreach (var action in controller.Actions) {
                    if (!IsExcluded(action)) {
                        ActionBase action1 = action;
                        action.Enabled.Changed += (sender, args) => {
                            if (!(_disposing))
                                SyncState(action1, (BoolList)sender, simpleAction => simpleAction.Enabled,
                                          _enableChildBoolLists);
                        };
                        action.Active.Changed += (sender, args) => {
                            if (!_disposing)
                                SyncState(action1, (BoolList)sender, simpleAction => simpleAction.Active,
                                          _activeChildBoolLists);
                        };
                    }
                }
                //                }
            }
        }

        protected virtual bool IsExcluded(ActionBase action) {
            return false;
        }

        void SyncState(ActionBase actionBase, BoolList sender, Func<ActionBase, BoolList> func, Dictionary<GridView, Dictionary<string, BoolList>> childBoolLists) {
            if (View == null || !IsMasterDetail)
                return;
            var gridControl = (View.Editor.Control as GridControl);
            if (gridControl == null)
                return;
            var xpandXafGridView = gridControl.MainView as XpandXafGridView;
            if (xpandXafGridView == null)
                return;

            Frame masterFrame = xpandXafGridView.MasterFrame;
            if (masterFrame != null) {
                BoolList boolList = GetBoolList(masterFrame, actionBase, func);
                if (sender.GetKeys().FirstOrDefault() != null) {
                    var activeChildBool = new BoolList();
                    var focusedView = (GridView)GridListEditor.Grid.FocusedView;
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

        static void StoreStatesCore(ActionBase actionBase, Dictionary<string, BoolList> boolLists, BoolList boolList) {
            boolLists[actionBase.Id] = new BoolList();
            foreach (var key in boolList.GetKeys()) {
                boolLists[actionBase.Id].SetItemValue(key, boolList[key]);
            }
        }

        static BoolList GetBoolList(Frame masterFrame, ActionBase sender, Func<ActionBase, BoolList> func) {
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
