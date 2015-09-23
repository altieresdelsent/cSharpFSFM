using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PapaLeguasFuzzy.Norms;
using System.Text.RegularExpressions;
using PapaLeguasFuzzy.Terms;
using LinearAlgebra;

namespace PapaLeguasFuzzy
{
    public class FuzzyEngineFactory
    {
        public FuzzyEngineFactory()
        {
            this.Accumulation = new PapaLeguasFuzzy.Norms.SNorms.Maximum();
            this.Activation = new PapaLeguasFuzzy.Norms.TNorms.Minimum();
            this.Disjunction = new PapaLeguasFuzzy.Norms.SNorms.ProbabilisticSum();
            this.Conjunction = new PapaLeguasFuzzy.Norms.TNorms.Product();
            this.Rules = new List<string>();
            this.OutputVariables = new List<Variable>();
            this.InputVariables = new List<Variable>();
        }
        public List<string> Rules {get; set;}
        public List<Variable> InputVariables { get; set; }
        public List<Variable> OutputVariables { get; set; }
        public TNorm Conjunction { get; set; }
        public SNorm Disjunction { get; set; }
        public TNorm Activation { get; set; }
        public SNorm Accumulation { get; set; }
        
        public static string RemoveDoubleEspaces(string str)
        {
            Regex reg = new Regex(@"\s+");
            return reg.Replace(str, " ");
        }

