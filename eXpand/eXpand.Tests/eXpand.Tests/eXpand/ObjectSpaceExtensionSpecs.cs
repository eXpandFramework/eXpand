using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;
using eXpand.ExpressApp.Core;

namespace eXpand.Tests.eXpand
{
    public class When_trying_to_find_a_bussiness_object_from_an_interface_given_a_lamda:With_In_Memory_DataStore {
        static IPersistentClassInfo _persistentClassInfo;

        Establish context = () => {
            new PersistentClassInfo(UnitOfWork) {Name = "Test"};
            XafTypesInfo.Instance.RegisterEntity(typeof(PersistentClassInfo));    
        };

        Because of = () => { _persistentClassInfo = ObjectSpace.FindObject<IPersistentClassInfo>(info => info.Name == "test", PersistentCriteriaEvaluationBehavior.InTransaction); };

        It should_find_the_object = () => _persistentClassInfo.ShouldNotBeNull();
    }
}
