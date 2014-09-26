using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.BaseImpl.PersistentMetaData;

namespace FeatureCenter.Module.WorldCreator.NorthWind {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(PersistentAssemblyInfo)) yield break;
            yield return new CloneViewAttribute(CloneViewType.DetailView, "NorthWind_DetailView");
            yield return new XpandNavigationItemAttribute("WorldCreator/NorthWind/NorthWind AssemblyInfo", "NorthWind_DetailView") { ObjectKey = CriteriaOperator.Parse("Name=?", "NorthWind").ToString() };
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderNorthWind, "1=1", "1=1", Captions.ViewMessageNorthWind, Position.Bottom) { View = "NorthWind_DetailView" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderNorthWind, "1=1", "1=1", Captions.HeaderNorthWind, Position.Top) { View = "NorthWind_DetailView" };
        }
    }
}
