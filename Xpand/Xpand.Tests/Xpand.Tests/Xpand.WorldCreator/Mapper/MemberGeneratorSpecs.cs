using System.Collections.Generic;
using System.Collections.Specialized;
using DevExpress.Xpo.DB;
using Machine.Specifications;
using Xpand.ExpressApp.WorldCreator.DBMapper;
using Xpand.ExpressApp.WorldCreator.SqlDBMapper;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using System.Linq;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Xpand.ExpressApp.WorldCreator.Core;

namespace Xpand.Tests.Xpand.WorldCreator.Mapper {
    [Subject(typeof(MemberGenerator), "Create CoreMemberInfo")]
    public class When_column_is_not_a_foreign_key_column : With_In_Memory_DataStore {
        static PersistentCoreTypeMemberInfo _persistentMemberInfo;
        static RefMemberGeneratorHelper _generatorHelper;

        Establish context = () => {
            _generatorHelper = new RefMemberGeneratorHelper(ObjectSpace);
        };

        Because of = () => {
            _persistentMemberInfo = new MemberGenerator(_generatorHelper.DbTable, _generatorHelper.ClassGeneratorInfos).Create().Select(info => info.PersistentMemberInfo).OfType<PersistentCoreTypeMemberInfo>().FirstOrDefault();
        };

        It should_return_a_persistentcorememberinfo = () => _persistentMemberInfo.ShouldNotBeNull();
        It should_have_as_name_the_column_name = () => _persistentMemberInfo.Name.ShouldEqual(_generatorHelper.Column.Name);
        It should_have_as_owner_the_passed_in_owner_classinfo = () => _persistentMemberInfo.Owner.ShouldEqual(_generatorHelper.PersistentAssemblyInfo.PersistentClassInfos.First());

        It should_have_as_datatype_the_one_taken_from_datatypemapper =
            () => _persistentMemberInfo.DataType.ShouldEqual(DBColumnType.Int32);

        It should_have_a_ReadWriteMember_template =
                    () => _persistentMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType.ShouldEqual(TemplateType.XPReadWritePropertyMember);

    }


    [Subject(typeof(MemberGenerator), "Create ReferenceMemberInfo")]
    public class When_column_is_a_foreign_key_column : With_In_Memory_DataStore {
        static PersistentCollectionMemberInfo _persistentCollectionMemberInfo;
        static PersistentReferenceMemberInfo _persistentReferenceMemberInfo;
        static RefMemberGeneratorHelper _generatorHelper;

        Establish context = () => {
            _generatorHelper = new RefMemberGeneratorHelper(ObjectSpace);

        };

        Because of = () => {
            _persistentReferenceMemberInfo = new MemberGenerator(_generatorHelper.DbTable, _generatorHelper.ClassGeneratorInfos).Create().Select(info => info.PersistentMemberInfo).OfType<PersistentReferenceMemberInfo>().FirstOrDefault();
        };

        It should_create_a_persistenetReferenceMember = () => _persistentReferenceMemberInfo.ShouldNotBeNull();

        It should_have_as_name_the_name_of_the_column =
            () => _persistentReferenceMemberInfo.Name.ShouldEqual(_generatorHelper.RefColumn.Name);

        It should_have_as_ReferenceClassInfo_the_one_with_the_name_of_the_FK_reference_table = () => _persistentReferenceMemberInfo.ReferenceClassInfo.Name.ShouldEqual(_generatorHelper.RefDbTable.Name);

        It should_create_a_collection_association_to_the_referenceclassinfo =
            () => {
                _persistentCollectionMemberInfo = _persistentReferenceMemberInfo.ReferenceClassInfo.OwnMembers.OfType<PersistentCollectionMemberInfo>().FirstOrDefault();
                _persistentCollectionMemberInfo.ShouldNotBeNull();
            };

        It should_have_as_name_the_name_of_the_column_plus_an_s = () => _persistentCollectionMemberInfo.Name.ShouldEqual(_generatorHelper.DbTable.Name + _generatorHelper.RefColumn.Name + "s");

