using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Data.SqlClient;

namespace UnitTests
{
    [TestClass]
    public abstract class DatabaseTestBase
    {
        private string databaseName;

        [TestInitialize]
        public void Initialize()
        {
            InitializeCore();
        }


        protected virtual string DatabaseName
        {
            get
            {
                if (databaseName == null)
                    databaseName = CreateDatabaseName();

                return databaseName;
            }
        }

        protected virtual string CreateDatabaseName()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}_Test_{1}", DatabaseNamePrefix, Guid.NewGuid());
        }


        protected abstract void InitializeCore();
        protected abstract object ExecuteSql(string sql);
        protected string ConnectionString
        {
            get
            {
                return CreateConnectionString(DatabaseName);
            }
        }

        protected string MasterConnectionString
        {
            get { return CreateConnectionString("master"); }
        }

        private string CreateConnectionString(string dbName)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Integrated Security=SSPI;Pooling=false;Data Source={0};Initial Catalog={1}", DataSourceName, dbName);
        }

        protected abstract string DataSourceName { get; }
        protected abstract string DatabaseNamePrefix { get; }


        protected bool IsValidId(object value)
        {
            return value is int || value is Int16;
        }
        protected object ExecuteMasterSql(string sql, params  object[] parameters)
        {
            return ExecuteSqlWithConnectionString(string.Format(sql, parameters), MasterConnectionString);
        }


        protected object ExecuteSqlWithConnectionString(string sql, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;
                    command.CommandText = sql;
                    return command.ExecuteScalar();
                }
            }
        }
        protected bool DatabaseExists(string name)
        {
            return IsValidId(ExecuteMasterSql("Select DB_ID('{0}')", name));
        }
        protected void DropDatabase(string dbName)
        {
            if (DatabaseExists(dbName))
                ExecuteSql(string.Format(CultureInfo.InvariantCulture, "use master; Drop Database [{0}]", dbName));
        }

        public virtual void Cleanup()
        {
            DropDatabase(DatabaseName);
        }



    }
}
