using System;
using System.Security;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using Xpand.ExpressApp.Logic.Security;
using Xpand.ExpressApp.MasterDetail.Logic;

namespace Xpand.ExpressApp.MasterDetail.Security {
    [NonPersistent]
    public class MasterDetailPermission : LogicRulePermission, IMasterDetailRule {
        public override IPermission Copy() {
            return new MasterDetailPermission();
        }

        public string ChildListView { get; set; }

        public string CollectionMember { get; set; }
        public bool SynchronizeActions { get; set; }

        IModelListView IMasterDetailRule.ChildListView {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        IModelMember IMasterDetailRule.CollectionMember {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