        It should_have_as_owner_the_referenceClassInfo = () => _persistentCollectionMemberInfo.Owner.ShouldEqual(_persistentReferenceMemberInfo.ReferenceClassInfo);
        It should_have_as_classinfo_the_owner_of_the_reference_memberInfo = () => _persistentCollectionMemberInfo.CollectionClassInfo.ShouldEqual(_persistentReferenceMemberInfo.Owner);

        It should_have_an_association_with_the_same_name_as_the_reference_member_association = () =>
                    _persistentCollectionMemberInfo.TypeAttributes.OfType<PersistentAssociationAttribute>().Single().
                        AssociationName.ShouldEqual("refDbColumn-MainTablerefDbColumnss");

        It should_have_a_collectionMember_template =
                    () =>
                    _persistentCollectionMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType.ShouldEqual(TemplateType.XPCollectionMember);
    }

    [Subject(typeof(MemberGenerator), "Create Compound primary key MemberInfo")]
    public class When_column_table_has_compound_primary_keys : With_In_Memory_DataStore {
        static IPersistentReferenceMemberInfo _persistentMemberInfo;
        static IPersistentClassInfo _persistentStructClassInfo;
        static IPersistentCoreTypeMemberInfo _persistentCoreTypeMemberInfo;
        static CompoundPKMemberGeneratorHelper _generatorHelper;

        Establish context = () => {
            _generatorHelper = new CompoundPKMemberGeneratorHelper(ObjectSpace);
        };

        Because of = () => {
            _persistentMemberInfo = new MemberGenerator(_generatorHelper.DbTable, _generatorHelper.ClassGeneratorInfos).Create().Select(info => info.PersistentMemberInfo).OfType<IPersistentReferenceMemberInfo>().FirstOrDefault();
        };

        It should_create_areference_member_info = () => _persistentMemberInfo.ShouldNotBeNull();

        It should_have_as_reference_classinfo_the_one_with_the_same_name_as_the_column_table_name_plus_KeyStruct =
            () => {
                _persistentStructClassInfo = _persistentMemberInfo.ReferenceClassInfo;
                _persistentStructClassInfo.Name.ShouldEqual(_generatorHelper.DbTable.Name + TableMapper.KeyStruct);
                _persistentStructClassInfo.PersistentAssemblyInfo.ShouldEqual(_generatorHelper.PersistentAssemblyInfo);
            };

        It should_have_as_teamplate_The_readWriteMember =
            () => _persistentMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType.ShouldEqual(TemplateType.FieldMember);

        It should_add_the_primary_key_as_core_member_info_in_the_reference_class_info = () => {
            _persistentCoreTypeMemberInfo = _persistentStructClassInfo.OwnMembers.OfType<IPersistentCoreTypeMemberInfo>().FirstOrDefault();
        };

        It should_set_the_template_of_the_core_member_to_readWrite =
            () =>
            _persistentCoreTypeMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType.ShouldEqual(TemplateType.ReadWriteMember);

        It should_have_the_name_of_the_column = () => _persistentCoreTypeMemberInfo.Name.ShouldEndWith(_generatorHelper.DbColumn1.Name);
    }

    [Subject(typeof(MemberGenerator), "Create compound and foreing MemberInfo")]
    public class When_table_has_compound_primary_keys_and_they_are_foreignKeys : With_In_Memory_DataStore {
        static PersistentClassInfo _refPersistentClassInfo;
        static List<IPersistentMemberInfo> _persistentMemberInfos;
        static CompoundPKMemberGeneratorHelper _generatorHelper;

