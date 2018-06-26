using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private const string Format = "The result is exported to: {0}";
        private MenuItem _rootItem;


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    _rootItem = BuildTreeView(folderBrowserDialog.SelectedPath);
                    FETreeView.Items.Clear();
                    FETreeView.Items.Add(_rootItem);
                }
            }
        }

        private MenuItem BuildTreeView(string path)
        {
            MenuItem item = new MenuItem { Title = Path.GetFileName(path) };
            try
            {
                Directory.GetFiles(path).ToList().ForEach(file =>
                {
                    item.Items.Add(new MenuItem { Title = Path.GetFileName(file) });
                });
                Directory.GetDirectories(path).ToList().ForEach(dir =>
                {
                    item.Items.Add(BuildTreeView(dir));
                });
            }
            catch (Exception ex)
            {
                FELog.Items.Add(ex.Message);
            }
            return item;
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    var exportPlace = Path.Combine(folderBrowserDialog.SelectedPath, string.Format("export_{0:yyyy-MM-dd-hh-mm-ss}.json", DateTime.Now));

                    using (var file = File.CreateText(exportPlace))
                    {
                        var serializer = new JsonSerializer
                        {
                            Formatting = Formatting.Indented
                        };
                        serializer.Serialize(file, _rootItem);
                        System.Windows.MessageBox.Show(string.Format(Format, exportPlace));
                    }
                }
            }
        }
    }

    public class MenuItem
    {
        public MenuItem()
        {
            Items = new ObservableCollection<MenuItem>();
        }

        [JsonProperty("Name")]
        public string Title { get; set; }

        [JsonProperty("SubItems")]
        public ObservableCollection<MenuItem> Items { get; set; }
    }
}
