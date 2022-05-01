using FluentAssertions;
using Lib.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class SoapServiceTests
    {
        [TestMethod]
        public void ShouldInspectMethodsWithoutErrors()
        {
            // arrange
            var credentials = new SoapServiceCredentials(@"http://footballpool.dataaccess.eu/data/info.wso?wsdl");
            var service = SoapService.BuildSoapService(credentials);

            // act
            var methods = service.GetMethodInfos();

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
            var credentials = new SoapServiceCredentials(@"http://footballpool.dataaccess.eu/data/info.wso?wsdl");
            var service = SoapService.BuildSoapService(credentials);

            // act
            var result = service.RunMethod(methodName);

            // assert
            result.Should().NotBeNull();
        }
    }
}