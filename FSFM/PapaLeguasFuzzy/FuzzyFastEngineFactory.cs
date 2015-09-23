using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using LinearAlgebra;
using PapaLeguasFuzzy.Terms;
using System.Reflection;
namespace PapaLeguasFuzzy
{
    public class FuzzyFastEngineFactory : FuzzyEngineFactory
    {
        public FuzzyFastEngineFactory() :base()
        {

        }
        const bool isDebugging = false;
        private Dictionary<string, ParameterExpression> variablesNames = null;
        public Dictionary<string,ParameterExpression> VariablesNames
        {
            get 
            {
                if(variablesNames == null)
                {
                    GenerateVariablesFastEngine();
                }
                return variablesNames;
            }
        }
        private void GenerateVariablesFastEngine()
        {
            variablesNames = new Dictionary<string, ParameterExpression>();
            variablesNames.Add("area", Expression.Parameter(typeof(double), "area"));
            var counter = 0;
            foreach (var rule in Rules)
                variablesNames.Add("rule_" + (counter), Expression.Parameter(typeof(double), "rule_" + (counter++)));
            foreach (var variable in this.InputVariables)
            {
                variablesNames.Add(variable.Name, Expression.Parameter(typeof(double), variable.Name));
                foreach (var term in variable.Terms)
                    variablesNames.Add(term.FullName, Expression.Parameter(typeof(double), term.FullName));
            }

            foreach (var variable in this.OutputVariables)
            {
                variablesNames.Add(variable.Name, Expression.Parameter(typeof(double), variable.Name));
                variablesNames.Add(variable.Name + "_area", Expression.Parameter(typeof(double), variable.Name + "_area"));
                variablesNames.Add(variable.Name + "_areaCenter", Expression.Parameter(typeof(double), variable.Name + "_areaCenter"));
                foreach (var term in variable.Terms)
                    variablesNames.Add(term.FullName, Expression.Parameter(typeof(double), term.FullName));
            }
        }
        public static Tuple<string,string> GetTermAndVariableByFullName(string fullName)
        {
            var variableAndTerm = fullName.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            return new Tuple<string, string>(variableAndTerm[0], variableAndTerm[1]);
        }
        public static Tuple<string, string> GetTermAndVariableByFullName(ParameterExpression fullName)
        {
            return GetTermAndVariableByFullName(fullName);
        }
        public ITerm FindTermByFullName(string FullName, bool inputVariable = true)
        {
            var variableAndTerm = GetTermAndVariableByFullName(FullName);
            return FindTermByFullName(variableAndTerm,inputVariable);
        }

        private ITerm FindTermByFullName(Tuple<string, string> variableAndTerm, bool inputVariable = true)
        {
            var variableName = variableAndTerm.Item1;
            var termName = variableAndTerm.Item2;
            var variables = inputVariable ? this.InputVariables : this.OutputVariables;
            foreach (var variable in variables)
                if (variable.Name == variableName)
                    foreach (var term in variable.Terms)
                        if (term.Name == termName)
                            return term;
            return null;
        }

        public IVariable FindVariableByName(string variableName, bool inputVariable = true)
        {
            var variables = inputVariable ? this.InputVariables : this.OutputVariables;
            foreach (var variable in variables)
                if (variable.Name == variableName)
                    return variable;
            return null;
        }
        public bool ValidateVariableAndTerm(string fullName, out string message, bool inputVariable = true)
        {
            return ValidateVariableAndTerm(GetTermAndVariableByFullName(fullName), out message, inputVariable);
        }
        public bool ValidateVariableAndTerm(Tuple<string, string> variableAndTerm, out string message, bool inputVariable = true)
        {
            var variableName = variableAndTerm.Item1;
            var termName = variableAndTerm.Item2;
            var variable = FindVariableByName(variableName,inputVariable);
            if(variable == null)
            {
                message = "The variable name $variableName could not be found, the search was caseinsensitive";
                return false;
            }
            var term = FindTermByFullName(variableAndTerm,inputVariable);
            if (variable == null)
            {
                message = "The variable name $variableName could be found, but his term $termName could not be found, the search was caseinsensitive";
                return false;
            }
            message = string.Empty;
            return true;
        }
        public bool ValidateEngine(out string message)
        {
            foreach (var rule in this.Rules)
            {
                string[] antecedent;
                string[] consequent;
                ParseRule(rule, out antecedent, out consequent);
                if (!ValidateAntecedent(antecedent, out message))
                    return false;
                return ValidateConsequent(consequent, out message);
            }
            message = string.Empty;
            return true;
        }

