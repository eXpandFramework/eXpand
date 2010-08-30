using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers
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
