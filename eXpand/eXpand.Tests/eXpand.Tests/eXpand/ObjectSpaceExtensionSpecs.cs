using System;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Utils.Linq;
using Machine.Specifications;
using eXpand.ExpressApp.Core;

namespace eXpand.Tests.eXpand
{
    [Subject(typeof(ObjectSpace),"findobject")]
    public class When_trying_to_find_a_bussiness_object_from_an_interface_given_an_expression:With_In_Memory_DataStore {
        static IPersistentClassInfo _persistentClassInfo;
        static Expression<Func<IPersistentClassInfo, bool>>  _expression;
        Establish context = () => {
            new PersistentClassInfo(UnitOfWork) {Name = "Test"};
            XafTypesInfo.Instance.RegisterEntity(typeof(PersistentClassInfo));    
        };

        Because of = () => {

            _expression = info => info.Name == "test";
            var transform = (Expression<Func<PersistentClassInfo, bool>>) new ExpressionTransformer().Transform(typeof(PersistentClassInfo), _expression);
            Expression<Func<PersistentClassInfo, bool>> expression = classInfo => classInfo.Name=="test";
            CriteriaOperator transformExpression = new XPQuery<PersistentClassInfo>(UnitOfWork).TransformExpression(expression);

            _persistentClassInfo = ObjectSpace.FindObject<IPersistentClassInfo>(info => info.Name == "test", PersistentCriteriaEvaluationBehavior.InTransaction);
        };

        It should_find_the_object = () => _persistentClassInfo.ShouldNotBeNull();
    }
}