        Establish context = () => {
            _generatorHelper = new CompoundPKMemberGeneratorHelper(ObjectSpace);
            _refPersistentClassInfo = ObjectSpace.CreateObject<PersistentClassInfo>();
            _refPersistentClassInfo.Name = "RefTable";
            _refPersistentClassInfo.PersistentAssemblyInfo = (PersistentAssemblyInfo)_generatorHelper.PersistentAssemblyInfo;
            _refPersistentClassInfo.SetDefaultTemplate(TemplateType.Class);

            var refColumn = new DBColumn("RefColumn", true, "int", 0, DBColumnType.Int32);
            _generatorHelper.DbTable.PrimaryKey.Columns.Add(refColumn.Name);
            _generatorHelper.DbTable.Columns.Add(refColumn);
            _generatorHelper.DbTable.ForeignKeys.Add(new DBForeignKey(new[] { refColumn }, "RefTable", new StringCollection { "Oid" }));
            _generatorHelper.ClassGeneratorInfos.Add("RefTable", new ClassGeneratorInfo(_refPersistentClassInfo, new DBTable("RefTable") { PrimaryKey = new DBPrimaryKey() }));
        };

        Because of = () => {
            _persistentMemberInfos = new MemberGenerator(_generatorHelper.DbTable, _generatorHelper.ClassGeneratorInfos).Create().Select(info => info.PersistentMemberInfo).ToList();
        };

        //        It should_create_a_collection_to_the_referenced_classInfo =
        //            () => _refPersistentClassInfo.OwnMembers.OfType<PersistentCollectionMemberInfo>().Single().Owner.ShouldEqual(_generatorHelper.PersistentClassInfo);

        It should_create_a_reference_member_to_the_struct_classinfo =
            () => _persistentMemberInfos.OfType<IPersistentReferenceMemberInfo>().First().Owner.ShouldEqual(_generatorHelper.StructPersistentClassInfo);
    }

    [Subject(typeof(MemberGenerator), "One to one to the key member")]
    public class When_column_is_primary_key_and_foreign_key_to_a_table_that_has_its_primary_key_as_foreign_key_back_to_the_primary_table : With_In_Memory_DataStore {
        static IPersistentReferenceMemberInfo _persistentReferenceMemberInfo;
        static RefMemberGeneratorHelper _refMemberGeneratorHelper;
        static IPersistentCoreTypeMemberInfo _persistentCoreTypeMemberInfo;

        Establish context = () => {
            _refMemberGeneratorHelper = new RefMemberGeneratorHelper(ObjectSpace);
            DBTable refDbTable = _refMemberGeneratorHelper.RefDbTable;
            refDbTable.ForeignKeys.Add(new DBForeignKey(refDbTable.PrimaryKey.Columns, "Oid2", new StringCollection { "Oid2" }));
            _refMemberGeneratorHelper.DbTable.ForeignKeys.Add(new DBForeignKey(_refMemberGeneratorHelper.DbTable.PrimaryKey.Columns, refDbTable.Name, new StringCollection { "Oid" }));
        };

        Because of = () => {
            var persistentMemberInfos = new MemberGenerator(_refMemberGeneratorHelper.DbTable, _refMemberGeneratorHelper.ClassGeneratorInfos).Create().Select(info => info.PersistentMemberInfo);
            _persistentCoreTypeMemberInfo = persistentMemberInfos.OfType<IPersistentCoreTypeMemberInfo>().FirstOrDefault();
            _persistentReferenceMemberInfo = persistentMemberInfos.OfType<IPersistentReferenceMemberInfo>().FirstOrDefault();
        };

        It should_return_a_core_typeinfo = () => _persistentCoreTypeMemberInfo.ShouldNotBeNull();
        It should_return_a_ref_typeinfo = () => _persistentReferenceMemberInfo.ShouldNotBeNull();
        It should_have_an_one_to_one_readOnly_templates = () => _persistentReferenceMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType.ShouldEqual(TemplateType.XPOneToOneReadOnlyPropertyMember);
    }
    [Subject(typeof(MemberGenerator), "Compund One to one to the key member")]
    public class When_column_is_combound_primary_key_and_foreign_key_to_a_table_that_has_its_primary_key_as_foreign_key_back_to_the_primary_table : With_In_Memory_DataStore {
        static IPersistentReferenceMemberInfo _persistentReferenceMemberInfo;
        static RefMemberGeneratorHelper _refMemberGeneratorHelper;
        static IPersistentCoreTypeMemberInfo _persistentCoreTypeMemberInfo;

