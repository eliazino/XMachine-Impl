using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace XMachine.SchemaInterpreter {
    public class TestGenerator {
        private StringBuilder testFile;
        private readonly TestModel testModel;
        public TestGenerator(TestModel testModel) {
            testFile = new StringBuilder();
            this.testModel = testModel;
            testFile.Append("using NUnit.Framework;\nusing System;\n");
            testFile.Append("namespace "+testModel.Project.name+".Test {\n[TestFixture]\n");
            testFile.Append("public class "+testModel.Project.name+"_TEST {\n");
            createSetup();
        }
        private void createSetup() {
            testFile.Append("private "+testModel.Project.name+" testJig;\n");
            testFile.Append("[SetUp]\n");
            testFile.Append("public void Setup() {\n");
            if (testModel.Project.typeIstatic) {
                testFile.Append("this.testJig = "+testModel.Project.name);
            } else {
                testFile.Append("this.testJig = new " + testModel.Project.name);
            }
            testFile.Append("("+testModel.Project.constructorValue+"); \n}\n");
        }
        private string confirmInitialState() {
            string testCase = string.Concat("Assert.That(this.testJig.", this.testModel.State.memoryName,  ", Is.EqualTo(", this.testModel.State.type ,".", this.testModel.State.initialValue, "), \"The initial State must be "+this.testModel.State.initialValue+" \");");
            return generateTestSkeleton("Initial_state_must_be_"+this.testModel.State.initialValue, testCase);
        }
        private string generateATestCase(TestParameter test, StringBuilder? builder = null, int innerTest = 0) {
            if(builder == null) 
                builder = new StringBuilder();
            string functionName = getTestName(test);
            string arg = getParameters(test.withParameter);
            if (test.should.@throw) {
                builder.Append("Assert.Throws<Exception>(() => { this.testJig." + test.function + "(" + arg + "); }, \"" + functionName + "\");\n");
            } else {
                if (test.should.@return == "void") {
                    builder.Append("this.testJig." + test.function + "(" + arg + ");\n");
                } else {
                    string randomName = "result_" + Utilities.Utilities.genID(7);
                    builder.Append("var " + randomName + " = this.testJig." + test.function + "(" + arg + ");\n");
                    if (test.should.@return.StartsWith("typeof(")) {
                        string randomNameType = "result_Type_" + Utilities.Utilities.genID(7);
                        builder.Append("Type "+ randomNameType + " = " + randomName + ".GetType();\n");
                        builder.Append("Assert.That("+ randomNameType + ", Is.EqualTo(" + test.should.@return + "), \"" + functionName + "\"); \n");
                    } else {
                        builder.Append("Assert.That(" + randomName + ", Is.EqualTo(" + test.should.@return + "), \"" + functionName + "\"); \n");
                    }
                    if (test.should.propertyOf != default(PropertyCriteria)) {
                        builder.Append("Assert.That(" + randomName + "."+test.should.propertyOf.name+", Is.EqualTo(" + test.should.propertyOf.value + "), \"" + functionName + "\"); \n");
                    }
                }
                var nextState = getNextState(test.function, this.testModel.State.type);
                builder.Append("Assert.IsTrue(new List<" + this.testModel.State.type + ">(){" + nextState + "}.Contains(this.testJig."+this.testModel.State.memoryName+"), \" this.testJig."+this.testModel.State.memoryName+" Must be the Final State\");\n");
            }
            foreach(TestParameter m in test.continueWith) {
                generateATestCase(m, builder, 0);
            }
            return builder.ToString();
        }
        public string generateTestFile() {
            this.testFile.Append(confirmInitialState()+"\n");
            this.testFile.Append(generateTestCases());
            this.testFile.Append("\n\n}\n}\n");
            return testFile.ToString();
        }

        public string generateTestCases() {
            StringBuilder files = new StringBuilder();
            int counter = 0;
            foreach(TestParameter p in this.testModel.TestParameters) {
                string functionName = "TestCase_" + counter;
                var res = generateTestSkeleton(functionName, generateATestCase(p, null, 0).ToString())+"\n\n";
                files.Append(res);
                counter++;
            }
            return files.ToString();
        }
        private string getNextState(string function, string type) {
            var data = this.testModel.Transitions.FindAll(G=> G.function == function)?.Select(F=>F.to).ToList();
            for(int i=0; i < data.Count;i++) {
                data[i] = type + "." + data[i];
            }
            return string.Join(',', data);
        }

        private string generateTestSkeleton(string functionName, string body) {
            StringBuilder builder = new StringBuilder();
            builder.Append("[TestCase]\n");
            builder.Append("public void " + functionName + "() {\n");
            builder.Append(body+" \n");
            builder.Append("}\n");
            return builder.ToString();
        }
        private string getParameters(string[] param) {
            List<string> p = new List<string>();
            foreach(string s in param) {
                p.Add(s);
            }
            return string.Join(",", p);
        }
        private string getTestName(TestParameter test) {
            if (test.should.@throw)
                return test.function + "_should_throw_an_exception";
            return test.function + "_should_return_" + test.should.@return;
        }
    }
}
