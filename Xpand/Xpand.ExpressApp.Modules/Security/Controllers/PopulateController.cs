using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Security.Controllers
{
    public abstract partial class PopulateController<T> : ViewController
    {
        private IModelMember modelMember;

        protected PopulateController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (T);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            populate();
        }
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            if (modelMember != null){
                modelMember.PredefinedValues = string.Empty;
            }
        }
        protected virtual void populate()
        {
            LambdaExpression lambdaExpression = GetPropertyName();
            var propertyInfo = ReflectionExtensions.GetExpression(lambdaExpression) as PropertyInfo;
            if (propertyInfo != null)
                modelMember =
                    (View.Model.ModelClass.AllMembers.Where(
                        wrapper =>
                        wrapper.Name == propertyInfo.Name)).FirstOrDefault();
            if (modelMember != null){
                modelMember.PredefinedValues = GetPredefinedValues(modelMember);
            }
        }

        protected abstract string GetPredefinedValues(IModelMember wrapper);

        protected abstract Expression<Func<T, object>> GetPropertyName();
    }
}