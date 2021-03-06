﻿using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Xpo;

namespace Xpand.Persistent.Base.General {
    public class XpoObjectHacker {

        public void EnsureIsNotIdentity(IEnumerable<DBTable> tables) {
            var firstOrDefault = tables.FirstOrDefault(table => table.Name == nameof(XPObjectType));
            if (firstOrDefault != null) {
                var dbColumn = firstOrDefault.Columns[0];
                dbColumn.IsIdentity = false;
            }
        }

        public ParameterValue CreateObjectTypeIdentifier(InsertStatement insertStatement, IDataLayer simpleDataLayer, int value) {
            var identityParameter = insertStatement.IdentityParameter;
            insertStatement.IdentityParameter = null;
            insertStatement.Parameters.Add(new ParameterValue(3) { Value = value });
            var oidQueryOperand = insertStatement.Operands.OfType<QueryOperand>().FirstOrDefault(operand => operand.ColumnName == "Oid");
            if (ReferenceEquals(oidQueryOperand, null))
                insertStatement.Operands.Add(new QueryOperand { ColumnName = "Oid", ColumnType = DBColumnType.Int32 });
            return identityParameter;
        }

        public void CreateObjectTypeIdentifier(InsertStatement insertStatement, IDataLayer simpleDataLayer) {
            var identityValue = FindIdentityValue(insertStatement.Parameters, simpleDataLayer);
            CreateObjectTypeIdentifier(insertStatement, simpleDataLayer, identityValue);
        }

        int FindIdentityValue(QueryParameterCollection queryParameterCollection, IDataLayer simpleDataLayer) {
            using var session = new Session(simpleDataLayer) { IdentityMapBehavior = IdentityMapBehavior.Strong };
            var typeName = queryParameterCollection[0].Value.ToString();
            var assemblyName = queryParameterCollection[1].Value.ToString();
            return session.FindObject<XPObjectType>(type => type.TypeName == typeName && type.AssemblyName == assemblyName).Oid;
        }

    }
}