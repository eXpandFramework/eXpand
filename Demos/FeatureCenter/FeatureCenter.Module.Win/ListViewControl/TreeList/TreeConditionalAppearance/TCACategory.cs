using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.ListViewControl.TreeList.TreeConditionalAppearance {
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderTreeConditionalAppearance, "1=1", "1=1",
        Captions.ViewMessageTreeConditionalAppearance, Position.Bottom)]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderTreeConditionalAppearance, "1=1", "1=1",
        Captions.HeaderTreeConditionalAppearance, Position.Top)]
    [XpandNavigationItem(Module.Captions.ListViewCotrol + Module.Captions.TreeListView + "Conditional Appearance", "TCACategory_ListView")]
    [DisplayFeatureModel("TCACategory_ListView", "TreeConditionalAppearance")]
    public class TCACategory : HCategory {
        public TCACategory(Session session)
            : base(session) {
        }

        public TCACategory(Session session, string name)
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
