using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Web.Model {
    public interface IModelOptionsCollectionEditMode : IModelOptions {
        [DefaultValue(ViewEditMode.Edit)]
        [Category("eXpand")]
        ViewEditMode CollectionsEditMode { get; set; }
    }
}