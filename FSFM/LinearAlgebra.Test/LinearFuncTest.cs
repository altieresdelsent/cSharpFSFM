using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LinearAlgebra;
namespace LinearAlgebra.Test
{
    [TestClass]
    public class LinearFuncTest
    {
        [TestMethod]
        public void TestConditionConstantXParallelFalse()
        {
            LinearFunc l1 = new LinearFunc(1, 10, 1, 100);
            LinearFunc l2 = new LinearFunc(2, 10, 2, 100);
            Assert.IsFalse(LinearFunc.IsTheretAEncounterAtTheRange(l1, l2));
        }
        [TestMethod]
        public void TestConditionConstantYParallelFalse()
        {
            LinearFunc l1 = new LinearFunc(10, 1, 100, 1);
            LinearFunc l2 = new LinearFunc(10, 2, 100, 2);
            Assert.IsFalse(LinearFunc.IsTheretAEncounterAtTheRange(l1, l2));
        }
        [TestMethod]
        public void TestConditionConstantXSameLineTrue()
        {
            LinearFunc l1 = new LinearFunc(1, 2, 1, 50);
            LinearFunc l2 = new LinearFunc(1, 49, 1, 100);
            Assert.IsTrue(LinearFunc.IsTheretAEncounterAtTheRange(l1, l2));
        }
        [TestMethod]
        public void TestConditionConstantXSameLineFalse()
        {
            LinearFunc l1 = new LinearFunc(1, 2, 1, 50);
            LinearFunc l2 = new LinearFunc(1, 52, 1, 100);
            Assert.IsFalse(LinearFunc.IsTheretAEncounterAtTheRange(l1, l2));
        }
        [TestMethod]
        public void TestConditionConstantYSameLineTrue()
        {
            LinearFunc l1 = new LinearFunc(10, 1, 50, 1);
            LinearFunc l2 = new LinearFunc(49, 1, 100, 1);
            Assert.IsTrue(LinearFunc.IsTheretAEncounterAtTheRange(l1, l2));
        }
        [TestMethod]
        public void TestConditionConstantYSameLineFalse()
        {
            LinearFunc l1 = new LinearFunc(10, 1, 50, 1);
            LinearFunc l2 = new LinearFunc(52, 1, 100, 1);
            Assert.IsFalse(LinearFunc.IsTheretAEncounterAtTheRange(l1, l2));
        }
        [TestMethod]
        public void TestConditionConstantYConstantXTrue()
        {
            LinearFunc l1 = new LinearFunc(-50, 1, 50, 1);
            LinearFunc l2 = new LinearFunc(1, -50, 1, 50);
            Assert.IsTrue(LinearFunc.IsTheretAEncounterAtTheRange(l1, l2));
        }
        [TestMethod]
        public void TestConditionConstantYConstantXFalse()
        {
            LinearFunc l1 = new LinearFunc(100, 1, 50, 1);
            LinearFunc l2 = new LinearFunc(1, -50, 1, 50);
            Assert.IsFalse(LinearFunc.IsTheretAEncounterAtTheRange(l1, l2));
        }
        [TestMethod]
        public void TestConditionConstantXAndMovingTrue()
        {
            LinearFunc l1 = new LinearFunc(-100, -10, -100, 10);
            LinearFunc l2 = new LinearFunc(-101, -9, -99, 11);
            Assert.IsTrue(LinearFunc.IsTheretAEncounterAtTheRange(l1, l2));
        }
        [TestMethod]
        public void TestConditionConstantXAndMovingFalse()
        {
            LinearFunc l1 = new LinearFunc(-100, -10, -100, 10);
            LinearFunc l2 = new LinearFunc(-150, -110, -101, 50);
            Assert.IsFalse(LinearFunc.IsTheretAEncounterAtTheRange(l1, l2));
        }
        [TestMethod]
        public void TestConditionConstantYAndMovingTrue()
        {
            LinearFunc l1 = new LinearFunc(100, 1, 150, 1);
            LinearFunc l2 = new LinearFunc(100, -10, 150, 50);
            Assert.IsTrue(LinearFunc.IsTheretAEncounterAtTheRange(l1, l2));
        }
        [TestMethod]
        public void TestConditionConstantYAndMovingFalse()
        {
            LinearFunc l1 = new LinearFunc(100, 1, 150, 1);
            LinearFunc l2 = new LinearFunc(1000, -50, -1000, -1);
            Assert.IsFalse(LinearFunc.IsTheretAEncounterAtTheRange(l1, l2));
        }
        [TestMethod]
        public void TestConditionBothMovingTrue()
        {
            LinearFunc l1 = new LinearFunc(45, 45, -45, -45);
            LinearFunc l2 = new LinearFunc(45, -45, -45, 45);
            Assert.IsTrue(LinearFunc.IsTheretAEncounterAtTheRange(l1, l2));
        }
        [TestMethod]
        public void TestConditionBothMovingFalse()
        {
            LinearFunc l1 = new LinearFunc(45, 45, -45, -45);
            LinearFunc l2 = new LinearFunc(45, -45, -2, -7);
            Assert.IsFalse(LinearFunc.IsTheretAEncounterAtTheRange(l1, l2));
        }
    }
}
