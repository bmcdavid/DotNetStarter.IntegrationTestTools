namespace DotNetStarter.IntegrationTestTools.Tests
{
    public class MockLocalDbIntegrationTestSetup : LocalDbIntegrationTestSetup
    {
        internal static bool ShutdownCheck = false;

        public override void Shutdown()
        {
            base.Shutdown();

            ShutdownCheck = true;            
        }
    }
}