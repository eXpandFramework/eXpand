using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using System.ComponentModel;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IModelClassHighlightFocusedItem : IModelNode
    {
        [Category("eXpand")]
        [DefaultValue(true)]
        bool HighlightFocusedLayoutItem { get; set; }
    }

    public interface IModelDetailViewHighlightFocusedItem : IModelNode
    {
        [Category("eXpand")]
        [DefaultValue(true)]
        [ModelValueCalculator("((IModelClassHighlightFocusedItem)ModelClass)", "HighlightFocusedLayoutItem")]
        bool HighlightFocusedLayoutItem { get; set; }
    }


    public class HighlightFocusedLayoutItemDetailViewControllerBase : ViewController<DetailView>, IModelExtender
    {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass, IModelClassHighlightFocusedItem>();
            extenders.Add<IModelDetailView, IModelDetailViewHighlightFocusedItem>();
        }

        

        protected virtual void AssignStyle(object control) {
            throw new NotImplementedException();
        }
    }
}