        private bool ValidateConsequent(string[] consequent, out string message)
        {
            foreach (var outputVariableFullName in consequent)
                if (!ValidateVariableAndTerm(outputVariableFullName, out message, false))
                    return false;
            foreach (var outputVariableFullName in consequent)
            {
                var term = FindTermByFullName(outputVariableFullName,false) as Triangle;
                if (term == null)
                {
                    message = "Term $outputVariableFullName is not of type Triangle, to use this kind of engine and generate a function which has a significant speed boost, all output Variables must has all terms as Triangles";
                    return false;
                }
                else if(Math.Round(term.ALinearFunc.Slope,6) != Math.Round(-term.CLinearFunc.Slope,6))
                {
                    message = "The B parameter must be between the A and C parameters, at $(term.FullName)";
                    return false;
                }
            }
            message = string.Empty;
            return true;
        }

        private bool ValidateAntecedent(string[] antecendent, out string message)
        {
            if (ValidateVariableAndTerm(antecendent[0], out message))
            {
                for (var indice = 1; indice < antecendent.Length; indice += 2)
                {
                    var operatorLogic = antecendent[indice];
                    if ((operatorLogic != "and") && (operatorLogic != "or"))
                    {
                        message = "The only logic operator acceptable are \"and\" and \"or\"";
                        return false;
                    }
                    else if (!ValidateVariableAndTerm(antecendent[0], out message))
                    {
                        return false;
                    }
                }
                message = string.Empty;
                return true;
            }
            else
            {
                return false;
            }
        }
        public new Dictionary<ParameterExpression, List<ParameterExpression>> GetRulesRelatedToOutputVariableTerm()
        {
            var relations = base.GetRulesRelatedToOutputVariableTerm();
            var ret = new Dictionary<ParameterExpression, List<ParameterExpression>>();
            foreach(var relation in relations)
            {
                var rules = new List<ParameterExpression>();
                ret.Add(VariablesNames[relation.Key], rules);
                foreach(var ruleName in relation.Value)
                    rules.Add(VariablesNames[ruleName]);
            }

            return ret;
        }
        public Expression<T> GenerateFunc<T>()
        {
            string message;
            if (!ValidateEngine(out message))
                throw new Exception(message);

            var variables = new List<ParameterExpression>();
            var assignOperations = new List<Expression>();            
            var blockFuzzify = GenerateFuzzify();
            var blockInferenceAntecedent = GenerateInferenceRulesAntecedent();
            var blockConsequent = GenerateInferenceConsequent(GetRulesRelatedToOutputVariableTerm());
            var blockDefuzzify = GenerateDefuzzify();

            assignOperations.AddRange(blockFuzzify.Expressions);
            assignOperations.AddRange(blockInferenceAntecedent.Expressions);
            assignOperations.AddRange(blockConsequent.Expressions);
            assignOperations.AddRange(blockDefuzzify.Expressions);

            variables.AddRange(blockFuzzify.Variables);
            variables.AddRange(blockInferenceAntecedent.Variables);
            variables.AddRange(blockConsequent.Variables);
            variables.AddRange(blockDefuzzify.Variables);
            if (this.OutputVariables.Count > 1)
            {
                var doubles = this.OutputVariables.Select(c => typeof(double)).ToArray();
                var parametersToCreateTuple = this.OutputVariables.Select(c => VariablesNames[c.Name]).ToArray();
                var methodBase = typeof(Tuple).GetMethods().Where(method => method.IsGenericMethod && method.Name == "Create").Skip(doubles.Count() - 1).First();
                var createTupleMethod = methodBase.MakeGenericMethod(doubles);
                //line of return...
                assignOperations.Add(Expression.Call(createTupleMethod, parametersToCreateTuple));
            }
            else
                assignOperations.Add(variablesNames[this.OutputVariables[0].Name]);

            var correctType = GetTypeExpression();
            if (correctType != typeof(T))
                throw new Exception("The right type is $correctType");

            var bodyFunction = Expression.Block(variables, assignOperations);
            return Expression.Lambda<T>(bodyFunction, this.InputVariables.Select(c => VariablesNames[c.Name]));

        }

