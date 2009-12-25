using System.ComponentModel;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.ImportExport;

namespace eXpand.Persistent.BaseImpl.ImportExport {
    public class MemberCategory : BaseObject, IMemberCategory {
        string _name;

        MemberCategory _parentCategory;

        [Association("MemberCategory-MemberCategorys")]
        public MemberCategory ParentCategory {
            get { return _parentCategory; }
            set { SetPropertyValue("ParentCategory", ref _parentCategory, value); }
        }

        [Association("MemberCategory-MemberCategorys")]
        public XPCollection<MemberCategory> Children {
            get { return GetCollection<MemberCategory>("Childern"); }
        }
        #region IMemberCategory Members
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        ITreeNode ITreeNode.Parent {
            get { return _parentCategory; }
        }

        IBindingList ITreeNode.Children {
            get { return Children; }
        }
        #endregion
    }
}