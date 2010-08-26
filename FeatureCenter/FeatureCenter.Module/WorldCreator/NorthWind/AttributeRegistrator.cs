using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;
using eXpand.Persistent.BaseImpl.PersistentMetaData;

namespace FeatureCenter.Module.WorldCreator.NorthWind
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type!=typeof(PersistentAssemblyInfo))yield break;
            yield return new CloneViewAttribute(CloneViewType.DetailView, "NorthWind_DetailView");
            yield return new NavigationItemAttribute("WorldCreator/NorthWind/NorthWind AssemblyInfo", "NorthWind_DetailView") { ObjectKey = CriteriaOperator.Parse("Name=?", "NorthWind").ToString() };
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderNorthWind, "1=1", "1=1", Captions.ViewMessageNorthWind, Position.Bottom) { ViewType = ViewType.ListView, View = "NorthWind_DetailView" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderNorthWind, "1=1", "1=1", Captions.HeaderRuntimeMemberFromModel, Position.Top) { View = "NorthWind_DetailView" };
        }
    }
}
