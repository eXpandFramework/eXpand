using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Web.Model {
    public interface IModelOptionsQueryStringParameter:IModelOptions {
        [Description("XpandHttpRequestManager is required. Override WebApplication CreateHttpRequestManager method and return this.NewHttpRequestManager().")]
        IModelQueryStringParameters QueryStringParameters { get; }
    }

    [ModelNodesGenerator(typeof(ModelReadOnlyParametersNodesGenerator))]
    public interface IModelQueryStringParameters : IModelList<IModelQueryStringParameter>,IModelNode {

    }

    public interface IModelQueryStringParameter:IModelNode {
        [Required]
        string Key { get; set; }
        [DataSourceProperty("Application.ReadOnlyParameters")]
        [Required]
        IModelReadOnlyParameter ReadOnlyParameter { get; set; }
    }

    public class ModelReadOnlyParametersNodesGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
        }
    }
}