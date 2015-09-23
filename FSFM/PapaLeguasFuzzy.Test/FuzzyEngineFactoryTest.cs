using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq.Expressions;
using PapaLeguasFuzzy.Terms;

namespace PapaLeguasFuzzy.Test
{
    [TestClass]
    public class FuzzyEngineFactoryTest
    {      
        [TestMethod]
        public void RemoveDoubleEspaces()
        {
            var result = FuzzyEngineFactory.RemoveDoubleEspaces("a  a aaa   a   a a  a       a");
            Assert.AreEqual(result, "a a aaa a a a a a");
        }
        [TestMethod]
        public void ParseRule()
        {
            string[] antecedent;
            string[] consequent;
            FuzzyEngineFactory.ParseRule("if INPUT is LOW and INPUT2 IS FAST then  OUTPUT is   HIGH and OUTPUT2 is BIG", out antecedent, out consequent);
            Assert.AreEqual(antecedent[0], "INPUT_LOW".ToLowerInvariant());
            Assert.AreEqual(antecedent[1], "and".ToLowerInvariant());
            Assert.AreEqual(antecedent[2], "INPUT2_FAST".ToLowerInvariant());
            Assert.AreEqual(consequent[0], "OUTPUT_HIGH".ToLowerInvariant());
            Assert.AreEqual(consequent[1], "OUTPUT2_BIG".ToLowerInvariant());
        }

        [TestMethod]
        public void Fuzzify()
        {
            FuzzyEngineFactory engine = FuzzyEngineMock.CreateFuzzyEngine();
            var inputVariables = new Dictionary<string, double>()
            {
                {"INPUT".ToLowerInvariant(),2.5}
            };
            var fuzzified = engine.Fuzzify(inputVariables);
            Assert.AreEqual(0.5d, fuzzified["INPUT_LOW".ToLowerInvariant()]);
            Assert.AreEqual(0.5d, fuzzified["INPUT_HIGH".ToLowerInvariant()]);

        }

