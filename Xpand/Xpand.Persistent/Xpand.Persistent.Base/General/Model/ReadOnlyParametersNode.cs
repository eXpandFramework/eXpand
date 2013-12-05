using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.ModelDifference;
using Fasterflect;
using System.Linq;

namespace Xpand.Persistent.Base.General.Model {
    public interface IModelApplicationReadonlyParameters {
        IModelReadOnlyParameters ReadOnlyParameters { get; }
    }
    [ModelReadOnly(typeof(ModelReadOnlyCalculator))]
    [ModelNodesGenerator(typeof(ModelReadOnlyParametersNodesGenerator))]
    public interface IModelReadOnlyParameters:IModelList<IModelReadOnlyParameter>,IModelNode {
         
    }

    public interface IModelReadOnlyParameter:IModelNode {
        [TypeConverter(typeof (StringToTypeConverter))]
        [Browsable(false)]
        Type Type { get; set; }
    }

    public class ModelReadOnlyParametersNodesGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
            var typesInfo = ((IModelTypesInfoProvider) node.Application).TypesInfo;
            var typeInfo = typesInfo.FindTypeInfo<ReadOnlyParameter>();
            foreach (var descendant in typeInfo.Descendants.Where(info => !info.IsAbstract)) {
                var readOnlyParameter = (ReadOnlyParameter) descendant.Type.CreateInstance();
                var modelReadOnlyParameter = node.AddNode<IModelReadOnlyParameter>(readOnlyParameter.Name);
                modelReadOnlyParameter.Type = descendant.Type;
            }
        }
    }

}