        Establish context = () => {

            _refMemberGeneratorHelper = new RefMemberGeneratorHelper(ObjectSpace);
            Setup(_refMemberGeneratorHelper.DbTable, _refMemberGeneratorHelper.RefDbTable);
            Setup(_refMemberGeneratorHelper.RefDbTable, _refMemberGeneratorHelper.DbTable);
            var persistentClassInfo = ObjectSpace.CreateObject<PersistentClassInfo>();
            persistentClassInfo.Name = _refMemberGeneratorHelper.DbTable.Name + ClassGenerator.KeyStruct;
            persistentClassInfo.PersistentAssemblyInfo = (PersistentAssemblyInfo)_refMemberGeneratorHelper.PersistentAssemblyInfo;
            persistentClassInfo.SetDefaultTemplate(TemplateType.Class);
            _refMemberGeneratorHelper.ClassGeneratorInfos.Add(persistentClassInfo.Name, new ClassGeneratorInfo(persistentClassInfo, null));
        };

        static void Setup(DBTable refTable, DBTable mainTable) {
            var dbColumns = new List<DBColumn>();
            var dbColumn = new DBColumn { Name = "Oid2" };
            dbColumns.Add(dbColumn);
            dbColumns.Add(mainTable.GetColumn("Oid"));
            mainTable.PrimaryKey = new DBPrimaryKey(dbColumns);
            mainTable.Columns.Add(dbColumn);
            mainTable.AddForeignKey(new DBForeignKey(dbColumns, refTable.Name, new StringCollection { "Oid", "Oid2" }));
        }

        Because of = () => {
            var persistentMemberInfos = new MemberGenerator(_refMemberGeneratorHelper.DbTable, _refMemberGeneratorHelper.ClassGeneratorInfos).Create().Select(info => info.PersistentMemberInfo);
            _persistentCoreTypeMemberInfo = persistentMemberInfos.OfType<IPersistentCoreTypeMemberInfo>().FirstOrDefault();
            _persistentReferenceMemberInfo = persistentMemberInfos.OfType<IPersistentReferenceMemberInfo>().LastOrDefault();
        };

        It should_return_a_core_typeinfo = () => _persistentCoreTypeMemberInfo.ShouldNotBeNull();
        It should_return_a_ref_typeinfo = () => _persistentReferenceMemberInfo.ShouldNotBeNull();
        It should_have_an_one_to_one_readOnly_templates = () => _persistentReferenceMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType.ShouldEqual(TemplateType.XPOneToOneReadOnlyPropertyMember);
    }
    [Subject(typeof(MemberGenerator), "One to one")]
    public class When_column_is_foreign_key_to_a_table_that_has_a_foreign_key_back_to_the_primary_table : With_In_Memory_DataStore {
        static RefMemberGeneratorHelper _refMemberGeneratorHelper;
        static IPersistentReferenceMemberInfo _persistentReferenceMemberInfo;

        Establish context = () => {
            _refMemberGeneratorHelper = new RefMemberGeneratorHelper(ObjectSpace);
            var dbColumn = new DBColumn("ref2", true, "int", 0, DBColumnType.Int32);
            _refMemberGeneratorHelper.RefDbTable.Columns.Add(dbColumn);
            _refMemberGeneratorHelper.RefDbTable.AddForeignKey(new DBForeignKey(new[] { dbColumn }, _refMemberGeneratorHelper.DbTable.Name, new StringCollection { "Oid" }));
        };

        Because of = () => {
            _persistentReferenceMemberInfo = new MemberGenerator(_refMemberGeneratorHelper.DbTable, _refMemberGeneratorHelper.ClassGeneratorInfos).Create().Select(info => info.PersistentMemberInfo).OfType<IPersistentReferenceMemberInfo>().First();
        };
        It should_return_a_referenced_typeinfo = () => _persistentReferenceMemberInfo.ShouldNotBeNull();

        It should_have_an_one_to_one_template =
            () =>
            _persistentReferenceMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType.ShouldEqual(
                TemplateType.XPOneToOnePropertyMember);
    }
}

class When_column_is_primary_key_and_Foreign {

}
