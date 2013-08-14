using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.MasterDetail.Logic {
    public interface IContextMasterDetailRule:IContextLogicRule,IMasterDetailRule {
         
    }
    public interface IMasterDetailRule : ILogicRule {

        [DataSourceProperty("ChildListViews")]
        [Category("Data")]
        [Required]
        IModelListView ChildListView { get; set; }


        [Category("Data")]
        [Required]
        [Description("The collection member that is going to be used as child collection")]
        [DataSourceProperty("CollectionMembers")]
        [RefreshProperties(RefreshProperties.All)]
        IModelMember CollectionMember { get; set; }
        [DefaultValue(true)]
        bool SynchronizeActions { get; set; }
    }
}