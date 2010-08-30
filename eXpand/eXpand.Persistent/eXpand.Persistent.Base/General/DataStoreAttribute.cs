using System;
using DevExpress.Persistent.Base;

namespace eXpand.Persistent.Base.General {
    [AttributeUsage(AttributeTargets.Assembly,AllowMultiple = true)]
    public class DataStoreAttribute : Xpo.DB.DataStoreAttribute
    {
        

        public DataStoreAttribute(Type nameSpaceType, string dataStoreNameSuffix) : base(nameSpaceType, dataStoreNameSuffix) {
        }

        public DataStoreAttribute(string connectionString, string nameSpaceType)
            : base(ReflectionHelper.FindType(nameSpaceType), null)
        {
            _connectionString = connectionString;
        }

        


    }
}