        private BlockExpression GenerateInferenceConsequent(Dictionary<ParameterExpression, List<ParameterExpression>> relationRulesOutputTerms)
        {
            var maximum = new PapaLeguasFuzzy.Norms.SNorms.Maximum();
            List<Expression> assignOperations = new List<Expression>();
            List<ParameterExpression> variables = new List<ParameterExpression>();
            foreach (var outputVariable in this.OutputVariables)
            {
                var rulesRelatedToOutputVariable = new List<ParameterExpression>();
                foreach (var term in outputVariable.Terms)
                {
                    variables.Add(VariablesNames[term.FullName]);
                    if (relationRulesOutputTerms.TryGetValue(VariablesNames[term.FullName], out rulesRelatedToOutputVariable))
                    {
                        assignOperations.Add(Expression.Assign(VariablesNames[term.FullName], rulesRelatedToOutputVariable[0]));
                        for (var indexResultant = 1; indexResultant < rulesRelatedToOutputVariable.Count; indexResultant++)
                            assignOperations.Add(maximum.GeneratePertinencyCode(VariablesNames[term.FullName], rulesRelatedToOutputVariable[indexResultant],null,false, false));  
                    }
                    else
                        assignOperations.Add(Expression.Assign(VariablesNames[term.FullName], Expression.Constant(0.0d, typeof(double))));
                }
            }
            return Expression.Block(
                       typeof(double),
                       variables,
                       assignOperations
                   );
        }

