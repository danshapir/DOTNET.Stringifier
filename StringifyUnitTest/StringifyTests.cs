using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stringify;

namespace StringifyUnitTest
{
    [TestClass]
    public class StringifyTests
    {
        [TestMethod]
        public void Stringify_SimpleObject()
        {
            // Arrange
            var dummyClass = new DummyClassPerson
            {
                Name = "Gal",
                Age = 27
            };

            // Act
            var strRes = dummyClass.Stringify();

            // Assert
            var expected = "Age=27, Name=Gal";
            Assert.AreEqual(expected, strRes);
        }

        [TestMethod]
        public void Stringify_WithHiddenAndMasked()
        {
            // Arrange
            var dummyClass = new DummyClass
            {
                CCNumber = "1234123412341234", // Has MaskAttribute
                Hidden = "HIDDEN", // Has HideLogAttribute
                Regular = "RegularText",
                Inner = new DummyInnerClass
                {
                    InnerText = "InnerText"
                }
            };

            // Act
            var strRes = dummyClass.Stringify();

            // Assert
            var expected = "CCNumber=XXXXXXXXXXXXXXXX, Inner={InnerText=InnerText}, Regular=RegularText";
            Assert.AreEqual(expected, strRes);
        }

        [TestMethod]
        public void Stringify_ObjectWithEnumerableProperty()
        {
            // Arrange
            var list = new DummyClassPersonsList
            {
                Persons = new List<DummyClassPerson>
                {
                    new DummyClassPerson {Name = "Uza", Age = 5},
                    new DummyClassPerson {Name = "Shabi", Age = 6}
                },
                OneMoreProperty = "Hello World"
            };

            // Act
            var strRes = list.Stringify();

            // Assert
            var expected = "OneMoreProperty=Hello World, Persons=2";
            Assert.AreEqual(expected, strRes);
        }

        [TestMethod]
        public void Stringify_Enumerable()
        {
            // Arrange
            var list = new List<object>
            {
                new DummyClass
                {
                    CCNumber = "1234123412341234",
                    Hidden = "HIDDEN",
                    Regular = "RegularText",
                    Inner = new DummyInnerClass
                    {
                        InnerText = "InnerText"
                    }
                },
                new DummyClassPerson
                {
                    Name = "Gal",
                    Age = 27
                },
                new DummyClassPersonsList
                {
                    Persons = new List<DummyClassPerson>
                    {
                        new DummyClassPerson(),
                        new DummyClassPerson(),
                        new DummyClassPerson()
                    },
                    OneMoreProperty = null
                }
            };

            // Act
            var strRes = list.Stringify();

            // Assert
            var expected = "[{CCNumber=XXXXXXXXXXXXXXXX, Inner={InnerText=InnerText}, Regular=RegularText},{Age=27, Name=Gal},{Persons=3}]";
            Assert.AreEqual(expected, strRes);
        }

        [TestMethod]
        public void Stringify_EnumerableGeneric()
        {
            // Arrange
            var list = new List<int>
            {
                5,10, 15
            };

            // Act
            var strRes = list.Stringify();

            // Assert
            var expected = "[{5},{10},{15}]";
            Assert.AreEqual(expected, strRes);
        }

        [TestMethod]
        public void Stringify_ClassOnlyWithHidden()
        {
            // Arrange
            var list = new DummyClassOnlyWithHidden
            {
                Hidden = "Pacman"
            };

            // Act
            var strRes = list.Stringify();

            // Assert
            var expected = "";
            Assert.AreEqual(expected, strRes);
        }

        [TestMethod]
        public void Stringify_EnumerableOfClassOnlyWithHidden()
        {
            // Arrange
            IEnumerable list = new List<DummyClassOnlyWithHidden>
            {
                new DummyClassOnlyWithHidden
                {
                    Hidden = "Tut"
                },
                new DummyClassOnlyWithHidden
                {
                    Hidden = "Twix"
                }
            };

            // Act
            var strRes = list.Stringify();

            // Assert
            var expected = "[{},{}]";
            Assert.AreEqual(expected, strRes);
        }
    }
}
