using System;
using System.Reflection;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Xpo;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.WorldCreator {
    [Subject(typeof(CodeEngine))]
    [Isolated]
    public class When_generating_code_from_persistentMemberInfo:With_In_Memory_DataStore {
        static string _generateCode;

        static PersistentMemberInfo _persistentMemberInfo;

        Establish context = () => {
            _persistentMemberInfo = new PersistentCoreTypeMemberInfo(Session.DefaultSession);
            var codeTemplate = new CodeTemplate(Session.DefaultSession){TemplateType = TemplateType.ReadWriteMember};
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
    [Isolated]
    public class When_generating_code_from_persistentClassinfo_with_no_base_type_defined:With_In_Memory_DataStore {
        static PersistentClassInfo _persistentClassInfo;

        static string _generateCode;

        Establish context = () => {
            var codeTemplate = new CodeTemplate(Session.DefaultSession){TemplateType = TemplateType.Class};
            codeTemplate.SetDefaults();
            _persistentClassInfo = new PersistentClassInfo(Session.DefaultSession){Name ="TestClass",CodeTemplateInfo = {TemplateInfo = codeTemplate},PersistentAssemblyInfo = new PersistentAssemblyInfo(Session.DefaultSession)};
        };

        Because of = () => { _generateCode = CodeEngine.GenerateCode(_persistentClassInfo); };

        It should_use_default_Base_type =() => _generateCode.IndexOf(typeof (eXpandCustomObject).FullName).ShouldBeGreaterThan(-1);
    }


    [Subject(typeof(CodeEngine))]
    [Isolated]
    public class When_generating_code_from_persistentClassinfo:With_In_Memory_DataStore {
        static PersistentClassInfo _persistentClassInfo;

        static string _generateCode;

        Establish context = () => {
            var classCodeTemplate = new CodeTemplate(Session.DefaultSession){TemplateType = TemplateType.Class};
            classCodeTemplate.SetDefaults();
            _persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) {Name ="TestClass",  BaseType = typeof(User) ,CodeTemplateInfo = {TemplateInfo = classCodeTemplate},PersistentAssemblyInfo = new PersistentAssemblyInfo(Session.DefaultSession)};
            _persistentClassInfo.TypeAttributes.Add(new PersistentDefaultClassOptionsAttribute(Session.DefaultSession));

            var memberCodeTemplate = new CodeTemplate(Session.DefaultSession) { TemplateType = TemplateType.ReadWriteMember};
            memberCodeTemplate.SetDefaults();
            var persistentCoreTypeMemberInfo = new PersistentCoreTypeMemberInfo(Session.DefaultSession) { Name = "property1",CodeTemplateInfo = {TemplateInfo = memberCodeTemplate}};
            persistentCoreTypeMemberInfo.TypeAttributes.Add(new PersistentSizeAttribute(Session.DefaultSession));            
            _persistentClassInfo.OwnMembers.AddRange(new[] {
                                                               persistentCoreTypeMemberInfo,
                                                               new PersistentCoreTypeMemberInfo(Session.DefaultSession){Name = "property2",CodeTemplateInfo = {TemplateInfo = memberCodeTemplate}}
                                                           });
            var interfaceInfo = new InterfaceInfo(Session.DefaultSession){Assembly =new AssemblyName(typeof(IDummyString).Assembly.FullName+"").Name,Name = typeof(IDummyString).FullName};
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

    public class When_generating_code_from_2_CSharp_classes:With_In_Memory_DataStore {
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        static string _generateCode;

        Establish context = () => {
            _persistentAssemblyInfo = new PersistentAssemblyInfo(Session.DefaultSession);
            createClass();
            createClass();
        };

        static void createClass() {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession)
                                      {PersistentAssemblyInfo = _persistentAssemblyInfo};
            var codeTemplate = new CodeTemplate(Session.DefaultSession){TemplateType = TemplateType.Class};
            codeTemplate.SetDefaults();
            persistentClassInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
        }

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentAssemblyInfo);
        };

        It should_group_all_usings_together_at_the_top_of_generated_code=() => _generateCode.ShouldStartWith("using");
    }
    public class When_generating_code_from_2_VB_classes:With_In_Memory_DataStore {
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        static string _generateCode;

        Establish context = () => {
            _persistentAssemblyInfo = new PersistentAssemblyInfo(Session.DefaultSession){CodeDomProvider = CodeDomProvider.VB};
            createClass();
            createClass();
        };

        static void createClass() {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession)
                                      {PersistentAssemblyInfo = _persistentAssemblyInfo};
            var codeTemplate = new CodeTemplate(Session.DefaultSession){TemplateType = TemplateType.Class,CodeDomProvider = CodeDomProvider.VB};
            codeTemplate.SetDefaults();
            persistentClassInfo.CodeTemplateInfo.TemplateInfo = codeTemplate;
        }

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentAssemblyInfo);
        };

        It should_group_all_usings_together_at_the_top_of_generated_code=() => _generateCode.ShouldStartWith("Imports");
    }
}