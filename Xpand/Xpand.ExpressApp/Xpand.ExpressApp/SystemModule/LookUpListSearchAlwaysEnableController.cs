using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.SystemModule {
    public enum LookUpListSearch {
        Default,
        AlwaysEnable
    }

    public interface IModelClassLookUpListSearch {
        [Category(AttributeCategoryNameProvider.Xpand)]
        LookUpListSearch LookUpListSearch { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassLookUpListSearch), "ModelClass")]
    public interface IModelListViewLookUpListSearch : IModelClassLookUpListSearch {
    }

    public class LookUpListSearchAlwaysEnableController : ViewController, IModelExtender {
        protected override void OnActivated() {
            base.OnActivated();
            if (Frame.Template is ILookupPopupFrameTemplate) {
                if (((IModelListViewLookUpListSearch)View.Model).LookUpListSearch == LookUpListSearch.AlwaysEnable)
                    ((ILookupPopupFrameTemplate)Frame.Template).IsSearchEnabled = true;
            }
        }


        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelClass, IModelClassLookUpListSearch>();
            extenders.Add<IModelListView, IModelListViewLookUpListSearch>();
        }
    }
}