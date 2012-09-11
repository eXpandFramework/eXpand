using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using Xpand.Persistent.Base.General;

namespace Xpand.Tests.Xpand.Xpo {

    [ObjectToViewAttribute("RootView")]
    public class RootObject : BaseObject {
        public RootObject(Session session)
            : base(session) {
        }
        [ExplicitLoading]
        public Ref2Object RootRef2Object { get; set; }
        [ExplicitLoading]
        public Ref1Object RootRef1Object { get; set; }
        [ExplicitLoading]
        public Ref1Object RootRef3Object { get; set; }

    }

    [DefaultProperty("DefaultProperty")]
    public class Ref1Object : BaseObject {
        public Ref1Object(Session session)
            : base(session) {
        }

        public string DefaultProperty { get; set; }
        public double Ref1ObjectDouble { get; set; }
        public double Ref1ObjectPropertyName { get; set; }
    }
    [DefaultProperty("DefaultProperty2")]
    public class Ref2Object : BaseRef {
        public Ref2Object(Session session)
            : base(session) {
        }

        public string Ref2ObjectName { get; set; }

        public int DefaultProperty2 { get; set; }
    }

    public class BaseRef : BaseObject {
        public BaseRef(Session session)
            : base(session) {
        }
    }

    public class With_Root_Object {
        Establish context = () => {
            XpoTypesInfoHelper.ForceInitialize();
            XafTypesInfo.Instance.RegisterEntity(typeof(RootObject));
            XafTypesInfo.Instance.RegisterEntity(typeof(Ref1Object));
            XafTypesInfo.Instance.RegisterEntity(typeof(Ref2Object));
        };
    }
    [Subject(typeof(ObjectToViewMapper))]
    public class When_the_Type_for_the_view_is_created : With_Root_Object {
        static ObjectToViewMapper _objectToViewMapper;
        static string _generateSql;
        static ITypeInfo _viewTypeInfo;
        const int oidmember = 2;

        Establish context = () => {
            _objectToViewMapper = new ObjectToViewMapper();
        };

        Because of = () => _objectToViewMapper.BuildTypeInfos();

        It should_create_a_typeinfo_with_the_name_of_the_view = () => {
            var type = ReflectionHelper.GetType("RootView");
            type.ShouldNotBeNull();
            _viewTypeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
        };

        It should_be_aN_XpLiteObject = () => typeof(XPLiteObject).IsAssignableFrom(_viewTypeInfo.Type).ShouldBeTrue();
        It should_have_the_same_number_of_properties = () => _viewTypeInfo.OwnMembers.Count().ShouldEqual(2 + oidmember);

        It should_have_a_keymember =
            () => _viewTypeInfo.FindMember("Oid").FindAttribute<KeyAttribute>().ShouldNotBeNull();
        It should_have_as_type_in_each_proeprty_the_default_property_type = () => {
            _viewTypeInfo.FindMember("RootRef1Object").MemberType.ShouldEqual(typeof(string));
            _viewTypeInfo.FindMember("RootRef2Object").MemberType.ShouldEqual(typeof(int));
        };
    }

    [Subject(typeof(ObjectToViewMapper))]
    public class When_The_Sql_Is_Generated : With_Root_Object {
        static string sql;
        static IList<ITypeInfo> _typeInfos;
        static ObjectToViewMapper _objectToViewMapper;

        Establish context = () => {
            _objectToViewMapper = new ObjectToViewMapper();
            _typeInfos = _objectToViewMapper.BuildTypeInfos();
        };

        Because of = () => {
            var dbConnection = Isolate.Fake.Instance<SqlConnection>();
            sql = _objectToViewMapper.GenerateSql(new MSSqlConnectionProvider(dbConnection, AutoCreateOption.None), _typeInfos)[_typeInfos[0]];
        };

        It should_return_the_default_properties_of_ref_objects_having_as_alias_their_typename = () => {
            sql.ShouldNotBeNull();
            sql.Contains(@"N1.""DefaultProperty2"" AS RootRef2Object").ShouldBeTrue();
            sql.Contains(@"N3.""DefaultProperty"" AS RootRef1Object").ShouldBeTrue();
            sql.Contains(@"N3.""DefaultProperty"" AS RootRef3Object").ShouldBeTrue();
        };
    }
}
