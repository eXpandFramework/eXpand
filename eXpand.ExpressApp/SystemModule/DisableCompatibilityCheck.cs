using System.Diagnostics;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class DisableCompatibilityCheck : WindowController
    {
        public DisableCompatibilityCheck()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        [CoverageExclude]
        public override Schema GetSchema()
        {
            const string CommonTypeInfos =
                @"<Element Name=""Application"">
                    <Element Name=""Options"">
                        <Attribute Name=""DisableCompatibilityCheck"" Choice=""False,True""/>
                    </Element>
                </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }

    }
}