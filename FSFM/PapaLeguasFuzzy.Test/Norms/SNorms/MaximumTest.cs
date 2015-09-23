using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PapaLeguasFuzzy.Norms.SNorms;
namespace PapaLeguasFuzzy.Test.Norms.SNorms
{
    [TestClass]
    public class MaximumTest
    {
        [TestMethod]
        public void CallFunctionTestFirstMax()
        {
            Maximum max = new Maximum();
            Assert.AreEqual<double>(max.Pertinency(2.0d, 1.4d),2.0d);
        }

        [TestMethod]
        public void CallFunctionTestSecondMax()
        {
            Maximum max = new Maximum();
            Assert.AreEqual<double>(max.Pertinency(2.0d, 2.4d), 2.4d);
        }

        [TestMethod]
        public void CallFunctionTestFirstAndSecondEqual()
        {
            Maximum max = new Maximum();
            Assert.AreEqual<double>(max.Pertinency(1.4d, 1.4d), 1.4d);
        }
    }
}
