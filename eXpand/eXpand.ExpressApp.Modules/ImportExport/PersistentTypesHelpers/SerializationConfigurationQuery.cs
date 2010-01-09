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
        public static CriteriaOperator GetCriteria(Type serializationConfigurationType)
        {
            const ISerializationConfiguration serializationConfiguration = null;
            return new BinaryOperator(serializationConfiguration.GetPropertyName(x => x.TypeToSerialize),
                                      serializationConfigurationType);
        }

        public static bool ConfigurationExists(Session session, Type type)
        {
            return Find(session, type) != null;
        }

        public static ISerializationConfiguration Find(Session session, Type type) {
            return session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction,
                                      TypesInfo.Instance.SerializationConfigurationType,
                                      GetCriteria(type)) as ISerializationConfiguration;
        }
    }
}


