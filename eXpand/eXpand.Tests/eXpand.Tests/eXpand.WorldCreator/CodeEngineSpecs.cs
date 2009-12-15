using System;
using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Xpo;
using Machine.Specifications;

namespace eXpand.Tests.eXpand.WorldCreator {
    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_persistentMemberInfo {
        static string _generateCode;

        static PersistentMemberInfo _persistentMemberInfo;

        Establish context = () => {
            var artifactHandler = new TestAppLication<PersistentCoreTypeMemberInfo>().Setup();
            _persistentMemberInfo=artifactHandler.CurrentObject;

            _persistentMemberInfo = new PersistentCoreTypeMemberInfo(artifactHandler.UnitOfWork) { CodeTemplateInfo = new CodeTemplateInfo(artifactHandler.UnitOfWork) };
            var codeTemplate = new CodeTemplate(artifactHandler.UnitOfWork) { TemplateType = TemplateType.ReadWriteMember };
            codeTemplate.SetDefaults();            
            _persistentMemberInfo.CodeTemplateInfo.TemplateInfo=codeTemplate;

        };

        Because of = () => {_generateCode = CodeEngine.GenerateCode(_persistentMemberInfo);};

        It should_have_memberInfo_attributes_generated = () => {
            _generateCode.IndexOf("System.ComponentModel.BrowsableAttribute[false]");
            _generateCode.IndexOf("$TYPEATTRIBUTES$").ShouldEqual(-1);
        };
    }
        [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_persistentClassinfo_with_no_base_type_defined {
        static PersistentClassInfo _persistentClassInfo;

        static string _generateCode;

        Establish context = () => {
            var artifactHandler = new TestAppLication<CodeTemplate>().Setup();
            var codeTemplate = artifactHandler.CurrentObject;
            codeTemplate.TemplateType = TemplateType.Class ;
            codeTemplate.SetDefaults();
            _persistentClassInfo = new PersistentClassInfo(artifactHandler.UnitOfWork)
            {
                                                                           Name = "TestClass",
                                                                           CodeTemplateInfo = new CodeTemplateInfo(artifactHandler.UnitOfWork) { TemplateInfo = codeTemplate },
                                                                           PersistentAssemblyInfo = new PersistentAssemblyInfo(artifactHandler.UnitOfWork)
                                                                       };
        };

        Because of = () => { _generateCode = CodeEngine.GenerateCode(_persistentClassInfo); };

        It should_use_default_Base_type =() => _generateCode.IndexOf(typeof (eXpandCustomObject).FullName).ShouldBeGreaterThan(-1);
    }
        [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_persistentClassinfo_with_baseclassinfo_defined:With_Isolations {
        static string _generateCode;
        static PersistentClassInfo _info;

        Establish context = () => new TestAppLication<PersistentClassInfo>().Setup(null, info => {
            info.Name = "ParentClass";
            var persistentAssemblyInfo = new PersistentAssemblyInfo(info.Session){Name = "TestAssembly"};
            var parentPersistentClassInfo = new PersistentClassInfo(info.Session) { Name = "ParentClass", PersistentAssemblyInfo = persistentAssemblyInfo };
            info.Name = "ChildClass";
            info.BaseClassInfo = parentPersistentClassInfo;
            info.PersistentAssemblyInfo = persistentAssemblyInfo;
            _info=info;
        }).WithArtiFacts(() => new[]{typeof (WorldCreatorModule)}).CreateDetailView().CreateFrame().RaiseControlsCreated();

        Because of = () => { _generateCode = CodeEngine.GenerateCode(_info); };

        It should_derive_child_from_parent_classInfo =
            () => _generateCode.IndexOf("ChildClass:TestAssembly.ParentClass").ShouldBeGreaterThan(-1);
    }
    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_persistentClassinfo_with_baseType_defined {
        static PersistentClassInfo _persistentClassInfo;

        static string _generateCode;

        Establish context = () => {
            var artifactHandler = new TestAppLication<PersistentClassInfo>().Setup();
            _persistentClassInfo = artifactHandler.CurrentObject;
            var classCodeTemplate = new CodeTemplate(artifactHandler.UnitOfWork) { TemplateType = TemplateType.Class };
            classCodeTemplate.SetDefaults();
            _persistentClassInfo = new PersistentClassInfo(artifactHandler.UnitOfWork) { Name = "TestClass", BaseType = typeof(User), CodeTemplateInfo = new CodeTemplateInfo(artifactHandler.UnitOfWork) { TemplateInfo = classCodeTemplate }, PersistentAssemblyInfo = new PersistentAssemblyInfo(artifactHandler.UnitOfWork) };
            _persistentClassInfo.TypeAttributes.Add(new PersistentDefaultClassOptionsAttribute(artifactHandler.UnitOfWork));

            var memberCodeTemplate = new CodeTemplate(artifactHandler.UnitOfWork) { TemplateType = TemplateType.ReadWriteMember };
            memberCodeTemplate.SetDefaults();
            var persistentCoreTypeMemberInfo = new PersistentCoreTypeMemberInfo(artifactHandler.UnitOfWork) { Name = "property1", CodeTemplateInfo = new CodeTemplateInfo(artifactHandler.UnitOfWork) { TemplateInfo = memberCodeTemplate } };
            persistentCoreTypeMemberInfo.TypeAttributes.Add(new PersistentSizeAttribute(artifactHandler.UnitOfWork));            
            _persistentClassInfo.OwnMembers.AddRange(new[] {
                                                               persistentCoreTypeMemberInfo,
                                                               new PersistentCoreTypeMemberInfo(artifactHandler.UnitOfWork){Name = "property2",CodeTemplateInfo =new CodeTemplateInfo(artifactHandler.UnitOfWork) {TemplateInfo = memberCodeTemplate}}
                                                           });
            var interfaceInfo = new InterfaceInfo(artifactHandler.UnitOfWork) { Assembly = new AssemblyName(typeof(IDummyString).Assembly.FullName + "").Name, Name = typeof(IDummyString).FullName };
            _persistentClassInfo.Interfaces.Add(interfaceInfo);
            _persistentClassInfo.Save();
        };

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentClassInfo);
        };


        It should_inject_code_from_all_members_in_classinfo_generated_code = () => _generateCode.IndexOf("property1" + Environment.NewLine + "property2" + Environment.NewLine + "}");
        It should_have_class_type_typeattributes_generated=() => {
            _generateCode.IndexOf("$TYPEATTRIBUTES$").ShouldEqual(-1);
            _generateCode.IndexOf("[DevExpress.Persistent.Base.DefaultClassOptionsAttribute()]").ShouldBeGreaterThan(-1);
        };
        It should_have_propertytype_typeattributes_generated=() => _generateCode.IndexOf("[DevExpress.Xpo.SizeAttribute(100)]").ShouldBeGreaterThan(-1);
        It should_replace_CLASSNAME_with_persistentClassInfo_name=() => {
            _generateCode.IndexOf("TestClass").ShouldBeGreaterThan(-1);
            _generateCode.IndexOf("CLASSNAME").ShouldEqual(-1);
        };

        It should_replace_BASECLASSNAME_with_persistentClassInfo_name = () => {
            _generateCode.IndexOf(typeof(User).FullName).ShouldBeGreaterThan(-1);
            _generateCode.IndexOf("BASECLASSNAME").ShouldEqual(-1);
        };
        It should_replace_ASEEMBLYNAME_with_persistentClassInfo_name = () => {
            _generateCode.IndexOf(typeof(User).FullName).ShouldBeGreaterThan(-1);
            _generateCode.IndexOf("ASEEMBLYNAME").ShouldEqual(-1);
        };

        It should_add_all_interfaces_after_baseclassName =
            () => _generateCode.IndexOf("," + typeof (IDummyString).FullName).ShouldBeGreaterThan(-1);
        It should_replace_PROPERTYNAME_with_persistentClassInfo_name = () => _generateCode.IndexOf("PROPERTYNAME").ShouldEqual(-1);
        It should_replace_PROPERTYTYPE_with_persistentClassInfo_type = () => _generateCode.IndexOf("PROPERTYTYPE").ShouldEqual(-1);
    }
    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_2_CSharp_classes {
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        static string _generateCode;

        Establish context = () => {
            var artifactHandler = new TestAppLication<PersistentAssemblyInfo>().Setup();
            _persistentAssemblyInfo = artifactHandler.CurrentObject;
            createClass(artifactHandler);
            createClass(artifactHandler);
        };

        static void createClass(IArtifactHandler<PersistentAssemblyInfo> artifactHandler) {
            var persistentClassInfo = new PersistentClassInfo(artifactHandler.UnitOfWork) { PersistentAssemblyInfo = _persistentAssemblyInfo, CodeTemplateInfo = new CodeTemplateInfo(artifactHandler.UnitOfWork) };
            var codeTemplate = new CodeTemplate(artifactHandler.UnitOfWork) { TemplateType = TemplateType.Class };
            codeTemplate.SetDefaults();
            persistentClassInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
        }

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentAssemblyInfo);
        };

        It should_group_all_usings_together_at_the_top_of_generated_code=() => _generateCode.ShouldStartWith("using");
    }
    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_2_VB_classes {
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        static string _generateCode;

        Establish context = () => {
            var artifactHandler = new TestAppLication<PersistentAssemblyInfo>().Setup();
            _persistentAssemblyInfo = artifactHandler.CurrentObject;
            _persistentAssemblyInfo.CodeDomProvider = CodeDomProvider.VB;
            createClass(artifactHandler);
            createClass(artifactHandler);
        };

        static void createClass(IArtifactHandler<PersistentAssemblyInfo> artifactHandler) {
            var persistentClassInfo = new PersistentClassInfo(artifactHandler.UnitOfWork) { PersistentAssemblyInfo = _persistentAssemblyInfo, CodeTemplateInfo = new CodeTemplateInfo(artifactHandler.UnitOfWork) };
            var codeTemplate = new CodeTemplate(artifactHandler.UnitOfWork) { TemplateType = TemplateType.Class, CodeDomProvider = CodeDomProvider.VB };
            codeTemplate.SetDefaults();
            persistentClassInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
        }

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentAssemblyInfo);
        };

        It should_group_all_usings_together_at_the_top_of_generated_code=() => _generateCode.ShouldStartWith("Imports");
    }
    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_Persistent_attribute_with_enum_parameter {
        static PeristentMapInheritanceAttribute _peristentMapInheritanceAttribute;

        static string _generateCode;

        Establish context = () => {
            var artifactHandler = new TestAppLication<PeristentMapInheritanceAttribute>().Setup();
            _peristentMapInheritanceAttribute = new PeristentMapInheritanceAttribute(artifactHandler.UnitOfWork);
        };

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_peristentMapInheritanceAttribute);
        };

        It should_create_arg_with_enumTypename_dot_enumName = () => _generateCode.ShouldStartWith("["+typeof(MapInheritanceAttribute).FullName+"("+typeof(MapInheritanceType).FullName+"."+MapInheritanceType.ParentTable+")]");
    }
    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_Persistent_attribute_with_string_parameter {
        static PersistentCustomAttribute _persistentCustomAttribute;

        Establish context = () => {
            var artifactHandler = new TestAppLication<PersistentCustomAttribute>().Setup();
            _persistentCustomAttribute = new PersistentCustomAttribute(artifactHandler.UnitOfWork) { PropertyName = "PropertyName", Value = "Value" };
        };

        static string _generateCode;

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentCustomAttribute);
        };

        It should_create_arg_enclosed_with_quotes=() => _generateCode.ShouldStartWith("["+typeof(CustomAttribute).FullName+@"(""PropertyName"",""Value"")"+"]");
    }
    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_Persistent_attribute_with_type_parameter {
        static PersistentValueConverter _persistentValueConverter;

        static string _generateCode;

        Establish context = () => {
            var artifactHandler = new TestAppLication<PersistentValueConverter>().Setup();
            _persistentValueConverter = new PersistentValueConverter(artifactHandler.UnitOfWork) { ConverterType = typeof(CompressionConverter) };
        };

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentValueConverter);

        };
        
        It should_create_arg_as_typeof_parameter=() => _generateCode.ShouldStartWith("["+typeof(ValueConverterAttribute).FullName+"(typeof("+typeof(CompressionConverter).FullName+"))]");
    }
    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_Persistent_attribute_with_int_parameter {
        It should_create_arg_the_same_as_parameter;
    }
}