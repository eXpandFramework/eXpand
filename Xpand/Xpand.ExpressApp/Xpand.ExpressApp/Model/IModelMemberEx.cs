using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.ExpressApp.Model {
    [ModelAbstractClass]
    public interface IModelMemberEx : IModelMember {
        [Description("Specifies the current property type."), Category("Data")]
        [TypeConverter(typeof(StringToTypeConverterExtended))]
        [ModelBrowsable(typeof(ModelTypeVisibilityCalculator))]
        new Type Type { get; set; }
    }

    public class ModelTypeVisibilityCalculator:IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            if (propertyName=="Type"&&node is IModelRuntimeOrphanedColection) {
                return false;
            }
            return true;
        }
    }
}
