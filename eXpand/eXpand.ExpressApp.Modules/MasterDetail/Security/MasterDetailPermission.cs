using System;
using System.Security;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using eXpand.ExpressApp.Logic.Conditional.Security;
using eXpand.ExpressApp.MasterDetail.Logic;

namespace eXpand.ExpressApp.MasterDetail.Security
{
    [NonPersistent]
    public class MasterDetailPermission : ConditionalLogicRulePermission, IMasterDetailRule
    {
        public override IPermission Copy() {
            return new MasterDetailPermission();
        }

        public IModelListView ChildListView {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IModelMember CollectionMember {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
