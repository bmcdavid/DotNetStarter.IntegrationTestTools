using System;
using System.Data.SqlClient;

namespace DotNetStarter.IntegrationTestTools.Abstractions
{
    /// <summary>
    /// Contract for db test setup, requires build agent with LocalDb installed such as VSTS
    /// </summary>
    public interface IDbIntegrationTestSetup
    {
        /// <summary>
        /// Executes db cleanup on shutdown
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Executes db setup on test start
        /// </summary>
        /// <param name="initializeDatabase"></param>
        IDbIntegrationTestSetup Startup(Action initializeDatabase = null);

        /// <summary>
        /// Full filepath of generated mdf file
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Master connection string
        /// </summary>
        SqlConnectionStringBuilder MasterConnectionString { get; }

        /// <summary>
        /// Set database name to match intial catalog in the connection string
        /// </summary>
        /// <param name="databaseName"></param>
        IDbIntegrationTestSetup SetDatabaseName(string databaseName);
    }
}