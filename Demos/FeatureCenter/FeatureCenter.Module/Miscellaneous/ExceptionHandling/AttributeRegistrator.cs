using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Miscellaneous.ExceptionHandling
{
    public class AttributeRegistrator:Xpand.ExpressApp.Core.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(ExceptionHandlingObject)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderExceptionHandling, "1=1", "1=1", Captions.ViewMessageExceptionHandling, Position.Bottom) { ViewType = ViewType.DetailView, View = "ExceptionHandling_DetailView" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderExceptionHandling, "1=1", "1=1", Captions.HeaderExceptionHandling, Position.Top) { View = "ExceptionHandling_DetailView" };
            yield return new CloneViewAttribute(CloneViewType.DetailView, "ExceptionHandling_DetailView");
            yield return new XpandNavigationItemAttribute(Captions.Miscellaneous + "ExceptionHandling", "ExceptionHandling_DetailView");
        }
    }
}
