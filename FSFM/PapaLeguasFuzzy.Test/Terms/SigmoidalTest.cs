using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using PapaLeguasFuzzy.Terms;
namespace PapaLeguasFuzzy.Test.Terms
{
    [TestClass]
    public class SigmoidalTest
    {
        private static Variable varTest = new Variable("TESTE");
        private double SigmoidClone(double slope, double inflection, double x)
        {
            slope = -slope;
            inflection = -inflection;
            return 1.0 / (1.0 + Math.Exp(slope * (x + inflection)));
        }
        private double SigmoidReal(double slope, double inflection, double x)
        {
            return (new Sigmoid(slope, inflection, "GaussianTESte", varTest)).Pertinency(x);
        }
        [TestMethod]
        public void TestMethod1andmius3and2()
        {
            var test1 = SigmoidClone(1, -3, 2);
            var test2 = SigmoidReal(1, -3, 2);
            Assert.AreEqual<double>(test1, test2);
        }
    }
}
