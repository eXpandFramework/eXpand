using System;
using DevExpress.Xpo.DB;

namespace Xpand.Xpo.ConnectionProviders {
    public static class DBColumnTypeExtensions {
        public static Type GetType(this DBColumnType dbColumnType, Type propertyType) {
            Type underlyingNullableType = Nullable.GetUnderlyingType(propertyType);
            if ((((underlyingNullableType != null && underlyingNullableType == typeof(TimeSpan) || propertyType == typeof(TimeSpan))) && dbColumnType == DBColumnType.Double))
                return propertyType;
            if (dbColumnType == DBColumnType.ByteArray)
                return typeof(byte[]);
            return Type.GetType(string.Format("System.{0}", dbColumnType));
        }

    }
}
