using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PapaLeguasFuzzy.Norms.SNorms;
namespace PapaLeguasFuzzy.Test.Norms.SNorms
{
    [TestClass]
    public class ProbabilisticSumTest
    {
        private double ProbabilisticSumClone(double y1, double y2)
        {
            return (y1 + y2) - (y1 * y2);
        }
        [TestMethod]
        public void TestMethod1()
        {
            ProbabilisticSum max = new ProbabilisticSum();
            Assert.AreEqual<double>(max.Pertinency(0.1d, 0.4d), ProbabilisticSumClone(0.1d, 0.4d));
        }

        [TestMethod]
        public void TestMethod2()
        {
            ProbabilisticSum max = new ProbabilisticSum();
            Assert.AreEqual<double>(max.Pertinency(1.0d, 0.0d), ProbabilisticSumClone(1.0d, 0.0d));
        }

        [TestMethod]
        public void TestMethod3()
        {
            ProbabilisticSum max = new ProbabilisticSum();
            Assert.AreEqual<double>(max.Pertinency(0.0d, 1.0d), ProbabilisticSumClone(0.0d, 1.0d));
        }

        [TestMethod]
        public void TestMethod4()
        {
            ProbabilisticSum max = new ProbabilisticSum();
            Assert.AreEqual<double>(max.Pertinency(0.5d, 0.5d), ProbabilisticSumClone(0.5d, 0.5d));
        }
    }
}
