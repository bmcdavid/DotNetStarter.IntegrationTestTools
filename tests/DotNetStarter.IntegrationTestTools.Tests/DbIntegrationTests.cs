using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetStarter.IntegrationTestTools.Tests
{
    [TestClass]
    public class DbIntegrationTests
    {
        [TestMethod]
        public void ShouldSetFileNameGivenDabase()
        {
            var filename = new LocalDbIntegrationTestSetup().SetDatabaseName("Mock").FileName;

            Assert.IsTrue(filename.Contains("Mock.mdf"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowExceptionGettingFileNameWithNoDbName()
        {
            var filename = new LocalDbIntegrationTestSetup().FileName;
        }

        [TestMethod]
        public void ShouldStartupIntegrationSetup()
        {
            Assert.IsTrue(TestSetup.StartedCheck);
        }

        [TestMethod]
        public void ShouldShutdownIntegrationSetup()
        {
            new MockLocalDbIntegrationTestSetup().SetDatabaseName("Mock").Shutdown();

            Assert.IsTrue(MockLocalDbIntegrationTestSetup.ShutdownCheck);
        }

        [TestMethod]
        public void ShouldHaveMasterConnectionstring()
        {
            var connection = new LocalDbIntegrationTestSetup().MasterConnectionString;

            Assert.IsNotNull(connection);
            Assert.IsTrue(connection.ConnectionString.ToLower().Contains("master"));
        }
    }
}