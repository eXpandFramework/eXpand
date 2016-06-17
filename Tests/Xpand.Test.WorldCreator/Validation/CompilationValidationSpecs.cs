using FluentAssertions;
using Moq;
using Ploeh.AutoFixture;
using Xpand.ExpressApp.WorldCreator.CodeProvider;
using Xpand.ExpressApp.WorldCreator.CodeProvider.Validation;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Test.WorldCreator.TestArtifacts;
using Xpand.Test.WorldCreator.TestArtifacts.Autofixture;
using Xunit;

namespace Xpand.Test.WorldCreator.Validation{
    public class ValidCompilationSpecs : BaseSpecs{
        [Theory, WorldCreatorAutoData()]
        public void Compilable_Code(WCTestData testData, IPersistentClassInfo persistentClassInfo,
            Mock<IAssemblyValidator> assemblyValidatorMock, Mock<ICompiler> compilerMock){
            assemblyValidatorMock.Setup(validator => validator.Validate(It.IsAny<string>()))
                .Returns(() => new ValidatorResult());
            var codeValidator = testData.Fixture.Create<CodeValidator>();
            
            var compilerResults = new Mock<ICompilerResult>();
            compilerMock.Setup(compiler => compiler.Compile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>()))
                .Returns(compilerResults.Object);
            compilerMock.SetupGet(compiler => compiler.AssemblyPath).Returns(AssemblyPath);
            var persistentAssemblyInfo = persistentClassInfo.PersistentAssemblyInfo;
            var code = persistentAssemblyInfo.GenerateCode();

            var result = codeValidator.Validate(code);

            result.Message.Should().BeNull();
            result.Valid.Should().BeTrue();
            assemblyValidatorMock.Verify(validator => validator.Validate(It.IsAny<string>()));
        }
    }

    public class InvalidCompilationValidationSpecs : BaseSpecs{
        [Theory, WorldCreatorAutoData]
        public void Compile_Time_Exceptions(WCTestData testData, IPersistentCoreTypeMemberInfo memberInfo){
            var persistentAssemblyInfo = memberInfo.Owner.PersistentAssemblyInfo;
            memberInfo.TypeAttributes.Add(new PersistentKeyAttribute(memberInfo.Session));
            memberInfo.TypeAttributes.Add(new PersistentKeyAttribute(memberInfo.Session));

            var result = persistentAssemblyInfo.Validate(AssemblyPath);


            result.Valid.Should().Be(false);
            result.Message.Should().Contain("Duplicate 'DevExpress.Xpo.KeyAttribute' attribute");
        }

        [Theory, WorldCreatorAutoData]
        public void Non_Compilable_Code(WCTestData testData, IPersistentClassInfo persistentClassInfo){
            persistentClassInfo.CodeTemplateInfo.TemplateInfo.TemplateCode = "invalid";

            var result = persistentClassInfo.PersistentAssemblyInfo.Validate(AssemblyPath);

            result.Valid.Should().Be(false);
            result.Message.Should().Contain("A namespace cannot directly contain members such as fields or methods");
        }
    }
}