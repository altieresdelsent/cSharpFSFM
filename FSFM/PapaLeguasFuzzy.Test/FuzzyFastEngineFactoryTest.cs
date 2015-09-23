using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using PapaLeguasFuzzy;
using PapaLeguasFuzzy.Terms;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
namespace PapaLeguasFuzzy.Test
{
    [TestClass]
    public class FuzzyFastEngineFactoryTest
    {
        private static MethodInfo AssertDynamic = ((typeof(Assert).GetMethods()).First(c => c.IsGenericMethod && c.Name == "AreEqual")).MakeGenericMethod( typeof(double));
        private static MethodInfo GetMethod(Type t, string name)
        {
            var all = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            return t.GetMethod(name,all);
        }
        private MethodCallExpression AreEqual(Expression a, Expression b)
        {
            a = MathMethods.CreateRound(a, 7);
            b = MathMethods.CreateRound(b, 7);
            return Expression.Call(AssertDynamic, a, b);
        }
        [TestMethod]
        public void TestMethodFuzzifyVariable()
        {
            var method = GetMethod(typeof(FuzzyFastEngineFactory), "GenerateFuzzifyVariable");
        }
        [TestMethod]
        public void TestMethodFuzzify()
        {
            var instance = FuzzyEngineMock.CreateFuzzyEngine();
            var method = GetMethod(typeof(FuzzyFastEngineFactory), "GenerateFuzzify");
            var inputVariables = new Dictionary<string, double>()
            {
                {"INPUT".ToLowerInvariant(),2.5}
            };
            var fuzzified = instance.Fuzzify(inputVariables);
            var blockFuzzify = method.Invoke(instance, new object[]{}) as BlockExpression;
            List<Expression> assignOperations = new List<Expression>();
            assignOperations.AddRange(blockFuzzify.Expressions);
            var zero5 = Expression.Constant(0.5d,typeof(double));
            assignOperations.Add(AreEqual(zero5,instance.VariablesNames["INPUT_LOW".ToLowerInvariant()]));
            assignOperations.Add(AreEqual(zero5, instance.VariablesNames["INPUT_HIGH".ToLowerInvariant()]));
            assignOperations.Add(Expression.Constant(0.0d, typeof(double)));
            var parameters = instance.InputVariables.Select(c => instance.VariablesNames[c.Name]).ToArray();
            var body = Expression.Block(blockFuzzify.Variables, assignOperations);
            var lambda = Expression.Lambda<Func<double, double>>(body, parameters).Compile();
            lambda.Invoke(2.5d);
        }

        [TestMethod]
        public void TestMethodFuzzifyTwoInputs()
        {
            var instance = FuzzyEngineMock.CreateFuzzyEngineTwoInputs();
            var method = GetMethod(typeof(FuzzyFastEngineFactory), "GenerateFuzzify");
            var inputVariables = new Dictionary<string, double>()
            {
                {"INPUT".ToLowerInvariant(),2.5},
                {"INPUT2".ToLowerInvariant(),2.4}
            };
            var fuzzified = instance.Fuzzify(inputVariables);
            var blockFuzzify = method.Invoke(instance, new object[] { }) as BlockExpression;
            List<Expression> assignOperations = new List<Expression>();
            assignOperations.AddRange(blockFuzzify.Expressions);

            foreach (var fuzzyTerm in fuzzified)
                assignOperations.Add(AreEqual(Expression.Constant(fuzzyTerm.Value, typeof(double)), instance.VariablesNames[fuzzyTerm.Key]));

            assignOperations.Add(Expression.Constant(0.0d, typeof(double)));
            var parameters = instance.InputVariables.Select(c => instance.VariablesNames[c.Name]).ToArray();
            var body = Expression.Block(blockFuzzify.Variables, assignOperations);
            var lambda = Expression.Lambda<Func<double, double, double>>(body, parameters).Compile();
            lambda.Invoke(2.5d, 2.4d);
        }

