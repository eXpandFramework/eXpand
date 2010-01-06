using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using eXpand.ExpressApp.IO;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using eXpand.Persistent.BaseImpl.ImportExport;
using eXpand.Tests.eXpand.WorldCreator;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using eXpand.ExpressApp.Core;

namespace eXpand.Tests.eXpand.IO {
    public abstract class With_Customer_Orders:With_Isolations {
        protected static Type OrderType;
        protected static ObjectSpace ObjectSpace;
        protected static Type CustomerType;

        Establish context = () => {
            var artifactHandler = new TestAppLication<ClassInfoGraphNode>().Setup();
            ObjectSpace = artifactHandler.ObjectSpace;
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(ObjectSpace,"a"+ Guid.NewGuid().ToString().Replace("-",""));
            var classHandler = persistentAssemblyBuilder.CreateClasses(new[] { "Customer", "Order" });
            classHandler.CreateReferenceMembers(info => info.Name == "Customer" ? new[] { typeof(User) } : null, true);
            classHandler.CreateReferenceMembers(info => info.Name == "Order" ? info.PersistentAssemblyInfo.PersistentClassInfos.Where(classInfo => classInfo.Name == "Customer") : null,true);
            classHandler.CreateSimpleMembers<string>(persistentClassInfo => persistentClassInfo.Name == "Customer" ? new[] { "Name" } : null);
            ObjectSpace.CommitChanges();
            Type compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            CustomerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "Customer").Single();
            OrderType = compileModule.Assembly.GetTypes().Where(type => type.Name == "Order").Single();
            XafTypesInfo.Instance.CreateCollection(typeof (User), CustomerType, "User",XafTypesInfo.XpoTypeInfoSource.XPDictionary);
        };
    }
    public abstract class With_Isolations:WorldCreator.With_Isolations
    {
        
        protected static Func<Type[]> IOArtifacts;

        Establish context = () =>
        {
            IOArtifacts = () => new[] { typeof(IOModule) };
            Isolate.Fake.IOTypesInfo();
        };
    }

}