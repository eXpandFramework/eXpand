using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Thumbnail.Web
{
    public interface IModelListViewThumbnailWeb:IModelListView
    {
        IModelThumbnailWeb ThumbnailWeb { get; set; }    
    }

    public interface IModelThumbnailWeb:IModelNode {
        [DefaultValue("inline")]
        string DisplayStyle { get; set; }
        bool HideImages { get; set; }
    }

    public sealed class ThumbnailWebModule : ModuleBase{
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelListView,IModelListViewThumbnailWeb>();
        }

        protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors){
            editorDescriptors.Add(new ListEditorDescriptor(new AliasAndEditorTypeRegistration(typeof(ThumbnailListEditor).Name,
                                                                            typeof(IPictureItem), true,
                                                                            typeof(ThumbnailListEditor), true)));
        }

    }
}
