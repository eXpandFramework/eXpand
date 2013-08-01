using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.Win.Miscellaneous.LookUpListSearch {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        private const string LookUpListSearch_LookupListView = "LookUpListSearch_LookupListView";
        private const string LookUpListSearch_DetailView = "LookUpListSearch_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type == typeof(Customer)) {
                yield return new CloneViewAttribute(CloneViewType.ListView, LookUpListSearch_LookupListView);
            }
            if (typesInfo.Type == typeof(Order)) {
                yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Module.Captions.HeaderLookUpListSearch, "1=1", "1=1", Module.Captions.ViewMessageLookUpListSearch, Position.Bottom) { ViewType = ViewType.DetailView, View = LookUpListSearch_DetailView };
                yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Module.Captions.HeaderLookUpListSearch, "1=1", "1=1", Module.Captions.HeaderLookUpListSearch, Position.Top) { ViewType = ViewType.DetailView, View = LookUpListSearch_DetailView };
                yield return new XpandNavigationItemAttribute(Module.Captions.Miscellaneous + "Lookup List Search", LookUpListSearch_DetailView);
                yield return new CloneViewAttribute(CloneViewType.DetailView, LookUpListSearch_DetailView);
                yield return new DisplayFeatureModelAttribute(LookUpListSearch_DetailView);
            }
        }
    }
}
