using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using FeatureCenter.Module.ExpandAbleMembers;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.Web.MasterDetail.Web {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        private const string MasterDetailModeListView = "MasterDetailMode_ListView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
#if EASYTEST
            yield break;
#endif 
            if (typesInfo.Type == typeof(EAMCustomer)) {
                yield return new CloneViewAttribute(CloneViewType.ListView, MasterDetailModeListView);
                var xpandNavigationItemAttribute = new XpandNavigationItemAttribute("MasterDetail/MasterDetailMode", MasterDetailModeListView);
                yield return xpandNavigationItemAttribute;
                yield return new WhatsNewAttribute(new DateTime(2012, 8, 16), xpandNavigationItemAttribute);
            }

        }
    }
}
