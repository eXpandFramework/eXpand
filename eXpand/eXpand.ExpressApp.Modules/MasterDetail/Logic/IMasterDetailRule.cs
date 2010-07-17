using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Logic.Conditional.Logic;

namespace eXpand.ExpressApp.MasterDetail.Logic {
    public interface IMasterDetailRule : IConditionalLogicRule {
        [DataSourceProperty("Application.Views")]
        [Category("Data")]
        [Required]
        IModelListView ChildListView { get; set; }


        [Category("Data")]
        [Required]
        [Description("The collection member that is going to be used as child collection")]
        [DataSourceProperty("ModelClass.AllMembers")]
        IModelMember CollectionMember { get; set; }
    }
}