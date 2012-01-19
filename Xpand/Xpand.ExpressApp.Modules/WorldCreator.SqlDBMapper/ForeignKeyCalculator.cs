using System;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;

namespace Xpand.ExpressApp.WorldCreator.SqlDBMapper {
    public class ForeignKeyCalculator {
        public bool IsOneToOne(ForeignKey foreignKey, string referenceColumn) {
            var refTableForeignKey = GetRefTableForeignKey(foreignKey, referenceColumn);
            return refTableForeignKey != null && !IsSelfReference(foreignKey, refTableForeignKey);
        }

        bool IsSelfReference(ForeignKey foreignKey, ForeignKey refTableForeignKey) {
            return refTableForeignKey.Parent.Name == foreignKey.Parent.Name;
        }

        public ForeignKey GetRefTableForeignKey(ForeignKey foreignKey, string referencedColumn) {
            return foreignKey.Parent.Parent.GetTable(foreignKey.ReferencedTable, foreignKey.ReferencedTableSchema).ForeignKeys.OfType<ForeignKey>().Where(
                key => key.ReferencedTable == foreignKey.Parent.Name && key.Columns.OfType<ForeignKeyColumn>().Any(column => column.ReferencedColumn == referencedColumn)).FirstOrDefault();
        }
        public string GetForeignKeyName(string name, Table table) {
            var foreignTableName = (from ForeignKey key in table.ForeignKeys
                                    from ForeignKeyColumn column in key.Columns
                                    where column.Name == name
                                    select key.Name).FirstOrDefault();
            if (foreignTableName != null)
                return foreignTableName;
            throw new NotImplementedException(table.Name + " " + name);
        }

        public ForeignKey GetForeignKey(Database database, string columnName, Table tableName) {
            Table table = database.GetTable(tableName.Name, tableName.Schema);
            string name = table.Columns[columnName].Name;
            var foreignKey = (from ForeignKey key in table.ForeignKeys
                              from ForeignKeyColumn column in key.Columns
                              where column.Name == name
                              select key).FirstOrDefault();
            if (foreignKey != null)
                return foreignKey;
            throw new NotImplementedException(table.Name + " " + name);
        }

        public ForeignKey GetForeignKey(Column column) {
            var table = ((Table)column.Parent);
            return column.IsForeignKey ? GetForeignKey(table.Parent, column.Name, table) : null;
        }


    }
}