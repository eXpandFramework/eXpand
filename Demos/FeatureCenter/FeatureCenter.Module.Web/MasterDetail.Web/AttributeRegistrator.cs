using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using FeatureCenter.Module.ExpandAbleMembers;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.Web.MasterDetail.Web {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        private const string MasterDetailMode_ListView = "MasterDetailMode_ListView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type == typeof(EAMCustomer)) {
                yield return new CloneViewAttribute(CloneViewType.ListView, MasterDetailMode_ListView);
                var xpandNavigationItemAttribute = new XpandNavigationItemAttribute("MasterDetail/MasterDetailMode", MasterDetailMode_ListView);
                yield return xpandNavigationItemAttribute;
                yield return new WhatsNewAttribute(new DateTime(2012, 8, 16), xpandNavigationItemAttribute);
            }

        }
    }
}
