using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using System.ComponentModel;

namespace eXpand.ExpressApp.SystemModule
{

    public interface IModelOptionsHighlightFocusedItem
    {
        [Category("eXpand")]
        [DefaultValue(true)]
        bool HighlightFocusedLayoutItem { get; set; }
    }
    
    public interface IModelClassHighlightFocusedItem :IModelNode
    {
        [Category("eXpand")]
        bool HighlightFocusedLayoutItem { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassHighlightFocusedItem), "ModelClass")]
    public interface IModelDetailViewHighlightFocusedItem : IModelClassHighlightFocusedItem
    {
    }
    [DomainLogic(typeof(IModelClassHighlightFocusedItem))]
    public class ModelDetailViewHighlightFocusedLayoutItemLogic
    {
        public static bool Get_HighlightFocusedLayoutItem(IModelClassHighlightFocusedItem model)
        {
            if (model != null)
                return ((IModelOptionsHighlightFocusedItem)model.Application.Options).HighlightFocusedLayoutItem;
            return false;
        }
    }


    public class HighlightFocusedLayoutItemDetailViewControllerBase : ViewController<DetailView>, IModelExtender
    {
        protected override void OnViewChanging(View view)
        {
            base.OnViewChanging(view);
            var dv = view as DetailView;
            if (dv != null)
                Active["ActiveKeyHighlightFocusedEditor"] = ((IModelDetailViewHighlightFocusedItem)dv.Model).HighlightFocusedLayoutItem;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelOptions, IModelOptionsHighlightFocusedItem>();
            extenders.Add<IModelClass, IModelClassHighlightFocusedItem>();
            extenders.Add<IModelDetailView, IModelDetailViewHighlightFocusedItem>();
        }

        

        protected virtual void AssignStyle(object control) {
            throw new NotImplementedException();
        }
    }
}