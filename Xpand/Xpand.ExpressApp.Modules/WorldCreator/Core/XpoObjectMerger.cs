using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.Core {
    public class XpoObjectMerger {
        public void MergeTypes(UnitOfWork unitOfWork, List<Type> persistentTypes, IDbCommand command) {
            var findBussinessObjectType = WCTypesInfo.Instance.FindBussinessObjectType<IPersistentClassInfo>();
            var collection = new XPCollection(unitOfWork, findBussinessObjectType).Cast<IPersistentClassInfo>().Where(info => info.MergedObjectFullName != null).ToList();
            foreach (IPersistentClassInfo classInfo in collection) {
                XPClassInfo xpClassInfo = GetClassInfo(classInfo.Session, classInfo.PersistentAssemblyInfo.Name + "." + classInfo.Name, persistentTypes);
                var mergedXPClassInfo = GetClassInfo(classInfo.Session, classInfo.MergedObjectFullName, persistentTypes) ?? classInfo.Session.GetClassInfo(ReflectionHelper.GetType(classInfo.MergedObjectFullName));
                if (xpClassInfo != null) {
                    unitOfWork.UpdateSchema(xpClassInfo.ClassType, mergedXPClassInfo.ClassType);
                    if (ObjectType(unitOfWork, xpClassInfo, mergedXPClassInfo, command)>0)
                        classInfo.MergedObjectFullName = null;
                }
            }
        }
        private int ObjectType(UnitOfWork unitOfWork, XPClassInfo xpClassInfo, XPClassInfo mergedXPClassInfo, IDbCommand command) {
            var propertyName = XPObject.Fields.ObjectType.PropertyName;
            command.CommandText = "UPDATE [" + getTableName(mergedXPClassInfo) + "] SET " + propertyName + "=" + unitOfWork.GetObjectType(xpClassInfo).Oid +
                                  " WHERE " + propertyName + " IS NULL OR " + propertyName + "=" + unitOfWork.GetObjectType(mergedXPClassInfo).Oid;
            return command.ExecuteNonQuery();
        }

        string getTableName(XPClassInfo mergedXPClassInfo) {
            string tableName = mergedXPClassInfo.TableName;
            while (mergedXPClassInfo.BaseClass != null && mergedXPClassInfo.BaseClass.IsPersistent) {
                mergedXPClassInfo = mergedXPClassInfo.BaseClass;
                tableName = mergedXPClassInfo.TableName;
            }
            return tableName;
        }


        private XPClassInfo GetClassInfo(Session session, string assemblyQualifiedName, IEnumerable<Type> persistentTypes) {
            Type classType = persistentTypes.Where(type => type.FullName == assemblyQualifiedName).SingleOrDefault();
            if (classType != null) {
                return session.GetClassInfo(classType);
            }
            return null;
        }
    }
}