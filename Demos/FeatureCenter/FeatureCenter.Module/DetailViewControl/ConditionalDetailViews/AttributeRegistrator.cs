using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.Persistent.Base.AdditionalViewControls;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.XAF.Modules.CloneModelView;

namespace FeatureCenter.Module.DetailViewControl.ConditionalDetailViews {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public const string ConditionalDetailViews_ListView = "ConditionalDetailViews_ListView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderConditionalDetailViews, "1=1", "1=1", Captions.ViewMessageConditionalDetailViews, Position.Bottom) { ViewType = ViewType.ListView, View = ConditionalDetailViews_ListView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderConditionalDetailViews, "1=1", "1=1", Captions.HeaderConditionalDetailViews, Position.Top) { ViewType = ViewType.ListView, View = ConditionalDetailViews_ListView };
            yield return new XpandNavigationItemAttribute(Captions.DetailViewCotrol + "Conditional DetailViews", ConditionalDetailViews_ListView);
            yield return new CloneModelViewAttribute(CloneViewType.ListView, ConditionalDetailViews_ListView) { DetailView = "ConditionalDetailViewsDefault_DetailView" };
            yield return new CloneModelViewAttribute(CloneViewType.DetailView, "ConditionalDetailViewsDefault_DetailView");
            yield return new CloneModelViewAttribute(CloneViewType.DetailView, "ConditionalDetailViews_DetailView");
            yield return new DisplayFeatureModelAttribute(ConditionalDetailViews_ListView);
        }
    }
}
