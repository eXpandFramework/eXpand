using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;

namespace Xpand.Persistent.Base.Security {
    [ModelAbstractClass]
    public interface IModelOptionsRegistration : IModelOptions {
        [ModelBrowsable(typeof(ModelOptionsRegistrationVisibilityCalculator))]
        IModelRegistrationEnabled Registration { get; }
    }

    public class ModelOptionsRegistrationVisibilityCalculator:IModelIsVisible{
        public bool IsVisible(IModelNode node, string propertyName){
            return node.Application.BOModel.Any(@class => typeof (ISecurityRole).IsAssignableFrom(@class.TypeInfo.Type));
        }
    }
}