using OtpGenerator.Logic.Generator;
using OtpGenerator.Entities.Generator;
using OtpGenerator.Tests.Mocks;
using System.Diagnostics;

namespace OtpGenerator.Tests.TotpTests
{
    [TestClass]
    public class LiveGeneratorTest
    {
        private const int IntervalMs = 100;
        private const int MaximumTestRunTimeMs = 2000;
        private LiveGenerator _liveGenerator;
        private List<TimedOneTimePassword> _passwords = [];

        private void OnTotpUpdated(object sender, TotpUpdatedEventArgs e)
            => _passwords.Add(e.Password);

        [TestInitialize]
        public void Initialise()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            var generator = new CodeGenerator();
            var timer = new GeneratorTimer(IntervalMs);
            _liveGenerator = new LiveGenerator(generator, timer);
            _liveGenerator.AddService(definition);
            _liveGenerator.TotpUpdated += OnTotpUpdated;
        }

        [TestCleanup]
        public void CleanUp()
        {
            _liveGenerator.TotpUpdated -= OnTotpUpdated;
            _liveGenerator.Dispose();
        }

        [TestMethod]
        public void GenerateTest()
        {
            Assert.AreEqual(1, _liveGenerator.Services.Count);

            // Start a stopwatch, that's used to make sure the test doesn't run continuously if
            // something goes awry
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();

            // Start the generator and wait until all the messages have been sent or it's clear there's a problem
            _liveGenerator.Start();
            while (stopwatch.ElapsedMilliseconds < MaximumTestRunTimeMs)
            {
            }

            // Stop the generator
            _liveGenerator.Stop();

            // Confirm that all the generated codes are correct
            foreach (var totp in _passwords)
            {
                Assert.IsNotNull(totp);
                Assert.IsFalse(string.IsNullOrEmpty(totp.Code));
                Assert.AreEqual(6, totp.Code.Length);
                Assert.IsTrue(int.TryParse(totp.Code, out int intCode));
                Assert.IsTrue(totp.RemainingSeconds >= 0);
            }
        }

        [TestMethod]
        public void AddListOfServicesTest()
        {
            _liveGenerator.ClearServices();
            Assert.AreEqual(0, _liveGenerator.Services.Count);
            _liveGenerator.AddServices([new()]);
            Assert.AreEqual(1, _liveGenerator.Services.Count);
        }

        [TestMethod]
        public void ClearTest()
        {
            Assert.AreEqual(1, _liveGenerator.Services.Count);
            _liveGenerator.ClearServices();
            Assert.AreEqual(0, _liveGenerator.Services.Count);
        }
    }
}