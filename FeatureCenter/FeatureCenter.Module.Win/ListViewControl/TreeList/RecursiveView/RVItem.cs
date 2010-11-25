using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.ListViewControl.TreeList.RecursiveView
{
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderRecursiveView, "1=1", "1=1",
        Captions.ViewMessageRecursiveView, Position.Bottom)]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderRecursiveView, "1=1", "1=1",
        Captions.HeaderRecursiveView, Position.Top)]
    [XpandNavigationItem(Module.Captions.ListViewCotrol + Module.Captions.TreeListView + "RecursiveView", "RVItem_ListView")]
    [DisplayFeatureModel("RVItem_ListView", "RecursiveView")]
    public class RVItem : BaseObject, ICategorizedItem
    {
        public RVItem(Session session) : base(session) { }
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }
        private RVCategory _category;
        public RVCategory Category
        {
            get { return _category; }
            set { SetPropertyValue("Category", ref _category, value); }
        }
        #region ICategorizedItem Members
        ITreeNode ICategorizedItem.Category {
            get { return Category; }
            set { Category = (RVCategory) value; }
        }
        #endregion
    }
}
