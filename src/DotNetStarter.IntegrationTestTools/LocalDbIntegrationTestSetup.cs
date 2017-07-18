using DotNetStarter.Abstractions;
using DotNetStarter.IntegrationTestTools.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

/* 
 * Project: EfIntegrationTesting (https://github.com/michaellperry/EFIntegrationTesting)
 * Author Michael Perry
 * Copyright: 2017
 */

namespace DotNetStarter.IntegrationTestTools
{
    /// <summary>
    /// Default LocalDb integration
    /// </summary>
    [Register(typeof(IDbIntegrationTestSetup), LifeTime.Singleton)]
    public class LocalDbIntegrationTestSetup : IDbIntegrationTestSetup
    {
        private string DbName;

        /// <summary>
        /// Full filename for mdf file
        /// </summary>
        public virtual string FileName
        {
            get
            {
                ThrowIfDbNameEmpty();

                return Path.Combine(GetAppDirectory(), $"{DbName}.mdf");
            }
        }

        /// <summary>
        /// Master db connection string value
        /// </summary>
        public virtual SqlConnectionStringBuilder MasterConnectionString =>
            new SqlConnectionStringBuilder
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                InitialCatalog = "master",
                IntegratedSecurity = true
            };

        /// <summary>
        /// Fluently setups up database name, should match connection string initial catalog
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public virtual IDbIntegrationTestSetup SetDatabaseName(string databaseName)
        {
            DbName = databaseName;

            return this;
        }

        /// <summary>
        /// Executes db cleanup on test shutdown
        /// </summary>
        public virtual void Shutdown()
        {
            DestroyDatabase();
        }

        /// <summary>
        /// Executes db setup on test start
        /// </summary>
        /// <param name="initializeDatabase">optional action to run database migrations</param>
        public virtual IDbIntegrationTestSetup Startup(Action initializeDatabase = null)
        {
            DestroyDatabase();
            CreateDatabase(initializeDatabase);

            return this;
        }

        /// <summary>
        /// Creates database with given
        /// </summary>
        /// <param name="initializeDatabase"></param>
        protected virtual void CreateDatabase(Action initializeDatabase)
        {
            ThrowIfDbNameEmpty();

            ExecuteSqlCommand(MasterConnectionString, $@"
                CREATE DATABASE [{DbName}]
                ON (NAME = '{DbName}',
                FILENAME = '{FileName}')");

            initializeDatabase?.Invoke();
        }

        /// <summary>
        /// Closes connnections, detaches db, and deletes files
        /// </summary>
        protected virtual void DestroyDatabase()
        {
            ThrowIfDbNameEmpty();

            var fileNames = ExecuteSqlQuery(MasterConnectionString, $@"
                SELECT [physical_name] FROM [sys].[master_files]
                WHERE [database_id] = DB_ID('{DbName}')",
                row => (string)row["physical_name"]);

            if (fileNames.Any())
            {
                // alter closes connections
                ExecuteSqlCommand(MasterConnectionString, $@"
                    ALTER DATABASE [{DbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    EXEC sp_detach_db '{DbName}'");

                fileNames.ForEach(File.Delete);
            }
        }

        /// <summary>
        /// Executes command
        /// </summary>
        /// <param name="connectionStringBuilder"></param>
        /// <param name="commandText"></param>
        protected virtual void ExecuteSqlCommand(SqlConnectionStringBuilder connectionStringBuilder, string commandText)
        {
            using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Executes query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionStringBuilder"></param>
        /// <param name="queryText"></param>
        /// <param name="read"></param>
        /// <returns></returns>
        protected virtual List<T> ExecuteSqlQuery<T>(SqlConnectionStringBuilder connectionStringBuilder, string queryText, Func<SqlDataReader, T> read)
        {
            var result = new List<T>();
            using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryText;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(read(reader));
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets base directory for mdf file
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAppDirectory()
        {
#if NETSTANDARD1_3
            return AppContext.BaseDirectory;
#else
            return System.AppDomain.CurrentDomain.BaseDirectory;
#endif
        }

        private void ThrowIfDbNameEmpty()
        {
            if (string.IsNullOrWhiteSpace(DbName))
                throw new ArgumentNullException($"Must call {nameof(SetDatabaseName)} before invoking {nameof(Startup)} or {nameof(Shutdown)} or getting {nameof(FileName)}!");
        }
    }
}
 