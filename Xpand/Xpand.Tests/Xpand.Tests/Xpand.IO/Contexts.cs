using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.IO;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Xpand.Persistent.BaseImpl.ImportExport;
using Xpand.Tests.Xpand.WorldCreator;
using Xpand.Persistent.Base.General;
using Xpand.Xpo.Converters.ValueConverters;

namespace Xpand.Tests.Xpand.IO {
    public abstract class With_Customer_Orders : With_Isolations {
        protected static Session Session;
        protected static Type CompileModule;
        protected static Type OrderType;
        protected static XPObjectSpace XPObjectSpace;
        protected static Type CustomerType;

        Establish context = () => {
            //            var artifactHandler = new TestAppLication<ClassInfoGraphNode>().Setup();
            XPObjectSpace = (XPObjectSpace)ObjectSpaceInMemory.CreateNew();
            Session = XPObjectSpace.Session;
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(XPObjectSpace, "a" + Guid.NewGuid().ToString().Replace("-", ""));
            var classHandler = persistentAssemblyBuilder.CreateClasses(new[] { "Customer", "Order" });
            classHandler.CreateReferenceMembers(info => info.Name == "Customer" ? new[] { typeof(User) } : null, true);
            classHandler.CreateReferenceMembers(info => info.Name == "Order" ? info.PersistentAssemblyInfo.PersistentClassInfos.Where(classInfo => classInfo.Name == "Customer") : null, true);
            classHandler.CreateSimpleMembers<string>(persistentClassInfo => persistentClassInfo.Name == "Customer" ? new[] { "Name" } : null);
            XPObjectSpace.CommitChanges();
            CompileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder.PersistentAssemblyInfo, Path.GetDirectoryName(Application.ExecutablePath));
            CustomerType = CompileModule.Assembly.GetTypes().Single(type => type.Name == "Customer");
            OrderType = CompileModule.Assembly.GetTypes().Single(type => type.Name == "Order");
            XafTypesInfo.Instance.CreateCollection(typeof(User), CustomerType, "User", XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary);


        };
    }
    public interface IOrder {
    }
    public interface ICustomer {
    }
    public class ImagePropertyObject : BaseObject {
        public ImagePropertyObject(Session session)
            : base(session) {
        }
        private readonly XPDelayedProperty _Photo = new XPDelayedProperty();
        [ValueConverter(typeof(ImageValueConverter))]
        [Delayed("_Photo")]
        public Image Photo {
            get {
                return (Image)_Photo.Value;
            }
            set {
                _Photo.Value = value;
                OnChanged("_Photo");
            }
        }
    }
    public class DateTimePropertyObject : BaseObject {
        public DateTimePropertyObject(Session session)
            : base(session) {
        }

        DateTime _date;

        public DateTime Date {
            get { return _date; }
            set { SetPropertyValue("Date", ref _date, value); }
        }
    }

    public abstract class With_Isolations : With_WC_types {

        public static string GetUniqueAssemblyName() {
            return "a" + Guid.NewGuid().ToString().Replace("-", "");
        }

        protected static Func<Type[]> IOArtifacts;

        Establish context = () => {
            XafTypesInfo.Instance.RegisterEntity(typeof(PEnumClass));
            XafTypesInfo.Instance.RegisterEntity(typeof(DateTimePropertyObject));
            XafTypesInfo.Instance.RegisterEntity(typeof(ImagePropertyObject));
            IOArtifacts = () => new[] { typeof(IOModule) };
            Isolate.Fake.IOTypesInfo();
            Type type1 = typeof(IOError);
            var types = type1.Assembly.GetTypes().Where(type => (type.Namespace + "").StartsWith(type1.Namespace + "")).ToList();
            TypesInfo.Instance.RegisterTypes(types.ToList());
            foreach (var type in types) {
                XafTypesInfo.Instance.RegisterEntity(type);
            }
        };
    }
    public enum MyEnum {
        Val1, Val2
    }

    public class PEnumClass : BaseObject {
        public PEnumClass(Session session)
            : base(session) {
        }
        private DateTime _date;
        [ValueConverter(typeof(XpandUtcDateTimeConverter))]
        public DateTime Date {
            get {
                return _date;
            }
            set {
                SetPropertyValue("Date", ref _date, value);
            }
        }
        private string _longProperty;
        [Size(SizeAttribute.Unlimited)]
        public string LongProperty {
            get {
                return _longProperty;
            }
            set {
                SetPropertyValue("LongProperty", ref _longProperty, value);
            }
        }
        private MyEnum? _myEnum;
        public MyEnum? MyEnum {
            get {
                return _myEnum;
            }
            set {
                SetPropertyValue("MyEnum", ref _myEnum, value);
            }
        }
    }

}