        [TestMethod]
        public void FuzzifyTwoInputs()
        {
            FuzzyEngineFactory engine = FuzzyEngineMock.CreateFuzzyEngineTwoInputs();
            var inputVariables = new Dictionary<string, double>()
            {
                {"INPUT".ToLowerInvariant(),2.5},
                {"INPUT2".ToLowerInvariant(),2}
            };
            var fuzzified = engine.Fuzzify(inputVariables);
            Assert.AreEqual(0.5d, fuzzified["INPUT_LOW".ToLowerInvariant()]);
            Assert.AreEqual(0.5d, fuzzified["INPUT_HIGH".ToLowerInvariant()]);
            Assert.AreEqual(1.0d, fuzzified["INPUT2_SLOW".ToLowerInvariant()]);
            Assert.AreEqual(0.0d, fuzzified["INPUT2_FAST".ToLowerInvariant()]);

        }
        [TestMethod]
        public void GetRulesRelatedToOutputVariable()
        {
            FuzzyEngineFactory engine = FuzzyEngineMock.CreateFuzzyEngine();
            var rulesRelated = engine.GetRulesRelatedToOutputVariableTerm();
            Assert.AreEqual("rule_1", rulesRelated["OUTPUT_LOW".ToLowerInvariant()][0]);
            Assert.AreEqual("rule_0", rulesRelated["OUTPUT_HIGH".ToLowerInvariant()][0]);
        }
        [TestMethod]
        public void GetRulesRelatedToOutputVariableTwoOutputs()
        {
            FuzzyEngineFactory engine = FuzzyEngineMock.CreateFuzzyEngineTwoInputsTwoOutputs();
            var rulesRelated = engine.GetRulesRelatedToOutputVariableTerm();
            Assert.AreEqual("rule_1", rulesRelated["OUTPUT_LOW".ToLowerInvariant()][0]);
            Assert.AreEqual("rule_3", rulesRelated["OUTPUT_LOW".ToLowerInvariant()][1]);
            Assert.AreEqual("rule_0", rulesRelated["OUTPUT_HIGH".ToLowerInvariant()][0]);
            Assert.AreEqual("rule_2", rulesRelated["OUTPUT_HIGH".ToLowerInvariant()][1]);
            Assert.AreEqual("rule_2", rulesRelated["OUTPUT2_SLOW".ToLowerInvariant()][0]);
            Assert.AreEqual("rule_3", rulesRelated["OUTPUT2_SLOW".ToLowerInvariant()][1]);
            Assert.AreEqual("rule_0", rulesRelated["OUTPUT2_FAST".ToLowerInvariant()][0]);
            Assert.AreEqual("rule_1", rulesRelated["OUTPUT2_FAST".ToLowerInvariant()][1]);
        }
        [TestMethod]
        public void GetInnerTrapezoid()
        {
            double previousTermVertexC = 3.0d, tanprevious = 1.0d, previousActivation = 0.45d, currentTermVertexA = 1.0d, tanCurrent = 1.0d, currentActivation = 0.46d, sumAreaName = 0.0d, sumAreaCenterName = 0.0d;
            FuzzyEngineFactory.GetInnerTrapezoid(previousTermVertexC, tanprevious, previousActivation, currentTermVertexA, tanCurrent, currentActivation,ref sumAreaName,ref sumAreaCenterName);
            Assert.AreEqual(0.6975d, -sumAreaName);
            Assert.AreEqual(0.6975d*2.0d, -sumAreaCenterName);
        }
        [TestMethod]
        public void InferenceRulesAntecedent()
        {
            FuzzyEngineFactory engine = FuzzyEngineMock.CreateFuzzyEngine();
            var inputTermsVariables = new Dictionary<string, double>()
            {
                {"INPUT_LOW".ToLowerInvariant(),0.5d},
                {"INPUT_HIGH".ToLowerInvariant(),1.0d}
            };
            var inferenceRules = engine.InferenceRulesAntecedent(inputTermsVariables);
            Assert.AreEqual(0.5d, inferenceRules["rule_0".ToLowerInvariant()]);
            Assert.AreEqual(1.0d, inferenceRules["rule_1".ToLowerInvariant()]);
        }
        [TestMethod]
        public void InferenceRulesAntecedentTwoInputs()
        {
            FuzzyEngineFactory engine = FuzzyEngineMock.CreateFuzzyEngineTwoInputs();
            var inputTermsVariables = new Dictionary<string, double>()
            {
                {"INPUT_LOW".ToLowerInvariant(),0.5d},
                {"INPUT_HIGH".ToLowerInvariant(),1.0d},
                {"INPUT2_SLOW".ToLowerInvariant(),0.4d},
                {"INPUT2_FAST".ToLowerInvariant(),0.8d}
            };
            var inferenceRules = engine.InferenceRulesAntecedent(inputTermsVariables);
            Assert.AreEqual(0.2d, inferenceRules["rule_0".ToLowerInvariant()]);
            Assert.AreEqual(0.4d, inferenceRules["rule_1".ToLowerInvariant()]);
            Assert.AreEqual(0.4d, inferenceRules["rule_2".ToLowerInvariant()]);
            Assert.AreEqual(0.8d, inferenceRules["rule_3".ToLowerInvariant()]);
        }
        [TestMethod]
        public void InferenceRulesConsequent()
        {
            FuzzyEngineFactory engine = FuzzyEngineMock.CreateFuzzyEngine();
            var rules = new Dictionary<string, double>()
            {
                {"rule_0".ToLowerInvariant(),0.5d},
                {"rule_1".ToLowerInvariant(),1.0d}
            };
            var relations = engine.GetRulesRelatedToOutputVariableTerm();
            var outputTermsVariables = engine.InferenceRulesConsequent(rules, relations);
            Assert.AreEqual(0.5d, outputTermsVariables["OUTPUT_HIGH".ToLowerInvariant()]);
            Assert.AreEqual(1.0d, outputTermsVariables["OUTPUT_LOW".ToLowerInvariant()]);
        }
        [TestMethod]
        public void InferenceRulesConsequentTwoInputsTwoOutputs()
        {
            FuzzyEngineFactory engine = FuzzyEngineMock.CreateFuzzyEngineTwoInputsTwoOutputs();
            var rulesVariables = new Dictionary<string, double>()
            {
                {"rule_0".ToLowerInvariant(),1.0d},
                {"rule_1".ToLowerInvariant(),2.0d},
                {"rule_2".ToLowerInvariant(),1.5d},
                {"rule_3".ToLowerInvariant(),1.7d}
            };
            var relations = engine.GetRulesRelatedToOutputVariableTerm();
            var outputTermsVariables = engine.InferenceRulesConsequent(rulesVariables,relations);
            Assert.AreEqual(Math.Max(rulesVariables["rule_0"], rulesVariables["rule_2"]), outputTermsVariables["OUTPUT_HIGH".ToLowerInvariant()]);
            Assert.AreEqual(Math.Max(rulesVariables["rule_1"], rulesVariables["rule_3"]), outputTermsVariables["OUTPUT_LOW".ToLowerInvariant()]);
            Assert.AreEqual(Math.Max(rulesVariables["rule_2"], rulesVariables["rule_3"]), outputTermsVariables["OUTPUT2_SLOW".ToLowerInvariant()]);
            Assert.AreEqual(Math.Max(rulesVariables["rule_1"], rulesVariables["rule_0"]), outputTermsVariables["OUTPUT2_FAST".ToLowerInvariant()]);
        }
        private double Defuzzify(double firstPertinency, double secondPertinency,Triangle first, Triangle second)
        {
            return 0;
        }
        [TestMethod]
        public void Defuzzify()
        {
            FuzzyEngineFactory engine = FuzzyEngineMock.CreateFuzzyEngine();
            var termsOutputVariable = new Dictionary<string, double>()
            {
                {"OUTPUT_HIGH".ToLowerInvariant(),0.5d},
                {"OUTPUT_LOW".ToLowerInvariant(),0.5d},
            };
            var teste = engine.Defuzzify(termsOutputVariable);
            Assert.AreEqual(2.5d, teste["OUTPUT".ToLowerInvariant()]);
        }

