using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.SystemModule;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.Persistent.Base.General.Controllers {
    public interface IModelOptionsCollectionEditMode : IModelOptionsWeb {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [DefaultValue(ViewEditMode.Edit)]
        new ViewEditMode CollectionsEditMode { get; set; }
    }

}
