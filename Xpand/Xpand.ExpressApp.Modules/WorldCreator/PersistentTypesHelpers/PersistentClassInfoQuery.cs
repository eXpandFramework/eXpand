using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers
{
    public static class PersistentClassInfoQuery
    {
        public static IPersistentClassInfo Find(Session session, string referenceTypeFullName)
        {
            const IPersistentClassInfo persistentClassInfo = null;
            string propertyName = persistentClassInfo.GetPropertyName(x => x.PersistentAssemblyInfo) + "." + persistentClassInfo.GetPropertyName(x => x.PersistentAssemblyInfo.Name);
            var binaryOperator = new BinaryOperator(propertyName, referenceTypeFullName.Substring(0,referenceTypeFullName.IndexOf(".")));


            var operands = new BinaryOperator(persistentClassInfo.GetPropertyName(x=>x.Name),referenceTypeFullName.Substring(referenceTypeFullName.IndexOf(".")+1));
            return session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, WCTypesInfo.Instance.FindBussinessObjectType<IPersistentClassInfo>(), new GroupOperator(binaryOperator, operands)) as IPersistentClassInfo;
        }
    }
}
