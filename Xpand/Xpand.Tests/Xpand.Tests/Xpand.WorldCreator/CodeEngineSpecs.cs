using System;
using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Xpand.Persistent.Base;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

using Machine.Specifications;
using System.Linq;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Tests.Xpand.WorldCreator {

    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_persistentMemberInfo : With_In_Memory_DataStore {
        static string _generateCode;

        static PersistentMemberInfo _persistentMemberInfo;

        Establish context = () => {

            _persistentMemberInfo = ObjectSpace.CreateObject<PersistentCoreTypeMemberInfo>();

            _persistentMemberInfo = new PersistentCoreTypeMemberInfo(UnitOfWork) { CodeTemplateInfo = new CodeTemplateInfo(UnitOfWork) };
            var codeTemplate = new CodeTemplate(UnitOfWork) { TemplateType = TemplateType.XPReadWritePropertyMember };
            codeTemplate.SetDefaults();
            _persistentMemberInfo.Owner = new PersistentClassInfo(UnitOfWork);
            _persistentMemberInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;

        };

        Because of = () => { _generateCode = CodeEngine.GenerateCode(_persistentMemberInfo); };

        It should_have_memberInfo_attributes_generated = () => _generateCode.IndexOf("$TYPEATTRIBUTES$").ShouldEqual(-1);
    }

    public class When_persistent_member_name_is_not_valid : With_In_Memory_DataStore {
        static PersistentCoreTypeMemberInfo _persistentMemberInfo;
        static string _generateCode;


        Establish context = () => {
            _persistentMemberInfo = ObjectSpace.CreateObject<PersistentCoreTypeMemberInfo>();

            _persistentMemberInfo = new PersistentCoreTypeMemberInfo(UnitOfWork) { CodeTemplateInfo = new CodeTemplateInfo(UnitOfWork) };
            var codeTemplate = new CodeTemplate(UnitOfWork) { TemplateType = TemplateType.XPReadWritePropertyMember };
            codeTemplate.SetDefaults();
            _persistentMemberInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
            _persistentMemberInfo.Owner = new PersistentClassInfo(UnitOfWork);
            _persistentMemberInfo.Name = "in valid";
        };

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentMemberInfo);
        };

        It should_clean_the_invalid_name = () => _generateCode.IndexOf("invalid").ShouldBeGreaterThan(-1);
    }
    [Subject(typeof(CodeEngine), "Generate Code")]
    public class When_persistent_memberinfo_name_is_the_same_as_its_owner : With_In_Memory_DataStore {
        static string _generateCode;
        static PersistentCoreTypeMemberInfo _persistentCoreTypeMemberInfo;

        Establish context = () => {
            _persistentCoreTypeMemberInfo = new PersistentCoreTypeMemberInfo(UnitOfWork) {
                Name = "Key",
                Owner = new PersistentClassInfo(UnitOfWork) { Name = "Key" },
                CodeTemplateInfo = new CodeTemplateInfo(UnitOfWork)
            };
            var codeTemplate = new CodeTemplate(UnitOfWork) { TemplateType = TemplateType.XPReadWritePropertyMember };
            codeTemplate.SetDefaults();
            _persistentCoreTypeMemberInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
        };

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentCoreTypeMemberInfo);
        };

        It should_extend_generated_code_property_name_with_a_member_suffix =
            () => _generateCode.IndexOf("KeyMember").ShouldBeGreaterThan(-1);
    }
    [Subject(typeof(CodeEngine), "Generate Code")]
    public class When_persistent_class_name_is_not_valid : With_In_Memory_DataStore {
        static string _generateCode;
        static IPersistentClassInfo _persistentClassInfo;

        Establish context = () => {
            _persistentClassInfo = ObjectSpace.CreateObject<PersistentClassInfo>();
            _persistentClassInfo.PersistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            var codeTemplate = new CodeTemplate(UnitOfWork) { TemplateType = TemplateType.Class };
            codeTemplate.SetDefaults();
            _persistentClassInfo.CodeTemplateInfo = new CodeTemplateInfo(UnitOfWork);
            _persistentClassInfo.Name = "in valid";
            _persistentClassInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
        };

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentClassInfo);
        };

        It should_clean_the_invalid_name = () => _generateCode.IndexOf("invalid").ShouldBeGreaterThan(-1);
    }

    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_persistentClassinfo_with_no_base_type_defined : With_In_Memory_DataStore {
        static PersistentClassInfo _persistentClassInfo;

        static string _generateCode;

        Establish context = () => {

            var codeTemplate = ObjectSpace.CreateObject<CodeTemplate>();
            codeTemplate.TemplateType = TemplateType.Class;
            codeTemplate.SetDefaults();
            _persistentClassInfo = new PersistentClassInfo(UnitOfWork) {
                Name = "TestClass",
                CodeTemplateInfo = new CodeTemplateInfo(UnitOfWork) { TemplateInfo = codeTemplate },
                PersistentAssemblyInfo = new PersistentAssemblyInfo(UnitOfWork)
            };
        };

        Because of = () => { _generateCode = CodeEngine.GenerateCode(_persistentClassInfo); };

        It should_use_default_Base_type = () => _generateCode.IndexOf(typeof(XpandBaseCustomObject).FullName + "").ShouldBeGreaterThan(-1);
    }
    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_persistentClassinfo_with_baseclassinfo_defined : With_In_Memory_DataStore {
        static PersistentClassInfo _childPersistentClassInfo;
        static string _generateCode;

        Establish context = () => {
            IClassInfoHandler classInfoHandler = PersistentAssemblyBuilder.BuildAssembly(ObjectSpace, "TestAssembly").CreateClasses(new[] { "ParentClass", "ChildClass" });
            classInfoHandler.SetInheritance(info => {
                if (info.Name == "ChildClass") {
                    _childPersistentClassInfo = (PersistentClassInfo)info;
                    return info.PersistentAssemblyInfo.PersistentClassInfos.Where(
                        classInfo => classInfo.Name == "ParentClass").Single();
                }
                return null;
            });
        };

        Because of = () => { _generateCode = CodeEngine.GenerateCode(_childPersistentClassInfo); };

        It should_derive_child_from_parent_classInfo =
            () => _generateCode.IndexOf("ChildClass:TestAssembly.ParentClass").ShouldBeGreaterThan(-1);
    }
    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_persistentClassinfo_with_baseType_defined : With_In_Memory_DataStore {
        static PersistentClassInfo _persistentClassInfo;

        static string _generateCode;

        Establish context = () => {

            _persistentClassInfo = ObjectSpace.CreateObject<PersistentClassInfo>();
            var classCodeTemplate = new CodeTemplate(UnitOfWork) { TemplateType = TemplateType.Class };
            classCodeTemplate.SetDefaults();
            _persistentClassInfo = new PersistentClassInfo(UnitOfWork) { Name = "TestClass", BaseType = typeof(User), CodeTemplateInfo = new CodeTemplateInfo(UnitOfWork) { TemplateInfo = classCodeTemplate }, PersistentAssemblyInfo = new PersistentAssemblyInfo(UnitOfWork) };
            _persistentClassInfo.TypeAttributes.Add(new PersistentDefaultClassOptionsAttribute(UnitOfWork));

            var memberCodeTemplate = new CodeTemplate(UnitOfWork) { TemplateType = TemplateType.XPReadWritePropertyMember };
            memberCodeTemplate.SetDefaults();
            var persistentCoreTypeMemberInfo = new PersistentCoreTypeMemberInfo(UnitOfWork) { Name = "property1", CodeTemplateInfo = new CodeTemplateInfo(UnitOfWork) { TemplateInfo = memberCodeTemplate } };
            persistentCoreTypeMemberInfo.TypeAttributes.Add(new PersistentSizeAttribute(UnitOfWork));
            _persistentClassInfo.OwnMembers.AddRange(new[] {
                                                               persistentCoreTypeMemberInfo,
                                                               new PersistentCoreTypeMemberInfo(UnitOfWork){Name = "property2",CodeTemplateInfo =new CodeTemplateInfo(UnitOfWork) {TemplateInfo = memberCodeTemplate}}
                                                           });
            var interfaceInfo = new InterfaceInfo(UnitOfWork) { Assembly = new AssemblyName(typeof(IDummyString).Assembly.FullName + "").Name, Name = typeof(IDummyString).FullName };
            _persistentClassInfo.Interfaces.Add(interfaceInfo);
            _persistentClassInfo.Save();
        };

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentClassInfo);
        };


        It should_inject_code_from_all_members_in_classinfo_generated_code = () => _generateCode.IndexOf("property1" + Environment.NewLine + "property2" + Environment.NewLine + "}");
        It should_have_class_type_typeattributes_generated = () => {
            _generateCode.IndexOf("$TYPEATTRIBUTES$").ShouldEqual(-1);
            _generateCode.IndexOf("[DevExpress.Persistent.Base.DefaultClassOptionsAttribute()]").ShouldBeGreaterThan(-1);
        };
        It should_have_propertytype_typeattributes_generated = () => _generateCode.IndexOf("[DevExpress.Xpo.SizeAttribute(100)]").ShouldBeGreaterThan(-1);
        It should_replace_CLASSNAME_with_persistentClassInfo_name = () => {
            _generateCode.IndexOf("TestClass").ShouldBeGreaterThan(-1);
            _generateCode.IndexOf("CLASSNAME").ShouldEqual(-1);
        };

        It should_replace_BASECLASSNAME_with_persistentClassInfo_name = () => {
            _generateCode.IndexOf(typeof(User).FullName + "").ShouldBeGreaterThan(-1);
            _generateCode.IndexOf("BASECLASSNAME").ShouldEqual(-1);
        };
        It should_replace_ASEEMBLYNAME_with_persistentClassInfo_name = () => {
            _generateCode.IndexOf(typeof(User).FullName + "").ShouldBeGreaterThan(-1);
            _generateCode.IndexOf("ASEEMBLYNAME").ShouldEqual(-1);
        };

        It should_add_all_interfaces_after_baseclassName =
            () => _generateCode.IndexOf("," + typeof(IDummyString).FullName).ShouldBeGreaterThan(-1);
        It should_replace_PROPERTYNAME_with_persistentClassInfo_name = () => _generateCode.IndexOf("PROPERTYNAME").ShouldEqual(-1);
        It should_replace_PROPERTYTYPE_with_persistentClassInfo_type = () => _generateCode.IndexOf("PROPERTYTYPE").ShouldEqual(-1);
    }
    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_2_CSharp_classes : With_In_Memory_DataStore {
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        static string _generateCode;

        Establish context = () => {

            _persistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            createClass();
            createClass();
        };

        static void createClass() {
            var persistentClassInfo = new PersistentClassInfo(UnitOfWork) { PersistentAssemblyInfo = _persistentAssemblyInfo, CodeTemplateInfo = new CodeTemplateInfo(UnitOfWork) };
            var codeTemplate = new CodeTemplate(UnitOfWork) { TemplateType = TemplateType.Class };
            codeTemplate.SetDefaults();
            persistentClassInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
        }

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentAssemblyInfo);
        };

        It should_group_all_usings_together_at_the_top_of_generated_code = () => _generateCode.ShouldStartWith("using");
    }

    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_2_VB_classes : With_In_Memory_DataStore {
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        static string _generateCode;

        Establish context = () => {

            _persistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            _persistentAssemblyInfo.CodeDomProvider = CodeDomProvider.VB;
            createClass();
            createClass();
        };

        static void createClass() {
            var persistentClassInfo = new PersistentClassInfo(UnitOfWork) { PersistentAssemblyInfo = _persistentAssemblyInfo, CodeTemplateInfo = new CodeTemplateInfo(UnitOfWork) };
            var codeTemplate = new CodeTemplate(UnitOfWork) { TemplateType = TemplateType.Class, CodeDomProvider = CodeDomProvider.VB };
            codeTemplate.SetDefaults();
            persistentClassInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
        }

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentAssemblyInfo);
        };

        It should_group_all_usings_together_at_the_top_of_generated_code = () => _generateCode.ShouldStartWith("Imports");
    }

    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_Persistent_attribute_with_enum_parameter : With_In_Memory_DataStore {
        static PersistentMapInheritanceAttribute _persistentMapInheritanceAttribute;

        static string _generateCode;

        Establish context = () => {
            _persistentMapInheritanceAttribute = new PersistentMapInheritanceAttribute(UnitOfWork);
        };

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentMapInheritanceAttribute);
        };

        It should_create_arg_with_enumTypename_dot_enumName = () => _generateCode.ShouldStartWith("[" + typeof(MapInheritanceAttribute).FullName + "(" + typeof(MapInheritanceType).FullName + "." + MapInheritanceType.ParentTable + ")]");
    }

    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_Persistent_attribute_with_string_parameter : With_In_Memory_DataStore {
        static PersistentCustomAttribute _persistentCustomAttribute;

        Establish context = () => {

            _persistentCustomAttribute = new PersistentCustomAttribute(UnitOfWork) { PropertyName = "PropertyName", Value = "Value" };
        };

        static string _generateCode;

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentCustomAttribute);
        };

        It should_create_arg_enclosed_with_quotes = () => _generateCode.ShouldStartWith("[" + typeof(CustomAttribute).FullName + @"(@""PropertyName"",@""Value"")" + "]");
    }

    [Subject(typeof(CodeEngine))]
    public class When_generating_code_from_Persistent_attribute_with_type_parameter : With_In_Memory_DataStore {
        static PersistentValueConverter _persistentValueConverter;

        static string _generateCode;

        Establish context = () => {
            _persistentValueConverter = new PersistentValueConverter(UnitOfWork) { ConverterType = typeof(CompressionConverter) };
        };

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentValueConverter);

        };

        It should_create_arg_as_typeof_parameter = () => _generateCode.ShouldStartWith("[" + typeof(ValueConverterAttribute).FullName + "(typeof(" + typeof(CompressionConverter).FullName + "))]");
    }

    public class When_persistent_attribute_member_is_marked_with_attributeinfo : With_In_Memory_DataStore {
        static PersistentRuleRequiredFieldAttribute _persistentRuleRequiredFieldAttribute;
        static string _generateCode;


        Establish context = () => {
            _persistentRuleRequiredFieldAttribute = new PersistentRuleRequiredFieldAttribute(UnitOfWork) { ID = "id", Context = "Save", TargetCriteria = "TargetCriteria" };
        };

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentRuleRequiredFieldAttribute);
        };

        It should_generate_extra_properties = () => _generateCode.ShouldEqual(@"[DevExpress.Persistent.Validation.RuleRequiredFieldAttribute(@""id"",@""Save""){TargetCriteria=@""TargetCriteria""}]");
    }
}