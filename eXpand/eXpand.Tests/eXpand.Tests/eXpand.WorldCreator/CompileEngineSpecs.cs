using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
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
    public class When_cannot_compile_a_dynamic_module {
        static IPersistentAssemblyInfo _persistentAssemblyInfo;

        Establish context = () => {
            new TestAppLication<PersistentAssemblyInfo>().Setup(null,info => _persistentAssemblyInfo=info);
            Isolate.WhenCalled(() => CodeEngine.GenerateCode(_persistentAssemblyInfo)).WillReturn("1111");
        };

        static Exception _exception;

        Because of = () => {
            _exception = Catch.Exception(() => new CompileEngine().CompileModule(_persistentAssemblyInfo));
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
            _persistentAssemblyInfo.Name = "TestAssembly222";
        };

        Because of = () => {type=new CompileEngine().CompileModule(_persistentAssemblyInfo);};

        It should_not_contain_any_compilation_error = () => _persistentAssemblyInfo.CompileErrors.ShouldBeNull();

        It should_Create_A_Dynamic_module = () => {
            type.ShouldNotBeNull();
            typeof(ModuleBase).IsAssignableFrom(type).ShouldBeTrue();
        };
    }

    [Subject(typeof(CompileEngine), "specs")]
    public class When_compiling_class_with_members {
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        static Type _compileModule;

        Establish context = () => {
            new TestAppLication<PersistentAssemblyInfo>().Setup(null,info => _persistentAssemblyInfo=info);
            var persistentAssociationAttribute = _persistentAssemblyInfo.Session;
            var classCodeTemplate = new CodeTemplate(persistentAssociationAttribute){TemplateType = TemplateType.Class};
            classCodeTemplate.SetDefaults();
            _persistentAssemblyInfo = new PersistentAssemblyInfo(persistentAssociationAttribute){Name = "TestAssembly"};
            var persistentClassInfo = new PersistentClassInfo(persistentAssociationAttribute) {Name = "TestClass", CodeTemplateInfo =new CodeTemplateInfo(persistentAssociationAttribute) {TemplateInfo = classCodeTemplate},PersistentAssemblyInfo = _persistentAssemblyInfo};

            var memberCodeTemplate = new CodeTemplate(persistentAssociationAttribute){TemplateType = TemplateType.ReadWriteMember};
            memberCodeTemplate.SetDefaults();
            new PersistentCoreTypeMemberInfo(persistentAssociationAttribute){Name = "Property",CodeTemplateInfo =new CodeTemplateInfo(persistentAssociationAttribute) {TemplateInfo = memberCodeTemplate},Owner = persistentClassInfo};
            new PersistentReferenceMemberInfo(persistentAssociationAttribute){Name = "RefProperty",CodeTemplateInfo=new CodeTemplateInfo(persistentAssociationAttribute)  {TemplateInfo = memberCodeTemplate},Owner = persistentClassInfo,ReferenceType = typeof(User)};
            new PersistentCollectionMemberInfo(persistentAssociationAttribute) { Name = "CollProperty", CodeTemplateInfo =new CodeTemplateInfo(persistentAssociationAttribute) { TemplateInfo = memberCodeTemplate }, Owner = persistentClassInfo, CollectionType = typeof(User) };
        };

        Because of = () => { _compileModule = new CompileEngine().CompileModule(_persistentAssemblyInfo); };

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
    public class When_PersistentClassInfo_BaseType_Belongs_to_different_assemmbly {
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        static Type _compileModule;

        Establish context = () => {
            new TestAppLication<PersistentAssemblyInfo>().Setup(null, info => _persistentAssemblyInfo = info);
            _persistentAssemblyInfo.Name = "TestAssembly";
            var unitOfWork = _persistentAssemblyInfo.Session;
            var persistentClassInfo = new PersistentClassInfo(unitOfWork){Name = "TestClass",BaseType = typeof(User),CodeTemplateInfo = new CodeTemplateInfo(unitOfWork)};
            var codeTemplate = new CodeTemplate(unitOfWork){TemplateType = TemplateType.Class};
            codeTemplate.SetDefaults();
            persistentClassInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
            _persistentAssemblyInfo.PersistentClassInfos.Add(persistentClassInfo);
            Assembly.GetAssembly(typeof (ObjectMerger));
        };

        Because of = () => {
            _compileModule = new CompileEngine().CompileModule(_persistentAssemblyInfo);
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
    public class When_PersistentClassInfo_BaseType_Belongs_to_same_assemmbly {
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        static Type _compileModule;
        Establish context = () => {
            new TestAppLication<PersistentAssemblyInfo>().Setup(null, info => _persistentAssemblyInfo = info);
            var unitOfWork = _persistentAssemblyInfo.Session;
            _persistentAssemblyInfo = new PersistentAssemblyInfo(unitOfWork){Name = "TestAssembly"};
            var persistentClassInfo = new PersistentClassInfo(unitOfWork){Name = "TestClass",BaseType = typeof(User),CodeTemplateInfo = new CodeTemplateInfo(unitOfWork)};
            var codeTemplate = new CodeTemplate(unitOfWork){TemplateType = TemplateType.Class};
            codeTemplate.SetDefaults();
            persistentClassInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
            _persistentAssemblyInfo.PersistentClassInfos.Add(persistentClassInfo);
            Assembly.GetAssembly(typeof (ObjectMerger));
        };

        Because of = () => {
            _compileModule = new CompileEngine().CompileModule(_persistentAssemblyInfo);
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
    public class When_compiling_assembly_with_strong_key:With_Isolations {
        static Type _compileModule;
        static PersistentAssemblyInfo _info;

        Establish context = () => new TestAppLication<PersistentAssemblyInfo>().Setup(null, info => {
            var strongKeyFile = new FileData(info.Session);
            strongKeyFile.LoadFromStream("test", new FileStream(@"../eXpand.Key/eXpand.snk", FileMode.Open));
            info.StrongKeyFile = strongKeyFile;
            info.Name = "TestAssembly";
            _info = info;
        });

        Because of = () => {
             _compileModule = new CompileEngine().CompileModule(_info);
         };

        It should_compile_with_no_error = () => _info.CompileErrors.ShouldBeNull();

        It should_have_public_token_set =
            () => (_compileModule.Assembly.FullName + "").IndexOf("c52ffed5d5ff0958").ShouldBeGreaterThan(-1);
    }
    [Subject(typeof(CompileEngine))]
    public class When_compiling_assembly_with_version:With_Isolations {
        static Type _compileModule;
        static PersistentAssemblyInfo _info;

        Establish context = () => new TestAppLication<
            PersistentAssemblyInfo>().Setup(null, info => {
                info.Name = "TestAssembly";
                info.Version = new Version(2, 2, 2, 2).ToString();
                _info = info;
        });

        Because of = () => {
             _compileModule = new CompileEngine().CompileModule(_info);
         };

        It should_compile_with_no_error = () => _info.CompileErrors.ShouldBeNull();

        It should_have_version_set =
            () => (_compileModule.Assembly.FullName + "").IndexOf("2.2.2.2").ShouldBeGreaterThan(-1);
    }
    [Subject(typeof(CompileEngine))]
    public class When_compiling_a_list_of_assemblies {
        static IList<IPersistentAssemblyInfo> _persistentAssemblyInfos;

        static CompileEngine _compileEngine;

        static IPersistentAssemblyInfo _persistnetAssembly;

        Establish context = () => {
            new TestAppLication<PersistentAssemblyInfo>().Setup(null,info => {
                info.Name = "SecondAssembly";
                info.CompileOrder = 1;
                var persistentAssemblyInfo = new PersistentAssemblyInfo(info.Session){Name = "FirstAssembly"};
                _persistentAssemblyInfos = new List<PersistentAssemblyInfo>{persistentAssemblyInfo,info}.Cast<IPersistentAssemblyInfo>().ToList();
            });
            _compileEngine = new CompileEngine();
            Isolate.WhenCalled(() => _compileEngine.CompileModule(null)).DoInstead(callContext => {
                _persistnetAssembly = (IPersistentAssemblyInfo) callContext.Parameters[0];
                return null;
            });
            
        };

        Because of = () => _compileEngine.CompileModules(_persistentAssemblyInfos);

        It should_compile_the_one_with_lowest_compile_order_firt =
            () => _persistnetAssembly.Name.ShouldEqual("FirstAssembly");
    }
}
