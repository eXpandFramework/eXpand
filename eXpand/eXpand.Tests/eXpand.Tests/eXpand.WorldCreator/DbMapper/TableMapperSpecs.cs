using System.Collections.Generic;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.SqlDBMapper;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Machine.Specifications;
using Microsoft.SqlServer.Management.Smo;
using TypeMock.ArrangeActAssert;
using eXpand.Xpo;
using System.Linq;

namespace eXpand.Tests.eXpand.WorldCreator.DbMapper
{
    [Subject(typeof(TableMapper))]
    public class When_mapping_a_table : With_table
    {
        static PersistentPersistentAttribute _persistentPersistentAttribute;
        static AttributeMapper _attributeMapper;
        static IPersistentClassInfo _persistentClassInfo;

        Establish context = () => {
            _persistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            Isolate.WhenCalled(() => _table.Name).WillReturn("test");
            _attributeMapper = new AttributeMapper(ObjectSpace);
            _persistentPersistentAttribute = new PersistentPersistentAttribute(UnitOfWork);
            Isolate.WhenCalled(() => _attributeMapper.Create(_table, null, null)).WillReturn(new List<IPersistentAttributeInfo> { _persistentPersistentAttribute });
        };

        Because of = () =>
        {
            _persistentClassInfo = new TableMapper(ObjectSpace, _database, _attributeMapper).Create(_table, _persistentAssemblyInfo, Isolate.Fake.Instance<IMapperInfo>());
        };

        It should_return_a_persistent_classinfo_with_name_the_table_name = () => _persistentClassInfo.Name.ShouldEqual("test");

        It should_belong_to_the_passed_in_assembly =
            () => _persistentClassInfo.PersistentAssemblyInfo.ShouldEqual(_persistentAssemblyInfo);

        It should_derive_from_XpLiteObject =
            () => _persistentClassInfo.BaseType.ShouldEqual(typeof(XPLiteObject));

        It should_have_a_class_template =
            () => _persistentClassInfo.CodeTemplateInfo.CodeTemplate.TemplateType.ShouldEqual(TemplateType.Class);

        It should_have_the_attributes_of_the_attributemapper =
            () => _persistentClassInfo.TypeAttributes[0].ShouldEqual(_persistentPersistentAttribute);
    }
    [Subject(typeof(TableMapper))]
    public class When_a_persistent_class_info_with_the_same_table_name_exists_and_create_that_table:With_table {
        static PersistentClassInfo _info;
        static IPersistentClassInfo _persistentClassInfo;

        Establish context = () => {
            Isolate.WhenCalled(() => _table.Name).WillReturn("test");
            _info = new PersistentClassInfo(UnitOfWork) {Name = _table.Name};
        };

        Because of = () => {
            _persistentClassInfo = new TableMapper(ObjectSpace, _database, new AttributeMapper(ObjectSpace)).Create(_table, _persistentAssemblyInfo, Isolate.Fake.Instance<IMapperInfo>());
        };

        It should_return_the_classinfo_from_the_datastore = () => _persistentClassInfo.ShouldEqual(_info);
    }
    [Subject(typeof(TableMapper))]
    public class When_creating_a_table_that_has_foreigh_keys:With_table {
        private const string RefName = "RefName";
        

        Establish context = () => {
            _persistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            Isolate.WhenCalled(() => _table.Name).WillReturn("PrimaryTable");
            var refTable = AddTable(RefName);
            AddPrimaryKey(refTable);
            var foreignKey = Isolate.Fake.Instance<ForeignKey>();
            Isolate.WhenCalled(() => foreignKey.ReferencedTable).WillReturn(refTable.Name);            
            Isolate.WhenCalled(() => _table.ForeignKeys).WillReturnCollectionValuesOf(new List<ForeignKey>{foreignKey});
        };

        Because of = () => new TableMapper(ObjectSpace, _database, new AttributeMapper(ObjectSpace)).Create(_table, _persistentAssemblyInfo, Isolate.Fake.Instance<IMapperInfo>());

        It should_create_a_persistent_classInfo_for_the_foreignkey_as_well =
            () =>
            UnitOfWork.FindObject<PersistentClassInfo>(PersistentCriteriaEvaluationBehavior.InTransaction,
                                                     info => info.Name == RefName).ShouldNotBeNull();
    }
    [Subject(typeof(TableMapper))]
    public class When_mapping_a_table_that_has_more_than_one_primary_keys:With_table {
        static PersistentClassInfo _persistentClassInfo;

        Establish context = () => {
            var pk1 = Isolate.Fake.Instance<Column>();
            Isolate.WhenCalled(() => pk1.InPrimaryKey).WillReturn(true);
            var pk2 = Isolate.Fake.Instance<Column>();
            Isolate.WhenCalled(() => pk2.InPrimaryKey).WillReturn(true);
            Isolate.WhenCalled(() => _table.Columns).WillReturnCollectionValuesOf(new List<Column>{pk1,pk2});
        };

        Because of = () => new TableMapper(ObjectSpace, _database, new AttributeMapper(ObjectSpace)).Create(_table, _persistentAssemblyInfo, Isolate.Fake.Instance<IMapperInfo>());
        It should_add_an_empty_templateinfo_with_name_Support_Persistemt_Objects_as_part_of_a_composite_key = () =>
        {
            var templateInfo = _persistentAssemblyInfo.PersistentClassInfos[0].TemplateInfos.SingleOrDefault();
            templateInfo.ShouldNotBeNull();
            templateInfo.Name.ShouldEqual(ExtraInfoBuilder.SupportPersistentObjectsAsAPartOfACompositeKey);
        };
        It should_create_a_classInfo_with_name_the_name_of_the_table__plus_KeyStruct = () => {
            _persistentClassInfo = ObjectSpace.Session.FindObject<PersistentClassInfo>(PersistentCriteriaEvaluationBehavior.InTransaction, info => info.Name == TableName + TableMapper.KeyStruct);
            _persistentClassInfo.ShouldNotBeNull();
        };

        It should_set_the_template_of_the_classInfo_to_struct = () => _persistentClassInfo.CodeTemplateInfo.CodeTemplate.TemplateType.ShouldEqual(TemplateType.Struct);

        
    }
}
