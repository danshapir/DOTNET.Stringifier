using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stringify;

namespace StringifyUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAll()
        {
            var dummyClass = new DummyClass
            {
                CCNumber = "1234123412341234",
                Hidden = "HIDDEN",
                Regular = "RegularText",
                Inner = new DummyInnerClass
                {
                    InnerText = "InnerText"
                }
            };

            var strRes = dummyClass.Stringify();

            var result = "CCNumber=XXXXXXXXXXXXXXXX, Inner={InnerText=InnerText}, Regular=RegularText, ";

            Assert.AreEqual(result, strRes);
        }
    }
}
