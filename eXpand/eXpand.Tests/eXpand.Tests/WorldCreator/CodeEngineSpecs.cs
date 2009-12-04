using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Xpo;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.WorldCreator
{
//    public class When_adding_a_typed_attribute_to_a_persistentTypeInfo_with_no_generated_code {
//        It should_not_change_persistentInfoType_generated_code;
//    }
//    [Subject(typeof(CodeEngine),"Code generation")]
//    public class When_adding_a_typed_attribute_to_a_persistentTypeInfo_with_generated_code {
//        static TrackNewObjectSavedController _trackNewObjectSavedController;
//
//        static PersistentCustomAttribute _persistentCustomAttribute;
//
//        Establish context = () => {
//            var persistentTypeInfo = Isolate.Fake.Instance<PersistentTypeInfo>(Members.CallOriginal,ConstructorWillBe.Called,Session.DefaultSession);
//            _persistentCustomAttribute = new PersistentCustomAttribute(Session.DefaultSession){PropertyName = "Visible",Value = false.ToString(),Owner = persistentTypeInfo};
//            var viewControllerFactory = new ViewControllerFactory();
//            var controller = viewControllerFactory.CreateController<GenreratePersistentTypeInfoAttributeCodeController>(ViewType.DetailView, _persistentCustomAttribute);
//            _trackNewObjectSavedController = new TrackNewObjectSavedController();
//            Isolate.WhenCalled(() => controller.Frame.GetController<TrackNewObjectSavedController>()).WillReturn(_trackNewObjectSavedController);
//            viewControllerFactory.Activate(controller);
//        };
//
//        Because of = () => Isolate.Invoke.Event(() => _trackNewObjectSavedController.NewObjectSaved += null, null, new NewObjectSavedEventArgs(_persistentCustomAttribute));
//
//        It should_add_that_code_as_1st_line_in_the_generated_code_of_persistentTypeInfo=() => {
//            _persistentCustomAttribute.Owner.GeneratedCode = "[" + typeof (CustomAttribute).FullName + "(Visible,false)]";
//        };
//    }

//    [Subject(typeof(CodeEngine), "Code generation")]
//    public class When_removing_an_attribute_from_a_persistentTypeInfo {
//        It should_remove_it_from_generated_code;
//    }

    [Subject(typeof(CodeEngine), "Code compilization when code has been already generated")]
    public class When_combiling_code_from_persistentClassinfo_with_generated_code_and_no_base_type_defined {
        static PersistentClassInfo _persistentClassInfo;

        static string _generateCode;

        Establish context = () => {
            _persistentClassInfo = new PersistentClassInfo(Session.DefaultSession){Name ="TestClass", GeneratedCode = "$CLASSNAME$:$BASECLASSNAME${}"};
        };

        Because of = () => { _generateCode = CodeEngine.GenerateCode(_persistentClassInfo); };

        It should_use_default_Base_type =() => _generateCode.IndexOf(typeof (eXpandCustomObject).FullName).ShouldBeGreaterThan(-1);
    }

    [Subject(typeof(CodeEngine), "Code compilization when code has been already generated")]
    public class When_combiling_code_from_persistentClassinfo_with_generated_code {
        static PersistentClassInfo _persistentClassInfo;
        static string _generateCode;

        Establish context = () => {
            _persistentClassInfo = new PersistentClassInfo(Session.DefaultSession){Name ="TestClass", GeneratedCode = "$CLASSNAME$:$BASECLASSNAME${}"};
            _persistentClassInfo.OwnMembers.Add(new PersistentCoreTypeMemberInfo(Session.DefaultSession) { Name = "property1", GeneratedCode = "$PROPERTYTYPE$$PROPERTYNAME$" });
        };

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentClassInfo);
        };

        It should_use_codegenerated_for_compilization=() => {
            _generateCode.IndexOf("TestClass").ShouldBeGreaterThan(-1);
            _generateCode.IndexOf("property1").ShouldBeGreaterThan(-1);
        };
    }

    [Subject(typeof(CodeEngine), "Code generation")]
    public class When_combiling_code_from_persistentClassinfo_with_ungenerated_code {
        static PersistentClassInfo _persistentClassInfo;

        static string _generateCode;

        Establish context = () => {
            _persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) {Name ="TestClass", CodeTemplate = new CodeTemplate(Session.DefaultSession) { TemplateCode = "$CLASSNAME$:$BASECLASSNAME${}" }, BaseType = typeof(User) };
            var persistentCoreTypeMemberInfo = new PersistentCoreTypeMemberInfo(Session.DefaultSession){Name = "property1",GeneratedCode = "$PROPERTYTYPE$$PROPERTYNAME$"};
            persistentCoreTypeMemberInfo.TypeAttributes.Add(new PersistentSizeAttribute(Session.DefaultSession));            
            _persistentClassInfo.OwnMembers.AddRange(new[] {
                                                               persistentCoreTypeMemberInfo,
                                                               new PersistentCoreTypeMemberInfo(Session.DefaultSession){Name = "property2",GeneratedCode = "$PROPERTYTYPE$$PROPERTYNAME$"}
                                                           });            
        };

        Because of = () => {
            _generateCode = CodeEngine.GenerateCode(_persistentClassInfo);
        };

        It should_inject_code_from_all_members_in_classinfo_generated_code = () => _generateCode.IndexOf("property1" + Environment.NewLine + "property2" + Environment.NewLine + "}");
        It should_have_class_type_typeattributes_generated;
        It should_have_propertytype_typeattributes_generated;
        It should_replace_CLASSNAME_with_persistentClassInfo_name=() => {
            _generateCode.IndexOf("TestClass").ShouldBeGreaterThan(-1);
            _generateCode.IndexOf("CLASSNAME").ShouldEqual(-1);
        };

        It should_replace_BASECLASSNAME_with_persistentClassInfo_name = () => {
            _generateCode.IndexOf(typeof(User).FullName).ShouldBeGreaterThan(-1);
            _generateCode.IndexOf("BASECLASSNAME").ShouldEqual(-1);
        };

        It should_replace_PROPERTYNAME_with_persistentClassInfo_name = () => _generateCode.IndexOf("PROPERTYNAME").ShouldEqual(-1);
        It should_replace_PROPERTYTYPE_with_persistentClassInfo_type = () => _generateCode.IndexOf("PROPERTYTYPE").ShouldEqual(-1);
    }

}
