using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule {

    public interface IModelViewPropertySearchMode : IModelNode
    {
        [Category("eXpand")]
        SearchMemberMode SearchMemberMode { get; set; }
    }

    public abstract class SearchFromViewController : ViewController, IModelExtender {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelView,IModelViewPropertySearchMode>();
        }
    }
}