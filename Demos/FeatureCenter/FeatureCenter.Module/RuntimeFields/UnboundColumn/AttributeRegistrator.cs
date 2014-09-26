using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.RuntimeFields.UnboundColumn {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        private const string UnboundColumn_ListView = "UnboundColumn_ListView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderRuntimeUnboundColumn, "1=1", "1=1", Captions.ViewMessageRuntimeUnboundColumn, Position.Bottom) { ViewType = ViewType.ListView, View = UnboundColumn_ListView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderRuntimeUnboundColumn, "1=1", "1=1",
                Captions.HeaderRuntimeUnboundColumn, Position.Top) { View = UnboundColumn_ListView };
            yield return new CloneViewAttribute(CloneViewType.ListView, UnboundColumn_ListView);
            var xpandNavigationItemAttribute = new XpandNavigationItemAttribute("Runtime Fields/Unbound Column", UnboundColumn_ListView);
            yield return xpandNavigationItemAttribute;
            yield return new DisplayFeatureModelAttribute(UnboundColumn_ListView, "UnboundColumn");
            yield return new WhatsNewAttribute(new DateTime(2011, 9, 14), xpandNavigationItemAttribute);
        }
    }
}
