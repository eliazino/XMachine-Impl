using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMachine.SchemaInterpreter {
    public class TestModel {
        public ProjectDetails Project { get; set; }
        public State State { get; set; }
        public List<Function> Functions { get; set; } = new List<Function>();
        public List<Transition> Transitions { get; set; } = new List<Transition>();
        public List<TestParameter> TestParameters { get; set; } = new List<TestParameter>();
    }
    public class ProjectDetails {
        public string name { get; set; }
        public bool typeIstatic { get; set; }
        public string constructorValue { get; set; }
    }
    public class State {
        public string type { get; set; }
        public string memoryName { get; set; }
        public string initialValue { get; set; }
        public string values { get; set; }
    }
    public class Function {
        public string name { get; set; }
        public FunctionArgIn[] args { get; set; }
        public string output { get; set; }
    }
    public class FunctionArgIn {
        public string name { get; set; }
        public string type { get; set; }
    }
    public class Transition {
        public string from { get; set; }
        public string function { get; set; }
        public string to { get; set; }
    }
    public class TestParameter {
        public string function { get; set; }
        public string[] withParameter { get; set; }
        public ShouldExpect should { get; set; }
        public List<TestParameter> continueWith { get; set; } = new List<TestParameter>() { };
    }
    public class ShouldExpect {
        public string @return { get; set; }
        public bool @throw { get; set; } = false;
    }
}
