using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using Machine.Specifications;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.Base.General;

namespace Xpand.Tests.Xpand {
    [Subject(typeof(XPObjectSpace), "findobject")]
    public class When_trying_to_find_a_bussiness_object_from_an_interface_given_an_expression : With_In_Memory_DataStore {
        static IPersistentClassInfo _persistentClassInfo;

        static Expression<Func<IPersistentClassInfo, bool>> _expression;

        Establish context = () => {
            XafTypesInfo.Instance.RegisterEntity(typeof(PersistentClassInfo));
            var persistentClassInfo = XPObjectSpace.CreateObject<PersistentClassInfo>();
            persistentClassInfo.Name = "test";
        };

        Because of = () => {
            _persistentClassInfo = XPObjectSpace.FindObject<IPersistentClassInfo>(info => info.Name == "test", PersistentCriteriaEvaluationBehavior.InTransaction);
        };

        It should_find_the_object = () => _persistentClassInfo.ShouldNotBeNull();
    }
}
