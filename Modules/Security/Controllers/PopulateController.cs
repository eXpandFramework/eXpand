using System.Linq;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Security.Controllers
{
    public abstract partial class PopulateController : BaseViewController
    {
        private PropertyInfoNodeWrapper propertyInfoNodeWrapper;

        protected PopulateController()
        {
            InitializeComponent();
            RegisterActions(components);
            
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
            propertyInfoNodeWrapper =
                (classInfoNodeWrapper.AllProperties.Where(
                    wrapper =>
                    wrapper.Name == GetPermissionPropertyName())).FirstOrDefault();
            if (propertyInfoNodeWrapper != null){
                propertyInfoNodeWrapper.Node.SetAttribute("PredefinedValues",GetPredefinedValues(propertyInfoNodeWrapper));
            }
        }

        protected abstract string GetPredefinedValues(PropertyInfoNodeWrapper wrapper);

        protected abstract string GetPermissionPropertyName();
    }
}