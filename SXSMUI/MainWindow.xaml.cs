using MdXaml;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using XMachine.SchemaInterpreter;
using System.Windows;
using System.Drawing;
using System.Windows.Media.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace SXSMUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private string testDetails;
        private Bitmap stateDiag;
        public MainWindow() {
            InitializeComponent();
        }
        private JObject schemaObj;
        public static TreeViewItem Json2Tree(JObject root, string rootName = "") {
            var parent = new TreeViewItem() { Header = rootName, IsExpanded = true };
            var type = root.Type;
            foreach (KeyValuePair<string, JToken> token in root) {
                switch (token.Value.Type) {
                    case JTokenType.Array:
                        var jArray = token.Value as JArray;
                        if (jArray?.Any() ?? false) {
                            parent.Items.Add(Json2Tree(token.Value as JArray, token.Key));
                        }//.                            
                        else
                            parent.Items.Add($"\x22{token.Key}\x22 : [ ]"); // Empty array   
                        break;

                    case JTokenType.Object:
                        parent.Items.Add(Json2Tree((JObject)token.Value, token.Key));
                        break;

                    default:
                        parent.Items.Add(GetChild(token));
                        break;
                }
            }
            return parent;
        }

        private void loadCode(string code) {
            var markdown = string.Concat("```C#\n", code, "\n```");
            markdownview.Markdown = markdown;
        }

        public static TreeViewItem Json2Tree(JArray root, string rootName = "") {
            var parent = new TreeViewItem() { Header = rootName, IsExpanded = true };
            var type = root.Type;
            foreach (JToken token in root) {
                switch (token.Type) {
                    case JTokenType.Array:
                        var jArray = token as JArray;
                        if (jArray?.Any() ?? false) {
                            parent.Items.Add(Json2Tree(token as JArray));
                        }//.                            
                        else
                            parent.Items.Add($"\x22{rootName}\x22 : [ ]"); // Empty array   
                        break;

                    case JTokenType.Object:
                        parent.Items.Add(Json2Tree((JObject)token, rootName));
                        break;

                    default:
                        //parent.Items.Add(GetChild(token));
                        break;
                }
            }
            return parent;
        }
        private static TreeViewItem GetChild(KeyValuePair<string, JToken> token) {
            var value = token.Value.ToString();
            var outputValue = string.IsNullOrEmpty(value) ? "null" : value;
            return new TreeViewItem() { Header = $" \x22{token.Key}\x22 : \x22{outputValue}\x22" };
        }

        private void uploadSchemaBtn_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
            bool fileOpened = (bool)ofd.ShowDialog();
            if (fileOpened) {
                string filePath = ofd.FileName;
                var fileStream = ofd.OpenFile();
                using (StreamReader reader = new StreamReader(fileStream)) {
                    var fileContent = reader.ReadToEnd();
                    try {
                        schemaObj = JObject.Parse(fileContent);
                        TreeViewItem schema = Json2Tree(schemaObj, "TestModelSchema");
                        tView.Items.Add(schema);
                        statusText.Text += "\nLoaded "+filePath+" Successfully";
                        SchemaManager smangr = new SchemaManager();
                        var data = smangr.loadFile(fileContent);
                        var lexicErr = smangr.validateSchema(data);
                        if(lexicErr.Count == 0) {
                            var str = new TestGenerator(data).generateTestFile();
                            this.testDetails = str;
                            loadCode(str);
                            loadImage(data);
                            copyTestBtn.IsEnabled = true;
                            downloadStateBtn.IsEnabled = true;
                            return;
                        } else {
                            loadError(lexicErr);
                        }                        
                    } catch(Exception err) {
                        statusText.Text += "\nCould not load file: " + err.ToString();
                    }
                }
            }
            copyTestBtn.IsEnabled = false;
            downloadStateBtn.IsEnabled = false;
        }
        private void loadImage(TestModel data) {
            SchemaToGraph graph = new SchemaToGraph(data, data.Project.name);
            var bm = graph.createTransition();
            stateImage.Source = bitmapToImageSource(bm);
        }
        private BitmapImage bitmapToImageSource(Bitmap bitmap) {
            this.stateDiag = bitmap;
            using (MemoryStream memory = new MemoryStream()) {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        private void loadError(List<Error> errors) {
            foreach(Error err in errors) {
                statusText.Text += "\n" + err.error; 
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e) {

        }

        private void copyTestBtn_Click(object sender, RoutedEventArgs e) {
            Clipboard.SetText(testDetails);
            statusText.Text += "\nTest copied to clipboard";
        }

        private void downloadStateBtn_Click(object sender, RoutedEventArgs e) {
            SaveFileDialog dialog = new SaveFileDialog() {
                Filter = "JPG Files(*.jpg)|*.jpg|All(*.*)|*"
            };
            if (dialog.ShowDialog() == true) {
                stateDiag.Save(dialog.FileName);
                statusText.Text += "\nState Image saved to " + dialog.FileName;
            }
        }
    }
}
