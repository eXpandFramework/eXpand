using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Validation;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Validation{
    [CodeRule]
    public class XmlContentCodeRule : RuleBase<IXpoModelDifference> {
        public const string MDOXmlContentContext = "MDOXmlContentContext";

        public override ReadOnlyCollection<string> UsedProperties
            => new ReadOnlyCollection<string>(new[]{nameof(IXpoModelDifference.XmlContent)});

        protected override bool IsValidInternal(IXpoModelDifference target, out string errorMessageTemplate) {
            try {
                if (!string.IsNullOrWhiteSpace(target.XmlContent)){
                    var stringReader = new StringReader(target.XmlContent);
                    XDocument.Load(stringReader);
                    var modelApplication =
                        ((ModelApplicationBase) CaptionHelper.ApplicationModel).CreatorInstance.CreateModelApplication();
                    new ModelXmlReader().ReadFromString(modelApplication, "", target.XmlContent);
                }
                errorMessageTemplate = null;
                return true;
            }
            catch (Exception e) {
                errorMessageTemplate = e.Message;
            }
            return false;
        }

        public XmlContentCodeRule() : base("MDO_XmlContext_validation", ContextIdentifier.Save+ ContextIdentifiers.Separator+MDOXmlContentContext) {
        }

        public XmlContentCodeRule(IRuleBaseProperties properties) : base(properties) {
        }
    }


}