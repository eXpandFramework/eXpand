using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Mapping;
using System.Collections;
using Xpand.ExpressApp.NH.Core;

namespace Xpand.ExpressApp.NH.DataLayer
{
    

    public class PersistenceManager : IPersistenceManager
    {

        private readonly string connectionString;
        private readonly List<Assembly> mappingAssemblies = new List<Assembly>();
        private readonly List<Type> mappingTypes = new List<Type>();
        public PersistenceManager(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            this.connectionString = connectionString;
        }

        public void AddMappingAssembly(Assembly assembly)
        {
            mappingAssemblies.Add(assembly);
        }

        public void AddMappingType(Type type)
        {
            mappingTypes.Add(type);
        }

        public ISessionFactory CreateSessionFactory(string connectionString)
        {
            return GetConfiguration(connectionString).BuildSessionFactory();
        }

        private static void UpdateSchema(Configuration config)
        {
            new SchemaUpdate(config).Execute(true, true);
        }

        private FluentConfiguration GetConfiguration(string connectionString)
        {
            return Fluently.Configure()
                           .Database(MsSqlConfiguration.MsSql2008.ConnectionString(c => c.Is(connectionString)))
                           .Mappings(AddMappings)
                           .ExposeConfiguration(UpdateSchema);
        }

        private void AddMappings(MappingConfiguration m)
        {
            mappingAssemblies.ForEach(a => m.FluentMappings.AddFromAssembly(a));
            mappingTypes.ForEach(t => m.FluentMappings.Add(t));
        }

        private static Property GetPropertyOrNull(PersistentClass classMapping, string propertyName)
        {
            try
            {
                return classMapping.GetProperty(propertyName);
            }
            catch (MappingException)
            {
                return null;
            }
        }


        public System.Collections.IList GetObjects(string hql)
        {
            using (var factory = CreateSessionFactory(connectionString))
            using (var session = factory.OpenSession())
            {
                List<object> results = new List<object>();
                var query = session.CreateQuery(hql);
                query.List(results);
                return results;
            }
        }

        public IList UpdateObjects(IList updateList, IList deleteList)
        {
            using (var factory = CreateSessionFactory(connectionString))
            using (var session = factory.OpenSession())
            {
                if (updateList != null)
                {
                    foreach (var obj in updateList)
                    {
                        session.SaveOrUpdate(obj);
                    }
                }

                if (deleteList != null)
                {
                    foreach (var obj in deleteList)
                        session.Delete(obj);
                }
                session.Flush();
            
            }
            return updateList;
        }


        public object GetObjectByKey(Type type, object key)
        {
            using (var factory = CreateSessionFactory(connectionString))
            using (var session = factory.OpenSession())
            {
                return session.Get(type, key);
            }
        }

        public object GetObjectKey(object obj)
        {
            using (var factory = CreateSessionFactory(connectionString))
            using (var session = factory.OpenSession())
            {
                return session.GetIdentifier(obj);
            }
        }


        public IList<ITypeMetadata> GetMetadata()
        {
            var config = GetConfiguration(connectionString).BuildConfiguration();
            return config.ClassMappings
                .Select(cm => new TypeMetadata { Type = cm.MappedClass, KeyPropertyName = cm.IdentifierProperty.Name })
                .Cast<ITypeMetadata>()
                .ToList();
        }
    }
}