        [TestMethod]
        public void TestMethodInferenceAntecedent()
        {

            var instance = FuzzyEngineMock.CreateFuzzyEngine();
            var method = GetMethod(typeof(FuzzyFastEngineFactory), "GenerateInferenceRulesAntecedent");
            var inputVariablesTerms = new Dictionary<string, double>()
            {
                {"INPUT_LOW".ToLowerInvariant(),0.5},
                {"INPUT_HIGH".ToLowerInvariant(),0.7},
            };
            var rulesVariables = instance.InferenceRulesAntecedent(inputVariablesTerms);
            var blockFuzzify = method.Invoke(instance, new object[] { }) as BlockExpression;
            List<Expression> assignOperations = new List<Expression>();
            assignOperations.AddRange(blockFuzzify.Expressions);
            
            foreach(var ruleVariable in rulesVariables)
                assignOperations.Add(AreEqual(Expression.Constant(ruleVariable.Value, typeof(double)), instance.VariablesNames[ruleVariable.Key]));

            assignOperations.Add(Expression.Constant(0.0d, typeof(double)));
            var parameters = inputVariablesTerms.Select(c => instance.VariablesNames[c.Key]).ToArray();
            var body = Expression.Block(blockFuzzify.Variables, assignOperations);
            var lambda = Expression.Lambda<Func<double, double, double>>(body, parameters).Compile();
            lambda.Invoke(0.5,0.7);
        }

        [TestMethod]
        public void TestMethodInferenceAntecedentTwoInputsTwoOutputs()
        {

            var instance = FuzzyEngineMock.CreateFuzzyEngineTwoInputsTwoOutputs();
            var method = GetMethod(typeof(FuzzyFastEngineFactory), "GenerateInferenceRulesAntecedent");
            var inputVariablesTerms = new Dictionary<string, double>()
            {
                {"INPUT_LOW".ToLowerInvariant(),0.5},
                {"INPUT_HIGH".ToLowerInvariant(),0.5},
                {"INPUT2_SLOW".ToLowerInvariant(),0.6},
                {"INPUT2_FAST".ToLowerInvariant(),0.4},
            };
            var rulesVariables = instance.InferenceRulesAntecedent(inputVariablesTerms);
            var blockFuzzify = method.Invoke(instance, new object[] { }) as BlockExpression;
            List<Expression> assignOperations = new List<Expression>();
            assignOperations.AddRange(blockFuzzify.Expressions);

            foreach (var ruleVariable in rulesVariables)
                assignOperations.Add(AreEqual(Expression.Constant(ruleVariable.Value, typeof(double)), instance.VariablesNames[ruleVariable.Key]));

            assignOperations.Add(Expression.Constant(0.0d, typeof(double)));
            var parameters = inputVariablesTerms.Select(c => instance.VariablesNames[c.Key]).ToArray();
            var body = Expression.Block(blockFuzzify.Variables, assignOperations);
            var lambda = Expression.Lambda<Func<double, double, double, double, double>>(body, parameters).Compile();
            lambda.Invoke(0.5, 0.5,0.6,0.4);
        }

        [TestMethod]
        public void TestMethodInferenceConsequent()
        {

            var instance = FuzzyEngineMock.CreateFuzzyEngine();
            var method = GetMethod(typeof(FuzzyFastEngineFactory), "GenerateInferenceConsequent");
            var rulesVariables = new Dictionary<string, double>()
            {
                {"rule_0".ToLowerInvariant(),0.5d},
                {"rule_1".ToLowerInvariant(),0.7d},
            };
            var relation = (instance as FuzzyEngineFactory).GetRulesRelatedToOutputVariableTerm();
            var relation2 = instance.GetRulesRelatedToOutputVariableTerm();
            var outputTermsVariables = instance.InferenceRulesConsequent(rulesVariables, relation);
            var blockInferenceConsequent = method.Invoke(instance, new object[] { relation2 }) as BlockExpression;
            List<Expression> assignOperations = new List<Expression>();
            assignOperations.AddRange(blockInferenceConsequent.Expressions);

            foreach (var outputTermVariable in outputTermsVariables)
                assignOperations.Add(AreEqual(Expression.Constant(outputTermVariable.Value, typeof(double)), instance.VariablesNames[outputTermVariable.Key]));

            assignOperations.Add(Expression.Constant(0.0d, typeof(double)));
            var parameters = rulesVariables.Select(c => instance.VariablesNames[c.Key]).ToArray();
            var body = Expression.Block(blockInferenceConsequent.Variables, assignOperations);
            var lambda = Expression.Lambda<Func<double, double, double>>(body, parameters).Compile();
            lambda.Invoke(0.5d, 0.7d);
        }

