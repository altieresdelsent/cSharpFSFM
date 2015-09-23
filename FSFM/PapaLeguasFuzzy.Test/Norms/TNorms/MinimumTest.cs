using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PapaLeguasFuzzy.Norms.TNorms;
namespace PapaLeguasFuzzy.Test.Norms.TNorms
{
    [TestClass]
    public class MinimumTest
    {
        [TestMethod]
        public void CallFunctionTestSecondMin()
        {
            Minimum min = new Minimum();
            Assert.AreEqual<double>(min.Pertinency(2.0d, 1.4d), 1.4d);
        }

        [TestMethod]
        public void CallFunctionTestFirstMin()
        {
            Minimum min = new Minimum();
            Assert.AreEqual<double>(min.Pertinency(2.0d, 2.4d), 2.0d);
        }

        [TestMethod]
        public void CallFunctionTestFirstAndSecondEqual()
        {
            Minimum min = new Minimum();
            Assert.AreEqual<double>(min.Pertinency(1.4d, 1.4d), 1.4d);
        }
    }
}
