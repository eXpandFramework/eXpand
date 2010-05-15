using DevExpress.ExpressApp;
using DevExpress.ExpressApp.InfoGenerators;
using eXpand.NCarousel;
using eXpand.Persistent.Base.NCarousel;

namespace eXpand.ExpressApp.NCarousel.Web
{
    public sealed partial class NCarouselWebModule : ModuleBase
    {
        public const string NCarouselAttributeName = "NCarousel";
        public const string WidthAttributeName = "Width";
        public const string HeightAttributeName = "Height";
        public const string VisibleItemsCountAttributeName = "VisibleItemsCount";
        public const string ButtonPositionAttributeName = "ButtonPosition";
        public NCarouselWebModule()
        {
            InitializeComponent();
        }
        public override void UpdateModel(Dictionary model)
        {
            base.UpdateModel(model);
            var listEditorForClassCustomizer =
                            new ListEditorForClassCustomizer<NCarouselListEditor>(typeof(INCarouselItem));
            listEditorForClassCustomizer.Customize(model);
        }
        public override Schema GetSchema()
        {
            string s = @"<Element Name=""Application"">
                            <Element Name=""Views"">
                                <Element Name=""ListView"">
                                    <Element Name=""" + NCarouselAttributeName + @""">
                                        <Attribute Name="""+WidthAttributeName + @"""/>
                                        <Attribute Name="""+HeightAttributeName + @"""/>
                                        <Attribute Name="""+VisibleItemsCountAttributeName + @"""/>
                                        <Attribute Name="""+ButtonPositionAttributeName + @"""/>
                                        <Attribute Name="""+typeof(Alignment).Name + @""" Choice=""{"+typeof(Alignment).FullName + @"}""/>
                                    </Element>
                                </Element>; 
                            </Element>; 
                        </Element>"; 
            return new Schema(new DictionaryXmlReader().ReadFromString(s));
        }
    }
}
