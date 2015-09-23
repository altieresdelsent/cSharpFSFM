using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PapaLeguasFuzzy;
using System.Linq.Expressions;
namespace PapaLeguasFuzzy.Test
{
    [TestClass]
    public class MathMethodsTest
    {
        [TestMethod]
        public void TestCreateMin()
        {
            var param1 = Expression.Parameter(typeof(double));
            var param2 = Expression.Parameter(typeof(double));
            var func = Expression.Lambda<Func<double, double, double>>(MathMethods.CreateMin(param1,param2),param1,param2).Compile();
            Assert.AreEqual<double>(func(1, 2), Math.Min(1, 2));
        }
        [TestMethod]
        public void TestCreateMax()
        {
            var param1 = Expression.Parameter(typeof(double));
            var param2 = Expression.Parameter(typeof(double));
            var func = Expression.Lambda<Func<double, double, double>>(MathMethods.CreateMax(param1, param2), param1, param2).Compile();
            Assert.AreEqual<double>(func(1, 2), Math.Max(1, 2));
        }
        [TestMethod]
        public void TestCreateExp()
        {
            var param1 = Expression.Parameter(typeof(double));
            var func = Expression.Lambda<Func<double, double>>(MathMethods.CreateExp(param1), param1).Compile();
            Assert.AreEqual<double>(func(2.5), Math.Exp(2.5));
        }
    }
}
