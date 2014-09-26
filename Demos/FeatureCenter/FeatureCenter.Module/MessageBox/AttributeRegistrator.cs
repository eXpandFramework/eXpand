using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.General;

namespace FeatureCenter.Module.MessageBox {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        const string ShowMessageBoxObject_DetailView = "ShowMessageBoxObject_DetailView";

        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(ShowMessageBoxObject)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderShowMessageBox, "1=1", "1=1", Captions.ViewMessageShowMessageBox, Position.Bottom);
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderShowMessageBox, "1=1", "1=1", Captions.HeaderShowMessageBox, Position.Top);
            var xpandNavigationItemAttribute = new XpandNavigationItemAttribute(Captions.Miscellaneous + "Show MessageBox", ShowMessageBoxObject_DetailView);
            yield return xpandNavigationItemAttribute;
            yield return new WhatsNewAttribute(new DateTime(2012,3,22),xpandNavigationItemAttribute );
            yield return new DisplayFeatureModelAttribute("ShowMessageBoxObject_DetailView", "ShowMessageBox");
            
        }
    }
}
