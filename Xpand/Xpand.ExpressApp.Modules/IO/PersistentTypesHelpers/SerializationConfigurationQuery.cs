using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.IO.PersistentTypesHelpers
{
    public static class SerializationConfigurationQuery
    {
        public static CriteriaOperator GetCriteria(Type serializationConfigurationType, ISerializationConfigurationGroup serializationConfigurationGroup)
        {
            const ISerializationConfiguration serializationConfiguration = null;
            var groupOperator = new BinaryOperator(serializationConfiguration.GetPropertyName(x => x.SerializationConfigurationGroup),serializationConfigurationGroup);
            return new GroupOperator(new BinaryOperator(serializationConfiguration.GetPropertyName(x => x.TypeToSerialize),
                                      serializationConfigurationType), groupOperator);
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


