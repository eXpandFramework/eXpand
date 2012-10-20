using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.ListViewControl.TreeList.TreeListOptions {
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderTreeListOptions, "1=1", "1=1",
        Captions.ViewMessageTreeListOptions, Position.Bottom)]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderTreeListOptions, "1=1", "1=1",
        Captions.HeaderTreeListOptions, Position.Top)]
    [XpandNavigationItem(Module.Captions.ListViewCotrol +Module.Captions.TreeListView + "Tree Options", "OCategory_ListView")]
    [DisplayFeatureModel("OCategory_ListView", "TreeListOptions")]
    public class OCategory : HCategory {
        public OCategory(Session session)
            : base(session) {
        }

        public OCategory(Session session, string name)
            : base(session, name) {
        }
        private string _fullName;
        public string FullName {
            get {
                return _fullName;
            }
            set {
                SetPropertyValue("FullName", ref _fullName, value);
            }
        }
        private string _moreInfo;
        public string MoreInfo {
            get {
                return _moreInfo;
            }
            set {
                SetPropertyValue("MoreInfo", ref _moreInfo, value);
            }
        }
        private string _moreInfo2;
        public string MoreInfo2 {
            get {
                return _moreInfo2;
            }
            set {
                SetPropertyValue("MoreInfo2", ref _moreInfo2, value);
            }
        }
    }
}
