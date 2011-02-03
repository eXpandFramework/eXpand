using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Xpo;

namespace Xpand.ExpressApp {
    public class XpoObjectHacker {

        public void EnsureIsNotIdentity(IEnumerable<DBTable> tables) {
            var firstOrDefault = tables.Where(table => table.Name == typeof(XPObjectType).Name).FirstOrDefault();
            if (firstOrDefault != null) {
                var dbColumn = firstOrDefault.Columns[0];
                dbColumn.IsIdentity = false;
            }
        }

        public ParameterValue CreateObjectTypeIndetifier(InsertStatement insertStatement, SimpleDataLayer simpleDataLayer,int value) {
            var identityParameter = insertStatement.IdentityParameter;
            insertStatement.IdentityParameter = null;
            insertStatement.Parameters.Add(new ParameterValue(3) { Value = value });
            var oidQueryOperand = insertStatement.Operands.OfType<QueryOperand>().Where(operand => operand.ColumnName == "Oid").FirstOrDefault();
            if (ReferenceEquals(oidQueryOperand,null))
                insertStatement.Operands.Add(new QueryOperand { ColumnName = "Oid", ColumnType = DBColumnType.Int32 });
            return identityParameter;
        }

        public void CreateObjectTypeIndetifier(InsertStatement insertStatement, SimpleDataLayer simpleDataLayer) {
            var identityValue = FindIdentityValue(insertStatement.Parameters, simpleDataLayer);
            CreateObjectTypeIndetifier(insertStatement, simpleDataLayer, identityValue);
        }

        int FindIdentityValue(QueryParameterCollection queryParameterCollection, SimpleDataLayer simpleDataLayer) {
            using (var session = new Session(simpleDataLayer) { IdentityMapBehavior = IdentityMapBehavior.Strong }) {
                var typeName = queryParameterCollection[0].Value.ToString();
                var assemblyName = queryParameterCollection[1].Value.ToString();
                return session.FindObject<XPObjectType>(type => type.TypeName == typeName && type.AssemblyName == assemblyName).Oid;
            }
        }

    }
}