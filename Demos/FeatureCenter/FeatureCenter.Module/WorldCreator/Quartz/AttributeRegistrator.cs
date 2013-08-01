using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.BaseImpl.PersistentMetaData;

namespace FeatureCenter.Module.WorldCreator.Quartz {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        private const string Quartz_DetailView = "Quartz_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (!(typesInfo.Type == typeof (PersistentAssemblyInfo))) yield break;
            yield return new CloneViewAttribute(CloneViewType.DetailView, Quartz_DetailView);
            yield return new XpandNavigationItemAttribute("WorldCreator/Quartz/Quartz AssemblyInfo", Quartz_DetailView) { ObjectKey = CriteriaOperator.Parse("Name=?", "Quartz").ToString() };
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderQuartz, "1=1", "1=1", Captions.ViewMessageQuartz, Position.Bottom) { View = Quartz_DetailView };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderQuartz, "1=1", "1=1", Captions.HeaderQuartz, Position.Top) { View = Quartz_DetailView };
        }
    }
}