        [TestMethod]
        public void TestMethodInferenceConsequentTwoInputsTwoOutputs()
        {

            var instance = FuzzyEngineMock.CreateFuzzyEngineTwoInputsTwoOutputs();
            var method = GetMethod(typeof(FuzzyFastEngineFactory), "GenerateInferenceConsequent");
            var rulesVariables = new Dictionary<string, double>()
            {
                {"rule_0".ToLowerInvariant(),0.1d},
                {"rule_1".ToLowerInvariant(),0.2d},
                {"rule_2".ToLowerInvariant(),0.15d},
                {"rule_3".ToLowerInvariant(),0.4d},
            };
            var relation = (instance as FuzzyEngineFactory).GetRulesRelatedToOutputVariableTerm();
            var relation2 = instance.GetRulesRelatedToOutputVariableTerm();
            var outputTermsVariables = instance.InferenceRulesConsequent(rulesVariables, relation);
            var blockInferenceConsequent = method.Invoke(instance, new object[] { relation2 }) as BlockExpression;
            List<Expression> assignOperations = new List<Expression>();
            assignOperations.AddRange(blockInferenceConsequent.Expressions);

            foreach (var outputTermVariable in outputTermsVariables)
                assignOperations.Add(AreEqual(Expression.Constant(outputTermVariable.Value, typeof(double)), instance.VariablesNames[outputTermVariable.Key]));

            assignOperations.Add(Expression.Constant(0.0d, typeof(double)));
            var parameters = rulesVariables.Select(c => instance.VariablesNames[c.Key]).ToArray();
            var body = Expression.Block(blockInferenceConsequent.Variables, assignOperations);
            var lambda = Expression.Lambda<Func<double, double, double,double,double>>(body, parameters).Compile();
            lambda.Invoke(0.1d, 0.2d,0.15d,0.4d);
        }

        [TestMethod]
        public void TestMethodDefuzzify()
        {
            var instance = FuzzyEngineMock.CreateFuzzyEngine();
            var method = GetMethod(typeof(FuzzyFastEngineFactory), "GenerateDefuzzify");
            var outputsTermsVariables = new Dictionary<string, double>()
            {
                {"output_high".ToLowerInvariant(),0.5},
                {"output_low".ToLowerInvariant(),0.7},
            };

            var outputs = instance.Defuzzify(outputsTermsVariables);
            var blockDefuzzify = method.Invoke(instance, new object[] { }) as BlockExpression;
            List<Expression> assignOperations = new List<Expression>();
            assignOperations.AddRange(blockDefuzzify.Expressions);

            foreach (var output in outputs)
                assignOperations.Add(AreEqual(Expression.Constant(output.Value, typeof(double)), instance.VariablesNames[output.Key]));

            assignOperations.Add(Expression.Constant(0.0d, typeof(double)));
            var parameters = outputsTermsVariables.Select(c => instance.VariablesNames[c.Key]).ToArray();
            var body = Expression.Block(blockDefuzzify.Variables, assignOperations);
            var lambda = Expression.Lambda<Func<double, double, double>>(body, parameters).Compile();
            //lambda.Invoke(0.5, 0.7);
        }
        [TestMethod]
        public void GetInnerTrapezoid()
        {
            var instance = FuzzyEngineMock.CreateFuzzyEngine();
            double previousTermVertexC = 3.0d, tanprevious = 1.0d, previousActivation = 0.45d, currentTermVertexA = 1.0d, tanCurrent = 1.0d, currentActivation = 0.46d, sumAreaName = 0.0d, sumAreaCenterName = 0.0d;
            ConstantExpression previousTermVertexC_C = Expression.Constant(previousTermVertexC),
                tanprevious_c = Expression.Constant(tanprevious),
                currentTermVertexA_c = Expression.Constant(currentTermVertexA),
                tanCurrent_c = Expression.Constant(tanCurrent);

            ParameterExpression sumAreaName_c = Expression.Parameter(typeof(double),"sumAreaName"),
                sumAreaCenterName_c = Expression.Parameter(typeof(double),"sumAreaCenterName"),
                 previousActivation_c = Expression.Parameter(typeof(double),"previousActivation"),
                 currentActivation_c = Expression.Parameter(typeof(double),"currentActivation");
            
            FuzzyEngineFactory.GetInnerTrapezoid(previousTermVertexC, tanprevious, previousActivation, currentTermVertexA, tanCurrent, currentActivation, ref sumAreaName, ref sumAreaCenterName);
            
            var method = GetMethod(typeof(FuzzyFastEngineFactory), "GenerateInnerTrapezoid");
            var blockInnerTrapezoid = method.Invoke(null,new object[] {previousTermVertexC_C,
                tanprevious_c,
                previousActivation_c,
                currentTermVertexA_c,
                tanCurrent_c,
                currentActivation_c,
                sumAreaName_c, 
                sumAreaCenterName_c}) as BlockExpression;
            List<Expression> assignOperations = new List<Expression>(blockInnerTrapezoid.Expressions);
            List<ParameterExpression> variables = new List<ParameterExpression>(blockInnerTrapezoid.Variables);
            assignOperations.Add(AreEqual(Expression.Constant(sumAreaName), sumAreaName_c));
            assignOperations.Add(AreEqual(Expression.Constant(sumAreaCenterName), sumAreaCenterName_c));

            assignOperations.Add(Expression.Constant(0.0d, typeof(double)));
            var parameters = new ParameterExpression[]{
                sumAreaName_c ,
                sumAreaCenterName_c,
                 previousActivation_c,
                 currentActivation_c
            };
            var body = Expression.Block(variables, assignOperations);
            var lambda = Expression.Lambda<Func<double, double, double,double,double>>(body, parameters).Compile();
            lambda.Invoke(0.0d, 0.0d,previousActivation,currentActivation);

            //method.;
            Assert.AreEqual(0.6975d, -sumAreaName);
            Assert.AreEqual(0.6975d * 2.0d, -sumAreaCenterName);
        }

