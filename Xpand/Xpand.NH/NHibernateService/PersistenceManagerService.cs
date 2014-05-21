using System;
using System.Collections.Generic;
using System.Configuration;
using Xpand.ExpressApp.NH.Core;
using Xpand.ExpressApp.NH.DataLayer;

namespace Xpand.ExpressApp.NH.Service
{
    [XpandDataContractSerializer]
    public class PersistenceManagerService : IPersistenceManagerService
    {
        private PersistenceManager persistenceManager;

        private PersistenceManager PersistenceManager
        {
            get
            {
                if (persistenceManager == null)
                {
                    persistenceManager = new PersistenceManager(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
                    foreach (var type in ServiceTypesHelper.MappingTypes)
                    {
                        persistenceManager.AddMappingType(type);
                    }
                }


                return persistenceManager;
            }
        }

        public System.Collections.IList UpdateObjects(System.Collections.IList updateList, System.Collections.IList deleteList)
        {
            return PersistenceManager.UpdateObjects(updateList, deleteList);
        }

        public object GetObjectByKey(Type type, object key)
        {
            return PersistenceManager.GetObjectByKey(type, key);
        }

        public IList<ITypeMetadata> GetMetadata()
        {
            return PersistenceManager.GetMetadata();
        }

        public System.Collections.IList GetObjects(string typeName, string criteria, IList<ISortPropertyInfo> sorting, int topReturnedObjectsCount)
        {
            return PersistenceManager.GetObjects(typeName, criteria, sorting, topReturnedObjectsCount);
        }

        public int GetObjectsCount(string typeName, string criteria)
        {
            return PersistenceManager.GetObjectsCount(typeName, criteria);
        }
    }
}