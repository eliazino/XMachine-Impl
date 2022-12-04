using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace examples.Support {
    internal static class Visualizer {
        public static void Visualize<TVertex, TEdge>(this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            string fileName, string dir = "")
            where TEdge : IEdge<TVertex> {
            Visualize(graph, fileName, NoOpEdgeFormatter, dir);
        }

        public static bool Visualize<TVertex, TEdge>(this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            string fileName, FormatEdgeAction<TVertex, TEdge> edgeFormatter, string dir = "")
            where TEdge : IEdge<TVertex> {
            var fullFileName = Path.Combine(dir, fileName);
            var viz = new GraphvizAlgorithm<TVertex, TEdge>(graph);
            viz.FormatVertex += VizFormatVertex;
            viz.FormatEdge += edgeFormatter;
            string c = viz.Generate(new FileDotEngine(), fullFileName);
            return true;
        }

        static void NoOpEdgeFormatter<TVertex, TEdge>(object sender, FormatEdgeEventArgs<TVertex, TEdge> e)
            where TEdge : IEdge<TVertex> {
            // noop
        }

        public static string ToDotNotation<TVertex, TEdge>(this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex> {
            var viz = new GraphvizAlgorithm<TVertex, TEdge>(graph);
            viz.FormatVertex += VizFormatVertex;
            return viz.Generate(new DotPrinter(), "");
        }

        static void VizFormatVertex<TVertex>(object sender, FormatVertexEventArgs<TVertex> e) {
            e.VertexFormatter.Label = e.Vertex.ToString();
            e.VertexFormatter.Shape = GraphvizVertexShape.Box;
            e.VertexFormatter.FillColor = GraphvizColor.LightYellow;
        }
    }

    public sealed class FileDotEngine : IDotEngine {
        public string Run(GraphvizImageType imageType, string dot, string outputFileName) {
            string output = outputFileName;
            File.WriteAllText(output, dot);
            // assumes dot.exe is on the path:
            var args = string.Format(@"{0} -Tjpg -O", output);
            System.Diagnostics.Process.Start("dot.exe", args);
            return output;
        }
    }

    public sealed class DotPrinter : IDotEngine {
        public string Run(GraphvizImageType imageType, string dot, string outputFileName) {
            return dot;
        }
    }

    internal class DrawGraph {
        AdjacencyGraph<string, Edge<string>> _graph = new AdjacencyGraph<string, Edge<string>>();
        private Dictionary<Edge<string>, string> _costs;
        public DrawGraph() {
            _graph.AddVertex("A");
            _graph.AddVertex("B");
            _graph.AddVertex("C");
            _graph.AddVertex("D");
            _graph.AddVertex("E");
            _graph.AddVertex("F");
            _costs = new Dictionary<Edge<string>, string>();
        }
        void costEdgeFormatter(object sender, FormatEdgeEventArgs<string, Edge<string>> e) {
            e.EdgeFormatter.Label.Value = _costs[e.Edge];
            e.EdgeFormatter.StrokeGraphvizColor = GraphvizColor.LightYellow;
        }
        public void addEdges() {
            addEdgeWithCosts("A", "B", "A->B");
            addEdgeWithCosts("A", "D", "A->D");
            addEdgeWithCosts("A", "C", "A->C");
            addEdgeWithCosts("B", "F", "B->F");
            addEdgeWithCosts("D", "E", "D->E");
            addEdgeWithCosts("C", "E", "C->E");
            addEdgeWithCosts("F", "E", "F->E");
            _graph.Visualize("dotfile2", costEdgeFormatter);
        }

        private void addEdgeWithCosts(string source, string target, string cost) {
            var edge = new Edge<string>(source, target);
            _graph.AddVerticesAndEdge(edge);
            _costs.Add(edge, cost);
        }
    }
}