        [TestMethod]
        public void GetInnerTrapezoid2()
        {
            var instance = FuzzyEngineMock.CreateFuzzyEngine();
            double previousTermVertexC = 3.0d, tanprevious = 1.0d, previousActivation = 0.9d, currentTermVertexA = 2.0d, tanCurrent = 1.0d, currentActivation = 0.1d, sumAreaName = 1.18d, sumAreaCenterName = 2.55d;
            ConstantExpression previousTermVertexC_C = Expression.Constant(previousTermVertexC),
                tanprevious_c = Expression.Constant(tanprevious),
                currentTermVertexA_c = Expression.Constant(currentTermVertexA),
                tanCurrent_c = Expression.Constant(tanCurrent);

            ParameterExpression sumAreaName_c = Expression.Parameter(typeof(double), "sumAreaName"),
                sumAreaCenterName_c = Expression.Parameter(typeof(double), "sumAreaCenterName"),
                 previousActivation_c = Expression.Parameter(typeof(double), "previousActivation"),
                 currentActivation_c = Expression.Parameter(typeof(double), "currentActivation");

            FuzzyEngineFactory.GetInnerTrapezoid(previousTermVertexC, tanprevious, previousActivation, currentTermVertexA, tanCurrent, currentActivation, ref sumAreaName, ref sumAreaCenterName);

            var method = GetMethod(typeof(FuzzyFastEngineFactory), "GenerateInnerTrapezoid");
            var blockInnerTrapezoid = method.Invoke(null, new object[] {previousTermVertexC_C,
                tanprevious_c,
                previousActivation_c,
                currentTermVertexA_c,
                tanCurrent_c,
                currentActivation_c,
                sumAreaName_c, 
                sumAreaCenterName_c}) as BlockExpression;
            List<Expression> assignOperations = new List<Expression>(blockInnerTrapezoid.Expressions);
            List<ParameterExpression> variables = new List<ParameterExpression>(blockInnerTrapezoid.Variables);
            assignOperations.Add(AreEqual(Expression.Constant(sumAreaName), sumAreaName_c));
            assignOperations.Add(AreEqual(Expression.Constant(sumAreaCenterName), sumAreaCenterName_c));

            assignOperations.Add(Expression.Constant(0.0d, typeof(double)));
            var parameters = new ParameterExpression[]{
                sumAreaName_c ,
                sumAreaCenterName_c,
                 previousActivation_c,
                 currentActivation_c
            };
            var body = Expression.Block(variables, assignOperations);
            var lambda = Expression.Lambda<Func<double, double, double, double, double>>(body, parameters).Compile();
            lambda.Invoke(1.18d, 2.55d, previousActivation, currentActivation);
        }

