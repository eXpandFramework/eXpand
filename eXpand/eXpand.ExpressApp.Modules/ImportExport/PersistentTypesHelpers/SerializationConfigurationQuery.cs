using System;
using DevExpress.Data.Filtering;
using eXpand.Persistent.Base.ImportExport;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.ImportExport.PersistentTypesHelpers
{
    public static class SerializationConfigurationQuery
    {
        public static CriteriaOperator GetCriteria(Type serializationConfigurationType)
        {
            const ISerializationConfiguration serializationConfiguration = null;
            return new BinaryOperator(serializationConfiguration.GetPropertyName(x => x.TypeToSerialize),
                                      serializationConfigurationType);
        }
    }
}
