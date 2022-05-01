using System.Collections.Generic;
using FluentAssertions;
using Lib.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class GenericMapperTests
    {
        [TestMethod]
        public void CanMapObjectToObject()
        {
            // arrange
            var person = new Person
            {
                Id = 1,
                FirstName = "Jessie",
                LastName = "Pinkman"
            };

            var mapper = new GenericMapper();

            // act
            var user = mapper.Map<User>(person);

            // assert
            user.Should().NotBeNull();
            user.Should().BeEquivalentTo(person);
        }

        [TestMethod]
        public void CanMapDictionaryToObject()
        {
            // arrange
            var dic = new Dictionary<string, object>
            {
                { "Id", 1 },
                { "FirstName", "Clark" },
                { "LastName", "Kent" }
            };

            var mapper = new GenericMapper();

            // act
            var person = mapper.Map<Person>(dic);

            // assert
            person.Should().NotBeNull();
            person.Id.Should().Be((int)dic["Id"]);
            person.FirstName.Should().Be((string)dic["FirstName"]);
            person.LastName.Should().Be((string)dic["LastName"]);
        }
    }
}