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
using DevExpress.Utils;
using System.Globalization;
using DevExpress.Data.Filtering;

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
                           .Database(GetDatabaseConfiguration(connectionString))
                           .Mappings(AddMappings)
                           .ExposeConfiguration(UpdateSchema);
        }

        protected virtual IPersistenceConfigurer GetDatabaseConfiguration(string connectionString)
        {
            return MsSqlConfiguration.MsSql2008.ConnectionString(c => c.Is(connectionString));
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


        private IList GetObjects(string hql)
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
                .Select(cm => CreateTypeMetadata(cm))
                .Cast<ITypeMetadata>()
                .ToList();
        }

        private static TypeMetadata CreateTypeMetadata(PersistentClass cm)
        {
            var result = new TypeMetadata { Type = cm.MappedClass, KeyPropertyName = cm.IdentifierProperty.Name };
            foreach (var property in cm.PropertyIterator.Where(p => p.IsEntityRelation))
                result.RelationProperties.Add(property.Name);

            return result;
        }

        private static StringBuilder CreateFromAndWhereHql(Type objectType, string criteriaString)
        {
            Guard.ArgumentNotNull(objectType, "objectType");


            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(CultureInfo.InvariantCulture, "FROM {0}\r\n", objectType.Name);

            if (!string.IsNullOrWhiteSpace(criteriaString))
            {
                CriteriaOperator criteria = CriteriaOperator.Parse(criteriaString);
                string hqlWhere = new NHWhereGenerator().Process(criteria);
                sb.AppendFormat(CultureInfo.InvariantCulture, "Where {0}\r\n", hqlWhere);
            }
            return sb;
        }


        public IList GetObjects(string typeName, string criteria, IList<ISortPropertyInfo> sorting, int topReturnedObjectsCount)
        {

            Guard.ArgumentIsNotNullOrEmpty(typeName, "typeName");

            Type objectType = Type.GetType(typeName);
            if (objectType == null)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Type not found: {0}", typeName), "typeName");

            StringBuilder sb = CreateFromAndWhereHql(objectType, criteria);

            if (sorting != null && sorting.Count > 0)
                sb.AppendFormat(CultureInfo.InvariantCulture, "order by {0}\r\n", string.Join(",", sorting));

            return GetObjects(sb.ToString());
        }


        private string GetKeyPropertyName(Type type)
        {
            Guard.ArgumentNotNull(type, "type");
            var metadata = GetMetadata().FirstOrDefault(md => md.Type == type);
            if (metadata == null)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Metadata not found for the type: {0}", type.AssemblyQualifiedName), "type");

            return metadata.KeyPropertyName;
        }
        public int GetObjectsCount(string typeName, string criteria)
        {
            Guard.ArgumentIsNotNullOrEmpty(typeName, "typeName");

            Type objectType = Type.GetType(typeName);

            StringBuilder sb = CreateFromAndWhereHql(objectType, criteria);
            sb.Insert(0, string.Format(CultureInfo.InvariantCulture, "Select Count({0})", GetKeyPropertyName(objectType)));
            var result = GetObjects(sb.ToString());
            if (result.Count == 1)
                return Convert.ToInt32(result[0]);
            else
                return 0;


        }
    }
}
