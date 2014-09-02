﻿using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace FeatureCenter.Module.Win.ControllingXtraGrid.ColumnsOptions {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(WinCustomer)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderControlXtraGridColumns, "1=1", "1=1",
                Captions.ViewMessageControlXtraGridColumns, Position.Bottom) { View = "XtraGridColumnsOptions_ListView" };
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderControlXtraGridColumns, "1=1", "1=1",
                Captions.HeaderControlXtraGridColumns, Position.Top) { View = "XtraGridColumnsOptions_ListView" };
            yield return new CloneViewAttribute(CloneViewType.ListView, "XtraGridColumnsOptions_ListView");
            yield return new XpandNavigationItemAttribute("Controlling XtraGrid/Column options", "XtraGridColumnsOptions_ListView");
            yield return new DisplayFeatureModelAttribute("XtraGridColumnsOptions_ListView");
        }
    }
}
