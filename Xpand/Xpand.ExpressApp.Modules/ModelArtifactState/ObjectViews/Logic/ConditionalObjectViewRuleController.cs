using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Logic {
    public class ConditionalObjectViewRuleController : ViewController {
        readonly Dictionary<IObjectViewRule, IModelView> _defaultValuesRulesStorage = new Dictionary<IObjectViewRule, IModelView>();
        LogicRuleViewController _logicRuleViewController;
        IObjectViewRule _ruleForCustomProcessSelectedItem;
//        IModelView _previousModel;

        protected override void OnActivated() {
            base.OnActivated();
            if (LogicRuleManager.HasRules(View.ObjectTypeInfo)) {
                _logicRuleViewController = Frame.GetController<LogicRuleViewController>();
                _logicRuleViewController.LogicRuleExecute+=LogicRuleViewControllerOnLogicRuleExecute;
                if (View is ListView)
                    Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem += OnCustomProcessSelectedItem;
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (LogicRuleManager.HasRules(View.ObjectTypeInfo)) {
                _logicRuleViewController.LogicRuleExecute-=LogicRuleViewControllerOnLogicRuleExecute;
                if (View is ListView)
                    Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem -=OnCustomProcessSelectedItem;
                foreach (var defaultValuePair in _defaultValuesRulesStorage) {
                    defaultValuePair.Key.View = defaultValuePair.Value;
                }
            }
        }


        void LogicRuleViewControllerOnLogicRuleExecute(object sender, LogicRuleExecuteEventArgs logicRuleExecuteEventArgs) {
            LogicRuleInfo info = logicRuleExecuteEventArgs.LogicRuleInfo;
            var objectViewRule = info.Rule as IObjectViewRule;
            var executionContext = logicRuleExecuteEventArgs.ExecutionContext;
            if (objectViewRule!=null&&info.Active && !info.InvertingCustomization) {
                if (executionContext == ExecutionContext.CustomProcessSelectedItem)
                    _ruleForCustomProcessSelectedItem = objectViewRule;
                else if (executionContext == ExecutionContext.ViewOnSelectionChanged) {
                    if (CanAssignModel(info,objectViewRule)) {
                        //                        _previousModel = View.Model;
                        View.SetModel(objectViewRule.ObjectView);
                        if (!_defaultValuesRulesStorage.ContainsKey(objectViewRule))
                            _defaultValuesRulesStorage.Add(objectViewRule, objectViewRule.View);
                        objectViewRule.View = objectViewRule.ObjectView;
                    }
                } else if (executionContext == ExecutionContext.ViewShowing || info.Action == Frame.GetController<NewObjectViewController>().NewObjectAction) {
                    if (CanAssignModel(info,objectViewRule))
                        info.View.SetModel(objectViewRule.ObjectView);
                }
            }
            else if (info.InvertingCustomization) {
                if (executionContext == ExecutionContext.ViewOnSelectionChanged) {
                    //                    if (_previousModel != null && _previousModel != View.Model) {
                    View.SetModel(View.Model.AsObjectView.ModelClass.DefaultDetailView);
                    //                        info.Rule.View = View.Model.AsObjectView.ModelClass.DefaultDetailView;
                    //                    }
                }

            } else if (!info.Active && executionContext == ExecutionContext.ViewOnSelectionChanged) {
                if (View is DetailView && CanAssignModel(info,objectViewRule))
                    View.SetModel(View.Model.AsObjectView.ModelClass.DefaultDetailView);
            }

        }

        //        void ResetRules() {
//            var previousModelController = Frame.GetController<StartUpInfoController>();
//            if (previousModelController != null) {
////                _previousModel = previousModelController.PreviousModel;
//                if (previousModelController.ConditionalObjectViewRule != null)
//                    _defaultValuesRulesStorage.Add(previousModelController.ConditionalObjectViewRule, previousModelController.PreviousModel);
//            }
//        }


        void OnCustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
            if (_ruleForCustomProcessSelectedItem != null) {
                var modelDetailView = _ruleForCustomProcessSelectedItem.ObjectView as  IModelDetailView;
                if (modelDetailView != null) {
                    CreateDetailView(e, modelDetailView);
                } else if (_ruleForCustomProcessSelectedItem.ObjectView is IModelListView) {
                    CreateListView(e);
                }
                else {
                    throw new NotImplementedException();
                }
                e.Handled = true;
                _ruleForCustomProcessSelectedItem = null;
            }
        }

        void CreateListView(CustomProcessListViewSelectedItemEventArgs e) {
            var type = _ruleForCustomProcessSelectedItem.ObjectView.ModelClass.TypeInfo.Type;
            CriteriaWrapper criteriaWrapper = new LocalizedCriteriaWrapper(type, _ruleForCustomProcessSelectedItem.NormalCriteria, e.InnerArgs.CurrentObject);
            var objectSpace = Application.CreateObjectSpace();
            var listViewId = Application.FindListViewId(type);
            var collectionSource = Application.CreateCollectionSource(objectSpace, type, listViewId);
            collectionSource.Criteria["ConditionalObjectListViewFiltering"] = criteriaWrapper.CriteriaOperator;
            e.InnerArgs.ShowViewParameters.CreatedView = Application.CreateListView(listViewId, collectionSource, true);
        }

        void CreateDetailView(CustomProcessListViewSelectedItemEventArgs e, IModelDetailView modelDetailView) {
            var objectSpace = Application.CreateObjectSpace(modelDetailView.ModelClass.TypeInfo.Type);
            var o = objectSpace.GetObject(e.InnerArgs.CurrentObject);
            var startUpInfoController = MakeRulesApplicable(o, _ruleForCustomProcessSelectedItem);
            var showViewParameters = e.InnerArgs.ShowViewParameters;
            showViewParameters.Controllers.Add(startUpInfoController);
            showViewParameters.CreatedView = Application.CreateDetailView(objectSpace, modelDetailView, true, o);
        }

        StartUpInfoController MakeRulesApplicable(object o, IObjectViewRule ruleForCustomProcessSelectedItem) {
            var startUpInfoController = new StartUpInfoController(true);
            var viewId = Application.FindDetailViewId(o, View);
            var conditionalDetailViewRules = LogicRuleManager.Instance[View.ObjectTypeInfo].OfType<IObjectViewRule>().Where(rule => rule.ObjectView is IModelDetailView);
            var viewEditMode = (View is DetailView ? ((DetailView)View).ViewEditMode : (ViewEditMode?)null);
            var validModelLogicRules = conditionalDetailViewRules.Where(rule => _logicRuleViewController.Evaluator.IsValidRule(rule, new LogicRuleEvaluator.ViewInfo(viewId, true, true, View.ObjectTypeInfo, viewEditMode)));
            foreach (var validModelLogicRule in validModelLogicRules) {
                startUpInfoController.PreviousModel = validModelLogicRule.View;
                startUpInfoController.ConditionalObjectViewRule = validModelLogicRule;
                validModelLogicRule.View = ruleForCustomProcessSelectedItem.ObjectView;
            }
            return startUpInfoController;
        }

        public class StartUpInfoController : ViewController {
            public StartUpInfoController()
                : this(false) {
            }

            public StartUpInfoController(bool appropriateContext) {
                Active["AppropriateContext"] = appropriateContext;
            }

            public IModelView PreviousModel { get; set; }
            public IObjectViewRule ConditionalObjectViewRule { get; set; }
        }

        bool CanAssignModel(LogicRuleInfo info,IObjectViewRule rule) {
            return info.Active
                       ? info.View.Model != rule.ObjectView &&
                         info.View.Model.GetType() == rule.ObjectView.GetType()
                       : info.View.Model.GetType() == rule.ObjectView.GetType();
        }
    }
}