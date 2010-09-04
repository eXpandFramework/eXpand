using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Logic.Conditional.Logic;

namespace Xpand.ExpressApp.ConditionalDetailViews.Logic
{
    public interface IConditionalDetailViewRule : IConditionalLogicRule
    {
        [DataSourceProperty("DetailViews")]
        [Category("Data")]
        [Required]
        IModelDetailView DetailView { get; set; }
    }
}
