using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using Alignment = Xpand.NCarousel.Alignment;

namespace Xpand.ExpressApp.NCarousel.Web {
    public interface IModelListViewNCarousel : IModelNode {
        IModelNCarousel NCarousel { get; }
    }

    public interface IModelNCarousel : IModelNode {
        bool AllowOverride { get; set; }
        bool HideImages { get; set; }
        string ContainerStyle { get; set; }
        string ClipStyle { get; set; }
        string ItemStyle { get; set; }
        string ButtonStyle { get; set; }
        Alignment Alignment { get; set; }

        [DefaultValue(true)]
        bool UseNoImage { get; set; }
    }
    [ToolboxBitmap(typeof(NCarouselWebModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class NCarouselWebModule : XpandModuleBase {


        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelListView, IModelListViewNCarousel>();
        }
    }
}
