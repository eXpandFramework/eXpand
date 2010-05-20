using DevExpress.ExpressApp;
using DevExpress.ExpressApp.InfoGenerators;
using eXpand.NCarousel;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.NCarousel.Web
{
    public sealed partial class NCarouselWebModule : ModuleBase
    {
        public const string AllowOverrideAttributeName = "AllowOverride";
        public const string HideImagesAttributeName = "HideImages";
        public const string NCarouselAttributeName = "NCarousel";
        public const string ContainerStyleAttributeName = "ContainerStyle";
        public const string ClipStyleAttributeName = "ClipStyle";
        public const string ItemStyleAttributeName = "ItemStyle";
        public const string ButtonStyleAttributeName = "ButtonStyle";
        public const string UseNoImageAttributeName = "UseNoImage";
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
                                        <Attribute Name=""" + AllowOverrideAttributeName + @""" Choice=""True,False""/>
                                        <Attribute Name=""" + UseNoImageAttributeName + @""" Choice=""True,False""/>
                                        <Attribute Name=""" + ContainerStyleAttributeName + @"""/>
                                        <Attribute Name=""" + ClipStyleAttributeName + @"""/>
                                        <Attribute Name=""" + ItemStyleAttributeName + @"""/>
                                        
                                        <Attribute Name=""" + ButtonStyleAttributeName + @"""/>
                                        <Attribute Name=""" + HideImagesAttributeName + @""" Choice=""True,False""/>
                                        <Attribute Name=""" + typeof(Alignment).Name + @""" Choice=""{" + typeof(Alignment).FullName + @"}""/>
                                    </Element>
                                </Element>; 
                            </Element>; 
                        </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(s));
        }

    }
}
