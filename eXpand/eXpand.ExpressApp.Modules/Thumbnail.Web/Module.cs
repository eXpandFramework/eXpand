using DevExpress.ExpressApp;
using DevExpress.ExpressApp.InfoGenerators;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Thumbnail.Web
{
    public sealed partial class ThumbnailWebModule : ModuleBase
    {
        public ThumbnailWebModule()
        {
            InitializeComponent();
        }
        public override void UpdateModel(Dictionary model)
        {
            base.UpdateModel(model);
            var listEditorForClassCustomizer =
                            new ListEditorForClassCustomizer<ThumbnailListEditor>(typeof(IThumbNailItem));
            listEditorForClassCustomizer.Customize(model);
        }

    }
}
