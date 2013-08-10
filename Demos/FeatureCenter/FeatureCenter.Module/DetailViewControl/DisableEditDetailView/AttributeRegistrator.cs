using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.DetailViewControl.DisableEditDetailView {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderDisableEditDetailView, "1=1", "1=1",
                Captions.ViewMessageDisableEditDetailView, Position.Bottom) { View = "DisableEditDetailView_DetailView" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderDisableEditDetailView, "1=1", "1=1",
                Captions.HeaderDisableEditDetailView, Position.Top) { View = "DisableEditDetailView_DetailView" };
            yield return new CloneViewAttribute(CloneViewType.DetailView, "DisableEditDetailView_DetailView");
            yield return new XpandNavigationItemAttribute(Captions.DetailViewCotrol + "Disable Edit Detail View", "DisableEditDetailView_DetailView");
            yield return new DisplayFeatureModelAttribute("DisableEditDetailView_DetailView");
        }
    }
}
