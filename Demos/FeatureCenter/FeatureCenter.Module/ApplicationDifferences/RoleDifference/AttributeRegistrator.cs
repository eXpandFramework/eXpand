using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.Persistent.Base.AdditionalViewControls;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.XAF.Modules.CloneModelView;

namespace FeatureCenter.Module.ApplicationDifferences.RoleDifference {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderRoleDifference, "1=1", "1=1", Captions.ViewMessageRoleDifference, Position.Bottom) { ViewType = ViewType.DetailView, View = "RoleDifference_DetailView" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderRoleDifference, "1=1", "1=1", Captions.HeaderRoleDifference, Position.Top) { ViewType = ViewType.DetailView, View = "RoleDifference_DetailView" };
            yield return new CloneModelViewAttribute(CloneViewType.DetailView, "RoleDifference_DetailView");
            yield return new XpandNavigationItemAttribute("Application Differences/Role Differences", "RoleDifference_DetailView");
            yield return new DisplayFeatureModelAttribute("RoleDifference_DetailView", "Administrators_RoleDifference");
        }
    }
}
