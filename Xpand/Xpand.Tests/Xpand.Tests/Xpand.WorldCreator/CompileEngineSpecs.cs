using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using System.Linq;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Utils;

namespace Xpand.Tests.Xpand.WorldCreator
{
    [Subject(typeof(CompileEngine))][Isolated]
    public class When_cannot_compile_a_dynamic_module:With_In_Memory_DataStore {
        static IPersistentAssemblyInfo _persistentAssemblyInfo;

        Establish context = () => {
            _persistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            _persistentAssemblyInfo.Name="a0";
            Isolate.WhenCalled(() => CodeEngine.GenerateCode(_persistentAssemblyInfo)).WillReturn("1111");
        };

        static Exception _exception;

        Because of = () => {
            _exception = Catch.Exception(() => new CompileEngine().CompileModule(_persistentAssemblyInfo, Path.GetDirectoryName(Application.ExecutablePath)));
        };

        It should_swallow_exception=() => _exception.ShouldBeNull();
        It should_delegate_any_errors_to_assembly_info_compile_errors_property=() => _persistentAssemblyInfo.CompileErrors.ShouldNotBeNull();
        
    }

    [Subject(typeof(CompileEngine), "specs")]
    [Isolated]
    public class When_compiling_a_dynamic_assembly
    {
        static Type type;
        static IPersistentAssemblyInfo _persistentAssemblyInfo;

        Establish context = () => {
            _persistentAssemblyInfo = Isolate.Fake.Instance<IPersistentAssemblyInfo>();
            Isolate.WhenCalled(() => _persistentAssemblyInfo.FileData).WillReturn(null);
            _persistentAssemblyInfo.Name = "a1";
        };

        Because of = () => { type = new CompileEngine().CompileModule(_persistentAssemblyInfo, Path.GetDirectoryName(Application.ExecutablePath)); };

        It should_not_contain_any_compilation_error = () => _persistentAssemblyInfo.CompileErrors.ShouldBeNull();

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
            _persistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            var persistentAssociationAttribute = _persistentAssemblyInfo.Session;
            var classCodeTemplate = new CodeTemplate(persistentAssociationAttribute){TemplateType = TemplateType.Class};
            classCodeTemplate.SetDefaults();
            _persistentAssemblyInfo = new PersistentAssemblyInfo(persistentAssociationAttribute) { Name = "a2" };
            var persistentClassInfo = new PersistentClassInfo(persistentAssociationAttribute) {Name = "ClassWithMembers", CodeTemplateInfo =new CodeTemplateInfo(persistentAssociationAttribute) {TemplateInfo = classCodeTemplate},PersistentAssemblyInfo = _persistentAssemblyInfo};

            var memberCodeTemplate = new CodeTemplate(persistentAssociationAttribute){TemplateType = TemplateType.XPReadWritePropertyMember};
            memberCodeTemplate.SetDefaults();
            new PersistentCoreTypeMemberInfo(persistentAssociationAttribute){Name = "Property",CodeTemplateInfo =new CodeTemplateInfo(persistentAssociationAttribute) {TemplateInfo = memberCodeTemplate},Owner = persistentClassInfo,DataType = DBColumnType.Boolean};
            new PersistentReferenceMemberInfo(persistentAssociationAttribute){Name = "RefProperty",CodeTemplateInfo=new CodeTemplateInfo(persistentAssociationAttribute)  {TemplateInfo = memberCodeTemplate},Owner = persistentClassInfo,ReferenceType = typeof(User)};
            new PersistentCollectionMemberInfo(persistentAssociationAttribute) { Name = "CollProperty", CodeTemplateInfo =new CodeTemplateInfo(persistentAssociationAttribute) { TemplateInfo = memberCodeTemplate }, Owner = persistentClassInfo, CollectionType = typeof(User) };
        };

        Because of = () => { _compileModule = new CompileEngine().CompileModule(_persistentAssemblyInfo, Path.GetDirectoryName(Application.ExecutablePath)); };

