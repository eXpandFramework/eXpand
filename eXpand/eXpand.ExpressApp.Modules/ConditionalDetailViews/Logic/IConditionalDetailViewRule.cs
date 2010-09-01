using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Logic.Conditional.Logic;

namespace eXpand.ExpressApp.ConditionalDetailViews.Logic
{
    public interface IConditionalDetailViewRule : IConditionalLogicRule
    {
        [DataSourceProperty("DetailViews")]
        [Category("Data")]
        [Required]
        IModelDetailView DetailView { get; set; }
    }
}
