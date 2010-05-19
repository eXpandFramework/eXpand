using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule
{

    public interface IModelDetailViewEditMode : IModelNode
    {
        ViewEditMode? ViewEditMode { get; set; }
    }

    public class ViewEditModeController : ViewController<DetailView>, IModelExtender
    {

        protected override void OnActivated()
        {
            base.OnActivated();
            var attributeValue = ((IModelDetailViewEditMode)View.Model).ViewEditMode;
            if (attributeValue != null)
                View.ViewEditMode = attributeValue.Value;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelDetailView, IModelDetailViewEditMode>();
        }
    }
}