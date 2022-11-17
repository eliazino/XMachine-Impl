// See https://aka.ms/new-console-template for more information 
using examples.Support;
using OpenSoftware.DgmlTools;
using OpenSoftware.DgmlTools.Builders;
using OpenSoftware.DgmlTools.Model;
using QuickGraph;
using System.ComponentModel;
using XMachine;
using XMachine.SchemaInterpreter;

Console.WriteLine("Hello, World!");
SchemaManager smangr = new SchemaManager();
var data = smangr.loadFromFile("StateSchema.json").Result;
var str = new TestGenerator(data).generateTestFile();
//SchemaToGraph graph = new SchemaToGraph(data, "ATM");
//graph.createTransition();
Console.Write("Completed");