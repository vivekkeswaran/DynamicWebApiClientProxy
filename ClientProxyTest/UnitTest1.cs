using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClientProxyTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //arrange
            var proxyHandler = new SampleProxy();

            //act
            var result = proxyHandler.GetInformation("Test");
        }
    }
}
