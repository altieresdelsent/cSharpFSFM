using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PapaLeguasFuzzy.Norms.TNorms;
namespace PapaLeguasFuzzy.Test.Norms.TNorms
{
    [TestClass]
    public class ProductTest
    {
        private double ProductClone(double y1, double y2)
        {
            return y1 * y2;
        }
        [TestMethod]
        public void TestMethod1()
        {
            Product max = new Product();
            Assert.AreEqual<double>(max.Pertinency(0.1d, 0.4d), ProductClone(0.1d, 0.4d));
        }

        [TestMethod]
        public void TestMethod2()
        {
            Product max = new Product();
            Assert.AreEqual<double>(max.Pertinency(1.0d, 0.0d), ProductClone(1.0d, 0.0d));
        }

        [TestMethod]
        public void TestMethod3()
        {
            Product max = new Product();
            Assert.AreEqual<double>(max.Pertinency(0.0d, 1.0d), ProductClone(0.0d, 1.0d));
        }

        [TestMethod]
        public void TestMethod4()
        {
            Product max = new Product();
            Assert.AreEqual<double>(max.Pertinency(0.5d, 0.5d), ProductClone(0.5d, 0.5d));
        }
    }
}
