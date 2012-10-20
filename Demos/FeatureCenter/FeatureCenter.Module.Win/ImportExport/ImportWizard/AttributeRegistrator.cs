using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.ImportExport.ImportWizard {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(TestImportObject)) yield break;
            //yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderFilterControl, "1=1", "1=1",
            //    Captions.ViewMessageFilterControl, Position.Bottom){View = "FilterControl_ListView"};
            //yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderFilterControl, "1=1", "1=1",
            //    Captions.HeaderFilterControl, Position.Top){View = "FilterControl_ListView"};
            //yield return new CloneViewAttribute(CloneViewType.ListView, "FilterControl_ListView");
            var xpandNavigationItemAttribute = new XpandNavigationItemAttribute("ImportWizard", "TestImportObject_ListView");
            yield return xpandNavigationItemAttribute;

            yield return new DisplayFeatureModelAttribute("TestImportObject_ListView");
            yield return new WhatsNewAttribute(new DateTime(2011, 5, 10), xpandNavigationItemAttribute);
        }
    }
}
