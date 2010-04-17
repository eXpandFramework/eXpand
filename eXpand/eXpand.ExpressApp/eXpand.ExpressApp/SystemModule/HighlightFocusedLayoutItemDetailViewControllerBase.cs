using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using System.ComponentModel;

namespace eXpand.ExpressApp.SystemModule {

    public interface IModelDetailViewHighlightOptions : IModelNode
    {
        [DefaultValue(true)]
        bool? HighlightFocusedLayoutItem { get; set; }
    }

    [DomainLogic(typeof(IModelDetailViewHighlightOptions))]
    public static class ModelDetailViewHighlightOptionsDomainLogic
    {
        public static bool Get_ModelDetailViewHighlightOptions(IModelDetailViewHighlightOptions modelDetailViewHighlightOptions)
        {
            return (modelDetailViewHighlightOptions is IModelDetailView &&
                !((IModelDetailViewHighlightOptions)modelDetailViewHighlightOptions).HighlightFocusedLayoutItem.HasValue) ?
                ((IModelDetailViewHighlightOptions)modelDetailViewHighlightOptions.Application.Options).HighlightFocusedLayoutItem.Value :
                modelDetailViewHighlightOptions.HighlightFocusedLayoutItem.Value;
        }
    }

    public abstract class HighlightFocusedLayoutItemDetailViewControllerBase : ViewController<DetailView>
    {
        public const string ActiveKeyHighlightFocusedEditor = "HighlightFocusedLayoutItem";
        
        protected override void OnViewChanging(View view)
        {
            base.OnViewChanging(view);
            var dv = view as DetailView;
            if (dv != null)
                Active[ActiveKeyHighlightFocusedEditor] = (dv.Model as IModelDetailViewHighlightOptions).HighlightFocusedLayoutItem.Value;
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelOptions, IModelDetailViewHighlightOptions>();
            extenders.Add<IModelDetailView, IModelDetailViewHighlightOptions>();
        }

        protected abstract void AssignStyle(object control);
    }
}