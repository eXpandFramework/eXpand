using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using eXpand.ExpressApp.IO.Core;
using eXpand.Persistent.Base.ImportExport;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.IO.PersistentTypesHelpers
{
    public static class SerializationConfigurationQuery
    {
        public static CriteriaOperator GetCriteria(Type serializationConfigurationType, ISerializationConfigurationGroup serializationConfigurationGroup)
        {
            const ISerializationConfiguration serializationConfiguration = null;
            return new GroupOperator(new BinaryOperator(serializationConfiguration.GetPropertyName(x => x.TypeToSerialize),
                                      serializationConfigurationType), new BinaryOperator(serializationConfiguration.GetPropertyName(x => x.SerializationConfigurationGroup),
                                      serializationConfigurationGroup));
        }

        public static bool ConfigurationExists(Session session, Type type, ISerializationConfigurationGroup serializationConfigurationGroup)
        {
            return Find(session, type,serializationConfigurationGroup) != null;
        }

        public static ISerializationConfiguration Find(Session session, Type type, ISerializationConfigurationGroup serializationConfigurationGroup) {
            return session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction,
                                      TypesInfo.Instance.SerializationConfigurationType,
                                      GetCriteria(type,serializationConfigurationGroup)) as ISerializationConfiguration;
        }
    }
}