        public static void ParseRule(string rule, out string[] antecedents, out string[] consequents)
        {

            var ruleLower = RemoveDoubleEspaces(rule).ToLowerInvariant();
            var ruleParts = ruleLower.Split(new string[] { " then ", "if ", "if " }, StringSplitOptions.RemoveEmptyEntries);
            if (ruleParts.Length == 2)
            {
                var antecendent = ruleParts[0];
                var consequent = ruleParts[1];
                var antecendentParseForm = antecendent.Replace(" is ", "_");
                antecedents = antecendentParseForm.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                consequents = consequent.Replace(" is ", "_").Split(new string[] { " and ", " or ", ",", " " }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                antecedents = new string[0];
                consequents = new string[0];
            }            
        }
        public Dictionary<string,double> Inference(Dictionary<string,double> inputs)
        {
            var termsInputs = this.Fuzzify(inputs);
            var rulesValues = this.InferenceRulesAntecedent(termsInputs);
            var termsOutputVariable = this.InferenceRulesConsequent(rulesValues, this.GetRulesRelatedToOutputVariableTerm());
            return this.Defuzzify(termsOutputVariable);
        }
        public Dictionary<string, List<string>> GetRulesRelatedToOutputVariableTerm()
        {
            var outputVariablesRules = new Dictionary<string, List<string>>();
            int counter = 0;
            foreach (var rule in Rules)
            {
                string[] antecendents;
                string[] consequents;
                var currentRule = string.Concat("rule_", counter);
                ParseRule(rule, out antecendents, out consequents);
                List<string> rulesRelatedToVariable;
                foreach (var consequent in consequents)
                    if (outputVariablesRules.TryGetValue(consequent, out rulesRelatedToVariable))
                        rulesRelatedToVariable.Add(currentRule);
                    else
                        outputVariablesRules.Add(consequent, new List<string>(new string[] { currentRule }));
                counter++;
            }
            return outputVariablesRules;
        }
        public Dictionary<string, double> Fuzzify(Dictionary<string, double> inputVariables)
        {
            var ret = new Dictionary<string, double>();
            foreach (var inputVariable in this.InputVariables)
                foreach (var term in inputVariable.Terms)
                    ret.Add(term.FullName, term.Pertinency(inputVariables[inputVariable.Name]));
            return ret;
        }
        public Dictionary<string,double> InferenceRulesAntecedent(Dictionary<string,double> inputTermsVariables)
        {
            var ret = new Dictionary<string, double>();
            var outputVariablesRules = new Dictionary<string, List<string>>();
            int counter = 0;
            foreach (var rule in Rules)
            {
                string[] antecendents;
                string[] consequents;
                var currentRule = string.Concat("rule_",counter);
                ParseRule(rule, out antecendents, out consequents);
                if(antecendents.Length > 0)
                {
                    ret.Add(currentRule,inputTermsVariables[antecendents[0]]);
                    for(var indice = 1; indice < antecendents.Length; indice+=2)
                        ret[currentRule] = ((antecendents[indice] == "and") ? this.Conjunction as INorm : this.Disjunction as INorm)
                            .Pertinency(ret[currentRule],inputTermsVariables[antecendents[indice+1]]);
                }
                counter++;
            }
            return ret;

        }
        public Dictionary<string,double> InferenceRulesConsequent(Dictionary<string,double> rules, Dictionary<string,List<string>> rulesRelatedToVariableTerm)
        {
            var ret = new Dictionary<string, double>();
            foreach(var variable in this.OutputVariables)
            {
                foreach(var term in variable.Terms)
                {
                    var rulesRelated = rulesRelatedToVariableTerm[term.FullName];
                    ret.Add(term.FullName, rules[rulesRelated[0]]);
                    for (var indice = 1; indice < rulesRelated.Count; indice++)
                        ret[term.FullName] = Math.Max(ret[term.FullName], rules[rulesRelated[indice]]);
                }
            }
            return ret;
        }
        public Dictionary<string, double> Defuzzify(Dictionary<string, double> termsOutputVariable, bool debug = false)
        {
            var ret = new Dictionary<string, double>();
            foreach (var variable in this.OutputVariables)
            {
                var areaCenter = 0.0d;
                var area = 0.0d;
                var terms  = variable.Terms.Cast<Triangle>().OrderBy( c => c.A).ToList();
                
                foreach (var term in terms)
                {
                    var areaTerm = (term.C -term.A - (termsOutputVariable[term.FullName]*term.ALinearFunc.Slope))* termsOutputVariable[term.FullName];
                    area += areaTerm;
                    areaCenter += term.CenterOfGravity * areaTerm;
                }
                var previousTerm = terms[0];
                
                for(var indice = 1; indice < terms.Count; indice++)
                {
                    var currentTerm = terms[indice];
                    var pertinencyCurrent = termsOutputVariable[currentTerm.FullName];
                    var pertinencyPrevious = termsOutputVariable[previousTerm.FullName];
                    if ((pertinencyCurrent > 0.0d) && (pertinencyPrevious > 0.0d))
                    {
                        Point? p = LinearFunc.getEncounterPoint(previousTerm.CLinearFunc, currentTerm.ALinearFunc);
                        if ((p.HasValue)&&(p.Value.Y < 1.0d)&&(p.Value.Y > 0.0d)&& (p.Value.Y < Math.Min(pertinencyCurrent, pertinencyPrevious)))
                        {
                            var areaTriangle = ((previousTerm.C - currentTerm.A) * 0.5 * p.Value.Y);
                            area += areaTriangle;
                            areaCenter += ((currentTerm.A + previousTerm.C + p.Value.X) / 3.0d) * areaTriangle;
                        }
                        else
                        {
                            GetInnerTrapezoid(previousTerm.C, -previousTerm.CLinearFunc.Slope, pertinencyPrevious, currentTerm.A, currentTerm.ALinearFunc.Slope, pertinencyCurrent, ref area, ref areaCenter);
                        }
                    }
                    previousTerm = currentTerm;
                }
                if (debug)
                {
                    ret.Add(variable.Name + "_areaCenter", areaCenter);
                    ret.Add(variable.Name + "_area", area);
                }
                ret.Add(variable.Name, areaCenter / area);
                
            }

            return ret;
        }
        public static void GetInnerTrapezoid(double previousTermVertexC,double tanprevious,double  previousActivation,double  currentTermVertexA,double  tanCurrent,double currentActivation,ref double sumAreaName,ref double sumAreaCenterName)
        {
            /////////////////////////////
            ////////////****////////////
            ///////////******///////////
            //////////********//////////
            /*******&&&&*******/////////
            /////////////////////////////
            //second case is when the  triangle that occupy both triangules is sliced forming a trapezoid so I have to calculate
            //his area and centroid.
            double activationMin = Math.Min(previousActivation,currentActivation);
            //activationMinSquared = activationMin * activationMin * 0.5

            double baseTriangleprevious = tanprevious * activationMin;
            double baseTriangleCurrent = tanCurrent * activationMin;

            double centerOfMassPrevious = previousTermVertexC - baseTriangleprevious*0.666666666666666;
            double centerOfMassCurrent = currentTermVertexA + baseTriangleCurrent*0.666666666666666;

            double areaTrianglePrevious = baseTriangleprevious * activationMin *0.5;
            double areaTriangleCurrent = baseTriangleCurrent * activationMin *0.5;

            double innerSquareBase = previousTermVertexC - (currentTermVertexA + baseTriangleprevious + baseTriangleCurrent);
            double innerSquareArea = innerSquareBase * activationMin;
            double centerOfMassSquare = currentTermVertexA+baseTriangleCurrent + innerSquareBase*0.5;

            //sumAreaTrapezoid = innerSquareArea
            //sumAreaTrapezoid = sumAreaTrapezoid + areaTriangleLast
            //sumAreaTrapezoid = sumAreaTrapezoid + areaTriangleCurrent

            //sumAreaMultipliedByMidTermTrapezoid = innerSquareArea*centerOfMassSquare
            //sumAreaMultipliedByMidTermTrapezoid = sumAreaMultipliedByMidTermTrapezoid + areaTriangleLast*centerOfMassLast
            //sumAreaMultipliedByMidTermTrapezoid = sumAreaMultipliedByMidTermTrapezoid + areaTriangleCurrent*centerOfMassCurrent
            //centerOfMassTrapezoid = sumAreaMultipliedByMidTermTrapezoid / sumArea
            //print(nameStr)
            //print("=")
            //print(innerSquareArea*centerOfMassSquare - areaTriangleLast*centerOfMassLast - areaTriangleCurrent*centerOfMassCurrent)
            //print("\n")
            sumAreaName = sumAreaName - (innerSquareArea + areaTrianglePrevious + areaTriangleCurrent);
            sumAreaCenterName = sumAreaCenterName - (innerSquareArea * centerOfMassSquare + areaTrianglePrevious * centerOfMassPrevious + areaTriangleCurrent * centerOfMassCurrent);
        }
    }
}
