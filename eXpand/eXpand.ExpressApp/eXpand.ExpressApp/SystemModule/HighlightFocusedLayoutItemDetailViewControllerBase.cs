using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.DC;
using System.ComponentModel;

namespace eXpand.ExpressApp.SystemModule
{

    public interface IModelDetailViewHighlightOptions : IModelNode
    {
        [Category("eXpand")]
        [DefaultValue(true)]
        bool? HighlightFocusedLayoutItem { get; set; }
    }

    [DomainLogic(typeof(IModelDetailViewHighlightOptions))]
    public static class ModelDetailViewHighlightOptionsDomainLogic
    {
        public static bool Get_ModelDetailViewHighlightOptions(IModelDetailViewHighlightOptions modelDetailViewHighlightOptions)
        {
            if (modelDetailViewHighlightOptions is IModelDetailView &&
                !modelDetailViewHighlightOptions.HighlightFocusedLayoutItem.HasValue)
            {
                var detailViewHighlightOptions = ((IModelDetailViewHighlightOptions)modelDetailViewHighlightOptions.Application.Options);
                if (detailViewHighlightOptions.HighlightFocusedLayoutItem != null)
                    return detailViewHighlightOptions.HighlightFocusedLayoutItem.Value;
            }
            else if (modelDetailViewHighlightOptions.HighlightFocusedLayoutItem != null)
                return modelDetailViewHighlightOptions.HighlightFocusedLayoutItem.Value;
            return false;
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
            {
                bool? highlightFocusedLayoutItem = ((IModelDetailViewHighlightOptions)dv.Model).HighlightFocusedLayoutItem;
                if (highlightFocusedLayoutItem != null)
                    Active[ActiveKeyHighlightFocusedEditor] = highlightFocusedLayoutItem.Value;
            }
        }


        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelOptions, IModelDetailViewHighlightOptions>();
            extenders.Add<IModelDetailView, IModelDetailViewHighlightOptions>();
        }

        protected abstract void AssignStyle(object control);
    }
}