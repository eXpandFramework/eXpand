using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CThru;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.ExpressApp.WorldCreator.CThru;
using eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;

namespace eXpand.Tests.eXpand.WorldCreator
{
    public class When_validating_an_invalid_object_with_existentType_member_with_validation_rule:With_Isolations
    {
        static RuleSetValidationResult _ruleSetValidationResult;
        static XPBaseObject _customer;

        Establish context = () => {
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly();
            persistentAssemblyBuilder.CreateClasses(new[]{"Customer"});
            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            Type _customerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "Customer").Single();
            
            var session = persistentAssemblyBuilder.ObjectSpace.Session;
            new ExtendedCoreTypeMemberInfo(session) {Name = "Name",Owner = _customerType};
            persistentAssemblyBuilder.ObjectSpace.CommitChanges();
            new ExistentTypesMemberCreator().CreateMembers(session,TypesInfo.Instance);
            persistentAssemblyBuilder.ObjectSpace.CommitChanges();
            XafTypesInfo.Instance.RefreshInfo(_customerType);
            var memberInfo = XafTypesInfo.CastTypeToTypeInfo(_customerType).FindMember("Name");
            memberInfo.AddAttribute(new RuleRequiredFieldAttribute(null, DefaultContexts.Save));
            _customer = (XPBaseObject)persistentAssemblyBuilder.ObjectSpace.CreateObject(_customerType);

            CThruEngine.AddAspect(new ExistentMembersEnableValidationAspect());
            CThruEngine.StartListening();
        };

        Because of = () => {
            _ruleSetValidationResult = Validator.RuleSet.ValidateTarget(_customer,ContextIdentifier.Save);
        };


        It should_retun_an_invalid_state=() =>_ruleSetValidationResult.State.ShouldEqual(ValidationState.Invalid);
        
    }

}
