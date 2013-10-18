using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using System.ComponentModel;

namespace Xpand.ExpressApp.SystemModule {

    public interface IModelOptionsHighlightFocusedItem {
        [Category("eXpand")]
        [DefaultValue(true)]
        bool HighlightFocusedLayoutItem { get; set; }
    }

    public interface IModelClassHighlightFocusedItem : IModelNode {
        [Category("eXpand")]
        bool HighlightFocusedLayoutItem { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassHighlightFocusedItem), "ModelClass")]
    public interface IModelDetailViewHighlightFocusedItem : IModelClassHighlightFocusedItem {
    }
    [DomainLogic(typeof(IModelClassHighlightFocusedItem))]
    public class ModelDetailViewHighlightFocusedLayoutItemLogic {
        public static bool Get_HighlightFocusedLayoutItem(IModelClassHighlightFocusedItem model) {
            return model != null && ((IModelOptionsHighlightFocusedItem)model.Application.Options).HighlightFocusedLayoutItem;
        }
    }

    public abstract class HighlightFocusedLayoutItemDetailViewControllerBase : ViewController<DetailView>, IModelExtender {
        public const string HighlightFocusedLayoutItemAttributeName = "HighlightFocusedLayoutItem";
        public const string EnableHighlightFocusedLayoutItemAttributeName = "EnableHighlightFocusedLayoutItem";
        public const string ActiveKeyHighlightFocusedEditor = "HighlightFocusedLayoutItem";
        protected override void OnViewChanging(View view) {
            base.OnViewChanging(view);
            var dv = view as DetailView;
            if (dv != null)
                Active[ActiveKeyHighlightFocusedEditor] = ((IModelDetailViewHighlightFocusedItem)dv.Model).HighlightFocusedLayoutItem;
        }
        protected abstract void ApplyFocusedStyle(object element);
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsHighlightFocusedItem>();
            extenders.Add<IModelClass, IModelClassHighlightFocusedItem>();
            extenders.Add<IModelDetailView, IModelDetailViewHighlightFocusedItem>();
        }

    }

}