        [TestMethod]
        public void TestMethodDefuzzifyInnerTrapezoid()
        {
            var instance = FuzzyEngineMock.CreateFuzzyEngine();
            var method = GetMethod(typeof(FuzzyFastEngineFactory), "GenerateDefuzzify");
            var outputsTermsVariables = new Dictionary<string, double>()
            {
                {"output_high".ToLowerInvariant(),0.1},
                {"output_low".ToLowerInvariant(),0.9},
            };

            var outputs = instance.Defuzzify(outputsTermsVariables, true);
            var blockDefuzzify = method.Invoke(instance, new object[] { }) as BlockExpression;
            List<Expression> assignOperations = new List<Expression>();
            assignOperations.AddRange(blockDefuzzify.Expressions);

            foreach (var output in outputs)
                assignOperations.Add(AreEqual(Expression.Constant(output.Value, typeof(double)), instance.VariablesNames[output.Key]));

            assignOperations.Add(Expression.Constant(0.0d, typeof(double)));
            var parameters = outputsTermsVariables.Select(c => instance.VariablesNames[c.Key]).ToArray();
            var body = Expression.Block(blockDefuzzify.Variables, assignOperations);
            var lambda = Expression.Lambda<Func<double, double, double>>(body, parameters).Compile();
            lambda.Invoke(0.1, 0.9);
        }

        [TestMethod]
        public void TestMethodDefuzzifyTwoInputsTwoOutputs()
        {
            var instance = FuzzyEngineMock.CreateFuzzyEngineTwoInputsTwoOutputs();
            var method = GetMethod(typeof(FuzzyFastEngineFactory), "GenerateDefuzzify");
            var outputsTermsVariables = new Dictionary<string, double>()
            {
                {"output_high".ToLowerInvariant(),0.3},
                {"output_low".ToLowerInvariant(),0.4},
                {"output2_slow".ToLowerInvariant(),0.5},
                {"output2_fast".ToLowerInvariant(),0.6},
            };

            var outputs = instance.Defuzzify(outputsTermsVariables);
            var blockDefuzzify = method.Invoke(instance, new object[] { }) as BlockExpression;
            List<Expression> assignOperations = new List<Expression>();
            assignOperations.AddRange(blockDefuzzify.Expressions);

            foreach (var output in outputs)
                assignOperations.Add(AreEqual(Expression.Constant(output.Value, typeof(double)), instance.VariablesNames[output.Key]));

            assignOperations.Add(Expression.Constant(0.0d, typeof(double)));
            var parameters = outputsTermsVariables.Select(c => instance.VariablesNames[c.Key]).ToArray();
            var body = Expression.Block(blockDefuzzify.Variables, assignOperations);
            var lambda = Expression.Lambda<Func<double, double, double, double,double>>(body, parameters).Compile();
            lambda.Invoke(0.3, 0.4,0.5,0.6);
        }

        [TestMethod]
        public void TestMethodIntegrated()
        {
            var instance = FuzzyEngineMock.CreateFuzzyEngine();
            var lambdaExpression = instance.GenerateFunc<Func<double, double>>();
            var lambda = lambdaExpression.Compile();
            var result = lambda(2.5d);
        }

        [TestMethod]
        public void TestMethodIntegratedTwoInputs()
        {
            var instance = FuzzyEngineMock.CreateFuzzyEngineTwoInputs();
            var lambdaExpression = instance.GenerateFunc<Func<double,double, double>>();
            var lambda = lambdaExpression.Compile();
            var result = lambda(2.5d, 2.4d);
        }

        [TestMethod]
        public void TestMethodIntegratedTwoInputsTwoOutputs()
        {
            Dictionary<string,double> inputs = new Dictionary<string,double>()
            {
                {"INPUT".ToLowerInvariant(),2.5d},
                {"INPUT2".ToLowerInvariant(),2.4d}
            };
            var instance = FuzzyEngineMock.CreateFuzzyEngineTwoInputsTwoOutputs();
            var outputs = instance.Inference(inputs);
            var lambdaExpression = instance.GenerateFunc<Func<double, double, Tuple<double,double>>>();
            var lambda = lambdaExpression.Compile();
            var result = lambda(2.5d, 2.4d);
            //Assert.AreEqual(outputs["output"], result.Item1);
            //Assert.AreEqual(outputs["output2"], result.Item2);
        }
    }
}
