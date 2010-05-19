using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using System.ComponentModel;

namespace eXpand.ExpressApp.SystemModule {

    public interface IModelDetailViewHighlightOptions : IModelNode
    {
        [DefaultValue(true)]
        bool HighlightFocusedLayoutItem { get; set; }
    }

    [DomainLogic(typeof(IModelDetailViewHighlightOptions))]
    public static class ModelDetailViewHighlightOptionsDomainLogic
    {
        public static bool Get_ModelDetailViewHighlightOptions(IModelDetailViewHighlightOptions modelDetailViewHighlightOptions)
        {
            return (modelDetailViewHighlightOptions is IModelDetailView &&
                !((IModelDetailViewHighlightOptions)modelDetailViewHighlightOptions).HighlightFocusedLayoutItem) ?
                ((IModelDetailViewHighlightOptions)modelDetailViewHighlightOptions.Application.Options).HighlightFocusedLayoutItem :
                modelDetailViewHighlightOptions.HighlightFocusedLayoutItem;
        }
    }

    public abstract class HighlightFocusedLayoutItemDetailViewControllerBase : ViewController<DetailView>, IModelExtender
    {
        public const string ActiveKeyHighlightFocusedEditor = "HighlightFocusedLayoutItem";
        
        protected override void OnViewChanging(View view)
        {
            base.OnViewChanging(view);
            var dv = view as DetailView;
            if (dv != null)
                Active[ActiveKeyHighlightFocusedEditor] = (dv.Model as IModelDetailViewHighlightOptions).HighlightFocusedLayoutItem;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelOptions, IModelDetailViewHighlightOptions>();
            extenders.Add<IModelDetailView, IModelDetailViewHighlightOptions>();
        }

        protected abstract void AssignStyle(object control);
    }
}