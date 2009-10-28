using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator;
using eXpand.ExpressApp.WorldCreator.ClassTypeBuilder;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using MbUnit.Framework;
using eXpand.ExpressApp.Core;

namespace eXpand.Tests.WorldCreator
{
    [TestFixture]
    public class SynChronizing_Model:XpandBaseFixture
    {
        [Test]
        public void Test() {
            var wrapper = new ApplicationNodeWrapper(new Dictionary(new DictionaryNode(ApplicationNodeWrapper.NodeName), Schema.GetCommonSchema()));
            wrapper.Load(typeof(Person));
            wrapper.Load(typeof(User));
            var info = new PersistentClassInfo(Session.DefaultSession) {Name = "Customer", BaseType = typeof (User)};
            var define = PersistentClassTypeBuilder.BuildClass().WithAssemblyName(info.AssemblyName).Define(info);
            Session.DefaultSession.UpdateSchema(define);
            var module = new WorldCreatorModule();

            module.SyncModel(info, wrapper.Node.Dictionary);

            Assert.AreEqual(0, ((ListViewInfoNodeWrapper)wrapper.Views.FindViewById("Customer_ListView")).Columns.Items.Count);
        }
    }
}
