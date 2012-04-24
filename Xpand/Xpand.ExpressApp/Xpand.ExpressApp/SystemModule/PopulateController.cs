using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.SystemModule {
    public abstract class PopulateController<T> : ViewController<ObjectView> {
        string _oldPredefinedValues;

        protected PopulateController() {
            TargetObjectType = typeof(T);
        }
        protected override void OnActivated() {
            base.OnActivated();
            Populate(member => {
                _oldPredefinedValues = member.PredefinedValues;
                return GetPredefinedValues(member);
            });
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            Populate(member => _oldPredefinedValues);
        }
        protected virtual void Populate(Func<IModelMember, string> collect) {
            var name = PropertyName;
            if (name != null) {
                var model = ((ModelApplicationBase)Application.Model);
                var lastLayer = model.LastLayer;
                ModelApplicationHelper.RemoveLayer(lastLayer);
                PopulateCore(collect, name);
                ModelApplicationHelper.AddLayer(lastLayer,model );
            }
        }

        private void PopulateCore(Func<IModelMember, string> collect, string propertyName) {
            IModelMember modelMember = View.Model.ModelClass.AllMembers.FirstOrDefault(member => member.Name == propertyName);
            if (modelMember != null) {
                modelMember.PredefinedValues = collect.Invoke(modelMember);
            }
        }

        protected string PropertyName {
            get {
                var lambdaExpression = GetPropertyName();
                return GetPropertyName(lambdaExpression);
            }
        }

        string GetPropertyName(Expression<Func<T, object>> lambdaExpression) {
            var propertyInfo = ReflectionExtensions.GetExpression(lambdaExpression) as PropertyInfo;
            return propertyInfo != null ? propertyInfo.Name : null;
        }

        protected PropertyEditor GetPropertyEditor(Expression<Func<T, object>> expression) {
            var propertyName = GetPropertyName(expression);
            return View.GetItems<PropertyEditor>().Where(editor => editor.PropertyName == propertyName).SingleOrDefault();
        }

        protected abstract string GetPredefinedValues(IModelMember wrapper);

        protected abstract Expression<Func<T, object>> GetPropertyName();
    }
}