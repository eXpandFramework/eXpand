using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule
{

    public interface IModelClassEditMode : IModelNode
    {
        [Category("eXpand")]
        [Description("Control detail view default edit mode")]
        ViewEditMode? ViewEditMode { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassEditMode), "ModelClass")]
    public interface IModelDetailViewEditMode : IModelClassEditMode
    {
        
    }
    
    public class ViewEditModeController : ViewController<XpandDetailView>, IModelExtender
    {

        protected override void OnActivated()
        {
            base.OnActivated();
            var viewEditMode = ((IModelDetailViewEditMode)View.Model).ViewEditMode;
            if (viewEditMode.HasValue)
                View.ViewEditMode = viewEditMode.Value;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass, IModelClassEditMode>();
            extenders.Add<IModelDetailView, IModelDetailViewEditMode>();
        }
    }
}