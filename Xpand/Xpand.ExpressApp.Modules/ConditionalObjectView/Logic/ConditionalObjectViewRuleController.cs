using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.ConditionalObjectView.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Conditional.Logic;
using Xpand.ExpressApp.Logic.Model;

namespace Xpand.ExpressApp.ConditionalObjectView.Logic {
    public class ConditionalObjectViewRuleController : ConditionalLogicRuleViewController<IConditionalObjectViewRule> {
        IConditionalObjectViewRule _ruleForCustomProcessSelectedItem;
        IModelView _previousModel;

        protected override void OnActivated() {
            base.OnActivated();
            if (IsReady) {
                if (View is XpandListView)
                    Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem += OnCustomProcessSelectedItem;
                else {
                    ResetRules();
                }
            }
        }

        void ResetRules() {
            var previousModelController = Frame.GetController<StartUpInfoController>();
            if (previousModelController != null) {
                _previousModel = previousModelController.PreviousModel;
                if (previousModelController.ConditionalObjectViewRule != null)
                    defaultValuesRulesStorage.Add(previousModelController.ConditionalObjectViewRule, previousModelController.PreviousModel);
            }
        }


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

        StartUpInfoController MakeRulesApplicable(object o, IConditionalObjectViewRule ruleForCustomProcessSelectedItem) {
            var startUpInfoController = new StartUpInfoController(true);
            var viewId = Application.FindDetailViewId(o, View);
            var conditionalDetailViewRules = LogicRuleManager<IConditionalObjectViewRule>.Instance[View.ObjectTypeInfo].Where(rule => rule.ObjectView is IModelDetailView);
            var viewEditMode = (View is DetailView ? ((DetailView)View).ViewEditMode : (ViewEditMode?)null);
            var validModelLogicRules = conditionalDetailViewRules.Where(rule => IsValidRule(rule, new ViewInfo(viewId, true, true, View.ObjectTypeInfo, viewEditMode)));
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
            public IConditionalObjectViewRule ConditionalObjectViewRule { get; set; }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (View is XpandListView)
                Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem -= OnCustomProcessSelectedItem;
            foreach (var defaultValuePair in defaultValuesRulesStorage) {
                defaultValuePair.Key.View = defaultValuePair.Value;
            }
        }
        protected override IModelLogic GetModelLogic() {
            return ((IModelApplicationConditionalObjectView)Application.Model).ConditionalObjectView;
        }

        readonly Dictionary<IConditionalObjectViewRule, IModelView> defaultValuesRulesStorage = new Dictionary<IConditionalObjectViewRule, IModelView>();

        public override void ExecuteRule(LogicRuleInfo<IConditionalObjectViewRule> info, ExecutionContext executionContext) {
            if (info.Active && !info.InvertingCustomization) {
                if (executionContext == ExecutionContext.CustomProcessSelectedItem)
                    _ruleForCustomProcessSelectedItem = info.Rule;
                else if (executionContext == ExecutionContext.CurrentObjectChanged) {
                    _previousModel = View.Model;
                    View.SetModel(info.Rule.ObjectView);
                    if (!defaultValuesRulesStorage.ContainsKey(info.Rule))
                        defaultValuesRulesStorage.Add(info.Rule, info.Rule.View);
                    info.Rule.View = info.Rule.ObjectView;
                } else if (executionContext == ExecutionContext.ViewShowing || info.Action == Frame.GetController<NewObjectViewController>().NewObjectAction) {
                    info.View.SetModel(info.Rule.ObjectView);
                }
            } else if (info.InvertingCustomization) {
                if (executionContext == ExecutionContext.CurrentObjectChanged) {
                    if (_previousModel != null && _previousModel != View.Model) {
                        View.SetModel(_previousModel);
                        info.Rule.View = _previousModel;
                    }
                }
            }
        }

    }
}