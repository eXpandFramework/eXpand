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
        private static ISessionFactory sessionFactory;
        private static readonly object sessionFactoryLock = new object();
        private static Configuration configuration;
        private readonly object configurationLock = new object();

        public PersistenceManager(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            this.connectionString = connectionString;
        }


        private ISessionFactory SessionFactory
        {
            get
            {
                if (sessionFactory == null)
                {
                    lock (sessionFactoryLock)
                    {
                        if (sessionFactory == null)
                        {
                            sessionFactory = CreateSessionFactory(connectionString);
                        }
                    }
                }
                return sessionFactory;
            }
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
            return Configuration.BuildSessionFactory();
        }

        protected virtual void UpdateSchema(Configuration config)
        {
            new SchemaUpdate(config).Execute(true, true);
        }


        private Configuration Configuration
        {
            get
            {
                if (configuration == null)
                {
                    lock (configurationLock)
                    {
                        if (configuration == null)
                        {
                            configuration = GetConfiguration(connectionString).BuildConfiguration();
                        }
                    }
                }

                return configuration;
            }
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
            using (var session = SessionFactory.OpenSession())
            {
                List<object> results = new List<object>();
                var query = session.CreateQuery(hql);
                query.List(results);
                return results;
            }
        }


        public IList UpdateObjects(IList updateList, IList deleteList)
        {
            using (var session = SessionFactory.OpenSession())
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
            using (var session = SessionFactory.OpenSession())
            {
                return session.Get(type, key);
            }
        }

        public object GetObjectKey(object obj)
        {
            using (var session = SessionFactory.OpenSession())
            {
                return session.GetIdentifier(obj);
            }
        }


        public IList<ITypeMetadata> GetMetadata()
        {
            return Configuration.ClassMappings
                .Select(cm => CreateTypeMetadata(cm, Configuration.CollectionMappings))
                .Cast<ITypeMetadata>()
                .ToList();
        }

        private static TypeMetadata CreateTypeMetadata(PersistentClass cm, ICollection<NHibernate.Mapping.Collection> collectionMappings)
        {
            var result = new TypeMetadata { Type = cm.MappedClass };

            result.KeyProperty = AddPropertyMetadata(result, cm.IdentifierProperty);
            string classFullName = cm.MappedClass.FullName;
            foreach (var collectionMapping in collectionMappings)
            {
                if (collectionMapping.Owner == cm && collectionMapping.Role.StartsWith(classFullName) && collectionMapping.IsOneToMany)
                {
                    result.Properties.Add(new PropertyMetadata
                    {
                        Name = collectionMapping.Role.Substring(classFullName.Length + 1),
                        RelationType = collectionMapping.IsOneToMany ? RelationType.OneToMany : RelationType.ManyToMany
                    });
                }
            }
            foreach (var property in cm.PropertyIterator)
            {
                AddPropertyMetadata(result, property);
            }


            return result;
        }

        private static PropertyMetadata AddPropertyMetadata(TypeMetadata typeMetadata, Property property)
        {
            PropertyMetadata propertyMetadata = new PropertyMetadata
            {
                Name = property.Name,
                RelationType = property.IsEntityRelation ? RelationType.Reference : RelationType.Value
            };

            typeMetadata.Properties.Add(propertyMetadata);
            return propertyMetadata;
        }

        private StringBuilder CreateFromAndWhereHql(Type objectType, string criteriaString)
        {
            Guard.ArgumentNotNull(objectType, "objectType");


            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(CultureInfo.InvariantCulture, "FROM {0} as m \r\n", objectType.Name);

            var metadata = this.GetMetadata().FirstOrDefault(md => md.Type == objectType);
            if (metadata != null)
            {
                string[] relationPropertyNames = metadata.Properties
                .Where(p => p.RelationType == RelationType.OneToMany || p.RelationType == RelationType.ManyToMany)
                .Select(p => p.Name)
                .ToArray();

                foreach (var rp in relationPropertyNames)
                {
                    var propertyInfo = objectType.GetProperty(rp);
                    if (propertyInfo != null && !typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                        sb.AppendFormat(CultureInfo.InvariantCulture, "left join fetch m.{1}\r\n", objectType.Name, rp);
                }
            }
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

            if (metadata.KeyProperty == null)
                return null;

            return metadata.KeyProperty.Name;
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
