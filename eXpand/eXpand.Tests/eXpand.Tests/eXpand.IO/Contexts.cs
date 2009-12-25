using System;
using System.Diagnostics;
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
    public abstract class With_Customer_Orders_Serialization_Config:With_Isolations {
        protected static Type OrderType;
        protected static ObjectSpace ObjectSpace;
        protected static Type CustomerType;
        protected static SerializationConfiguration SerializationConfiguration;

        Establish context = () => {
//            Isolate.Fake.WCTypesInfo();
//            var persistentAssemblyInfoType = TypesInfo.Instance.PersistentAssemblyInfoType;
            var artifactHandler = new TestAppLication<ClassInfoGraphNode>().Setup();
            ObjectSpace = artifactHandler.ObjectSpace;
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(ObjectSpace,"a"+ Guid.NewGuid().ToString().Replace("-",""));
            var classHandler = persistentAssemblyBuilder.CreateClasses(new[] { "Customer", "Order" });
            classHandler.CreateRefenceMembers(info => info.Name == "Customer" ? new[] { typeof(User) } : null);
            classHandler.CreateRefenceMembers(info => info.Name == "Order" ? info.PersistentAssemblyInfo.PersistentClassInfos.Where(classInfo => classInfo.Name == "Customer") : null);
            classHandler.CreateSimpleMembers<string>(persistentClassInfo => persistentClassInfo.Name == "Customer" ? new[] { "Name" } : null);
            ObjectSpace.CommitChanges();
            Type compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder, Path.GetDirectoryName(Application.ExecutablePath));
            if (persistentAssemblyBuilder.PersistentAssemblyInfo.CompileErrors!= null)
                Debug.Print("");
            CustomerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "Customer").Single();
            OrderType = compileModule.Assembly.GetTypes().Where(type => type.Name == "Order").Single();
            XafTypesInfo.Instance.CreateCollection(typeof (User), CustomerType, "User",XafTypesInfo.XpoTypeInfoSource.XPDictionary);
            SerializationConfiguration = new SerializationConfiguration(artifactHandler.UnitOfWork) { TypeToSerialize = CustomerType };
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