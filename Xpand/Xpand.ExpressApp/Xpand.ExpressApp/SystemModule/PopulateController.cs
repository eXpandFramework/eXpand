using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.SystemModule {
    public abstract class PopulateController<T> : ViewController<ObjectView> {
        private IModelMember modelMember;

        protected PopulateController() {
            TargetObjectType = typeof(T);
        }
        protected override void OnActivated() {
            base.OnActivated();
            Populate();
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (modelMember != null) {
                modelMember.PredefinedValues = string.Empty;
            }
        }
        protected virtual void Populate() {
            var lambdaExpression = GetPropertyName();
            var propertyInfo = ReflectionExtensions.GetExpression(lambdaExpression) as PropertyInfo;
            if (propertyInfo != null)
                modelMember = (View.Model.ModelClass.AllMembers.Where(wrapper => wrapper.Name == propertyInfo.Name)).FirstOrDefault();
            if (modelMember != null) {
                modelMember.PredefinedValues = GetPredefinedValues(modelMember);
            }
        }

        protected abstract string GetPredefinedValues(IModelMember wrapper);

        protected abstract Expression<Func<T, object>> GetPropertyName();
    }
}