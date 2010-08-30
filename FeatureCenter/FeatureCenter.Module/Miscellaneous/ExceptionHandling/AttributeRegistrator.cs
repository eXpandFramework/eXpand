using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;
using NavigationItemAttribute = eXpand.ExpressApp.Attributes.NavigationItemAttribute;

namespace FeatureCenter.Module.Miscellaneous.ExceptionHandling
{
    public class AttributeRegistrator:Module.AttributeRegistrator
    {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type != typeof(ExceptionHandlingObject)) yield break;
            yield return new AdditionalViewControlsRuleAttribute(Captions.ViewMessage + " " + Captions.HeaderExceptionHandling, "1=1", "1=1", Captions.ViewMessageExceptionHandling, Position.Bottom) { ViewType = ViewType.DetailView, View = "ExceptionHandling_DetailView" };
            yield return new AdditionalViewControlsRuleAttribute(Captions.Header + " " + Captions.HeaderExceptionHandling, "1=1", "1=1", Captions.HeaderExceptionHandling, Position.Top) { View = "ExceptionHandling_DetailView" };
            yield return new CloneViewAttribute(CloneViewType.DetailView, "ExceptionHandling_DetailView");
            yield return new NavigationItemAttribute(Captions.Miscellaneous + "ExceptionHandling", "ExceptionHandling_DetailView");
        }
    }
}
