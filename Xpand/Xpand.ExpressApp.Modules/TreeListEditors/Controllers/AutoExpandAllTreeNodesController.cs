using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base.General;

namespace Xpand.ExpressApp.TreeListEditors.Controllers {
    [ModelAbstractClass]
    public interface IModelListViewAutoExpandAllTreeNodes : IModelListView {
        [Category("eXpand.TreeList")]
        bool AutoExpandAllTreeNodes { get; set; }
    }

    public class AutoExpandAllTreeNodesController: ObjectViewController<ListView, ITreeNode>, IModelExtender {
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewAutoExpandAllTreeNodes>();
        }

    }
}
