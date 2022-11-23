using QuickGraph.Graphviz.Dot;
using QuickGraph.Graphviz;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using examples.Support;
using System.Diagnostics;
using System.Drawing;

namespace XMachine.SchemaInterpreter {
    public class SchemaToGraph {
        private TestModel model;
        private string name;
        AdjacencyGraph<string, Edge<string>> _graph = new AdjacencyGraph<string, Edge<string>>();
        private Dictionary<Edge<string>, string> _costs = new Dictionary<Edge<string>, string>();
        public SchemaToGraph(TestModel model, string name) {
            this.model = model;
            this.name = name;
        }
        void costEdgeFormatter(object sender, FormatEdgeEventArgs<string, Edge<string>> e) {
            e.EdgeFormatter.Label.Value = _costs[e.Edge];
            e.EdgeFormatter.StrokeGraphvizColor = GraphvizColor.LightYellow;
        }

        public Bitmap createTransition() {
            foreach(Transition transition in model.Transitions) {
                Function function = model.Functions.Find(F => F.name == transition.function);
                addEdgeWithCosts(transition.from, transition.to, functionStr(function));
            }
            _graph.Visualize(name, costEdgeFormatter);
            Bitmap bm = new Bitmap(name+".jpg");
            return bm;
        }
        private string functionStr(Function function) {
            string functionName = function.name+"(";
            List<string> args = new List<string>();
            foreach(FunctionArgIn arg in function.args) {
                args.Add(arg.type+" "+arg.name);
            }
            functionName += string.Join(",", args) + ")";
            return functionName;
        }

        private void addEdgeWithCosts(string source, string target, string cost) {
            var edge = new Edge<string>(source, target);
            _graph.AddVerticesAndEdge(edge);
            _costs.Add(edge, cost);
        }
        
        private void viewImage() {
            Process photoViewer = new Process();
            photoViewer.StartInfo.FileName = name+".jpg";
            photoViewer.Start();
        }
    }
}
