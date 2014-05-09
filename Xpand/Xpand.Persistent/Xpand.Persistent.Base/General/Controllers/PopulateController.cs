using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.Utils.Helpers;

namespace Xpand.Persistent.Base.General.Controllers {
    public abstract class PopulateController<T> : ViewController<ObjectView> {
        string _oldPredefinedValues;

        protected PopulateController() {
            TargetObjectType = typeof(T);
        }

        protected override void OnActivated() {
            base.OnActivated();
            View.ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
            Populate(member => {
                _oldPredefinedValues = member.PredefinedValues;
                return GetPredefinedValues(member);
            });
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            View.ObjectSpace.ObjectChanged -= ObjectSpaceOnObjectChanged;
            Populate(member => _oldPredefinedValues);
        }

        void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs) {
            if (RefreshingProperties().Contains(objectChangedEventArgs.PropertyName) ) {
                var currentPropertyEditor = GetPropertyEditor(GetPropertyName());
                var propertyEditor = Application.EditorFactory.CreatePropertyEditorByType(currentPropertyEditor.GetType(), currentPropertyEditor.Model, currentPropertyEditor.ObjectType, Application, ObjectSpace);
                Populate(GetPredefinedValues);
                propertyEditor.CreateControl();
                propertyEditor.CurrentObject = View.CurrentObject;
                View.LayoutManager.ReplaceControl(currentPropertyEditor.Id, propertyEditor.Control);
            }
        }

        protected virtual IEnumerable<string> RefreshingProperties() {
            return Enumerable.Empty<string>();
        }

        protected virtual void Populate(Func<IModelMember, string> collect) {
            var name = PropertyName;
            if (name != null) {
                var model = ((ModelApplicationBase)Application.Model);
                var lastLayer = model.LastLayer;
                ModelApplicationHelper.RemoveLayer(model);
                PopulateCore(collect, name);
                ModelApplicationHelper.AddLayer(model, lastLayer);
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

        protected string GetPropertyName(Expression<Func<T, object>> lambdaExpression) {
            var propertyInfo = lambdaExpression.GetExpression() as PropertyInfo;
            return propertyInfo != null ? propertyInfo.Name : null;
        }

        protected PropertyEditor GetPropertyEditor(Expression<Func<T, object>> expression) {
            var propertyName = GetPropertyName(expression);
            return View.GetItems<PropertyEditor>().SingleOrDefault(editor => editor.PropertyName == propertyName);
        }

        protected abstract string GetPredefinedValues(IModelMember wrapper);

        protected abstract Expression<Func<T, object>> GetPropertyName();
    }
}