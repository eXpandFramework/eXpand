using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public class XpoObjectMerger {
        public void MergeTypes(UnitOfWork unitOfWork, Type persistentTypesInfoType, List<Type> persistentTypes, IDbCommand command)
        {
            var collection = new XPCollection(unitOfWork, persistentTypesInfoType).Cast<IPersistentClassInfo>().Where(info => info.MergedObjectType !=null).ToList();
            foreach (IPersistentClassInfo classInfo in collection){
                XPClassInfo xpClassInfo = getClassInfo(classInfo.Session,classInfo.PersistentAssemblyInfo.Name+"."+ classInfo.Name,persistentTypes);
                var mergedXPClassInfo = getClassInfo(classInfo.Session, classInfo.MergedObjectType.AssemblyQualifiedName,persistentTypes) ?? classInfo.Session.GetClassInfo(classInfo.MergedObjectType);
                unitOfWork.UpdateSchema(xpClassInfo);
                updateObjectType(unitOfWork, xpClassInfo, mergedXPClassInfo,command);
            }
        }
        private void updateObjectType(UnitOfWork unitOfWork, XPClassInfo xpClassInfo, XPClassInfo mergedXPClassInfo, IDbCommand command)
        {
            var propertyName = XPObject.Fields.ObjectType.PropertyName;
            command.CommandText = "UPDATE " + mergedXPClassInfo.TableName + " SET " + propertyName + "=" + unitOfWork.GetObjectType(xpClassInfo).Oid +
                                  " WHERE " + propertyName + " IS NULL OR " + propertyName + "=" +unitOfWork.GetObjectType(mergedXPClassInfo).Oid;
            command.ExecuteNonQuery();
        }


        private XPClassInfo getClassInfo(Session session, string assemblyQualifiedName,IEnumerable<Type> persistentTypes)
        {
            Type classType = persistentTypes.Where(type => type.FullName == assemblyQualifiedName).SingleOrDefault();
            if (classType != null){
                return session.GetClassInfo(classType);
            }
            return null;
        }
    }
}