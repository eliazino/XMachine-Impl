using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XMachine.SchemaInterpreter {
    internal class SchemaManager {
        private SchemaToGraph schema;
        public SchemaManager() { }
        public async Task<TestModel> loadFromFile(string fileName) {
            try {
                string json = await System.IO.File.ReadAllTextAsync(fileName);
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
    }
}
