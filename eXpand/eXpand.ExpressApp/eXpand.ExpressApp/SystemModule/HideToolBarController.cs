using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.SystemModule {
    public abstract class HideToolBarController : ViewController {
        public const string HideToolBarAttributeName = "HideToolBar";
        public override Schema GetSchema()
        {
            const string CommonTypeInfos =
                @"<Element Name=""Application"">
                    <Element Name=""Views"" >
                        <Element Name=""ListView"" >
                            <Attribute Name=""" +HideToolBarAttributeName +@""" Choice=""True,False""/>
                        </Element>
                        <Element Name=""DetailView"" >
                            <Attribute Name=""" + HideToolBarAttributeName + @""" Choice=""True,False""/>
                        </Element>
                    </Element>
                </Element>";

            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }
    }
}