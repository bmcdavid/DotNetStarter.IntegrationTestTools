using DotNetStarter.Abstractions;
using DotNetStarter.IntegrationTestTools.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace DotNetStarter.IntegrationTestTools.Tests
{
    [TestClass]
    public class TestSetup
    {
        internal static bool StartedCheck = false;
        static IDbIntegrationTestSetup DbIntegrationTestSetup;

        [AssemblyInitialize]
        public static void TestInitialize(TestContext testContext)
        {
            var locator = ConfigureDotNetStarter();

            DbIntegrationTestSetup = locator.Get<IDbIntegrationTestSetup>()
                .SetDatabaseName("IntegrationTestTools_IntegrationTest")
                .Startup(() =>
                {
                    StartedCheck = true;
                });
        }

        [AssemblyCleanup]
        public static void TestShutdown()
        {
            DbIntegrationTestSetup.Shutdown();
        }

        private static ILocator ConfigureDotNetStarter()
        {
            // set assemblies to discover registration in the dotnetstarter.startup process
            var testAssemblies = new Assembly[]
            {
                typeof(IDbIntegrationTestSetup).GetTypeInfo().Assembly,
                typeof(DotNetStarter.Abstractions.IAssemblyFilter).GetTypeInfo().Assembly,
                typeof(DotNetStarter.ApplicationContext).GetTypeInfo().Assembly,
                typeof(DotNetStarter.Locators.DryIocLocator).GetTypeInfo().Assembly
            };

            DotNetStarter.ApplicationContext.Startup(assemblies: testAssemblies);

            return DotNetStarter.ApplicationContext.Default.Locator;
        }
    }
}