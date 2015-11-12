using DevExpress.Data.Filtering;
using System;
using System.Collections.Generic;
using System.Configuration;
using Xpand.ExpressApp.NH.Core;
using Xpand.ExpressApp.NH.DataLayer;

namespace Xpand.ExpressApp.NH.Service
{
    [XpandDataContractSerializer(true)]
    public class PersistenceManagerService : IPersistenceManagerService
    {
        private NHPersistenceManager persistenceManager;

        private NHPersistenceManager PersistenceManager
        {
            get
            {
                if (persistenceManager == null)
                {
                    persistenceManager = CreatePersistenceManager();
                    foreach (var type in ServiceTypesHelper.MappingTypes)
                    {
                        persistenceManager.AddMappingType(type);
                    }
                }


                return persistenceManager;
            }
        }

        protected virtual NHPersistenceManager CreatePersistenceManager()
        {
            return new NHPersistenceManager(ConnectionString);
        }

        protected string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString; }
        }


        protected virtual string ConnectionStringName
        {
            get { return "ConnectionString"; }
        }
        public virtual System.Collections.IList UpdateObjects(System.Collections.IList updateList, System.Collections.IList deleteList)
        {
            return PersistenceManager.UpdateObjects(updateList, deleteList);
        }

        public virtual object GetObjectByKey(Type type, object key)
        {
            return PersistenceManager.GetObjectByKey(type, key);
        }

        public virtual IList<ITypeMetadata> GetMetadata()
        {
            return PersistenceManager.GetMetadata();
        }

        public virtual System.Collections.IList GetObjects(string typeName, IList<CriteriaOperator> members, string criteria, IList<ISortPropertyInfo> sorting, int topReturnedObjectsCount)
        {
            return PersistenceManager.GetObjects(typeName, members, criteria, sorting, topReturnedObjectsCount);
        }

        public virtual int GetObjectsCount(string typeName, string criteria)
        {
            return PersistenceManager.GetObjectsCount(typeName, criteria);
        }


        public object ExecuteExpression(Serialize.Linq.Nodes.ExpressionNode expressionNode)
        {
            return PersistenceManager.ExecuteExpression(expressionNode);
        }
    }
}