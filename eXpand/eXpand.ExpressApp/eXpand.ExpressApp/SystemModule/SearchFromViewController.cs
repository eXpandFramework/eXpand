using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule {

    public interface IModelViewPropertySearchMode : IModelNode
    {
        SearchMemberMode SearchMemberMode { get; set; }
    }

    public abstract class SearchFromViewController : ViewController {
        }
}