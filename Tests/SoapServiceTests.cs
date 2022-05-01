using FluentAssertions;
using Lib.Helpers;
using Lib.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class SoapServiceTests
    {
        private static SoapService _service;

        [ClassInitialize]
        public static void ClassInit(TestContext _)
        {
            var credentials = new SoapServiceCredentials(@"http://footballpool.dataaccess.eu/data/info.wso?wsdl");
            var mapper = new GenericMapper();
            var logger = NullLogger<SoapService>.Instance;
            _service = new SoapService(credentials, mapper, logger);
        }

        [TestMethod]
        public void ShouldInspectMethodsWithoutErrors()
        {
            // arrange
            // act
            var methods = _service.GetMethodInfos();

            // assert
            methods.Should().NotBeNullOrEmpty();
        }

        [DataTestMethod]
        [DataRow("Levels")]
        [DataRow("TeamNames")]
        [DataRow("CityNames")]
        public void ShouldRunMethodsWithoutErrors(string methodName)
        {
            // arrange
            // act
            var result = _service.RunMethod(methodName);

            // assert
            result.Should().NotBeNull();
        }
    }
}