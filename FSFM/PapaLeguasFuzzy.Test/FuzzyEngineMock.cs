using PapaLeguasFuzzy.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PapaLeguasFuzzy.Test
{
    public class FuzzyEngineMock
    {
        public static FuzzyFastEngineFactory CreateFuzzyEngine()
        {
            FuzzyFastEngineFactory engine = new FuzzyFastEngineFactory();
            engine.Accumulation = new PapaLeguasFuzzy.Norms.SNorms.Maximum();
            engine.Activation = new PapaLeguasFuzzy.Norms.TNorms.Minimum();
            engine.Disjunction = new PapaLeguasFuzzy.Norms.SNorms.ProbabilisticSum();
            engine.Conjunction = new PapaLeguasFuzzy.Norms.TNorms.Product();
            var INPUT = new Variable( "INPUT");
            var INPUT_LOW = new Triangle(1, 3,  "LOW", INPUT);
            var INPUT_HIGH = new Triangle(2, 4,  "HIGH", INPUT);

            var OUTPUT = new Variable("OUTPUT");
            var OUTPUT_LOW = new Triangle(1, 3, "LOW", OUTPUT);
            var OUTPUT_HIGH = new Triangle(2, 4, "HIGH", OUTPUT);

            engine.InputVariables.Add(INPUT);
            engine.OutputVariables.Add(OUTPUT);

            engine.Rules.Add("if INPUT is LOW then OUTPUT is HIGH");
            engine.Rules.Add("if INPUT is HIGH then OUTPUT is LOW");
            return engine;
        }

        public static FuzzyFastEngineFactory CreateFuzzyEngineTwoInputs()
        {
            FuzzyFastEngineFactory engine = new FuzzyFastEngineFactory();
            engine.Accumulation = new PapaLeguasFuzzy.Norms.SNorms.Maximum();
            engine.Activation = new PapaLeguasFuzzy.Norms.TNorms.Minimum();
            engine.Disjunction = new PapaLeguasFuzzy.Norms.SNorms.ProbabilisticSum();
            engine.Conjunction = new PapaLeguasFuzzy.Norms.TNorms.Product();
            var INPUT = new Variable( "INPUT");
            var INPUT_LOW = new Triangle(1, 3,  "LOW", INPUT);
            var INPUT_HIGH = new Triangle(2, 4,  "HIGH", INPUT);

            var INPUT2 = new Variable( "INPUT2");
            var INPUT2_LOW = new Triangle(1, 3,  "SLOW", INPUT2);
            var INPUT2_HIGH = new Triangle(2, 4,  "FAST", INPUT2);

            var OUTPUT = new Variable( "OUTPUT");
            var OUTPUT_LOW = new Triangle(1, 3,  "LOW", OUTPUT);
            var OUTPUT_HIGH = new Triangle(2, 4,  "HIGH", OUTPUT);

            engine.InputVariables.Add(INPUT);
            engine.InputVariables.Add(INPUT2);
            engine.OutputVariables.Add(OUTPUT);

            engine.Rules.Add("if INPUT is LOW and INPUT2 is SLOW then OUTPUT is HIGH");
            engine.Rules.Add("if INPUT is HIGH and INPUT2 is SLOW then OUTPUT is LOW");
            engine.Rules.Add("if INPUT is LOW and INPUT2 is FAST then OUTPUT is HIGH");
            engine.Rules.Add("if INPUT is HIGH and INPUT2 is FAST then OUTPUT is LOW");
            return engine;
        }

        public static FuzzyFastEngineFactory CreateFuzzyEngineTwoInputsTwoOutputs()
        {
            FuzzyFastEngineFactory engine = new FuzzyFastEngineFactory();
            engine.Accumulation = new PapaLeguasFuzzy.Norms.SNorms.Maximum();
            engine.Activation = new PapaLeguasFuzzy.Norms.TNorms.Minimum();
            engine.Disjunction = new PapaLeguasFuzzy.Norms.SNorms.ProbabilisticSum();
            engine.Conjunction = new PapaLeguasFuzzy.Norms.TNorms.Product();
            var INPUT = new Variable( "INPUT");
            var INPUT_LOW = new Triangle(1, 3,  "LOW", INPUT);
            var INPUT_HIGH = new Triangle(2, 4,  "HIGH", INPUT);

            var INPUT2 = new Variable( "INPUT2");
            var INPUT2_SLOW = new Triangle(1, 3,  "SLOW", INPUT2);
            var INPUT2_FAST = new Triangle(2, 4,  "FAST", INPUT2);

            var OUTPUT = new Variable( "OUTPUT");
            var OUTPUT_LOW = new Triangle(1, 3,  "LOW", OUTPUT);
            var OUTPUT_HIGH = new Triangle(2, 4,  "HIGH", OUTPUT);

            var OUTPUT2 = new Variable( "OUTPUT2");
            var OUTPUT_SLOW = new Triangle(1, 3,  "SLOW", OUTPUT2);
            var OUTPUT_FAST = new Triangle(2, 4,  "FAST", OUTPUT2);

            engine.InputVariables.Add(INPUT);
            engine.InputVariables.Add(INPUT2);
            engine.OutputVariables.Add(OUTPUT);
            engine.OutputVariables.Add(OUTPUT2);

            engine.Rules.Add("if INPUT is LOW and INPUT2 is SLOW then OUTPUT is HIGH and OUTPUT2 is FAST");
            engine.Rules.Add("if INPUT is HIGH and INPUT2 is SLOW then OUTPUT is LOW  and OUTPUT2 is FAST");
            engine.Rules.Add("if INPUT is LOW and INPUT2 is FAST then OUTPUT is HIGH  and OUTPUT2 is SLOW");
            engine.Rules.Add("if INPUT is HIGH and INPUT2 is FAST then OUTPUT is LOW  and OUTPUT2 is SLOW");


            return engine;
        }
    }
}
