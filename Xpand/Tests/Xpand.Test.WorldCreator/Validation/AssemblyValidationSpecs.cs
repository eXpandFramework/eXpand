using System;
using System.IO;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Exceptions;
using FluentAssertions;
using Mono.Cecil;
using Xpand.ExpressApp.WorldCreator.BusinessObjects;
using Xpand.ExpressApp.WorldCreator.CodeProvider;
using Xpand.ExpressApp.WorldCreator.CodeProvider.Validation;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Test.WorldCreator.TestArtifacts;
using Xpand.Test.WorldCreator.TestArtifacts.Autofixture;
using Xunit;

namespace Xpand.Test.WorldCreator.Validation{
    public class AssemblyValidationSpecs : BaseSpecs{
        [Theory, WorldCreatorAutoData]
        private void Not_Well_Form_Association(WCTestData testData, IPersistentReferenceMemberInfo referenceMemberInfo,
            CodeValidator codeValidator){
            referenceMemberInfo.CreateAssociation("invalid");

            var result = referenceMemberInfo.Owner.PersistentAssemblyInfo.Validate(AssemblyPath);

            result.Valid.Should().Be(false);
            result.Message.Should().Contain("Malformed association 'invalid'");
        }

        [Theory, WorldCreatorAutoData]
        public void Missing_Key_Property(WCTestData testData, IPersistentClassInfo persistentClassInfo){
            persistentClassInfo.BaseType = typeof(XPBaseObject);

            var result = persistentClassInfo.PersistentAssemblyInfo.Validate(AssemblyPath);

            result.Valid.Should().BeFalse();

            result.Message.Should().Contain(typeof(KeyPropertyAbsentException).Name);
        }

        [Theory, WorldCreatorAutoData]
        public void Double_Key_Properties(WCTestData testData, IPersistentCoreTypeMemberInfo[] memberInfos){
            foreach (var memberInfo in memberInfos){
                memberInfo.TypeAttributes.Add(new PersistentKeyAttribute(memberInfo.Session));
            }
            var info = memberInfos.First();
            info.Owner = memberInfos.Last().Owner;

            var result = info.Owner.PersistentAssemblyInfo.Validate(AssemblyPath);

            result.Valid.Should().BeFalse();
            result.Message.Should().Contain(typeof(DuplicateKeyPropertyException).Name);
        }

        [Theory, WorldCreatorAutoData]
        public void Validation_Failure_after_Succeful_Validation(WCTestData testData,
            IPersistentClassInfo persistentClassInfo){
            var result = persistentClassInfo.PersistentAssemblyInfo.Validate(AssemblyPath);
            result.Valid.Should().BeTrue();
            persistentClassInfo.BaseType = null;
            var assembliesLenght = AppDomain.CurrentDomain.GetAssemblies().Length;

            result = persistentClassInfo.PersistentAssemblyInfo.Validate(AssemblyPath);

            result.Valid.Should().BeFalse();
            result.Message.Should().Contain("Type expected");
            AppDomain.CurrentDomain.GetAssemblies().Length.Should().Be(assembliesLenght);
        }

        [Theory, WorldCreatorAutoData]
        public void Successfull_Validation_After_Validation_Failure(WCTestData testData,
            IPersistentClassInfo persistentClassInfo){
            persistentClassInfo.BaseType = null;
            var result = persistentClassInfo.PersistentAssemblyInfo.Validate(AssemblyPath);
            result.Valid.Should().BeFalse();
            persistentClassInfo.BaseType = typeof(XpandBaseCustomObject);

            result = persistentClassInfo.PersistentAssemblyInfo.Validate(AssemblyPath);

            result.Valid.Should().BeTrue();
            var assemblyFile =
                Directory.GetFiles(AssemblyPath, "*" + Compiler.XpandExtension, SearchOption.AllDirectories).First();
            var fullName = AssemblyDefinition.ReadAssembly(assemblyFile).FullName;
            AppDomain.CurrentDomain.GetAssemblies()
                .Any(assembly => assembly.FullName == fullName)
                .Should()
                .BeFalse();
        }

    }
}