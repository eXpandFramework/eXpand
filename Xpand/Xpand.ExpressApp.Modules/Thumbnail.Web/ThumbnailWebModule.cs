using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Thumbnail.Web
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

    public sealed class ThumbnailWebModule : XpandModuleBase{
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelListView,IModelListViewThumbnailWeb>();
        }
    }
}
