﻿# DotNetStarter.IntegrationTestTools

![Build Status](https://bmcdavid.visualstudio.com/_apis/public/build/definitions/65ce98f2-010c-44e5-a37d-0344b1602ef0/6/badge)

This tool provides [LocalDb](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/working-with-sql) database setup for running integration tests with tools such as [Entity Framework](https://msdn.microsoft.com/en-us/library/aa937723(v=vs.113).aspx).

To use this tool as part of continuous integration, the build agent must also support LocalDb, which can be provided by [Visual Studio Team Services](https://azure.microsoft.com/en-us/services/visual-studio-team-services/) hosted build agents.

## Usage Example without DotNetStarter

```cs
    // using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestSetup
    {
        private static IDbIntegrationTestSetup DbIntegrationTestSetup;

        [AssemblyInitialize]
        public static void TestInit(TestContext testContext)
        {
            DbIntegrationTestSetup = new LocalDbIntegrationTestSetup()
                .SetDatabaseName("IntegrationTestTools_IntegrationTest")
                .Startup(() => 
                {
                    // note can run migrations here
                });
        }

        [AssemblyCleanup]
        public static void TestCleanup()
        {
            DbIntegrationTestSetup.Shutdown();
        }
    }
```