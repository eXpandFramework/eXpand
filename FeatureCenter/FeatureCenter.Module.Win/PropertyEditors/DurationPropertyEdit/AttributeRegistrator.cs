using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.PropertyEditors {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            
            if (!Object.Equals(typesInfo.Type, typeof(DurationEditTestObject))) yield break;
            
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderDurationEdit, "1=1", "1=1",
                Captions.ViewMessageDurationEdit, Position.Bottom) { View = "DurationEditTestObject_DetailView" };
            
            //yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderDurationEdit, "1=1", "1=1",
            //    Captions.ViewMessageDurationEdit, Position.Top) { View = "DurationEditTestObject_DetailView" };
            
            //yield return new CloneViewAttribute(CloneViewType.DetailView, "DurationEditTestObject_DetailView");
            
            var xpandNavigationItemAttribute = new XpandNavigationItemAttribute(Module.Captions.PropertyEditors + "Duration Edit", "DurationEditTestObject_DetailView");
            
            yield return xpandNavigationItemAttribute;
            
            yield return new DisplayFeatureModelAttribute("DurationEditTestObject_DetailView");
            
            yield return new WhatsNewAttribute(new DateTime(2011, 5, 10), xpandNavigationItemAttribute);
        }
    }
}
