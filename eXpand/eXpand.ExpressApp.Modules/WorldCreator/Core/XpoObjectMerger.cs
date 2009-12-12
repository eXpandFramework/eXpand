using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public class XpoObjectMerger {
        public void MergeTypes(UnitOfWork unitOfWork, List<Type> persistentTypes, IDbCommand command)
        {
            var collection = new XPCollection(unitOfWork, TypesInfo.Instance.PersistentTypesInfoType).Cast<IPersistentClassInfo>().Where(info => info.MergedObjectType !=null).ToList();
            foreach (IPersistentClassInfo classInfo in collection){
                XPClassInfo xpClassInfo = getClassInfo(classInfo.Session,classInfo.PersistentAssemblyInfo.Name+"."+ classInfo.Name,persistentTypes);
                var mergedXPClassInfo = getClassInfo(classInfo.Session, classInfo.MergedObjectType.AssemblyQualifiedName,persistentTypes) ?? classInfo.Session.GetClassInfo(classInfo.MergedObjectType);
                if (xpClassInfo != null) {
                    unitOfWork.UpdateSchema(xpClassInfo.ClassType, mergedXPClassInfo.ClassType);
//                    if (unitOfWork.GetCount(xpClassInfo.ClassType) == 0)
//                        createObjectTypeColumn(xpClassInfo, unitOfWork);
                    updateObjectType(unitOfWork, xpClassInfo, mergedXPClassInfo,command);
                }
            }
        }
//        private void createObjectTypeColumn(XPClassInfo xpClassInfo, UnitOfWork unitOfWork)
//        {
//            unitOfWork.CreateObjectTypeRecords(xpClassInfo);
//            var newObject = xpClassInfo.CreateNewObject(unitOfWork);
//            unitOfWork.CommitChanges();
//            unitOfWork.Delete(newObject);
//            unitOfWork.CommitChanges();
//        }
        private void updateObjectType(UnitOfWork unitOfWork, XPClassInfo xpClassInfo, XPClassInfo mergedXPClassInfo, IDbCommand command)
        {
            var propertyName = XPObject.Fields.ObjectType.PropertyName;
            command.CommandText = "UPDATE [" + getTableName(mergedXPClassInfo) + "] SET " + propertyName + "=" + unitOfWork.GetObjectType(xpClassInfo).Oid +
                                  " WHERE " + propertyName + " IS NULL OR " + propertyName + "=" +unitOfWork.GetObjectType(mergedXPClassInfo).Oid;
            command.ExecuteNonQuery();
        }

        string getTableName(XPClassInfo mergedXPClassInfo) {
            string tableName = mergedXPClassInfo.TableName;
            while (mergedXPClassInfo.BaseClass!= null&&mergedXPClassInfo.BaseClass.IsPersistent) {
                mergedXPClassInfo=mergedXPClassInfo.BaseClass;
                tableName=mergedXPClassInfo.TableName;
            }
            return tableName;
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