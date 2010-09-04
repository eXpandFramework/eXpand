using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Tests;
using Xpand.ExpressApp.IO;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.Core;
using Xpand.Tests.Xpand.WorldCreator;

namespace Xpand.Tests.Xpand.IO {
    public abstract class With_Customer_Orders:With_Isolations {
        protected static Session Session;
        protected static Type OrderType;
        protected static ObjectSpace ObjectSpace;
        protected static Type CustomerType;

        Establish context = () => {
//            var artifactHandler = new TestAppLication<ClassInfoGraphNode>().Setup();
            ObjectSpace = ObjectSpaceInMemory.CreateNew();
            Session = ObjectSpace.Session;
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
    public interface IOrder
    {
    }
    public interface ICustomer
    {
    }
    public class ImagePropertyObject : BaseObject
    {
        public ImagePropertyObject(Session session)
            : base(session)
        {
        }
        private readonly XPDelayedProperty _Photo = new XPDelayedProperty();
        [ValueConverter(typeof(ImageValueConverter))]
        [Delayed("_Photo")]
        public Image Photo
        {
            get
            {
                return (Image)_Photo.Value;
            }
            set
            {
                _Photo.Value = value;
                OnChanged("_Photo");
            }
        }
    }
    public class DateTimePropertyObject : BaseObject
    {
        public DateTimePropertyObject(Session session)
            : base(session)
        {
        }

        DateTime _date;

        public DateTime Date
        {
            get { return _date; }
            set { SetPropertyValue("Date", ref _date, value); }
        }
    }

    public abstract class With_Isolations : With_WC_types
    {
        public static string GetUniqueAssemblyName()
        {
            return "a" + Guid.NewGuid().ToString().Replace("-", "");
        }

        protected static Func<Type[]> IOArtifacts;

        Establish context = () =>
        {
            IOArtifacts = () => new[] { typeof(IOModule) };
            Isolate.Fake.IOTypesInfo();
        };
    }

}