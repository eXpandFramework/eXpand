using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Logic {
    public class LogicRuleEvaluator  {
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

        public Frame Frame { get; set; }

        protected virtual bool ViewIsRoot(LogicRuleInfo info) {
            return !(info.Rule.IsRootView.HasValue) || info.Rule.IsRootView == Frame.View.IsRoot;
        }


        protected virtual bool TemplateContextIsValid(LogicRuleInfo info) {
            return TemplateContextGroupIsValid(info);
        }

        LogicRuleInfo GetInfo(View view, object currentObject, ILogicRuleObject logicRule, ExecutionContext executionContext, bool invertCustomization, ActionBaseEventArgs actionBaseEventArgs) {
            if (actionBaseEventArgs != null || ExecutionContextIsValid(executionContext, logicRule)) {
                var info = CalculateLogicRuleInfo(currentObject, logicRule, actionBaseEventArgs);
                if (info != null && TemplateContextIsValid(info) && ViewIsRoot(info)) {
                    info.InvertingCustomization = invertCustomization;
                    if (info.InvertingCustomization) {
                        info.Active = !info.Active;
                    }
                    info.View = view;
                    return info;
                }
            }
            return null;
        }

        protected IEnumerable<ILogicRuleObject> GetValidModelLogicRules(View view) {
            return LogicRuleManager.Instance[view.ObjectTypeInfo].Where(rule => IsValidRule(rule, view)).OrderBy(rule => rule.Index);
        }

        public virtual IEnumerable<LogicRuleInfo> GetValidRuleInfos(View view, object currentObject, ExecutionContext executionContext, bool invertCustomization, ActionBaseEventArgs actionBaseEventArgs) {
            var modelLogicRules = GetValidModelLogicRules(view);
            var logicRuleInfos = modelLogicRules.Select(rule => GetInfo(view, currentObject, rule, executionContext, invertCustomization, actionBaseEventArgs));
            return logicRuleInfos.Where(info => info != null);
        }

        public virtual bool IsValidRule(ILogicRuleObject rule, ViewInfo viewInfo) {
            return (IsValidViewId(viewInfo.ViewId, rule)) &&
                   IsValidViewType(viewInfo, rule) && IsValidNestedType(rule, viewInfo) && IsValidTypeInfo(viewInfo, rule) 
                   && IsValidViewEditMode(viewInfo, rule);
        }

        protected virtual bool IsValidViewEditMode(ViewInfo viewInfo, ILogicRuleObject rule) {
            return !rule.ViewEditMode.HasValue || viewInfo.ViewEditMode == rule.ViewEditMode;
        }

        public virtual bool ExecutionContextIsValid(ExecutionContext executionContext, ILogicRuleObject logicRule) {
            var context = logicRule.ExecutionContext;
            return (context | executionContext) == context;
        }

        bool TemplateContextGroupIsValid(LogicRuleInfo info) {
            var frameTemplateContext = info.Rule.FrameTemplateContext;
            if (frameTemplateContext!=FrameTemplateContext.None) {
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

        bool IsValidViewId(string viewId, ILogicRuleObject rule) {
            return rule.View != null ? viewId == rule.View.Id : !rule.Views.Any() || rule.Views.Contains(viewId);
        }

        bool IsValidTypeInfo(ViewInfo viewInfo, ILogicRuleObject rule) {
            return (((rule.TypeInfo != null && rule.TypeInfo.IsAssignableFrom(viewInfo.ObjectTypeInfo)) 
                || IsValidDCTypeInfo(viewInfo, rule)) || rule.TypeInfo == null);
        }

        bool IsValidDCTypeInfo(ViewInfo viewInfo, ILogicRuleObject rule) {
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

        private bool IsValidNestedType(ILogicRuleObject rule, ViewInfo viewInfo) {
            return viewInfo.IsDetailView || (rule.Nesting == Nesting.Any || viewInfo.IsRoot);
        }

        private bool IsValidViewType(ViewInfo viewInfo, ILogicRuleObject rule) {
            return (rule.ViewType == ViewType.Any || (viewInfo.IsDetailView ? rule.ViewType == ViewType.DetailView : rule.ViewType == ViewType.ListView));
        }

        protected virtual LogicRuleInfo CalculateLogicRuleInfo(object targetObject, ILogicRuleObject modelLogicRule, ActionBaseEventArgs actionBaseEventArgs) {
            var logicRuleInfo = new LogicRuleInfo{Active = true, Object = targetObject, Rule = modelLogicRule};
            if (actionBaseEventArgs == null)
                logicRuleInfo.ExecutionContext = modelLogicRule.ExecutionContext;
            else {
                logicRuleInfo.ActionBaseEventArgs = actionBaseEventArgs;
            }
            CalculateLogicRuleInfo(logicRuleInfo);
            return logicRuleInfo;
        }

        public bool Fit(object targetObject, ILogicRuleObject logicRule) {
            var criteria = CriteriaOperator.Parse(logicRule.NormalCriteria);
            return targetObject != null? criteria.Fit(targetObject): string.IsNullOrEmpty(logicRule.EmptyCriteria) 
                || CriteriaOperator.Parse(logicRule.EmptyCriteria).Fit(new object());
        }

        void CalculateLogicRuleInfo(LogicRuleInfo calculateLogicRuleInfo) {
            calculateLogicRuleInfo.Active = Fit(calculateLogicRuleInfo.Object, calculateLogicRuleInfo.Rule);
        }
    }
}