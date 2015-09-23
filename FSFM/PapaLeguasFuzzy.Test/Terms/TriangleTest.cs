using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using PapaLeguasFuzzy.Terms;

namespace PapaLeguasFuzzy.Test.Terms
{
    [TestClass]
    public class TriangleTest
    {
        private static Variable varTest = new Variable("TESTE");
        private double TriangleClone(double a, double c, double x)
        {
            double b = (a+c) / 2.0d;
             if ((x < a) || (x > c))
                return 0.0d;
            else if (x == b)
                return  1.0;
            else if (x < b)
                return  (x - a) / (b - a);
            else
                return (c - x) / (c - b);
        }
        private double TriangleReal(double a, double c, double x)
        {
            return (new Triangle(a, c, "GaussianTESte", varTest)).Pertinency(x);
        }
        [TestMethod]
        public void TestMethodLessThaA()
        {
            var test1 = TriangleClone(1, 3, 0);
            var test2 = TriangleReal(1, 3, 0);
            Assert.AreEqual<double>(test1, test2);
        }
        [TestMethod]
        public void TestMethodBiggerThanC()
        {
            var test1 = TriangleClone(1, 3, 4);
            var test2 = TriangleReal(1, 3, 4);
            Assert.AreEqual<double>(test1, test2);
        }
        [TestMethod]
        public void TestMethodEqualsB()
        {
            var test1 = TriangleClone(1, 3, 2);
            var test2 = TriangleReal(1, 3, 2);
            Assert.AreEqual<double>(test1, test2);
        }
        [TestMethod]
        public void TestMethodBetweenAandB()
        {
            var test1 = TriangleClone(1, 3, 1.5);
            var test2 = TriangleReal(1, 3, 1.5);
            Assert.AreEqual<double>(test1, test2);
        }
        [TestMethod]
        public void TestMethodBetweenBandC()
        {
            var test1 = TriangleClone(1, 3, 2.5);
            var test2 = TriangleReal(1, 3, 2.5);
            Assert.AreEqual<double>(test1, test2);
        }
        
    }
}
