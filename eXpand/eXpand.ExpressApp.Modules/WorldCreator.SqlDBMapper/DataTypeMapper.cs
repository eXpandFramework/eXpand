using DevExpress.Persistent.Base;
using DevExpress.Xpo.DB;
using Microsoft.SqlServer.Management.Smo;

namespace eXpand.ExpressApp.WorldCreator.SqlDBMapper {
    public class DataTypeMapper 
    {

        public DBColumnType GetDataType(Column column)
        {
            DataType dataType = column.DataType;
            switch (dataType.SqlDataType) {
                case SqlDataType.Int:
                    return DBColumnType.Int32;
                case SqlDataType.Image:
                case SqlDataType.VarBinaryMax:
                case SqlDataType.VarBinary:
                    return DBColumnType.ByteArray;
                case SqlDataType.Char:
                case SqlDataType.NChar:
                    if (column.DataType.MaximumLength == 1)
                        return DBColumnType.Char;
                    return DBColumnType.String;
                case SqlDataType.VarChar:
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                case SqlDataType.Xml:
                case SqlDataType.NText:
                case SqlDataType.Text:
                    return DBColumnType.String;
                case SqlDataType.Bit:
                    return DBColumnType.Boolean;
                case SqlDataType.TinyInt:
                    return DBColumnType.Byte;
                case SqlDataType.SmallInt:
                    return DBColumnType.Int16;
                case SqlDataType.BigInt:
                    return DBColumnType.Int64;
                case SqlDataType.Numeric:
                case SqlDataType.Decimal:
                    return DBColumnType.Decimal;
                case SqlDataType.Money:
                case SqlDataType.SmallMoney:
                    return DBColumnType.Decimal;
                case SqlDataType.Float:
                    return DBColumnType.Double;
                case SqlDataType.Real:
                    return DBColumnType.Single;
                case SqlDataType.UniqueIdentifier:
                    return DBColumnType.Guid;
                case SqlDataType.DateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.SmallDateTime:
                case SqlDataType.Date:
                    return DBColumnType.DateTime;
                    
                
            }
            Tracing.Tracer.LogText("Column " + column.Name + " with datatype " + column.DataType.SqlDataType+" on table "+((Table) column.Parent).Name+" is Unknown to "+GetType().Name);
            return DBColumnType.Unknown;
        }

    }
}