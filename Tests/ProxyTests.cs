using FluentAssertions;
using Lib.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class ProxyTests
    {
        private static ServiceInspector _inspector;

        [ClassInitialize]
        public static void ClassInit(TestContext _)
        {
            _inspector = new ServiceInspector(@"http://footballpool.dataaccess.eu/data/info.wso?wsdl");
        }

        [TestMethod]
        public void ShouldInspectMethodsWithoutErrors()
        {
            // arrange
            // act
            // assert
            _inspector.MethodInfos.Should().NotBeNullOrEmpty();
        }

        [DataTestMethod]
        [DataRow("Levels")]
        [DataRow("TeamNames")]
        [DataRow("CityNames")]
        public void ShouldRunMethodsWithoutErrors(string methodName)
        {
            // arrange
            // act
            var result = _inspector.RunMethod(methodName);

            // assert
            result.Should().NotBeNull();
        }
    }
}