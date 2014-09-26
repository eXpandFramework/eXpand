using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.Win.ListViewControl.LayoutViewGridListEditor {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        private const string LayoutViewGridListEditor_ListView = "LayoutViewGridListEditor_ListView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Module.Captions.HeaderLayoutViewGridListEditor, "1=1", "1=1", Module.Captions.HeaderLayoutViewGridListEditor, Position.Top) { View = LayoutViewGridListEditor_ListView };
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Module.Captions.HeaderLayoutViewGridListEditor, "1=1", "1=1",
                Module.Captions.ViewMessageLayoutViewGridListEditor, Position.Bottom) { View = LayoutViewGridListEditor_ListView };
            yield return new CloneViewAttribute(CloneViewType.ListView, LayoutViewGridListEditor_ListView);

            var xpandNavigationItemAttribute = new XpandNavigationItemAttribute(Module.Captions.ListViewCotrol + "LayoutView GridListEditor", LayoutViewGridListEditor_ListView);
            yield return xpandNavigationItemAttribute;
            yield return new DisplayFeatureModelAttribute(LayoutViewGridListEditor_ListView);
            yield return new WhatsNewAttribute(new DateTime(2011, 10, 15), xpandNavigationItemAttribute);
        }
    }
}
