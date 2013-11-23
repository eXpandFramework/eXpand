using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.MasterDetail.Logic {
    public interface IMasterDetailRule : ILogicRule {

        [DataSourceProperty("ChildListViews")]
        [Category("MasterDetail")]
        [Required]
        IModelListView ChildListView { get; set; }


        [Category("MasterDetail")]
        [Required]
        [Description("The collection member that is going to be used as child collection")]
        [DataSourceProperty("CollectionMembers")]
        [RefreshProperties(RefreshProperties.All)]
        IModelMember CollectionMember { get; set; }
        
    }
}