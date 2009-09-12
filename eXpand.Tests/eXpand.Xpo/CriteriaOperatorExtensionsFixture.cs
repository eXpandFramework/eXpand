using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.ModelArtifactState.Attributes;
using eXpand.ExpressApp.ModelArtifactState.Security.Permissions;
using eXpand.Xpo;
using eXpand.Xpo.PersistentMetaData;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using System.Linq;

namespace Fixtures.eXpand.Xpo
{
//    [ArtifactStateRule("fff", null,EditorState.Disabled, null,ViewType.DetailView)]
//    [ArtifactStateRule(null, null,EditorState.Disabled, null,ViewType.Any)]
////    [ControllerStateRuleAttribute(typeof(IsolatedAttribute), Nesting.Any, null, null, ViewType.Any, "ddd")]
////    [ControllerStateRuleAttribute(typeof(Isolate), Nesting.Any, null, null, ViewType.Any, "ddd")]
//    internal class MyClass:XPObject
//    {
//        
//    }
    [TestFixture]
    public class CriteriaOperatorExtensionsFixture
    {
//        [Test]
//        [Isolated]
//        public void Test()
//        {
//            XpoDefault.DataLayer =
//                new SimpleDataLayer(new InMemoryDataStore(new DataSet(), AutoCreateOption.DatabaseAndSchema));
//            var permission = new ControllerStateRuleAttribute(GetType(), Nesting.Any, null, null, ViewType.Any, null);
//            XPClassInfo classInfo = Session.DefaultSession.GetClassInfo(typeof(MyClass));
////            classInfo.AddAttribute(permission);
//            classInfo.AddAttribute(new ControllerStateRuleAttribute(GetType(), Nesting.Any, null, null, ViewType.Any, "ddd"));
//            ;
//            ITypeInfo value = XafTypesInfo.Instance.FindTypeInfo(typeof(MyClass));
//            Assert.IsNotNull(value);
//            Assert.AreEqual(2, value.FindAttributes<EditorStateRuleAttribute>().Count());
////            Assert.AreEqual(2, value);
////            Assert.AreEqual(2, classInfo.Attributes.Count());
//        }
        [Test]
        public void Parsing_Collection_Strings()
        {
            XpoDefault.DataLayer =
                new SimpleDataLayer(new InMemoryDataStore(new DataSet(), AutoCreateOption.DatabaseAndSchema));
            List<PersistentClassInfo> persistentClassInfos= PersistentClassInfo.CreateOneToMany("Customer", "Order");
            persistentClassInfos.Add(PersistentClassInfo.CreateOneToMany(persistentClassInfos[1],"OrderLine"));
            persistentClassInfos.Add(PersistentClassInfo.CreateOneToMany(persistentClassInfos[2],"Sale"));

            persistentClassInfos[3].OwnMembers.Add(new PersistentCoreTypeMemberInfo { Name = "Ammount", TypeName = typeof(int).FullName });
            PersistentClassInfo.FillDictionary(XpoDefault.DataLayer.Dictionary, persistentClassInfos);

            CriteriaOperator @operator =
                CriteriaOperatorExtensions.Parse("Orders.OrderLines.Sales", CriteriaOperator.Parse("Ammount=0"));


            CriteriaOperator criteria = CriteriaOperator.Parse("Orders[OrderLines[Sales[Ammount=0]]]");
            
            
            Assert.AreEqual(criteria.ToString(), @operator.ToString());
            object o = Session.DefaultSession.FindObject(Session.DefaultSession.GetClassInfo("", "Customer"), criteria);
            Assert.IsNull(o);
        }
    }
}