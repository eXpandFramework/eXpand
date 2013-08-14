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
using Xpand.Persistent.Base.Logic.Model;

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

        public virtual bool ExecutionContextIsValid(ExecutionContext executionContext, LogicRule logicRule) {
            var context = logicRule.ExecutionContext;
            return (context | executionContext) == context;
        }

        protected virtual bool TemplateContextIsValid(LogicRuleInfo info) {
            var frameTemplateContext = info.Rule.FrameTemplateContext;
            return frameTemplateContext != FrameTemplateContext.All 
                ? frameTemplateContext + "Context" == Frame.Context : TemplateContextGroupIsValid(info);
        }

        LogicRuleInfo GetInfo(View view, object currentObject, LogicRule logicRule, ExecutionContext executionContext, bool invertCustomization, ActionBaseEventArgs actionBaseEventArgs) {
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

        protected IEnumerable<LogicRule> GetValidModelLogicRules(View view) {
            return LogicRuleManager.Instance[view.ObjectTypeInfo].Where(rule => IsValidRule(rule, view)).OrderBy(rule => rule.Index);
        }

        public virtual IEnumerable<LogicRuleInfo> GetContextValidLogicRuleInfos(View view, object currentObject, ExecutionContext executionContext, bool invertCustomization, ActionBaseEventArgs actionBaseEventArgs) {
            var modelLogicRules = GetValidModelLogicRules(view);
            var logicRuleInfos = modelLogicRules.Select(rule => GetInfo(view, currentObject, rule, executionContext, invertCustomization, actionBaseEventArgs));
            return logicRuleInfos.Where(info => info != null);
        }

        public virtual bool IsValidRule(LogicRule rule, ViewInfo viewInfo) {
            return (IsValidViewId(viewInfo.ViewId, rule)) &&
                   IsValidViewType(viewInfo, rule) && IsValidNestedType(rule, viewInfo) && IsValidTypeInfo(viewInfo, rule) 
                   && IsValidViewEditMode(viewInfo, rule);
        }

        protected virtual bool IsValidViewEditMode(ViewInfo viewInfo, LogicRule rule) {
            return !rule.ViewEditMode.HasValue || viewInfo.ViewEditMode == rule.ViewEditMode;
        }

        bool TemplateContextGroupIsValid(LogicRuleInfo info) {
            var frameTemplateContextsGroup = GetModelLogic(info.Rule).FrameTemplateContextsGroup;
            var modelFrameTemplateContextsGroup = frameTemplateContextsGroup.FirstOrDefault(templateContexts 
                => templateContexts.Id == info.Rule.FrameTemplateContextGroup);
            return modelFrameTemplateContextsGroup == null || modelFrameTemplateContextsGroup.FirstOrDefault(context 
                => context.Name + "Context" == Frame.Context) != null;
        }

        protected virtual bool IsValidRule(LogicRule rule, View view) {
            var viewEditMode = view is DetailView ? ((DetailView)view).ViewEditMode : (ViewEditMode?)null;
            return view != null && IsValidRule(rule, new ViewInfo(view.Id, view is DetailView, view.IsRoot, view.ObjectTypeInfo, viewEditMode));
        }

        bool IsValidViewId(string viewId, LogicRule rule) {
            if (rule.View != null)
                return viewId == rule.View.Id;
            var viewContextsGroup = GetModelLogic(rule).ViewContextsGroup;
            var modelViewContexts = viewContextsGroup.FirstOrDefault(contexts => contexts.Id == rule.ViewContextGroup);
            return modelViewContexts == null ||modelViewContexts.FirstOrDefault(context => context.Name == viewId) != null;
        }

        bool IsValidTypeInfo(ViewInfo viewInfo, LogicRule rule) {
            return (((rule.TypeInfo != null && rule.TypeInfo.IsAssignableFrom(viewInfo.ObjectTypeInfo)) 
                || IsValidDCTypeInfo(viewInfo, rule)) || rule.TypeInfo == null);
        }

        bool IsValidDCTypeInfo(ViewInfo viewInfo, LogicRule rule) {
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

        private bool IsValidNestedType(LogicRule rule, ViewInfo viewInfo) {
            return viewInfo.IsDetailView || (rule.Nesting == Nesting.Any || viewInfo.IsRoot);
        }

        private bool IsValidViewType(ViewInfo viewInfo, LogicRule rule) {
            return (rule.ViewType == ViewType.Any || (viewInfo.IsDetailView ? rule.ViewType == ViewType.DetailView : rule.ViewType == ViewType.ListView));
        }

        protected virtual LogicRuleInfo CalculateLogicRuleInfo(object targetObject, LogicRule modelLogicRule, ActionBaseEventArgs actionBaseEventArgs) {
            var logicRuleInfo = new LogicRuleInfo{Active = true, Object = targetObject, Rule = modelLogicRule};
            if (actionBaseEventArgs == null)
                logicRuleInfo.ExecutionContext = modelLogicRule.ExecutionContext;
            else {
                logicRuleInfo.ActionBaseEventArgs = actionBaseEventArgs;
            }
            CalculateLogicRuleInfo(logicRuleInfo);
            return logicRuleInfo;
        }

        public bool Fit(object targetObject, LogicRule logicRule) {
            var criteria = CriteriaOperator.Parse(logicRule.NormalCriteria);
            return targetObject != null? criteria.Fit(targetObject): string.IsNullOrEmpty(logicRule.EmptyCriteria) 
                || CriteriaOperator.Parse(logicRule.EmptyCriteria).Fit(new object());
        }

        void CalculateLogicRuleInfo(LogicRuleInfo calculateLogicRuleInfo) {
            calculateLogicRuleInfo.Active = Fit(calculateLogicRuleInfo.Object, calculateLogicRuleInfo.Rule);
        }

        IModelLogic GetModelLogic(LogicRule logicRule) {
            var typesInfo = Frame.Application.TypesInfo;
            return ModelLogics.Select(logic 
                => new { logic, Info = typesInfo.FindTypeInfo(logic.GetType()) })
                .Select(arg => new{arg.logic,Attribute = arg.Info.ImplementedInterfaces.Select(info 
                    => info.FindAttribute<ModelLogicValidRuleAttribute>()).Single(attribute => attribute!=null)})
                .Where(arg => arg.Attribute.RuleType.IsInstanceOfType(logicRule))
                .Select(arg => arg.logic).Single();
        }

        IEnumerable<IModelLogic> ModelLogics {
            get { return LogicInstallerManager.Instance.LogicInstallers.Select(installer => installer.GetModelLogic()); }
        }
    }
}