        It should_have_no_compile_errors = () => _persistentAssemblyInfo.CompileErrors.ShouldBeNull();
        It should_generate_class_type = () => _compileModule.Assembly.GetTypes().Count().ShouldEqual(2);

        It should_have_those_members_as_proeprties =
            () => {
                PropertyInfo[] propertyInfos =
                    _compileModule.Assembly.GetTypes().Where(type => type.FullName.IndexOf("ClassWithMembers") > -1).Single().
                        GetProperties();
                propertyInfos.Where(info => info.Name == "Property").FirstOrDefault().ShouldNotBeNull();
                propertyInfos.Where(info => info.Name == "RefProperty").FirstOrDefault().ShouldNotBeNull();
                propertyInfos.Where(info => info.Name == "CollProperty").FirstOrDefault().ShouldNotBeNull();
            };
    }

    [Subject(typeof(CompileEngine), "specs")]
    public class When_PersistentClassInfo_BaseType_Belongs_to_different_assemmbly:With_In_Memory_DataStore {
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        static Type _compileModule;

        Establish context = () => {
            _persistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            _persistentAssemblyInfo.Name = "a3";
            var unitOfWork = _persistentAssemblyInfo.Session;
            var persistentClassInfo = new PersistentClassInfo(unitOfWork){Name = "ClassWithBaseType",BaseType = typeof(User),CodeTemplateInfo = new CodeTemplateInfo(unitOfWork)};
            var codeTemplate = new CodeTemplate(unitOfWork){TemplateType = TemplateType.Class};
            codeTemplate.SetDefaults();
            persistentClassInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
            _persistentAssemblyInfo.PersistentClassInfos.Add(persistentClassInfo);
            Assembly.GetAssembly(typeof (ObjectMerger));
        };

        Because of = () => {
            _compileModule = new CompileEngine().CompileModule(_persistentAssemblyInfo, Path.GetDirectoryName(Application.ExecutablePath));
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
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        static Type _compileModule;
        Establish context = () => {
            _persistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            var unitOfWork = _persistentAssemblyInfo.Session;
            _persistentAssemblyInfo = new PersistentAssemblyInfo(unitOfWork) { Name = "a4" };
            var persistentClassInfo = new PersistentClassInfo(unitOfWork){Name = "TestClass",BaseType = typeof(User),CodeTemplateInfo = new CodeTemplateInfo(unitOfWork)};
            var codeTemplate = new CodeTemplate(unitOfWork){TemplateType = TemplateType.Class};
            codeTemplate.SetDefaults();
            persistentClassInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
            _persistentAssemblyInfo.PersistentClassInfos.Add(persistentClassInfo);
            Assembly.GetAssembly(typeof (ObjectMerger));
        };

        Because of = () => {
            _compileModule = new CompileEngine().CompileModule(_persistentAssemblyInfo, Path.GetDirectoryName(Application.ExecutablePath));
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

    [Subject(typeof(CompileEngine))]
    public class When_compiling_assembly_with_strong_key:With_In_Memory_DataStore {
        static Type _compileModule;
        static PersistentAssemblyInfo _info;

        Establish context = () => {
            _info = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            var strongKeyFile = new StrongKeyFile(UnitOfWork);
            strongKeyFile.LoadFromStream("test", new FileStream(@"../Xpand.Key/Xpand.snk", FileMode.Open));
            _info.StrongKeyFile = strongKeyFile;
            _info.Name = "a5";

        };

        Because of = () => {
            _compileModule = new CompileEngine().CompileModule(_info, Path.GetDirectoryName(Application.ExecutablePath));
         };

        It should_compile_with_no_error = () => _info.CompileErrors.ShouldBeNull();

        It should_have_public_token_set =
            () => (_compileModule.Assembly.FullName + "").IndexOf("c52ffed5d5ff0958").ShouldBeGreaterThan(-1);
    }

    [Subject(typeof(CompileEngine))]
    public class When_compiling_assembly_with_version:With_In_Memory_DataStore {
        static Type _compileModule;
        static PersistentAssemblyInfo _info;

        Establish context = () => {
            _info = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            _info.Name = "a6";
            
        };

        Because of = () => {
            _compileModule = new CompileEngine().CompileModule(_info, Path.GetDirectoryName(Application.ExecutablePath));
         };

        It should_compile_with_no_error = () => _info.CompileErrors.ShouldBeNull();

        It should_have_version_set =
            () => (_compileModule.Assembly.FullName + "").IndexOf("1.0.").ShouldBeGreaterThan(-1);
    }

    [Subject(typeof(CompileEngine))]
    public class When_compiling_a_list_of_assemblies:With_In_Memory_DataStore {
        static IList<IPersistentAssemblyInfo> _persistentAssemblyInfos;

        static CompileEngine _compileEngine;

        static IPersistentAssemblyInfo _persistnetAssembly;

        Establish context = () => {
            var info = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            info.Name = "SecondAssembly";
            info.CompileOrder = 1;
            var persistentAssemblyInfo = new PersistentAssemblyInfo(info.Session) { Name = "FirstAssembly" };
            _persistentAssemblyInfos = new List<PersistentAssemblyInfo> { persistentAssemblyInfo, info }.Cast<IPersistentAssemblyInfo>().ToList();
            _compileEngine = new CompileEngine();
            string executablePath = Path.GetDirectoryName(Application.ExecutablePath);
            var persistentAssemblyBuilder = Isolate.Fake.Instance<IPersistentAssemblyInfo>();
            Isolate.WhenCalled(() => _compileEngine.CompileModule(persistentAssemblyBuilder, executablePath)).DoInstead(callContext =>
            {
                _persistnetAssembly = (IPersistentAssemblyInfo) callContext.Parameters[0];
                return null;
            });
            
        };

        Because of = () => _compileEngine.CompileModules(_persistentAssemblyInfos, Path.GetDirectoryName(Application.ExecutablePath));

        It should_compile_the_one_with_lowest_compile_order_firt =
            () => _persistnetAssembly.Name.ShouldEqual("FirstAssembly");
    }

    [Subject(typeof(CompileEngine))]
    public class When_compiling_an_assembly_with_dots_in_its_name:With_In_Memory_DataStore {
        static Type _compileModule;
        static IPersistentAssemblyInfo _persistentAssemblyInfo;

        Establish context = () => {

            _persistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            _persistentAssemblyInfo.Name = "TestAssembly.Win";
        };

        Because of = () => {
            _compileModule = new CompileEngine().CompileModule(_persistentAssemblyInfo, Path.GetDirectoryName(Application.ExecutablePath));
         };

        It should_compile_with_no_erros = () => _persistentAssemblyInfo.CompileErrors.ShouldBeNull();
        It should_an_assembly_with_dots_in_its_name =
            () => (_compileModule.Assembly.FullName + "").IndexOf("TestAssembly.Win").ShouldBeGreaterThan(-1);
    }
    [Subject(typeof(CompileEngine))]
    public class When_compiling_an_assembly_that_is_loaded:With_In_Memory_DataStore
    {
        static Type _compileModule;
        static Type _type;
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        Establish context = () =>
        {
            _persistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            _persistentAssemblyInfo.Name = "T";
            _type = new CompileEngine().CompileModule(_persistentAssemblyInfo, Path.GetDirectoryName(Application.ExecutablePath));
            _persistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            _persistentAssemblyInfo.Name = "T";
        };

        Because of = () =>
        {
            _compileModule = new CompileEngine().CompileModule(_persistentAssemblyInfo, Path.GetDirectoryName(Application.ExecutablePath));
        };

        It should_return_the_loaded_module_type = () => _compileModule.ShouldEqual(_type);
    }

}