        [TestMethod]
        public void DefuzzifyUpperUpperTriangle()
        {
            FuzzyEngineFactory engine = FuzzyEngineMock.CreateFuzzyEngine();
            var termsOutputVariable = new Dictionary<string, double>()
            {
                {"OUTPUT_HIGH".ToLowerInvariant(),0.7d},
                {"OUTPUT_LOW".ToLowerInvariant(),0.7d},
            };
            var teste = engine.Defuzzify(termsOutputVariable);
            var valueOUTPUT = Math.Round(teste["OUTPUT".ToLowerInvariant()], 6, MidpointRounding.AwayFromZero);
            Assert.AreEqual(2.5d, valueOUTPUT);
        }
        [TestMethod]
        public void DefuzzifyUpperLowerTriangle()
        {
            FuzzyEngineFactory engine = FuzzyEngineMock.CreateFuzzyEngine();
            var termsOutputVariable = new Dictionary<string, double>()
            {
                {"OUTPUT_HIGH".ToLowerInvariant(),0.3d},
                {"OUTPUT_LOW".ToLowerInvariant(),0.3d},
            };
            var teste = engine.Defuzzify(termsOutputVariable);
            var valueOUTPUT = Math.Round(teste["OUTPUT".ToLowerInvariant()], 6, MidpointRounding.AwayFromZero);
            Assert.AreEqual(2.5d, valueOUTPUT);
        }
        [TestMethod]
        public void DefuzzifyTwoInputsTwoOutputs()
        {
            FuzzyEngineFactory engine = FuzzyEngineMock.CreateFuzzyEngineTwoInputsTwoOutputs();
            var termsOutputVariable = new Dictionary<string, double>()
            {
                {"OUTPUT_HIGH".ToLowerInvariant(),0.5d},
                {"OUTPUT_LOW".ToLowerInvariant(),0.5d},
                {"OUTPUT2_SLOW".ToLowerInvariant(),0.3d},
                {"OUTPUT2_FAST".ToLowerInvariant(),0.3d},
            };
            var teste = engine.Defuzzify(termsOutputVariable);
            var valueOUTPUT = Math.Round(teste["OUTPUT".ToLowerInvariant()], 6, MidpointRounding.AwayFromZero);
            var valueOUTPUT2 = Math.Round(teste["OUTPUT2".ToLowerInvariant()], 6, MidpointRounding.AwayFromZero);
            Assert.AreEqual(2.5d, valueOUTPUT);
            Assert.AreEqual(2.5d, valueOUTPUT2);
        }

        [TestMethod]
        public void DefuzzifyTwoInputsTwoOutputsUpperTriangle()
        {
            FuzzyEngineFactory engine = FuzzyEngineMock.CreateFuzzyEngineTwoInputsTwoOutputs();
            var termsOutputVariable = new Dictionary<string, double>()
            {
                {"OUTPUT_HIGH".ToLowerInvariant(),1.0d},
                {"OUTPUT_LOW".ToLowerInvariant(),1.0d},
                {"OUTPUT2_SLOW".ToLowerInvariant(),0.7d},
                {"OUTPUT2_FAST".ToLowerInvariant(),0.7d},
            };
            var teste = engine.Defuzzify(termsOutputVariable);
            var valueOUTPUT = Math.Round(teste["OUTPUT".ToLowerInvariant()], 6, MidpointRounding.AwayFromZero);
            var valueOUTPUT2 = Math.Round(teste["OUTPUT2".ToLowerInvariant()], 6, MidpointRounding.AwayFromZero);
            Assert.AreEqual(2.5d, valueOUTPUT);
            Assert.AreEqual(2.5d, valueOUTPUT2);
        }
       
    }
}
