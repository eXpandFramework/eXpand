using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Utils;

namespace eXpand.ExpressApp.ModelArtifactState.Exceptions{
    [DisplayName("ConditionalArtifactState User Friendly Exseptions")]
    public class ConditionalArtifactStateExceptionResourceLocalizer : UserVisibleExceptionResourceLocalizer
    {
        protected override IXafResourceManagerParameters GetXafResourceManagerParameters()
        {
            var path = new DictionaryPath
                       {
                           new DictionaryPathItem(CaptionHelper.localizationGroupNodeName,
                                                  CaptionHelper.localizationGroupNameAttribute, "Exceptions"),
                           new DictionaryPathItem(CaptionHelper.localizationGroupNodeName,
                                                  CaptionHelper.localizationGroupNameAttribute,
                                                  "UserVisibleExceptions"),
                           new DictionaryPathItem(CaptionHelper.localizationGroupNodeName,
                                                  CaptionHelper.localizationGroupNameAttribute,
                                                  "ConditionalEditorState")
                       };
            return new XafResourceManagerParameters(
                null,
                path,
                "DevExpress.ExpressApp.ConditionalEditorState.LocalizationResources",
                String.Empty,
                GetType().Assembly
                );
        }

        public override void Activate()
        {
            ExceptionLocalizerTemplate
                <ConditionalArtifactStateExceptionResourceLocalizer, ConditionalArtifactStateExceptionId>.Instance = this;
        }
    }
}