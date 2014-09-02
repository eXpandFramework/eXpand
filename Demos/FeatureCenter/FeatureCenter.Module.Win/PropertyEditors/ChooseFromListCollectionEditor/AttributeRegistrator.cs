﻿using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.BaseImpl.JobScheduler.Calendars;

namespace FeatureCenter.Module.Win.PropertyEditors.ChooseFromListCollectionEditor {
    public class AttributeRegistrator : Xpand.Persistent.Base.General.AttributeRegistrator {
        private const string ChooseFromListCollectionEditor_DetailView = "ChooseFromListCollectionEditor_DetailView";
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (!ReferenceEquals(typesInfo.Type, typeof(XpandHolidayCalendar))) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.ViewMessage + " " + Captions.HeaderChooseFromListCollectionEditor, "1=1", "1=1",
                Captions.ViewMessageChooseFromListCollectionEditor, Position.Bottom) { View = ChooseFromListCollectionEditor_DetailView };
            yield return new AdditionalViewControlsRuleAttribute(Module.Captions.Header + " " + Captions.HeaderChooseFromListCollectionEditor, "1=1", "1=1",
                Captions.HeaderChooseFromListCollectionEditor, Position.Top) { View = ChooseFromListCollectionEditor_DetailView };
            yield return new CloneViewAttribute(CloneViewType.DetailView, ChooseFromListCollectionEditor_DetailView);
            var xpandNavigationItemAttribute = new XpandNavigationItemAttribute(Module.Captions.PropertyEditors + "List editors", ChooseFromListCollectionEditor_DetailView);
            yield return xpandNavigationItemAttribute;
            yield return new DisplayFeatureModelAttribute(ChooseFromListCollectionEditor_DetailView);
            yield return new WhatsNewAttribute(new DateTime(2011, 1, 28), xpandNavigationItemAttribute);
        }
    }
}
