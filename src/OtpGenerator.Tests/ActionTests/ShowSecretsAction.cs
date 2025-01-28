using OtpGenerator.Logic.Interfaces;
using OtpGenerator.Tests.Mocks;
using OtpGenerator.CommandLine.Actions;
using OtpGenerator.Logic.Generator;
using OtpGenerator.CommandLine.Interfaces;

namespace OtpGenerator.Tests.ActionTests
{
    [TestClass]
    public class ShowSecretsActionTest
    {
        private readonly MockOperationResultHandler _handler = new();
        private readonly IContextProvider _contextProvider = new ContextProvider();
        private IAction _action;


        [TestInitialize]
        public void Initialise()
        {
            _action = new ShowSecretsAction(_contextProvider, _handler);
        }

        [TestMethod]
        public void SetVerboseTest()
        {
            Assert.IsFalse(_contextProvider.ShowSecrets);

            _action.Execute(null);

            Assert.IsTrue(_contextProvider.ShowSecrets);
            Assert.IsNotNull(_handler.ObjectResults);
        }
    }
}