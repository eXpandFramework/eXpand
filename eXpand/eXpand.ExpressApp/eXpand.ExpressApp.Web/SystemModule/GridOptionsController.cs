using DevExpress.ExpressApp;
using DevExpress.Web.ASPxGridView;

namespace eXpand.ExpressApp.Web.SystemModule
{
    public partial class GridOptionsController : ExpressApp.SystemModule.GridOptionsController{
        public GridOptionsController(){
            InitializeComponent();
            RegisterActions(components);
        }

        public override Schema GetSchema(){
            string schema = @"<Element Name=""Application"">
                    <Element Name=""Views"" >
                        <Element Name=""ListView"" >
                            <Element Name=""ASPxGridView"" >
                                <Element Name=""Settings"" >
                                    " + GetSettingsSchema(typeof(ASPxGridViewSettings)) + @"
                                </Element>
                                <Element Name=""SettingsBehaviour"" >
                                    " + GetSettingsSchema(typeof(ASPxGridViewBehaviorSettings)) + @"
                                </Element>
                                <Element Name=""SettingsCookies"" >
                                    " + GetSettingsSchema(typeof(ASPxGridViewCookiesSettings)) + @"
                                </Element>
                                <Element Name=""SettingsCustomizationWindow"" >
                                    " + GetSettingsSchema(typeof(ASPxGridViewCustomizationWindowSettings)) + @"
                                </Element>
                                <Element Name=""SettingsDetail"" >
                                    " + GetSettingsSchema(typeof(ASPxGridViewDetailSettings)) + @"
                                </Element>
                                <Element Name=""SettingsLoadingPanel"" >
                                    " + GetSettingsSchema(typeof(ASPxGridViewLoadingPanelSettings)) + @"
                                </Element>
                                <Element Name=""SettingsPager"" >
                                    " + GetSettingsSchema(typeof(ASPxGridViewPagerSettings)) + @"
                                </Element>
                                <Element Name=""SettingsText"" >
                                    " + GetSettingsSchema(typeof(ASPxGridViewTextSettings)) + @"
                                </Element>
                            </Element>
                        </Element>
                    </Element>
                </Element>";

            return new Schema(new DictionaryXmlReader().ReadFromString(schema));
        }

        protected override string RootSettingsNode {
            get { return "ASPxGridView"; }
        }
    }
}
