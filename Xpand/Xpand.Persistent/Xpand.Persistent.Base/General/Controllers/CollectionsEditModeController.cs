using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.Persistent.Base.General.Controllers {
    public interface IModelOptionsCollectionEditMode:IModelNode {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [DefaultValue(ViewEditMode.Edit)]
#pragma warning disable 109
        new ViewEditMode CollectionsEditMode { get; set; }
#pragma warning restore 109
    }

}
