using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GateScheduler.FeatureTests
{
    /// <summary>
    /// This allows the Cucumber feature tests to be executed via unit test runner.
    /// </summary>
    [TestClass]
    public class FeatureTestsFixture
    {
        [TestMethod]
        public void RunFeatureTests()
        {
            if (!TestExecution.RunTests())
            {
                Assert.Fail("Feature tests failed");
            }
        }
    }
}
