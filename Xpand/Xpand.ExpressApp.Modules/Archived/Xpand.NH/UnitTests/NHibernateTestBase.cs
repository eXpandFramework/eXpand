using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestDataLayer.Maps;
using TestEntities;
using Xpand.ExpressApp.NH.DataLayer;

namespace UnitTests
{
    public abstract class NHibernateTestBase : DatabaseTestBase
    {
        protected override void InitializeCore()
        {
            ExecuteMasterSql("Create Database [{0}]", DatabaseName);
        }

        protected override object ExecuteSql(string sql)
        {
            return ExecuteSqlWithConnectionString(sql, ConnectionString);
        }

        protected override string DataSourceName
        {
            get { return "."; }
        }

        protected override string DatabaseNamePrefix
        {
            get { return "NHibernate"; }
        }
        
        protected PersistenceManager CreatePersistenceManager()
        {
            PersistenceManager pm = new PersistenceManager(ConnectionString);
            pm.AddMappingAssembly(typeof(PersonMap).Assembly);
            return pm;
        }

    }
}
