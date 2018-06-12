using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    var root = BuildTreeView(folderBrowserDialog.SelectedPath);
                    FETreeView.Items.Clear();
                    FETreeView.Items.Add(root);
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
    }

    public class MenuItem
    {
        public MenuItem()
        {
            Items = new ObservableCollection<MenuItem>();
        }

        public string Title { get; set; }

        public ObservableCollection<MenuItem> Items { get; set; }
    }
}
