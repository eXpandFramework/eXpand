using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Logic.Security {
    public class PopulateExecutionContextsController:PopulateController<IContextLogicRule> {
        string _predefinedValues;

        protected override void OnActivated() {
            var modelLogicWrapper = LogicInstallerManager.Instance[(IContextLogicRule)View.CurrentObject].GetModelLogic();
            _predefinedValues = string.Join(";", modelLogicWrapper.ExecutionContextsGroup.Select(contexts => contexts.Id));
            base.OnActivated();
        }
        protected override string GetPredefinedValues(IModelMember wrapper) {
            return _predefinedValues;
        }

        protected override Expression<Func<IContextLogicRule, object>> GetPropertyName() {
            return rule => rule.ExecutionContextGroup;
        }
    }
    public class PopulateActionContextsController:PopulateController<IContextLogicRule> {
        string _predefinedValues;

        protected override void OnActivated() {
            var modelLogicWrapper = LogicInstallerManager.Instance[(IContextLogicRule)View.CurrentObject].GetModelLogic();
            _predefinedValues = string.Join(";", modelLogicWrapper.ActionExecutionContextGroup.Select(contexts => contexts.Id));
            base.OnActivated();
        }
        protected override string GetPredefinedValues(IModelMember wrapper) {
            return _predefinedValues;
        }

        protected override Expression<Func<IContextLogicRule, object>> GetPropertyName() {
            return rule => rule.ActionExecutionContextGroup;
        }
    }
    public class PopulateFrameContextsController:PopulateController<IContextLogicRule> {
        string _predefinedValues;

        protected override void OnActivated() {
            var modelLogicWrapper = LogicInstallerManager.Instance[(IContextLogicRule)View.CurrentObject].GetModelLogic();
            _predefinedValues = string.Join(";", modelLogicWrapper.FrameTemplateContextsGroup.Select(contexts => contexts.Id));
            base.OnActivated();
        }
        protected override string GetPredefinedValues(IModelMember wrapper) {
            return _predefinedValues;
        }

        protected override Expression<Func<IContextLogicRule, object>> GetPropertyName() {
            return rule => rule.FrameTemplateContextGroup;
        }
    }
    public class PopulateViewContextsController:PopulateController<IContextLogicRule> {
        string _predefinedValues;

        protected override void OnActivated() {
            var modelLogicWrapper = LogicInstallerManager.Instance[(IContextLogicRule)View.CurrentObject].GetModelLogic();
            _predefinedValues = string.Join(";", modelLogicWrapper.ViewContextsGroup.Select(contexts => contexts.Id));
            base.OnActivated();
        }
        protected override string GetPredefinedValues(IModelMember wrapper) {
            return _predefinedValues;
        }

        protected override Expression<Func<IContextLogicRule, object>> GetPropertyName() {
            return rule => rule.ViewContextGroup;
        }
    }
    public class PopulateViewsController : PopulateController<IContextLogicRule> {

        protected override System.Collections.Generic.IEnumerable<string> RefreshingProperties() {
            var propertyName = ((IContextLogicRule) View.CurrentObject).GetPropertyName(rule => rule.TypeInfo);
            return new []{propertyName};
        }
        protected override string GetPredefinedValues(IModelMember wrapper) {
            var typeInfo = ((IContextLogicRule) View.CurrentObject).TypeInfo;
            if (typeInfo!=null) {
                var objectViews = Application.Model.Views.OfType<IModelObjectView>().Where(modelView => modelView.ModelClass.TypeInfo == typeInfo);
                return objectViews.Aggregate("", (current, view) => current + (view.Id + ";")).TrimEnd(';');
            }
            return ";";
        }

        protected override Expression<Func<IContextLogicRule, object>> GetPropertyName() {
            return x => x.View;
        }
    }
}