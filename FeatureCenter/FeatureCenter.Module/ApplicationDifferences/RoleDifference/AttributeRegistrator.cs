using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.ApplicationDifferences.RoleDifference
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderRoleDifference, "1=1", "1=1", Captions.ViewMessageRoleDifference, Position.Bottom) { ViewType = ViewType.DetailView, View = "RoleDifference" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderRoleDifference, "1=1", "1=1", Captions.HeaderRoleDifference, Position.Top) { ViewType = ViewType.DetailView, View = "RoleDifference" };
            yield return new CloneViewAttribute(CloneViewType.DetailView, "RoleDifference");
            yield return new NavigationItemAttribute("Application Differences/Role Differences", "RoleDifference");
            yield return new DisplayFeatureModelAttribute("RoleDifference", "Administrators_RoleDifference");
        }
    }
}
