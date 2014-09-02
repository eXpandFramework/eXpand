using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.Security {
    public interface IModelRegistrationActivation:IModelNode {
        [Category("Activation")]
        [DefaultValue(true)]
        bool ActivateUser { get; set; }

        [DataSourceProperty("ActivationIdMembers")]
        [Description("For this to work you need to inherit from XpandWebApplication")]
        [Category("Activation")]
        IModelMember ActivationIdMember { get; set; }

        [Localizable(true)]
        [DefaultValue("<b>Activation successful!</b>")]
        [Category("Activation")]
        string SuccessFulActivationOutput { get; set; }

        [DefaultValue("/")]
        [Category("Activation")]
        string SuccessFulActivationReturnUrl { get; set; }

    }

    [DomainLogic(typeof(IModelRegistrationActivation))]
    public class ModelRegistrationActivationDomainLogic{
        public static IModelList<IModelMember> Get_ActivationIdMembers(IModelRegistrationActivation modelRegistration){
            var userModelClass = modelRegistration.GetValue<IModelClass>("UserModelClass");
            return userModelClass == null ? new CalculatedModelNodeList<IModelMember>()
                       : new CalculatedModelNodeList<IModelMember>(userModelClass.AllMembers.Where(
                           member => member.MemberInfo.MemberType == typeof(string)));
        }

        public static IModelMember Get_ActivationIdMember(IModelRegistrationActivation modelRegistration){
            var userModelClass = modelRegistration.GetValue<IModelClass>("UserModelClass");
            return userModelClass != null ? userModelClass.AllMembers.FirstOrDefault(member => member.Name.StartsWith("Activation")) : null;
        }
    }

}