        private BlockExpression GenerateDefuzzify()
        {
            List<Expression> assignOperations = new List<Expression>();            
            var variables = new List<ParameterExpression>();
            variables.Add(VariablesNames["area"]);
            foreach (var outputVariable in this.OutputVariables)
            {
                var termsToConsider = (from term in outputVariable.Terms.Cast<Triangle>()
                                       where term != null
                                       orderby term.A
                                       select term).ToList<Triangle>();
                
                if (termsToConsider.Count > 0)
                {
                    ParameterExpression variableAreaCenter = VariablesNames[outputVariable.Name + "_areaCenter"],
                        variableArea = VariablesNames[outputVariable.Name + "_area"],
                        area = VariablesNames["area"];

                    var currentTerm = termsToConsider[0];
                    
                    ConstantExpression range = Expression.Constant(currentTerm.C - currentTerm.A, typeof(double)),
                        slope = Expression.Constant(currentTerm.ALinearFunc.Slope, typeof(double)),
                        centerOfGravity = Expression.Constant(currentTerm.CenterOfGravity, typeof(double));
                    var loadArea = Expression.Assign(area, Expression.Multiply(Expression.Subtract(range, Expression.Multiply(slope, VariablesNames[currentTerm.FullName])), VariablesNames[currentTerm.FullName]));
                    assignOperations.Add(loadArea);
                    var assignVariableAreaCenter = Expression.AddAssign(variableAreaCenter, Expression.Multiply(area, centerOfGravity));
                    var assignVariableArea = Expression.AddAssign(variableArea, area);
                    assignOperations.Add(assignVariableAreaCenter);
                    assignOperations.Add(assignVariableArea);

                    for (var indice = 1; indice < termsToConsider.Count; indice++)
                    {
                        var previousTerm = currentTerm;
                        currentTerm = termsToConsider[indice];
                        area = VariablesNames["area"];
                        range = Expression.Constant(currentTerm.C -currentTerm.A, typeof(double));
                        slope = Expression.Constant(currentTerm.ALinearFunc.Slope,typeof(double));
                        centerOfGravity = Expression.Constant(currentTerm.CenterOfGravity, typeof(double));
                        loadArea = Expression.Assign(area, Expression.Multiply(Expression.Subtract(range, Expression.Multiply(slope, VariablesNames[currentTerm.FullName])),VariablesNames[currentTerm.FullName]));
                        assignOperations.Add(loadArea);
                        assignVariableAreaCenter = Expression.AddAssign(variableAreaCenter, Expression.Multiply(area, centerOfGravity));
                        assignVariableArea = Expression.AddAssign(variableArea, area);
                        assignOperations.Add(assignVariableAreaCenter);
                        assignOperations.Add(assignVariableArea);
                        Point? p = LinearFunc.getEncounterPoint(previousTerm.CLinearFunc, currentTerm.ALinearFunc);
                        if ((p.HasValue) && (p.Value.Y > 0.0d) && (p.Value.Y < 1.0d))
                        {
                            ConstantExpression previousTermVertexC = Expression.Constant(previousTerm.C, typeof(double)),
                                previousSlopeCNegative = Expression.Constant(-previousTerm.CLinearFunc.Slope, typeof(double)),
                                currentTermVertexA = Expression.Constant(currentTerm.A, typeof(double)),
                                currentSlopeA = Expression.Constant(currentTerm.ALinearFunc.Slope, typeof(double));
                            var getInnerTrapezoid = GenerateInnerTrapezoid(previousTermVertexC, previousSlopeCNegative, VariablesNames[previousTerm.FullName], currentTermVertexA, currentSlopeA, VariablesNames[currentTerm.FullName], variableArea, variableAreaCenter);
                            var test = Expression.LessThanOrEqual(Expression.Constant(p.Value.Y, typeof(double)), MathMethods.CreateMin(VariablesNames[previousTerm.FullName], VariablesNames[currentTerm.FullName]));
                            var areaInnerTriangle = (previousTerm.C - currentTerm.A) * p.Value.Y * 0.5d;
                            var areaInnerTriangleCenter = (currentTerm.A + previousTerm.C + p.Value.X) * 0.3333333333333333333d * areaInnerTriangle;
                            var blockTakeInnerTriangle = Expression.Block(Expression.SubtractAssign(variableAreaCenter, Expression.Constant(areaInnerTriangleCenter, typeof(double))),
                                Expression.SubtractAssign(variableArea, Expression.Constant(areaInnerTriangle, typeof(double))));

                            assignOperations.Add(Expression.IfThenElse(test, blockTakeInnerTriangle, getInnerTrapezoid));
                            //assignOperations.Add(Expression.IfThen(test, blockTakeInnerTriangle));
                        }
                    }
                    assignOperations.Add(Expression.Assign(VariablesNames[outputVariable.Name], Expression.Divide(variableAreaCenter, variableArea)));
                    variables.AddRange(new ParameterExpression[] { 
                               variableAreaCenter, 
                               variableArea,
                               VariablesNames[outputVariable.Name]
                });
                }
            }
            return Expression.Block(
                       typeof(double),
                       variables,
                       assignOperations
                   );
        }

        private BlockExpression GenerateInferenceRulesAntecedent()
        {
            int counter = 0;
            var inferenceRule = new List<Expression>();
            List<Expression> assignOperations = new List<Expression>();
            List<ParameterExpression> variables = new List<ParameterExpression>();
            foreach (var rule in this.Rules)
            {
                ParameterExpression ruleName = VariablesNames["rule_" + counter++];
                variables.Add(ruleName);
                string[] antecedent;
                string[] consequent;
                ParseRule(rule, out antecedent, out consequent);
                var term = FindTermByFullName(antecedent[0]);
                
                assignOperations.Add(Expression.Assign(ruleName, VariablesNames[term.FullName]));
                for (var indiceConditions = 1; indiceConditions < antecedent.Length; indiceConditions += 2)
                    assignOperations.Add(
                        ((antecedent[indiceConditions] == "and") ? this.Conjunction as INorm : this.Disjunction)
                        .GeneratePertinencyCode(ruleName, VariablesNames[FindTermByFullName(antecedent[indiceConditions+1]).FullName], null, false, false));
            }
            return Expression.Block(
                    typeof(double),
                    variables,
                    assignOperations
                );
        }

        private BlockExpression GenerateFuzzify()
        {
            var variables = new List<ParameterExpression>();
            var assignOperations = new List<Expression>();
            foreach (var variable in this.InputVariables)
                GenerateFuzzifyVariable(variables, assignOperations, variable);

            return Expression.Block(
                    typeof(double),
                    variables,
                    assignOperations
                );

        }

