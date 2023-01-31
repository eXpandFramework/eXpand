using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.WorldCreator.BusinessObjects {
    public static class PersistentMemberInfoExtensions {
        public static IPersistentClassInfo FindReferenceClassInfo(this Session session, string referenceTypeFullName) {
            const IPersistentClassInfo persistentClassInfo = null;
            string propertyName = persistentClassInfo.GetPropertyName(x => x.PersistentAssemblyInfo) + "." + persistentClassInfo.GetPropertyName(x => x.PersistentAssemblyInfo.Name);
            var binaryOperator = new BinaryOperator(propertyName, referenceTypeFullName.Substring(0, referenceTypeFullName.IndexOf(".", StringComparison.Ordinal)));


            var operands = new BinaryOperator(persistentClassInfo.GetPropertyName(x => x.Name), referenceTypeFullName.Substring(referenceTypeFullName.IndexOf(".", StringComparison.Ordinal) + 1));
            return session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, XafTypesInfo.Instance.FindBusinessObjectType<IPersistentClassInfo>(), new GroupOperator(binaryOperator, operands)) as IPersistentClassInfo;
        }

        public static IPersistentCollectionMemberInfo GetAssociatedReference(this IPersistentAssociatedMemberInfo persistentMemberInfo, string elementTypeFullName){
            throw new NotImplementedException();
        }

        public static IPersistentCollectionMemberInfo GetAssociatedCollection(this IPersistentAssociatedMemberInfo persistentMemberInfo, string elementTypeFullName){
            throw new NotImplementedException();
        }

        public static IPersistentAssociatedMemberInfo GetAssociation(this IPersistentAssociatedMemberInfo persistentMemberInfo){
            var associationName = persistentMemberInfo.TypeAttributes.OfType<IPersistentAssociationAttribute>().First().AssociationName;
            return persistentMemberInfo.Owner.OwnMembers.OfType<IPersistentAssociatedMemberInfo>()
                    .Where(info =>info != persistentMemberInfo && info.TypeAttributes.OfType<IPersistentAssociationAttribute>()
                                .Any(attribute => attribute.AssociationName == associationName)).First();
        }

        public static bool IsAssociation(this IPersistentAssociatedMemberInfo persistentMemberInfo) {
            return persistentMemberInfo.Find<AssociationAttribute>() != null;
        }

        public static TAttribute Find<TAttribute>(this IPersistentMemberInfo persistentMemberInfo) where TAttribute : Attribute {
            AttributeInfoAttribute firstOrDefault =
                persistentMemberInfo.TypeAttributes.Select(info => info.Create()).FirstOrDefault(attributeInfo => attributeInfo.Constructor.DeclaringType == typeof(TAttribute));
            if (firstOrDefault != null)
                return (TAttribute)ReflectionHelper.CreateObject(firstOrDefault.Constructor.DeclaringType, firstOrDefault.InitializedArgumentValues);
            return null;
        }

    }
}