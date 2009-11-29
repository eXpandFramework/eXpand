using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.SystemModule;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.Security.Controllers
{
    public abstract partial class PopulateController<T> : BaseViewController
    {
        private PropertyInfoNodeWrapper propertyInfoNodeWrapper;

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
            if (propertyInfoNodeWrapper != null){
                propertyInfoNodeWrapper.Node.SetAttribute("PredefinedValues","");
            }
        }
        protected virtual void populate()
        {
            
            var classInfoNodeWrapper = GetClassInfoNodeWrapper();
            LambdaExpression lambdaExpression = GetPropertyName();
            var propertyInfo = ReflectionExtensions.GetExpression(lambdaExpression) as PropertyInfo;
            if (propertyInfo != null)
                propertyInfoNodeWrapper =
                    (classInfoNodeWrapper.AllProperties.Where(
                        wrapper =>
                        wrapper.Name == propertyInfo.Name)).FirstOrDefault();
            if (propertyInfoNodeWrapper != null){
                propertyInfoNodeWrapper.Node.SetAttribute("PredefinedValues",GetPredefinedValues(propertyInfoNodeWrapper));
            }
        }

        protected abstract string GetPredefinedValues(PropertyInfoNodeWrapper wrapper);

        protected abstract Expression<Func<T, object>> GetPropertyName();
    }
}