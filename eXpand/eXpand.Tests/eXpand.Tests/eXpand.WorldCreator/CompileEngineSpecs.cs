using System;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Utils;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using System.Linq;

namespace eXpand.Tests.eXpand.WorldCreator
{

    [Subject(typeof(CompileEngine))][Isolated]
    public class When_cannot_compile_a_dynamic_module:With_In_Memory_DataStore {
        static IPersistentAssemblyInfo _persistentAssemblyInfo;

        Establish context = () => {
            _persistentAssemblyInfo = new PersistentAssemblyInfo(Session.DefaultSession);
            Isolate.WhenCalled(() => CodeEngine.GenerateCode(_persistentAssemblyInfo)).WillReturn("1111");
        };

        static Exception _exception;

        Because of = () => {
            _exception = Catch.Exception(() => CompileEngine.CompileModule(_persistentAssemblyInfo));
        };

        It should_swallow_exception=() => _exception.ShouldBeNull();
        It should_delegate_any_errors_to_assembly_info_compile_errors_property=() => _persistentAssemblyInfo.CompileErrors.ShouldNotBeNull();
    }
    [Subject(typeof(CompileEngine), "specs")]
    [Isolated]
    public class When_compiling_a_dynamic_assembly:With_In_Memory_DataStore
    {
        static Type type;
        static IPersistentAssemblyInfo _persistentAssemblyInfo;

        Establish context = () => {
            _persistentAssemblyInfo = Isolate.Fake.Instance<IPersistentAssemblyInfo>();
            _persistentAssemblyInfo.Name = "TestAssembly222";
            Isolate.Fake.StaticMethods(typeof (CodeEngine));
            Isolate.WhenCalled(() => CodeEngine.GenerateCode(Isolate.Fake.Instance<IPersistentClassInfo>())).WillReturn(@"public class TestClass:" +typeof(XPBaseObject).FullName+ @"{}");
        };

        Because of = () => {type=CompileEngine.CompileModule(_persistentAssemblyInfo);};

        It should_not_contain_any_compilation_error = () => _persistentAssemblyInfo.CompileErrors.ShouldEqual("");

        It should_Create_A_Dynamic_module = () => {
            type.ShouldNotBeNull();
            typeof(ModuleBase).IsAssignableFrom(type).ShouldBeTrue();
        };
    }

    [Subject(typeof(CompileEngine), "specs")]
    public class When_compiling_class_with_members:With_In_Memory_DataStore {
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        static Type _compileModule;

        Establish context = () => {
            var classCodeTemplate = new CodeTemplate(Session.DefaultSession){TemplateType = TemplateType.Class};
            classCodeTemplate.SetDefaults();
            _persistentAssemblyInfo = new PersistentAssemblyInfo(Session.DefaultSession){Name = "TestAssembly"};
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) {Name = "TestClass", CodeTemplateInfo = {TemplateInfo = classCodeTemplate},PersistentAssemblyInfo = _persistentAssemblyInfo};

            var memberCodeTemplate = new CodeTemplate(Session.DefaultSession){TemplateType = TemplateType.ReadWriteMember};
            memberCodeTemplate.SetDefaults();
            new PersistentCoreTypeMemberInfo(Session.DefaultSession){Name = "Property",CodeTemplateInfo = {TemplateInfo = memberCodeTemplate},Owner = persistentClassInfo};
            new PersistentReferenceMemberInfo(Session.DefaultSession){Name = "RefProperty",CodeTemplateInfo = {TemplateInfo = memberCodeTemplate},Owner = persistentClassInfo,ReferenceType = typeof(User)};
            new PersistentCollectionMemberInfo(Session.DefaultSession) { Name = "CollProperty", CodeTemplateInfo = { TemplateInfo = memberCodeTemplate }, Owner = persistentClassInfo, CollectionType = typeof(User) };
        };

        Because of = () => { _compileModule = CompileEngine.CompileModule(_persistentAssemblyInfo); };

        It should_have_no_compile_errors = () => _persistentAssemblyInfo.CompileErrors.ShouldBeNull();
        It should_generate_class_type = () => _compileModule.Assembly.GetTypes().Count().ShouldEqual(2);

        It should_have_those_members_as_proeprties =
            () => {
                PropertyInfo[] propertyInfos =
                    _compileModule.Assembly.GetTypes().Where(type => type.FullName.IndexOf("TestClass") > -1).Single().
                        GetProperties();
                propertyInfos.Where(info => info.Name == "Property").FirstOrDefault().ShouldNotBeNull();
                propertyInfos.Where(info => info.Name == "RefProperty").FirstOrDefault().ShouldNotBeNull();
                propertyInfos.Where(info => info.Name == "CollProperty").FirstOrDefault().ShouldNotBeNull();
            };
    }
    [Subject(typeof(CompileEngine), "specs")]
    public class When_PersistentClassInfo_BaseType_Belongs_to_different_assemmbly:With_In_Memory_DataStore {
        static IPersistentAssemblyInfo _persistentAssemblyInfo;

        static Type _compileModule;

        Establish context = () => {
            _persistentAssemblyInfo = new PersistentAssemblyInfo(Session.DefaultSession){Name = "TestAssembly"};
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession){Name = "TestClass",BaseType = typeof(User)};
            var codeTemplate = new CodeTemplate(Session.DefaultSession){TemplateType = TemplateType.Class};
            codeTemplate.SetDefaults();
            persistentClassInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
            _persistentAssemblyInfo.PersistentClassInfos.Add(persistentClassInfo);
            Assembly.GetAssembly(typeof (ObjectMerger));
        };

        Because of = () => {
            _compileModule = CompileEngine.CompileModule(_persistentAssemblyInfo);
        };

        It should_compile_with_no_errors = () => {
            _persistentAssemblyInfo.CompileErrors.ShouldBeNull();
            _compileModule.ShouldNotBeNull();
        };

        It should_create_a_baseType_descenant_class =
            () =>
            _compileModule.Assembly.GetTypes().Where(type => typeof (User).IsAssignableFrom(type)).FirstOrDefault().
                ShouldNotBeNull();
    }
    [Subject(typeof(CompileEngine), "specs")]
    public class When_PersistentClassInfo_BaseType_Belongs_to_same_assemmbly:With_In_Memory_DataStore {
        static IPersistentAssemblyInfo _persistentAssemblyInfo;

        static Type _compileModule;
        Establish context = () => {
            _persistentAssemblyInfo = new PersistentAssemblyInfo(Session.DefaultSession){Name = "TestAssembly"};
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession){Name = "TestClass",BaseType = typeof(User)};
            var codeTemplate = new CodeTemplate(Session.DefaultSession){TemplateType = TemplateType.Class};
            codeTemplate.SetDefaults();
            persistentClassInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
            _persistentAssemblyInfo.PersistentClassInfos.Add(persistentClassInfo);
            Assembly.GetAssembly(typeof (ObjectMerger));
        };

        Because of = () => {
            _compileModule = CompileEngine.CompileModule(_persistentAssemblyInfo);
        };

        It should_compile_with_no_errors = () => {
            _persistentAssemblyInfo.CompileErrors.ShouldBeNull();
            _compileModule.ShouldNotBeNull();
        };

        It should_create_a_baseType_descenant_class =
            () =>
            _compileModule.Assembly.GetTypes().Where(type => typeof (User).IsAssignableFrom(type)).FirstOrDefault().
                ShouldNotBeNull();
    }

    
}