        private void GenerateFuzzifyVariable(List<ParameterExpression> variables, List<Expression> assignOperations, Variable variable)
        {
            
            foreach (var term in variable.Terms)
            {
                var termVariableName = VariablesNames[term.FullName];
                variables.Add(termVariableName);
                assignOperations.Add(term.GeneratePertinencyCode(VariablesNames[variable.Name], termVariableName, false, false));
            }
        }
        private static MethodInfo writeline = typeof(FuzzyFastEngineFactory).GetMethod("WriteLineeee",BindingFlags.Static | BindingFlags.NonPublic);
        private static void WriteLineeee(string variable, System.Double value)
        {
            System.Diagnostics.Debug.WriteLine(variable + ":" + value);
        }
        private static MethodCallExpression WriteLine(ParameterExpression variable)
        {
            return Expression.Call(writeline,Expression.Constant(variable.Name), variable);
        }
        private static Expression GenerateInnerTrapezoid(ConstantExpression previousTermVertexC, ConstantExpression previousSlopeCNegative, ParameterExpression previousActivation, ConstantExpression currentTermVertexA, ConstantExpression currentSlopeA, ParameterExpression currentActivation, ParameterExpression sumAreaName, ParameterExpression sumAreaCenterName)
        {
            var activationMin = Expression.Parameter(typeof(double),"activationMin");
            var baseTriangleLast = Expression.Parameter(typeof(double), "baseTriangleLast");
            var baseTriangleCurrent = Expression.Parameter(typeof(double), "baseTriangleCurrent");
            var centerOfMassLast = Expression.Parameter(typeof(double), "centerOfMassLast");
            var centerOfMassCurrent = Expression.Parameter(typeof(double), "centerOfMassCurrent");
            var areaTriangleLast = Expression.Parameter(typeof(double), "areaTriangleLast");
            var areaTriangleCurrent = Expression.Parameter(typeof(double), "areaTriangleCurrent");
            var innerSquareBase = Expression.Parameter(typeof(double), "innerSquareBase");
            var innerSquareArea = Expression.Parameter(typeof(double), "innerSquareArea");
            var centerOfMassSquare = Expression.Parameter(typeof(double), "centerOfMassSquare");
            
            //var sumAreaTrapezoid = Expression.Parameter(typeof(double), "sumAreaTrapezoid");
            //var sumAreaMultipliedByMidTermTrapezoid = Expression.Parameter(typeof(double), "sumAreaMultipliedByMidTermTrapezoid");
            //var centerOfMassTrapezoid = Expression.Parameter(typeof(double), "centerOfMassTrapezoid");
            var constant0dot5 = Expression.Constant(0.5d, typeof(double));
            var constant0dot666 = Expression.Constant(0.666666666666666666666d, typeof(double));
            ParameterExpression[] declareVariables = new ParameterExpression[] {
                activationMin,
                baseTriangleLast,
                baseTriangleCurrent,
                centerOfMassLast,
                centerOfMassCurrent,
                areaTriangleLast,
                areaTriangleCurrent,
                innerSquareBase,
                innerSquareArea,
                centerOfMassSquare,
                //sumAreaTrapezoid,
                //sumAreaMultipliedByMidTermTrapezoid,
                //centerOfMassTrapezoid
            };
            Expression[] assignLines = (isDebugging)? new Expression[] {
                //    nameStr = string(sumAreaName)
                //(quote
                //############################
                //############****############
                //###########******###########
                //##########********##########
                //#*******&&&&*******#########
                //#############################
                //#second case is when the  triangle that occupy both triangules is sliced forming a trapezoid so I have to calculate
                //#his area and centroid.
                //activationMin = min($lastActivation,$currentActivation)
                Expression.Assign(activationMin,MathMethods.CreateMin(previousActivation,currentActivation)),
                WriteLine(activationMin),
                //#activationMinSquared = activationMin * activationMin * 0.5

                //baseTriangleLast = $tanLast * activationMin
                Expression.Assign(baseTriangleLast,Expression.Multiply(previousSlopeCNegative,activationMin)),
                WriteLine(baseTriangleLast),
                //baseTriangleCurrent = $tanCurrent * activationMin
                Expression.Assign(baseTriangleCurrent,Expression.Multiply(currentSlopeA,activationMin)),
                WriteLine(baseTriangleCurrent),
                //centerOfMassLast = $lastTermVertexC - baseTriangleLast*0.666666666666666
                Expression.Assign(centerOfMassLast,Expression.Subtract(previousTermVertexC,Expression.Multiply(baseTriangleLast, constant0dot666))),
                WriteLine(centerOfMassLast),
                //centerOfMassCurrent = $currentTermVertexA + baseTriangleCurrent*0.666666666666666
                Expression.Assign(centerOfMassCurrent,Expression.Add(currentTermVertexA,Expression.Multiply(baseTriangleCurrent, constant0dot666))),
                WriteLine(centerOfMassCurrent),

                //areaTriangleLast = baseTriangleLast * activationMin *0.5
                Expression.Assign(areaTriangleLast,Expression.Multiply(baseTriangleLast,Expression.Multiply(activationMin, constant0dot5))),
                WriteLine(areaTriangleLast),
                //areaTriangleCurrent = baseTriangleCurrent * activationMin *0.5
                Expression.Assign(areaTriangleCurrent,Expression.Multiply(baseTriangleCurrent,Expression.Multiply(activationMin, constant0dot5))),
                WriteLine(areaTriangleCurrent),

                //innerSquareBase = $lastTermVertexC - $currentTermVertexA - baseTriangleLast - baseTriangleCurrent
                Expression.Assign(innerSquareBase,Expression.Subtract(previousTermVertexC,Expression.Add(currentTermVertexA,Expression.Add(baseTriangleLast,baseTriangleCurrent)))),
                WriteLine(innerSquareBase),
                //innerSquareArea = innerSquareBase * activationMin
                Expression.Assign(innerSquareArea,Expression.Multiply(innerSquareBase,activationMin)),
                WriteLine(innerSquareArea),
                //centerOfMassSquare = $currentTermVertexA+baseTriangleCurrent + innerSquareBase*0.5
                Expression.Assign(centerOfMassSquare,Expression.Add(currentTermVertexA,Expression.Add(baseTriangleCurrent,Expression.Multiply(innerSquareBase,constant0dot5)))),
                WriteLine(centerOfMassSquare),

                //#sumAreaTrapezoid = innerSquareArea
                //#sumAreaTrapezoid = sumAreaTrapezoid + areaTriangleLast
                //#sumAreaTrapezoid = sumAreaTrapezoid + areaTriangleCurrent
                //Expression.Assign(sumAreaTrapezoid,Expression.Add(innerSquareArea,Expression.Add(areaTriangleLast,areaTriangleCurrent))),
                //#sumAreaMultipliedByMidTermTrapezoid = innerSquareArea*centerOfMassSquare
                //#sumAreaMultipliedByMidTermTrapezoid = sumAreaMultipliedByMidTermTrapezoid + areaTriangleLast*centerOfMassLast
                //#sumAreaMultipliedByMidTermTrapezoid = sumAreaMultipliedByMidTermTrapezoid + areaTriangleCurrent*centerOfMassCurrent
               // Expression.Assign(sumAreaMultipliedByMidTermTrapezoid,Expression.Add(Expression.Multiply(innerSquareArea,centerOfMassSquare),Expression.Add(Expression.Multiply(areaTriangleLast,centerOfMassLast),Expression.Multiply(areaTriangleCurrent,centerOfMassCurrent)))),
                //#centerOfMassTrapezoid = sumAreaMultipliedByMidTermTrapezoid / sumArea
                //Expression.Assign(centerOfMassTrapezoid,Expression.Divide(sumAreaMultipliedByMidTermTrapezoid,sumAreaTrapezoid)),
                //#print($nameStr)
                //#print("=")
                //#print(innerSquareArea*centerOfMassSquare - areaTriangleLast*centerOfMassLast - areaTriangleCurrent*centerOfMassCurrent)
                //#print("\n")
                //$sumAreaName = $sumAreaName - innerSquareArea - areaTriangleLast - areaTriangleCurrent
                Expression.SubtractAssign(sumAreaName,Expression.Add(innerSquareArea,Expression.Add(areaTriangleLast,areaTriangleCurrent))),
                WriteLine(sumAreaName),
                //$sumAreaCenterName = $sumAreaCenterName - innerSquareArea*centerOfMassSquare - areaTriangleLast*centerOfMassLast - areaTriangleCurrent*centerOfMassCurrent
                Expression.SubtractAssign(sumAreaCenterName,Expression.Add(Expression.Multiply(innerSquareArea,centerOfMassSquare),Expression.Add(Expression.Multiply(areaTriangleLast,centerOfMassLast),Expression.Multiply(areaTriangleCurrent,centerOfMassCurrent)))),
                WriteLine(sumAreaCenterName),
                //end).args
            }
            :
            new Expression[] {
                //    nameStr = string(sumAreaName)
                //(quote
                //############################
                //############****############
                //###########******###########
                //##########********##########
                //#*******&&&&*******#########
                //#############################
                //#second case is when the  triangle that occupy both triangules is sliced forming a trapezoid so I have to calculate
                //#his area and centroid.
                //activationMin = min($lastActivation,$currentActivation)
                Expression.Assign(activationMin,MathMethods.CreateMin(previousActivation,currentActivation)),

                //#activationMinSquared = activationMin * activationMin * 0.5

                //baseTriangleLast = $tanLast * activationMin
                Expression.Assign(baseTriangleLast,Expression.Multiply(previousSlopeCNegative,activationMin)),
                //baseTriangleCurrent = $tanCurrent * activationMin
                Expression.Assign(baseTriangleCurrent,Expression.Multiply(currentSlopeA,activationMin)),
                //centerOfMassLast = $lastTermVertexC - baseTriangleLast*0.666666666666666
                Expression.Assign(centerOfMassLast,Expression.Subtract(previousTermVertexC,Expression.Multiply(baseTriangleLast, constant0dot666))),
                //centerOfMassCurrent = $currentTermVertexA + baseTriangleCurrent*0.666666666666666
                Expression.Assign(centerOfMassCurrent,Expression.Add(currentTermVertexA,Expression.Multiply(baseTriangleCurrent, constant0dot666))),

                //areaTriangleLast = baseTriangleLast * activationMin *0.5
                Expression.Assign(areaTriangleLast,Expression.Multiply(baseTriangleLast,Expression.Multiply(activationMin, constant0dot5))),
                //areaTriangleCurrent = baseTriangleCurrent * activationMin *0.5
                Expression.Assign(areaTriangleCurrent,Expression.Multiply(baseTriangleCurrent,Expression.Multiply(activationMin, constant0dot5))),

                //innerSquareBase = $lastTermVertexC - $currentTermVertexA - baseTriangleLast - baseTriangleCurrent
                Expression.Assign(innerSquareBase,Expression.Subtract(previousTermVertexC,Expression.Add(currentTermVertexA,Expression.Add(baseTriangleLast,baseTriangleCurrent)))),
                //innerSquareArea = innerSquareBase * activationMin
                Expression.Assign(innerSquareArea,Expression.Multiply(innerSquareBase,activationMin)),
                //centerOfMassSquare = $currentTermVertexA+baseTriangleCurrent + innerSquareBase*0.5
                Expression.Assign(centerOfMassSquare,Expression.Add(currentTermVertexA,Expression.Add(baseTriangleCurrent,Expression.Multiply(innerSquareBase,constant0dot5)))),

                //#sumAreaTrapezoid = innerSquareArea
                //#sumAreaTrapezoid = sumAreaTrapezoid + areaTriangleLast
                //#sumAreaTrapezoid = sumAreaTrapezoid + areaTriangleCurrent
                //Expression.Assign(sumAreaTrapezoid,Expression.Add(innerSquareArea,Expression.Add(areaTriangleLast,areaTriangleCurrent))),
                //#sumAreaMultipliedByMidTermTrapezoid = innerSquareArea*centerOfMassSquare
                //#sumAreaMultipliedByMidTermTrapezoid = sumAreaMultipliedByMidTermTrapezoid + areaTriangleLast*centerOfMassLast
                //#sumAreaMultipliedByMidTermTrapezoid = sumAreaMultipliedByMidTermTrapezoid + areaTriangleCurrent*centerOfMassCurrent
               // Expression.Assign(sumAreaMultipliedByMidTermTrapezoid,Expression.Add(Expression.Multiply(innerSquareArea,centerOfMassSquare),Expression.Add(Expression.Multiply(areaTriangleLast,centerOfMassLast),Expression.Multiply(areaTriangleCurrent,centerOfMassCurrent)))),
                //#centerOfMassTrapezoid = sumAreaMultipliedByMidTermTrapezoid / sumArea
                //Expression.Assign(centerOfMassTrapezoid,Expression.Divide(sumAreaMultipliedByMidTermTrapezoid,sumAreaTrapezoid)),
                //#print($nameStr)
                //#print("=")
                //#print(innerSquareArea*centerOfMassSquare - areaTriangleLast*centerOfMassLast - areaTriangleCurrent*centerOfMassCurrent)
                //#print("\n")
                //$sumAreaName = $sumAreaName - innerSquareArea - areaTriangleLast - areaTriangleCurrent
                Expression.SubtractAssign(sumAreaName,Expression.Add(innerSquareArea,Expression.Add(areaTriangleLast,areaTriangleCurrent))),
                //$sumAreaCenterName = $sumAreaCenterName - innerSquareArea*centerOfMassSquare - areaTriangleLast*centerOfMassLast - areaTriangleCurrent*centerOfMassCurrent
                Expression.SubtractAssign(sumAreaCenterName,Expression.Add(Expression.Multiply(innerSquareArea,centerOfMassSquare),Expression.Add(Expression.Multiply(areaTriangleLast,centerOfMassLast),Expression.Multiply(areaTriangleCurrent,centerOfMassCurrent)))),
                //end).args
            };
            
            return Expression.Block(declareVariables,assignLines);
        }
        private Type GetTypeExpression()
        {
            var typeExpression = this.InputVariables.Select(c => typeof(double)).ToList();
            typeExpression.Add(GetTypeReturnExpression());
            return GetFuncTypeByNumberArguments(typeExpression.Count).MakeGenericType(typeExpression.ToArray());
        }
        private static Type GetFuncTypeByNumberArguments(int n)
        {
            switch(n)
            {
                case 1:
                    return typeof(Func<>);
                case 2:
                    return typeof(Func<,>);
                case 3:
                    return typeof(Func<,,>);
                case 4:
                    return typeof(Func<,,,>);
                case 5:
                    return typeof(Func<,,,,>);
                case 6:
                    return typeof(Func<,,,,,>);
                case 7:
                    return typeof(Func<,,,,,,>);
                case 8:
                    return typeof(Func<,,,,,,,>);
                case 9:
                    return typeof(Func<,,,,,,,,>);
                case 10:
                    return typeof(Func<,,,,,,,,,>);
                case 11:
                    return typeof(Func<,,,,,,,,,,>);
                case 12:
                    return typeof(Func<,,,,,,,,,,,>);
                case 13:
                    return typeof(Func<,,,,,,,,,,,,>);
                case 14:
                    return typeof(Func<,,,,,,,,,,,,,>);
                case 15:
                    return typeof(Func<,,,,,,,,,,,,,,>);
                case 16:
                    return typeof(Func<,,,,,,,,,,,,,,,>);
                case 17:
                    return typeof(Func<,,,,,,,,,,,,,,,,>);
                default:
                    return typeof(Func<>);
            }
        }
        private Type GetTypeReturnExpression()
        {
            
            switch (this.OutputVariables.Count)
            {
                case 1:
                    return typeof(double);
                case 2:
                    return typeof(Tuple<double, double>);
                case 3:
                    return typeof(Tuple<double, double, double>);
                case 4:
                    return typeof(Tuple<double, double, double, double>);
                case 5:
                    return typeof(Tuple<double, double, double, double, double>);
                case 6:
                    return typeof(Tuple<double, double, double, double, double, double>);
                case 7:
                    return typeof(Tuple<double, double, double, double, double, double, double>);
                case 8:
                    return typeof(Tuple<double, double, double, double, double, double, double, double>);
                default:
                    throw new Exception("Number of returns excedeed.");
            }
        }
    }
}
