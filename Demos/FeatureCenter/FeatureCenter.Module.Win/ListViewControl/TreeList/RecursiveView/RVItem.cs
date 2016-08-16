using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.AdditionalViewControls;
using Xpand.Persistent.Base.General;

namespace FeatureCenter.Module.Win.ListViewControl.TreeList.RecursiveView {
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderRecursiveView, "1=1", "1=1",
        Captions.ViewMessageRecursiveView, Position.Bottom)]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderRecursiveView, "1=1", "1=1",
        Captions.HeaderRecursiveView, Position.Top)]
    [XpandNavigationItem(Module.Captions.ListViewCotrol + Module.Captions.TreeListView + "RecursiveView", "RVItem_ListView")]
    [DisplayFeatureModel("RVItem_ListView", "RecursiveView")]
    public class RVItem : BaseObject, ICategorizedItem {
        public RVItem(Session session) : base(session) { }
        private string _name;
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }
        private RVCategory _category;
        public RVCategory Category {
            get { return _category; }
            set { SetPropertyValue("Category", ref _category, value); }
        }
        #region ICategorizedItem Members
        ITreeNode ICategorizedItem.Category {
            get { return Category; }
            set { Category = (RVCategory)value; }
        }
        #endregion
    }
}
