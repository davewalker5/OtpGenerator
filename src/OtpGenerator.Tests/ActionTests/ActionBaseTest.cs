using OtpGenerator.Logic.Interfaces;
using OtpGenerator.Logic.Storage;
using OtpGenerator.CommandLine.Actions;
using OtpGenerator.Logic.Generator;

namespace OtpGenerator.Tests.ActionTests
{
    [TestClass]
    public class ActionBaseTest
    {
        private readonly IContextProvider _contextProvider = new ContextProvider();
        private MockAction _action;

        [TestInitialize]
        public void Initialise()
        {
            var manager = new ServiceDatabaseManager(null, null, null, null);
            var dummyManager = new DummyServiceDatabaseManager(null, null);
            _action = new MockAction(_contextProvider, manager, dummyManager);
        }

        [TestMethod]
        public void RealContextReturnsRealManagerTest()
        {
            _contextProvider.Dummy = false;
            var managerType = _action.ManagerType;
            Assert.AreEqual(typeof(ServiceDatabaseManager), managerType);
        }

        [TestMethod]
        public void DummyContextReturnsDummyManagerTest()
        {
            _contextProvider.Dummy = true;
            var managerType = _action.ManagerType;
            Assert.AreEqual(typeof(DummyServiceDatabaseManager), managerType);
        }
    }
}