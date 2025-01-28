using OtpGenerator.CommandLine.Entities;
using OtpGenerator.CommandLine.Logic;
using OtpGenerator.Tests.Mocks;
using OtpGenerator.CommandLine.Exceptions;

namespace OtpGenerator.Tests.CommandLineTests
{
    [TestClass]
    public class CommandLineParserTest
    {
        private readonly string ServiceName = DataGenerator.RandomServiceName();
        private readonly string AccountName = DataGenerator.RandomAccountName();
        private readonly string Secret = DataGenerator.RandomServiceSecret();

        private CommandLineParser _parser;

        [TestInitialize]
        public void TestInitialise()
        {
            _parser = new CommandLineParser();
            _parser.Add(CommandLineOptionType.Add, true, "--add", "-a", "Add a new service definition", "name account secret", 3, 3);
            _parser.Add(CommandLineOptionType.Delete, true, "--delete", "-d", "Delete an existing service definition", "name account", 2, 2);
        }

        [TestMethod]
        public void ValidUsingNamesTest()
        {
            string[] args = ["--add", ServiceName, AccountName, Secret];
            _parser!.Parse(args);

            var values = _parser?.GetValues(CommandLineOptionType.Add);
            Assert.IsNotNull(values);
            Assert.AreEqual(3, values.Count);
            Assert.AreEqual(ServiceName, values[0]);
            Assert.AreEqual(AccountName, values[1]);
            Assert.AreEqual(Secret, values[2]);
        }

        [TestMethod]
        public void ValidUsingShortNamesTest()
        {
            string[] args = ["-a", ServiceName, AccountName, Secret];
            _parser!.Parse(args);

            var values = _parser?.GetValues(CommandLineOptionType.Add);
            Assert.IsNotNull(values);
            Assert.AreEqual(3, values.Count);
            Assert.AreEqual(ServiceName, values[0]);
            Assert.AreEqual(AccountName, values[1]);
            Assert.AreEqual(Secret, values[2]);
        }

        [TestMethod]
        [ExpectedException(typeof(TooFewValuesException))]
        public void TooFewArgumentsFailsTest()
        {
            string[] args = ["-a", ServiceName];
            _parser!.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(TooManyValuesException))]
        public void TooManyArgumentsFailsTest()
        {
            string[] args = ["-a", ServiceName, AccountName, Secret, "Extra Argument"];
            _parser!.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(UnrecognisedCommandLineOptionException))]
        public void UnrecognisedOptionNameFailsTest()
        {
            string[] args = ["--oops", ServiceName, AccountName, Secret];
            _parser!.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(UnrecognisedCommandLineOptionException))]
        public void UnrecognisedOptionShortNameFailsTest()
        {
            string[] args = ["-o", ServiceName, AccountName, Secret];
            _parser!.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(MalformedCommandLineException))]
        public void MalformedCommandLineFailsTest()
        {
            string[] args = [ServiceName, "--add", AccountName, Secret];
            _parser!.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateOptionException))]
        public void DuplicateOptionTypeFailsTest()
        {
            _parser!.Add(CommandLineOptionType.Add, true, "--other-add", "-oa", "Duplicate option type", "", 2, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateOptionException))]
        public void DuplicateOptionNameFailsTest()
        {
            _parser!.Add(CommandLineOptionType.Unknown, true, "--add", "-oa", "Duplicate option name", "", 2, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateOptionException))]
        public void DuplicateOptionShortNameFailsTest()
        {
            _parser!.Add(CommandLineOptionType.Unknown, true, "--other-add", "-a", "Duplicate option shortname", "", 2, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(MultipleOperationsException))]
        public void MultipleOperationsFailsTest()
        {
            string[] args = ["--add", ServiceName, AccountName, Secret, "--delete", ServiceName, AccountName];
            _parser!.Parse(args);
        }
    }
}
