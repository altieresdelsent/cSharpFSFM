using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PapaLeguasFuzzy.Terms;
using System.Linq.Expressions;
namespace PapaLeguasFuzzy.Test.Terms
{
    [TestClass]
    public class GaussianTest
    {
        private static Variable varTest = new Variable("TESTE");
        private double GaussianClone(double sd, double mean, double x)
        {
            return Math.Exp(-0.5*(Math.Pow(x - mean, 2.0d) / Math.Pow(sd, 2.0d)));
        }        
        private double GaussianReal(double sd, double mean, double x)
        {
            return (new Gaussian(sd, mean, "GaussianTESte", varTest)).Pertinency(x);
        }
        [TestMethod]
        public void TestGaussian1andminus3and2()
        {
            var test1 = GaussianClone(1, -3, 2);
            var test2 = GaussianReal(1, -3, 2);
            Assert.AreEqual<double>(test1, test2);
        }
    }
}
