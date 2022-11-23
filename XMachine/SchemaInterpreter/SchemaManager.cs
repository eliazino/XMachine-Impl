using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XMachine.SchemaInterpreter {
    public class SchemaManager {
        private SchemaToGraph schema;
        public SchemaManager() { }
        public async Task<TestModel> loadFromFile(string fileName) {
            try {
                string json = await File.ReadAllTextAsync(fileName);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Found data!");
                var t = JObject.Parse(json).ToObject<TestModel>();
                return t;
            } catch (Exception err) {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("An exception occurs with the details: " + err.ToString());
                return default(TestModel);
            }
        }
        public TestModel loadFile(string json) {
            try {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Found data!");
                var t = JObject.Parse(json).ToObject<TestModel>();
                return t;
            } catch {
                throw;
            }
        }
        public List<Error> validateSchema(TestModel model) {
            List<Error> errors = new List<Error>();
            var t = validateSchemaChildren(model);
            errors.AddRange(t);
            errors.AddRange(validateProjectObj(model.Project));
            errors.AddRange(validateStateObj(model.State));
            if (model.Functions.Count < 1)
                errors.Add(new Error { error = "Schema.Functions must define at least 1 method." });
            foreach (Function f in model.Functions) {
                errors.AddRange(validateFunctionObj(f));
            }
            if (model.Transitions.Count < 1)
                errors.Add(new Error { error = "Schema.Transitions must define at least 1 Transition." });
            List<string> states = getStates(model.State?.values);
            List<Function> functions = model.Functions ?? new List<Function>();
            foreach (Transition f in model.Transitions) {
                errors.AddRange(validateTransitionsbj(f, states, functions));
            }
            return errors;
        }
        private List<string> getStates(string state) {
            if (state is null)
                return new List<string>();
            List<string> states = new List<string>();
            var stateStr = state.Split(",");
            for(int i =0; i < stateStr.Length; i++) {
                states.Add(stateStr[i].Trim());
            }
            return states;
        }
        private List<Error> validateProjectObj(ProjectDetails data) {
            List<Error> errors = new List<Error>();
            if (data is null)
                return errors;
            var t = Utilities.Utilities.isValidVariableName(data.name);
            if (!t.valid)
                errors.Add(new Error { error = t.message });
            return errors;
        }

        private List<Error> validateFunctionObj(Function data) {
            List<Error> errors = new List<Error>();
            if (data is null)
                return errors;
            var t = Utilities.Utilities.isValidVariableName(data.name);
            if (!t.valid)
                errors.Add(new Error { error = "Schema.State.Function_"+data.name+": " + t.message });
            return errors;
        }

        private List<Error> validateStateObj(State data) {
            List<Error> errors = new List<Error>();
            if (data is null)
                return errors;
            var t = Utilities.Utilities.isValidVariableName(data.type);
            if (!t.valid)
                errors.Add(new Error { error = "Schema.State.type: " + t.message });
            if(data.values == null) {
                errors.Add(new Error { error = "Schema.State.values: Invalid State Values" });
            } else {
                string[] states = data.values.Split(',');
                List<string> validState = new List<string>();
                int counter = 0;
                foreach (string _str in states) {
                    string str = _str?.Trim();
                    validState.Add(str);
                    t = Utilities.Utilities.isValidVariableName(str);
                    if (!t.valid)
                        errors.Add(new Error { error = "Schema.State.values[" + counter+"]: State Name must be a valid enum" });
                    counter++;
                }
                if(data.initialValue == null)
                    errors.Add(new Error { error = "Schema.State.initialValue: Initial State must be set" });
                if(!validState.Contains(data.initialValue))
                    errors.Add(new Error { error = "Schema.State.initialValue: Initial State must be a valid state" });
            }
            t = Utilities.Utilities.isValidVariableName(data.memoryName);
            if (!t.valid)
                errors.Add(new Error { error = "Schema.State.memoryName: " + t.message });
            return errors;
        }
        private List<Error> validateTransitionsbj(Transition data, List<string> states, List<Function> functions) {
            List<Error> errors = new List<Error>();
            if (data is null)
                return errors;
            if (!states.Contains(data.from))
                errors.Add(new Error { error = "Schema.Transitions->"+data.from+": State is not valid" });
            if (!states.Contains(data.to))
                errors.Add(new Error { error = "Schema.Transitions->" + data.to + ": State is not valid" });
            var function = functions.Find(F=>F.name == data.function);
            if(function is null)
                errors.Add(new Error { error = "Schema.Transitions Function " + data.function + ": Invalid Method Name "+data.function });
            return errors;
        }
        private List<Error> validateSchemaChildren(TestModel model) {
            List<Error> errors = new List<Error>();
            if (model.Functions is null || model.Functions.Count < 1)
                errors.Add(new Error { error = "Schema.Functions: Function must be an array of 'Functions'" });
            if (model.Project is null)
                errors.Add(new Error { error = "Schema.Project: Project must be defined" });
            if (model.State is null)
                errors.Add(new Error { error = "Schema.State: State must be a valid object" });
            if (model.TestParameters is null || model.TestParameters.Count < 1)
                errors.Add(new Error { error = "Schema.TestParameters: Test Cases must be an array of 'TestParameters'" });
            if (model.Transitions is null || model.Transitions.Count < 1)
                errors.Add(new Error { error = "State.Transitions: Transitions must be an array of 'Transition'" });
            return errors;
        }
    }
    public class Error {
        public int lineNumber { get; set; }
        public string error { get; set; }
    }
}
