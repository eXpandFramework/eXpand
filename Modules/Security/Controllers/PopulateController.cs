using System.Linq;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Security.Controllers
{
    public abstract partial class PopulateController : BaseViewController
    {
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

        protected virtual void populate()
        {
            var classInfoNodeWrapper = GetClassInfoNodeWrapper();
            PropertyInfoNodeWrapper propertyInfoNodeWrapper =
                (classInfoNodeWrapper.AllProperties.Where(
                    wrapper =>
                    wrapper.Name == GetPermissionPropertyName())).FirstOrDefault();
            if (propertyInfoNodeWrapper != null)
            {
                propertyInfoNodeWrapper.Node.SetAttribute("PredefinedValues", GetPredefinedValues());
                Active["Done"] = false;
            }
        }

        protected abstract string GetPredefinedValues();

        protected abstract string GetPermissionPropertyName();
    }
}