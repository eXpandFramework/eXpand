using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win.Miscellaneous.RecursiveFiltering
{
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderRecursiveFiltering, "1=1", "1=1",
        Captions.ViewMessageRecursiveFiltering, Position.Bottom)]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderRecursiveFiltering, "1=1", "1=1",
        Captions.HeaderRecursiveFiltering, Position.Top)]
    [NavigationItem(Module.Captions.Miscellaneous + "RecursiveFiltering", "RFCustomer_ListView")]
    public class RFCustomer:CustomerBase,ICategorizedItem
    {
        public RFCustomer(Session session) : base(session) {
        }

        ITreeNode ICategorizedItem.Category {
            get { return Category; }
            set { Category=value as Category; }
        }
        private Category _category;

        [Association("Category-RFCustomers")]
        public Category Category {
            get { return _category; }
            set { SetPropertyValue("Category", ref _category, value); }
        }
    }
}
