using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using System.ComponentModel;

namespace eXpand.ExpressApp.SystemModule {

    public interface IModelViewPropertySearchMode : IModelNode
    {
        [DefaultValue(SearchMemberMode.Include)]
        SearchMemberMode SearchMemberMode { get; set; }
    }

    public abstract class SearchFromViewController : ViewController {
        }
}