using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMachine {
    public class Graph {
        public List<Graph>? Nodes { get; private set; }
        public string Name { get; private set; }
        public string? transitionName { get; private set; }
        public Graph(string name, string? transitionName) {
            this.Name = name;
            this.transitionName = transitionName;
        }
        public void addNode(Graph graph) {
            if(Nodes == null)
                Nodes = new List<Graph> { graph };
            else
                Nodes.Add(graph);
        }
        public int getNodesCount() {
            return Nodes == null? 0 : Nodes.Count;
        }
    }
    public class GraphDS {
        private Graph graph;
        public GraphDS(string name, string? transitionName) {
            graph = new Graph(name, transitionName);
        }
        public Graph? searchGraph(string name) {
            if (graph == null)
                return null;
            return searchGraph(name, graph);
        }
        public Graph? searchGraph(string name, Graph node) {
            if (node == null)
                return null;
            if (node.Name == name)
                return node;   
            if(node.Nodes != null) {
                foreach (Graph v in node.Nodes) {
                    var r = searchGraph(name, v);
                    if(r != null)
                        return r;
                }
            }
            return null;
        }

        public void addNode() {

        }
    }

    public class GraphImpl {
        public class Graph {
            public string name { get; set; }
            public int id { get; set; }
        }
        private string[,] adjacency;
        private string[] vertexes;
        private List<int[]> transitions;
        private int vertexCount = 0;
        public GraphImpl(string[] vertexes) {
            this.adjacency = new string[vertexes.Length, vertexes.Length];
            this.vertexes = vertexes;
            this.vertexCount = vertexes.Length;
            this.transitions = new List<int[]>();
        }
        public void addEdge(int from, int to, string edgeName) {
            if (from < 0 || to < 0 || from >= vertexCount || to >= vertexCount)
                throw new ArgumentOutOfRangeException();
            adjacency[to, from] = edgeName;
            transitions.Add(new int[] { to, from });
        }
        /*public IEnumerable<int[]> getTransitions() {
            foreach (var edge in transitions) {

            }
        }*/
    }
}
