using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Logic {
    public class LogicRuleEvaluator {
        Frame _frame;

        internal bool IsNew { get; set; }

        public class ViewInfo {
            public ViewInfo(string viewId, bool isDetailView, bool isRoot, ITypeInfo objectTypeInfo, ViewEditMode? viewEditMode) {
                ViewId = viewId;
                IsDetailView = isDetailView;
                IsRoot = isRoot;
                ObjectTypeInfo = objectTypeInfo;
                ViewEditMode = viewEditMode;
            }

            public string ViewId { get; set; }
            public bool IsDetailView { get; set; }
            public bool IsRoot { get; set; }
            public ITypeInfo ObjectTypeInfo { get; set; }
            public ViewEditMode? ViewEditMode { get; set; }
        }

        public Frame Frame {
            get { return _frame; }
            set {
                if (value!=null)
                    value.ViewChanging +=ValueOnViewChanging;
                else {
                    _frame.ViewChanging -= ValueOnViewChanging;
                }
                _frame = value;
            }
        }

        void ValueOnViewChanging(object sender, ViewChangingEventArgs viewChangingEventArgs) {
            IsNew = viewChangingEventArgs.View.ObjectSpace.IsNewObject(viewChangingEventArgs.View.CurrentObject);
            viewChangingEventArgs.View.CurrentObjectChanged -= ViewOnCurrentObjectChanged;
            viewChangingEventArgs.View.CurrentObjectChanged += ViewOnCurrentObjectChanged;
        }

        void ViewOnCurrentObjectChanged(object sender, EventArgs eventArgs) {
            IsNew = _frame.View.ObjectSpace.IsNewObject(_frame.View.CurrentObject);
        }

        protected virtual bool ViewIsRoot(ILogicRuleObject ruleObject) {
            return !(ruleObject.IsRootView.HasValue) || ruleObject.IsRootView == Frame.View.IsRoot;
        }

        public virtual IEnumerable<ILogicRuleObject> GetValidRules(View view,  ExecutionContext executionContext) {
            if (view!=null) {
                var tuple = new Tuple<ITypeInfo, ExecutionContext>(view.ObjectTypeInfo, executionContext);
                return LogicRuleManager.Instance[tuple].Where(rule => IsValidRule(rule, view)).OrderBy(rule => rule.Index);
            }
            return Enumerable.Empty<ILogicRuleObject>();
        }

        protected virtual bool IsValidRule(ILogicRuleObject rule, ViewInfo viewInfo) {
            return (IsValidViewId(viewInfo.ViewId, rule)) &&IsValidNewObject(rule)&&
                   IsValidViewType(viewInfo, rule) && IsValidNestedType(rule, viewInfo) && IsValidTypeInfo(viewInfo, rule)
                   && IsValidViewEditMode(viewInfo, rule) && TemplateContextGroupIsValid(rule) && ViewIsRoot(rule);
        }

        bool IsValidNewObject(ILogicRuleObject rule) {
            return !rule.IsNew.HasValue || (rule.IsNew.Value ? IsNew : !IsNew);
        }

        protected virtual bool IsValidViewEditMode(ViewInfo viewInfo, ILogicRuleObject rule) {
            return !rule.ViewEditMode.HasValue || viewInfo.ViewEditMode == rule.ViewEditMode;
        }

        protected virtual bool TemplateContextGroupIsValid(ILogicRuleObject ruleObject) {
            var frameTemplateContext = ruleObject.FrameTemplateContext;
            if (frameTemplateContext!=FrameTemplateContext.All) {
                var templateContextStringValue = Frame.Context.ToString().TrimEnd("Context".ToCharArray());
                var templateContext =(FrameTemplateContext) Enum.Parse(typeof (FrameTemplateContext), templateContextStringValue);
                return (frameTemplateContext | templateContext) == frameTemplateContext;
            }
            return true;
        }

        protected virtual bool IsValidRule(ILogicRuleObject rule, View view) {
            var viewEditMode = view is DetailView ? ((DetailView)view).ViewEditMode : (ViewEditMode?)null;
            return view != null && IsValidRule(rule, new ViewInfo(view.Id, view is DetailView, view.IsRoot, view.ObjectTypeInfo, viewEditMode));
        }

        protected virtual bool IsValidViewId(string viewId, ILogicRuleObject rule) {
            return rule.View != null ? viewId == rule.View.Id : !rule.Views.Any() || rule.Views.Contains(viewId);
        }

        protected virtual bool IsValidTypeInfo(ViewInfo viewInfo, ILogicRuleObject rule) {
            return (((rule.TypeInfo != null && rule.TypeInfo.IsAssignableFrom(viewInfo.ObjectTypeInfo)) 
                || IsValidDCTypeInfo(viewInfo, rule)) || rule.TypeInfo == null);
        }

        protected virtual bool IsValidDCTypeInfo(ViewInfo viewInfo, ILogicRuleObject rule) {
            if (viewInfo.ObjectTypeInfo.IsDomainComponent) {
                var entityType = XpoTypesInfoHelper.GetXpoTypeInfoSource().GetGeneratedEntityType(viewInfo.ObjectTypeInfo.Type);
                var types = new List<Type> { entityType };
                while (entityType != null && entityType != typeof(object)) {
                    entityType = entityType.BaseType;
                    types.Add(entityType);
                }
                return types.Contains(rule.TypeInfo.Type);
            }
            return true;
        }

        protected virtual bool IsValidNestedType(ILogicRuleObject rule, ViewInfo viewInfo) {
            return viewInfo.IsDetailView || (rule.Nesting == Nesting.Any || viewInfo.IsRoot);
        }

        protected virtual bool IsValidViewType(ViewInfo viewInfo, ILogicRuleObject rule) {
            return (rule.ViewType == ViewType.Any || (viewInfo.IsDetailView ? rule.ViewType == ViewType.DetailView : rule.ViewType == ViewType.ListView));
        }

        public virtual bool Fit(object targetObject, ILogicRuleObject logicRule) {
            var criteria = CriteriaOperator.Parse(logicRule.NormalCriteria);
            return targetObject != null ? criteria.Fit(targetObject) : string.IsNullOrEmpty(logicRule.EmptyCriteria) || CriteriaOperator.Parse(logicRule.EmptyCriteria).Fit(new object());
        }

    }
}