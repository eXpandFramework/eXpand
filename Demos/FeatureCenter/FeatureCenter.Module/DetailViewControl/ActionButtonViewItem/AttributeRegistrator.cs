﻿using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.DetailViewControl.ActionButtonViewItem {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(Customer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderActionButtonViewItem, "1=1", "1=1",
                Captions.ViewMessageActionButtonViewItem, Position.Bottom) { View = "ActionButtonViewItem_DetailView" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderActionButtonViewItem, "1=1", "1=1",
                Captions.HeaderActionButtonViewItem, Position.Top) { View = "ActionButtonViewItem_DetailView" };
            yield return new CloneViewAttribute(CloneViewType.DetailView, "ActionButtonViewItem_DetailView");
            yield return new XpandNavigationItemAttribute(Captions.DetailViewCotrol + "Action Button View Item", "ActionButtonViewItem_DetailView");
            yield return new DisplayFeatureModelAttribute("ActionButtonViewItem_DetailView");
        }
    }
}
