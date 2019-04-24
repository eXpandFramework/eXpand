using System;
using System.Linq;
using DevExpress.Xpo;
using FluentAssertions;
using Mono.Cecil;
using Xpand.ExpressApp.WorldCreator.BusinessObjects;
using Xpand.ExpressApp.WorldCreator.CodeProvider;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Test.WorldCreator.TestArtifacts;
using Xpand.Test.WorldCreator.TestArtifacts.Autofixture;
using Xunit;

namespace Xpand.Test.WorldCreator.Compilation{
    [Trait(Traits.Compilation, "From a persistent class generate")]
    public class PersistentClassSpecs{
        [WorldCreatorAutoData,Theory]
        public void Primitive_type_member(IWCTestData testData,IPersistentCoreTypeMemberInfo memberInfo,Compiler compiler){
            var assemblyInfo = memberInfo.Owner.PersistentAssemblyInfo;
            var code = assemblyInfo.GenerateCode();

            var compilerResult = compiler.Compile(code, assemblyInfo.Name);

            var propertyDefinitions = compilerResult.AssemblyDefinition.MainModule.Types.SelectMany(definition => definition.Properties);
            var propertyDefinition = propertyDefinitions.FirstOrDefault(definition => definition.Name == memberInfo.Name);
            propertyDefinition.Should().NotBeNull();
            propertyDefinition?.PropertyType.Name.Should().Be(memberInfo.DataType.ToString());
        }

        [WorldCreatorAutoData, Theory]
        public void Reference_member(IWCTestData testData, IPersistentReferenceMemberInfo memberInfo, Compiler compiler) {
            var assemblyInfo = memberInfo.Owner.PersistentAssemblyInfo;
            var code = assemblyInfo.GenerateCode();

            var compilerResult = compiler.Compile(code, assemblyInfo.Name);

            var propertyDefinitions = compilerResult.AssemblyDefinition.MainModule.Types.SelectMany(definition => definition.Properties);
            var propertyDefinition = propertyDefinitions.FirstOrDefault(definition => definition.Name == memberInfo.Name);
            propertyDefinition.Should().NotBeNull();
            propertyDefinition?.PropertyType.Name.Should().Be(memberInfo.ReferenceClassInfo.Name);
        }

        [WorldCreatorAutoData, Theory]
        public void Collection_member(IWCTestData testData, IPersistentCollectionMemberInfo memberInfo, Compiler compiler) {
            var assemblyInfo = memberInfo.Owner.PersistentAssemblyInfo;
            var code = assemblyInfo.GenerateCode();

            var compilerResult = compiler.Compile(code, assemblyInfo.Name);

            var propertyDefinitions = compilerResult.AssemblyDefinition.MainModule.Types.SelectMany(definition => definition.Properties);
            var propertyDefinition = propertyDefinitions.FirstOrDefault(definition => definition.Name == memberInfo.Name);
            propertyDefinition.Should().NotBeNull();
            propertyDefinition?.PropertyType.ToString().Should().Contain(typeof(XPCollection).FullName);
            propertyDefinition?.PropertyType.ToString().Should().Contain(memberInfo.CollectionClassInfo.Name);
        }

        [WorldCreatorAutoData, Theory]
        public void OneToMany_association(IWCTestData testData, IPersistentReferenceMemberInfo memberInfo, Compiler compiler){
            CreateAndCheckAssociation(memberInfo, compiler);
        }

        private AssemblyDefinition CreateAndCheckAssociation(IPersistentAssociatedMemberInfo memberInfo, Compiler compiler, bool autogenerateOtherPart=false){
            memberInfo.CreateAssociation("association");
            var assemblyInfo = memberInfo.Owner.PersistentAssemblyInfo;
            var code = assemblyInfo.GenerateCode();

            var compilerResult = compiler.Compile(code, assemblyInfo.Name);

            return CheckAssociation(memberInfo, compilerResult.AssemblyDefinition);
        }

        private AssemblyDefinition CheckAssociation(IPersistentAssociatedMemberInfo memberInfo, AssemblyDefinition assemblyDefinition){
            var propertyDefinitions =
                assemblyDefinition.MainModule.Types.SelectMany(definition => definition.Properties);
            var propertyDefinition = propertyDefinitions.First(definition => definition.Name == memberInfo.Name);
            var customAttribute = propertyDefinition.CustomAttributes.FirstOrDefault(
                attribute => attribute.AttributeType.Name.Contains(typeof(AssociationAttribute).Name));
            customAttribute.Should().NotBeNull();
            customAttribute?.Properties.FirstOrDefault(argument => argument.Name == "assocation").Should().NotBeNull();
            return assemblyDefinition;
        }

        [WorldCreatorAutoData, Theory]
        public void ManyToOne_association(IWCTestData testData, IPersistentCollectionMemberInfo memberInfo, Compiler compiler) {
            CreateAndCheckAssociation(memberInfo, compiler);
        }

        [WorldCreatorAutoData, Theory]
        public void Many_to_many_association(IWCTestData testData, IPersistentCollectionMemberInfo collectionMemberInfo,
            IPersistentReferenceMemberInfo referenceMemberInfo, Compiler compiler){
            referenceMemberInfo.CreateAssociation("test");
            collectionMemberInfo.CreateAssociation("test");
            var persistentAssemblyInfo = referenceMemberInfo.Owner.PersistentAssemblyInfo;
            var code = persistentAssemblyInfo.GenerateCode();

            var assemblyDefinition = compiler.Compile(code, persistentAssemblyInfo.Name).AssemblyDefinition;

            
            CheckAssociation(referenceMemberInfo, assemblyDefinition);
            CheckAssociation(collectionMemberInfo, assemblyDefinition);
            
        }

        [WorldCreatorAutoData, Theory]
        public void Automatically_the_Many_part_of_the_association_from_the_one(IWCTestData testData, IPersistentReferenceMemberInfo memberInfo, Compiler compiler) {
            throw new NotImplementedException();
//            var assemblyDefinition = CreateAndCheckAssociation(memberInfo, compiler,true);
//
//            var persistentAssociatedMemberInfo = memberInfo.GetAssociation();
//            persistentAssociatedMemberInfo.Should().BeOfType<IPersistentCollectionMemberInfo>();
//            CheckAssociation(persistentAssociatedMemberInfo,assemblyDefinition);
        }
        [WorldCreatorAutoData, Theory]
        public void Automatically_The_One_part_of_the_association_from_the_many(){
            throw new NotImplementedException();
        }
        [WorldCreatorAutoData, Theory]
        public void Automatically_The_Many_part_of_the_association_from_the_many(){
            throw new NotImplementedException();
        }
    }
}