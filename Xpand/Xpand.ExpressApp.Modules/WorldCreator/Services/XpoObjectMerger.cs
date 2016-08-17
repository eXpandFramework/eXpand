using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.Services {
    public class XpoObjectMerger {
        private void MergeTypes(WorldCreatorObjectSpaceProvider worldCreatorObjectSpaceProvider,Func<Type,IObjectSpace> queryObjectSpace ){
            using (var worldCreatorObjectSpace = worldCreatorObjectSpaceProvider.CreateObjectSpace()){
                var persistentClassInfos = worldCreatorObjectSpace.QueryObjects<IPersistentClassInfo>(info => info.MergedObjectFullName != null);
                foreach (var persistentClassInfo in persistentClassInfos) {
                    var mergeFromTypeName = persistentClassInfo.PersistentAssemblyInfo.Name + "." + persistentClassInfo.Name;
                    var mergeFromClassInfo = XafTypesInfo.Instance.FindTypeInfo(mergeFromTypeName).QueryXPClassInfo();
                    if (mergeFromClassInfo != null){
                        var mergeToClassInfo = XafTypesInfo.Instance.FindTypeInfo(persistentClassInfo.MergedObjectFullName).QueryXPClassInfo();
                        MergeTypes(queryObjectSpace, mergeToClassInfo, mergeFromClassInfo, persistentClassInfo);
                    }
                }
                worldCreatorObjectSpace.CommitChanges();
            }
        }

        private void MergeTypes(Func<Type, IObjectSpace> queryObjectSpace, XPClassInfo mergeToClassInfo, XPClassInfo mergeFromClassInfo,
            IPersistentClassInfo persistentClassInfo){
            using (var updatingObjectSpace = queryObjectSpace(mergeToClassInfo.ClassType)){
                var session = updatingObjectSpace.Session();
                session.UpdateSchema(mergeFromClassInfo.ClassType, mergeToClassInfo.ClassType);
                if (ObjectType(session, mergeFromClassInfo, mergeToClassInfo) > 0)
                    persistentClassInfo.MergedObjectFullName = null;
            }
        }


        private int ObjectType(Session session, XPClassInfo xpClassInfo, XPClassInfo mergedXPClassInfo) {
            var command = ((ISqlDataStore) ((BaseDataLayer) session.DataLayer).ConnectionProvider).CreateCommand();
            var propertyName = XPObject.Fields.ObjectType.PropertyName;
            command.CommandText = "UPDATE [" + GetTableName(mergedXPClassInfo) + "] SET " + propertyName + "=" + session.GetObjectType(xpClassInfo).Oid +
                                  " WHERE " + propertyName + " IS NULL OR " + propertyName + "=" + session.GetObjectType(mergedXPClassInfo).Oid;
            return command.ExecuteNonQuery();
        }

        private string GetTableName(XPClassInfo mergedXPClassInfo) {
            string tableName = mergedXPClassInfo.TableName;
            while (mergedXPClassInfo.BaseClass != null && mergedXPClassInfo.BaseClass.IsPersistent) {
                mergedXPClassInfo = mergedXPClassInfo.BaseClass;
                tableName = mergedXPClassInfo.TableName;
            }
            return tableName;
        }

        public static void MergeTypes(WorldCreatorModule worldCreatorModule){
            var objectSpaceProviders = worldCreatorModule.Application.ObjectSpaceProviders;
            worldCreatorModule.Application.LoggedOn+= (sender, args) =>{
                var creatorObjectSpaceProvider =objectSpaceProviders.OfType<WorldCreatorObjectSpaceProvider>().First();
                var xpoObjectMerger = new XpoObjectMerger();
                xpoObjectMerger.MergeTypes(creatorObjectSpaceProvider, type => {
                    return objectSpaceProviders.First(
                            provider => provider.EntityStore.RegisteredEntities.Contains(type)).CreateObjectSpace();
                });
            };
        }
    }
}