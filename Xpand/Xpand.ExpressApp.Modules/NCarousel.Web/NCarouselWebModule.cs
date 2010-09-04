using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General;
using Alignment = Xpand.NCarousel.Alignment;

namespace Xpand.ExpressApp.NCarousel.Web
{
    public interface IModelListViewNCarousel:IModelNode {
        IModelNCarousel NCarousel { get; set; }
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
    public sealed partial class NCarouselWebModule : ModuleBase
    {
        
        public NCarouselWebModule()
        {
            InitializeComponent();
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelListView,IModelListViewNCarousel>();
        }

        protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors){
            editorDescriptors.Add(new ListEditorDescriptor(new AliasAndEditorTypeRegistration(typeof (NCarouselListEditor).Name,
                                                                            typeof (IPictureItem), true,
                                                                            typeof (NCarouselListEditor), true)));
        }

    }
}
