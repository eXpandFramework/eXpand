using System.Collections.Generic;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;
using Microsoft.SqlServer.Management.Smo;
using TypeMock.ArrangeActAssert;
using eXpand.ExpressApp.WorldCreator.Core;

namespace eXpand.Tests.eXpand.WorldCreator.DbMapper
{

    public class With_table:With_DataBase {
        protected static Table _table;
        protected const string TableName="TableName";
        protected static PersistentAssemblyInfo _persistentAssemblyInfo;

        Establish context = () => {
            _table=AddTable(TableName);
            AddPrimaryKey(_table);
            _persistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            Isolate.WhenCalled(() => _database.Tables[TableName]).WillReturn(_table);
        };

        protected static Table AddTable(string tableName) {
            var table = Isolate.Fake.Instance<Table>();
            Isolate.WhenCalled(() => table.Parent).WillReturn(_database);
            Isolate.WhenCalled(() => _database.Tables[tableName]).WillReturn(table);
            table.Name = tableName;
            return table;
        }

        protected static void AddPrimaryKey(Table table) {
            var column = Isolate.Fake.Instance<Column>();
            Isolate.WhenCalled(() => column.InPrimaryKey).WillReturn(true);
            Isolate.WhenCalled(() => table.Columns).WillReturnCollectionValuesOf(new List<Column>{column});
        }
    }
    public abstract class With_Column : With_table
    {
        protected const string ColumnName = "ColumnName";
        protected static PersistentClassInfo _owner;
        protected static Column _column;

        Establish context = () =>
        {
            _column=AddColumn(ColumnName,_table);
            _owner = new PersistentClassInfo(UnitOfWork){PersistentAssemblyInfo = new PersistentAssemblyInfo(UnitOfWork)};
            _owner.SetDefaultTemplate(TemplateType.Class);
        };

        protected static Column AddColumn(string columnName,Table table) {
            var column = Isolate.Fake.Instance<Column>();
            column.Name = columnName;
            Isolate.WhenCalled(() => column.Parent).WillReturn(table);
            return column;
        }
    }
    public abstract class With_DataBase:With_In_Memory_DataStore
    {
        protected static Database _database;

        Establish context = () => {_database = Isolate.Fake.Instance<Database>();          
        };
    }
    public class With_ForeignKey_Column : With_table
    {
        protected static Column _refColumn;
        protected const string ColumnName = "ColumnName";
        protected static Table _refTable;
        protected const string RefTablePKFKColumn = "RefTablePKFKColumn";
        protected static ForeignKey _foreignKey;
        protected static PersistentClassInfo _refPersistentClassInfo;
        protected const string FK_Name = "FK_Name";
        protected const string RefTable = "RefTable";
        protected static PersistentClassInfo _owner;

        protected static Column _column;

        Establish context = () =>
        {
        
            _column = AddColumn(_table, ColumnName);
            _foreignKey = AddForeignKey(FK_Name, RefTable, RefTablePKFKColumn,_table,_column);
            _refPersistentClassInfo = ObjectSpace.CreateObject<PersistentClassInfo>();
            _refPersistentClassInfo.PersistentAssemblyInfo = new PersistentAssemblyInfo(UnitOfWork);
            _refPersistentClassInfo.Name = RefTable;
            _refPersistentClassInfo.SetDefaultTemplate(TemplateType.Class);
            _owner = new PersistentClassInfo(UnitOfWork) { PersistentAssemblyInfo = new PersistentAssemblyInfo(UnitOfWork) };
            _owner.SetDefaultTemplate(TemplateType.Class);
            _refTable= AddTable(RefTable);
            _refColumn = AddColumn(_refTable, RefTablePKFKColumn);
        };

        protected static Column AddColumn(Table table, string name)
        {
            var column = Isolate.Fake.Instance<Column>();
            Isolate.WhenCalled(() => column.IsForeignKey).WillReturn(true);
            Isolate.WhenCalled(() => column.Name).WillReturn(name);
            Isolate.WhenCalled(() => column.Parent).WillReturn(table);
            Isolate.WhenCalled(() => table.Columns[name]).WillReturn(column);
            return column;
        }

        protected static void AddForeignKeyColumn(ForeignKey foreignKey, string name, string refColumnName)
        {
            var foreignKeyColumn = Isolate.Fake.Instance<ForeignKeyColumn>();
            Isolate.WhenCalled(() => foreignKeyColumn.ReferencedColumn).WillReturn(refColumnName);
            Isolate.WhenCalled(() => foreignKeyColumn.Name).WillReturn(name);
            if (foreignKey.Columns.Count==0)
                Isolate.WhenCalled(() => foreignKey.Columns).WillReturnCollectionValuesOf(new List<ForeignKeyColumn> { foreignKeyColumn });
            else {
                foreignKey.Columns.Add(foreignKeyColumn);
            }
        }

        protected static ForeignKey AddForeignKey(string name, string refTableName, string refColumnName,Table table, Column column)
        {
            var foreignKey = Isolate.Fake.Instance<ForeignKey>();
            foreignKey.Name = name;
            Isolate.WhenCalled(() => foreignKey.Parent).WillReturn(table);
            Isolate.WhenCalled(() => foreignKey.Parent.Parent).WillReturn(_database);
            Isolate.WhenCalled(() => foreignKey.ReferencedTable).WillReturn(refTableName);
            if (table.ForeignKeys.Count==0)
                Isolate.WhenCalled(() => table.ForeignKeys).WillReturnCollectionValuesOf(new List<ForeignKey> { foreignKey });
            else {
                table.ForeignKeys.Add(foreignKey);
            }
            Isolate.WhenCalled(() => foreignKey.ReferencedKey).WillReturn(refColumnName);
            AddForeignKeyColumn(foreignKey, column.Name, refColumnName);
            return foreignKey;
        }
    }

}
