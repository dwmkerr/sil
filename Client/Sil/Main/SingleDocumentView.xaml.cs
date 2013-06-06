using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Sil.Main
{
    /// <summary>
    /// Interaction logic for SingleDocumentView.xaml
    /// </summary>
    public partial class SingleDocumentView : UserControl
    {
        public SingleDocumentView()
        {
            InitializeComponent();
        }

        private void MenuItemDisassemble_OnClick(object sender, RoutedEventArgs e)
        {
            //  Show a file open dialog.
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "PE Files (*.dll)|*.dll";
            if (dialog.ShowDialog() == true)
            {
                LoadAssembly(dialog.FileName);
            }
        }

        private void LoadAssembly(string assemblyPath)
        {

            viewModel.DisassemblyPEFile(assemblyPath);
            documentView.Initialise(viewModel.Document);
        }
        
        private void GridDragAndDropHost_OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            bool isCorrect = true;

            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                var filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                foreach (string filename in filenames)
                {
                    if (File.Exists(filename) == false)
                    {
                        isCorrect = false;
                        break;
                    }
                    FileInfo info = new FileInfo(filename);
                    if (info.Extension != ".dll")
                    {
                        isCorrect = false;
                        break;
                    }
                }
            }
            if (isCorrect)
                e.Effects = DragDropEffects.All;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true; 
        }

        private void GridDragAndDropHost_OnPreviewDragOver(object sender, DragEventArgs e)
        {
            GridDragAndDropHost_OnPreviewDragEnter(sender, e);
        }

        private void GridDragAndDropHost_OnPreviewDrop(object sender, DragEventArgs e)
        {
            string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
            
            if(filenames.Any())
            LoadAssembly(filenames.First());
            e.Handled = true; 
        }

        private void MenuItemExit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}